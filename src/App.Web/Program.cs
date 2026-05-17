using App.Web.Extensions;
using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;
using Mapster;
using MapsterMapper;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.AddLoggingConfiguration();
builder.Services.AddWebServices(builder.Configuration);

builder.Services.AddLocalization(opts => opts.ResourcesPath = "");
builder.Services.AddControllersWithViews()
    .AddJsonOptions(opts => opts.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase)
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();

builder.Services.AddNotyf(config =>
{
    config.DurationInSeconds = 5;
    config.IsDismissable = true;
    config.Position = NotyfPosition.BottomRight;
});

var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
typeAdapterConfig.Scan(Assembly.GetExecutingAssembly());
builder.Services.AddSingleton(typeAdapterConfig);
builder.Services.AddScoped<IMapper, ServiceMapper>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseNotyf();
app.UseSession();

var supportedCultures = new[] { "tr-TR", "en-US" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture("tr-TR")
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);
localizationOptions.RequestCultureProviders.Insert(0,
    new App.Web.Helpers.SessionRequestCultureProvider());
app.UseRequestLocalization(localizationOptions);

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/hangfire", () => Results.Redirect("https://localhost:5000/hangfire", permanent: false));

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
