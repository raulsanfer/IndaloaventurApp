using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using IndaloaventurApp.SharedUI.Abstractions.FoodAlerts;
using IndaloaventurApp.Web.Client.DependencyInjection;
using IndaloaventurApp.Web.Client.Infrastructure.FoodAlerts;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddLocalization();
builder.Services.AddIndaloFrontendServices(GetApiBaseUrl(builder), GetGoogleClientId(builder));
builder.Services.AddHttpClient<IFoodAlertService, FoodAlertAppApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
    client.Timeout = TimeSpan.FromSeconds(20);
});

await builder.Build().RunAsync();

static string GetApiBaseUrl(WebAssemblyHostBuilder builder)
{
    var configuredBaseUrl = builder.Configuration["ApiSettings:BaseUrl"];
    return string.IsNullOrWhiteSpace(configuredBaseUrl)
        ? builder.HostEnvironment.BaseAddress
        : configuredBaseUrl;
}

static string GetGoogleClientId(WebAssemblyHostBuilder builder)
{
    return builder.Configuration["GoogleAuth:ClientId"] ?? string.Empty;
}
