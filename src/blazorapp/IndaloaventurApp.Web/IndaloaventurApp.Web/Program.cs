using IndaloaventurApp.Web.Components;
using IndaloaventurApp.Web.Client.DependencyInjection;
using IndaloaventurApp.SharedUI.Abstractions.FoodAlerts;
using IndaloaventurApp.Web.Infrastructure.FoodAlerts;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(options =>
    {
        options.DetailedErrors = builder.Environment.IsDevelopment();
    })
    .AddInteractiveWebAssemblyComponents();
builder.Services.AddLocalization();
builder.Services.AddIndaloFrontendServices(
    GetApiBaseUrl(builder.Configuration),
    GetGoogleClientId(builder.Configuration));
builder.Services.AddHttpClient<IFoodAlertService, ExternalFoodAlertService>(client =>
{
    client.BaseAddress = new Uri(GetFoodAlertsBaseUrl(builder.Configuration));
    client.Timeout = TimeSpan.FromSeconds(20);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("es-ES"),
    SupportedCultures = [new CultureInfo("es-ES")],
    SupportedUICultures = [new CultureInfo("es-ES")]
});

app.UseAntiforgery();

app.MapFoodAlertEndpoints();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(IndaloaventurApp.Web.Client._Imports).Assembly);

app.Run();

static string GetApiBaseUrl(IConfiguration configuration)
{
    var configuredBaseUrl = configuration["ApiSettings:BaseUrl"];
    return string.IsNullOrWhiteSpace(configuredBaseUrl) ? "https://localhost:7160" : configuredBaseUrl;
}

static string GetGoogleClientId(IConfiguration configuration)
{
    return configuration["GoogleAuth:ClientId"] ?? string.Empty;
}

static string GetFoodAlertsBaseUrl(IConfiguration configuration)
{
    var configuredBaseUrl = configuration["FoodAlerts:BaseUrl"];
    return string.IsNullOrWhiteSpace(configuredBaseUrl) ? "https://redalertas.runasp.net" : configuredBaseUrl;
}
