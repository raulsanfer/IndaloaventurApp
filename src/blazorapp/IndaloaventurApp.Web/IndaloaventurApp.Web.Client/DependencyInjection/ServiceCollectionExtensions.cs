namespace IndaloaventurApp.Web.Client.DependencyInjection;

using IndaloaventurApp.SharedUI.Abstractions.Auth;
using IndaloaventurApp.SharedUI.Abstractions.Cargos;
using IndaloaventurApp.SharedUI.Abstractions.Licenses;
using IndaloaventurApp.SharedUI.Abstractions.Member;
using IndaloaventurApp.SharedUI.Abstractions.Phonebook;
using IndaloaventurApp.SharedUI.Abstractions.Signals;
using IndaloaventurApp.SharedUI.Abstractions.Session;
using IndaloaventurApp.SharedUI.Abstractions.WordPress;
using IndaloaventurApp.SharedUI.Models.Auth;
using IndaloaventurApp.Web.Client.Infrastructure.Auth;
using IndaloaventurApp.Web.Client.Infrastructure.Cargos;
using IndaloaventurApp.Web.Client.Infrastructure.Licenses;
using IndaloaventurApp.Web.Client.Infrastructure.Member;
using IndaloaventurApp.Web.Client.Infrastructure.Phonebook;
using IndaloaventurApp.Web.Client.Infrastructure.Signals;
using IndaloaventurApp.Web.Client.Infrastructure.Session;
using IndaloaventurApp.Web.Client.Infrastructure.WordPress;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIndaloFrontendServices(this IServiceCollection services, string apiBaseUrl, string googleClientId)
    {
        services.AddScoped<ISessionService, SessionService>();
        services.AddSingleton(new GoogleAuthOptions(googleClientId));

        services.AddHttpClient<IAuthService, AuthApiClient>(client =>
        {
            client.BaseAddress = new Uri(apiBaseUrl);
            client.Timeout = TimeSpan.FromSeconds(20);
        });
        services.AddHttpClient<IMemberProfileService, MemberProfileApiClient>(client =>
        {
            client.BaseAddress = new Uri(apiBaseUrl);
            client.Timeout = TimeSpan.FromSeconds(20);
        });
        services.AddHttpClient<IAdminUserManagementService, AdminUserManagementApiClient>(client =>
        {
            client.BaseAddress = new Uri(apiBaseUrl);
            client.Timeout = TimeSpan.FromSeconds(20);
        });
        services.AddHttpClient<IWordPressPostService, WordPressPostApiClient>(client =>
        {
            client.BaseAddress = new Uri(apiBaseUrl);
            client.Timeout = TimeSpan.FromSeconds(20);
        });
        services.AddHttpClient<IPhonebookService, PhonebookApiClient>(client =>
        {
            client.BaseAddress = new Uri(apiBaseUrl);
            client.Timeout = TimeSpan.FromSeconds(20);
        });
        services.AddHttpClient<ISignalService, SignalApiClient>(client =>
        {
            client.BaseAddress = new Uri(apiBaseUrl);
            client.Timeout = TimeSpan.FromSeconds(20);
        });
        services.AddHttpClient<IFederativeLicenseService, FederativeLicenseApiClient>(client =>
        {
            client.BaseAddress = new Uri(apiBaseUrl);
            client.Timeout = TimeSpan.FromSeconds(20);
        });
        services.AddHttpClient<IAdminFederativeLicenseService, AdminFederativeLicenseApiClient>(client =>
        {
            client.BaseAddress = new Uri(apiBaseUrl);
            client.Timeout = TimeSpan.FromSeconds(20);
        });
        services.AddHttpClient<ICargoAdminService, CargoAdminApiClient>(client =>
        {
            client.BaseAddress = new Uri(apiBaseUrl);
            client.Timeout = TimeSpan.FromSeconds(20);
        });

        return services;
    }
}
