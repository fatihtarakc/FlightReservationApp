using FlightReservation.Business.Interfaces;
using FlightReservation.Business.Services;
using FlightReservation.Core.Entities;
using FlightReservation.Data.Context;
using FlightReservation.Data.Seed;
using FlightReservation.Infrastructure.Extensions;
using FlightReservation.Infrastructure.Hangfire;
using FlightReservation.Infrastructure.Hangfire.Jobs;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Globalization;

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .Build())
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((ctx, services, config) =>
        config.ReadFrom.Configuration(ctx.Configuration)
              .ReadFrom.Services(services));

    // ── Database ────────────────────────────────────────────────────────────
    builder.Services.AddDbContext<AppDbContext>(opt =>
        opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    // ── Identity ────────────────────────────────────────────────────────────
    builder.Services.AddIdentity<ApplicationUser, IdentityRole>(opt =>
    {
        opt.Password.RequiredLength = 8;
        opt.Password.RequireNonAlphanumeric = true;
        opt.Lockout.MaxFailedAccessAttempts = 5;
        opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        opt.SignIn.RequireConfirmedEmail = false; // geliştirme için kapalı
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

    // ── Business Services ───────────────────────────────────────────────────
    builder.Services.AddScoped<IRouteService, RouteService>();
    builder.Services.AddScoped<IFlightService, FlightService>();
    builder.Services.AddScoped<IReservationService, ReservationService>();
    builder.Services.AddScoped<IAircraftService, AircraftService>();

    // ── Infrastructure (Redis, Email, Hangfire, RabbitMQ) ───────────────────
    builder.Services.AddInfrastructure(builder.Configuration);

    // ── HTTP Client (API tüketimi için) ─────────────────────────────────────
    builder.Services.AddHttpClient("FlightApi", client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7001");
        client.DefaultRequestHeaders.Add("Accept", "application/json");
    });

    // ── Localization ────────────────────────────────────────────────────────
    builder.Services.AddLocalization(opt => opt.ResourcesPath = "Resources");
    builder.Services.Configure<RequestLocalizationOptions>(opt =>
    {
        var cultures = new[] { new CultureInfo("tr"), new CultureInfo("en") };
        opt.DefaultRequestCulture = new RequestCulture("tr");
        opt.SupportedCultures = cultures;
        opt.SupportedUICultures = cultures;
        opt.RequestCultureProviders.Insert(0, new CookieRequestCultureProvider());
    });

    // ── MVC ─────────────────────────────────────────────────────────────────
    builder.Services.AddControllersWithViews()
        .AddViewLocalization()
        .AddDataAnnotationsLocalization();

    builder.Services.AddRazorPages();

    // ── Auth cookie ─────────────────────────────────────────────────────────
    builder.Services.ConfigureApplicationCookie(opt =>
    {
        opt.LoginPath = "/Account/Login";
        opt.AccessDeniedPath = "/Account/AccessDenied";
        opt.ExpireTimeSpan = TimeSpan.FromDays(7);
        opt.SlidingExpiration = true;
    });

    // ── Session ─────────────────────────────────────────────────────────────
    builder.Services.AddDistributedMemoryCache(); // fallback; Redis zaten AddInfrastructure'da
    builder.Services.AddSession(opt => opt.IdleTimeout = TimeSpan.FromMinutes(30));

    var app = builder.Build();

    // ── Seed ─────────────────────────────────────────────────────────────────
    await DatabaseSeeder.SeedAsync(app.Services);

    // ── Pipeline ──────────────────────────────────────────────────────────────
    if (app.Environment.IsDevelopment())
        app.UseDeveloperExceptionPage();
    else
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseSerilogRequestLogging();
    app.UseRequestLocalization();
    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseSession();

    // ── Hangfire Dashboard ────────────────────────────────────────────────────
    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
        Authorization = [new HangfireDashboardAuthFilter()],
        DashboardTitle = "Flight Reservation — Jobs"
    });

    // ── Hangfire Recurring Jobs ───────────────────────────────────────────────
    RecurringJob.AddOrUpdate<MarkCompletedFlightsJob>(
        "mark-completed-flights",
        j => j.ExecuteAsync(),
        Cron.Hourly);

    RecurringJob.AddOrUpdate<FlightReminderJob>(
        "flight-reminders",
        j => j.ExecuteAsync(),
        Cron.Daily(8));

    RecurringJob.AddOrUpdate<CleanupAuditLogsJob>(
        "cleanup-audit-logs",
        j => j.ExecuteAsync(),
        Cron.Weekly(DayOfWeek.Sunday, 2));

    // ── Routes ────────────────────────────────────────────────────────────────
    app.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.MapRazorPages();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed.");
}
finally
{
    Log.CloseAndFlush();
}
