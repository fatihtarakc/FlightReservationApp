AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) =>
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services));

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = ctx =>
        {
            var errors = ctx.ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .Where(m => !string.IsNullOrWhiteSpace(m))
                .ToList();
            var message = errors.Count > 0 ? string.Join(" | ", errors) : "Validation failed.";
            return new BadRequestObjectResult(new { IsSuccess = false, Message = message });
        };
    });
builder.Services.AddEndpointsApiExplorer();

// Localization
builder.Services.AddLocalization();

// FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Swagger with JWT support
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "FlightReservation API",
        Version = "v1",
        Description = "Enterprise-grade Flight Reservation System API"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] then your token.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// JWT Authentication
var tokenOptions = builder.Configuration.GetSection(App.Core.Options.TokenOptions.TokenConfiguration).Get<App.Core.Options.TokenOptions>()!;
builder.Services.AddAuthentication()
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = tokenOptions.Issuer,
        ValidAudience = tokenOptions.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOptions.IssuerSigningSymmetricSecurityKey)),
        ClockSkew = TimeSpan.Zero
    };
});

// PostConfigure ensures JWT defaults win over AddIdentity's cookie defaults
builder.Services.PostConfigure<AuthenticationOptions>(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserPolicy", policy => policy.RequireRole("AppUser", "Admin"));
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });

    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5002", "https://localhost:5003")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Data Access
builder.Services.AddDataAccessServices(builder.Configuration);
builder.Services.AddDataAccessConcreteServices();

// Business Services
builder.Services.AddBusinessServices(builder.Configuration);

// Queue / MassTransit
builder.Services.AddQueueServices(builder.Configuration);

// Ensure Hangfire PostgreSQL database exists before Hangfire service registration
EnsureHangfireDatabaseExists(builder.Configuration);

// Background Jobs / Hangfire
builder.Services.AddBackgroundJobServices(builder.Configuration);

var app = builder.Build();

// Apply migrations and seed data on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FlightReservationDbContext>();
    db.Database.Migrate();

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await SeedDataService.SeedAsync(db, userManager, roleManager);
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "FlightReservation API v1");
    c.RoutePrefix = "swagger";
});

if (!app.Environment.IsProduction())
    app.UseHttpsRedirection();
app.UseSerilogRequestLogging();
app.UseRouting();

var supportedCultures = new[] { "tr-TR", "en-US" };
app.UseRequestLocalization(new RequestLocalizationOptions()
    .SetDefaultCulture("tr-TR")
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures));

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

// Hangfire Dashboard
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireAuthorizationFilter() },
    DashboardTitle = "FlightReservation - Hangfire Dashboard"
});

app.MapControllers();

// Configure recurring jobs
app.Services.ConfigureRecurringJobs();

app.Run();

static void EnsureHangfireDatabaseExists(IConfiguration configuration)
{
    var connectionOptions = configuration.GetSection(ConnectionOptions.Connections).Get<ConnectionOptions>();
    if (connectionOptions?.DatabaseProvider != "PostgreSql") return;

    var csb = new NpgsqlConnectionStringBuilder(connectionOptions.Hangfire);
    var dbName = csb.Database ?? "FlightReservationHangfireDb";
    csb.Database = "postgres";

    using var conn = new NpgsqlConnection(csb.ConnectionString);
    conn.Open();

    using var checkCmd = conn.CreateCommand();
    checkCmd.CommandText = $"SELECT 1 FROM pg_database WHERE datname = '{dbName}'";
    var exists = checkCmd.ExecuteScalar() != null;

    if (!exists)
    {
        using var createCmd = conn.CreateCommand();
        createCmd.CommandText = $"CREATE DATABASE \"{dbName}\"";
        createCmd.ExecuteNonQuery();
    }
}
