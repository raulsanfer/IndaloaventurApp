namespace IndaloaventurApp.Frontend.Tests;

using Bunit;
using IndaloaventurApp.SharedUI.Abstractions.Licenses;
using IndaloaventurApp.SharedUI.Abstractions.Session;
using IndaloaventurApp.SharedUI.Components.Settings;
using IndaloaventurApp.SharedUI.Models.Auth;
using IndaloaventurApp.SharedUI.Models.Common;
using IndaloaventurApp.SharedUI.Models.Licenses;
using IndaloaventurApp.SharedUI.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

public sealed class AdminFederativeLicenseManagementViewTests : BunitContext
{
    [Fact]
    public void AdminFederativeLicenseManagementView_LoadsWithInitialUserFilter()
    {
        var adminUserId = Guid.Parse("6b46bb4c-9bff-43b9-b31d-5098b76f115e");
        var targetUserId = Guid.Parse("2a5c3c5a-6aa5-4bcc-b22e-ef6f6955366a");

        var sessionService = new RecordingSessionService();
        sessionService.SetSession(new AuthSession("token", "Bearer", 3600, false, new[] { "Admin" }, adminUserId));

        var service = new RecordingAdminFederativeLicenseService
        {
            GetFederativeLicensesHandler = (query, _) =>
            {
                Assert.Equal(targetUserId, query.UserId);
                return Task.FromResult(ServiceResult<IReadOnlyList<AdminFederativeLicenseRequest>>.Success(new[]
                {
                    new AdminFederativeLicenseRequest(
                        Guid.NewGuid(),
                        targetUserId,
                        "socio@club.es",
                        2026,
                        "Pendiente",
                        DateTime.UtcNow,
                        1,
                        "A",
                        "Adulto",
                        "Autonomico",
                        45m,
                        null)
                }));
            }
        };

        Services.AddSingleton<ISessionService>(sessionService);
        Services.AddSingleton<IAdminFederativeLicenseService>(service);
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();

        var cut = Render<AdminFederativeLicenseManagementView>(parameters => parameters.Add(x => x.InitialUserId, targetUserId));

        cut.WaitForAssertion(() =>
        {
            Assert.Contains("socio@club.es", cut.Markup);
            Assert.Equal(targetUserId, service.LastQuery!.UserId);
            Assert.Equal(targetUserId.ToString("D"), cut.Find("#federative-license-user-filter").GetAttribute("value"));
        });
    }

    [Fact]
    public void AdminFederativeLicenseManagementView_UpdatesStatusAndReloadsList()
    {
        var adminUserId = Guid.Parse("6b46bb4c-9bff-43b9-b31d-5098b76f115e");
        var targetUserId = Guid.Parse("2a5c3c5a-6aa5-4bcc-b22e-ef6f6955366a");
        var requestId = Guid.Parse("a8d9cb45-7b4d-43e1-a658-1fd144f6b4ee");

        var sessionService = new RecordingSessionService();
        sessionService.SetSession(new AuthSession("token", "Bearer", 3600, false, new[] { "Admin" }, adminUserId));

        var currentItems = new List<AdminFederativeLicenseRequest>
        {
            new(
                requestId,
                targetUserId,
                "socio@club.es",
                2026,
                "Pendiente",
                DateTime.UtcNow,
                1,
                "A",
                "Adulto",
                "Autonomico",
                45m,
                null)
        };

        var service = new RecordingAdminFederativeLicenseService
        {
            GetFederativeLicensesHandler = (_, _) =>
                Task.FromResult(ServiceResult<IReadOnlyList<AdminFederativeLicenseRequest>>.Success(currentItems.ToArray())),
            UpdateFederativeLicenseStatusHandler = (request, _) =>
            {
                currentItems[0] = currentItems[0] with { Estado = request.Estado };
                return Task.FromResult(ServiceResult<AdminFederativeLicenseRequest>.Success(currentItems[0]));
            }
        };

        Services.AddSingleton<ISessionService>(sessionService);
        Services.AddSingleton<IAdminFederativeLicenseService>(service);
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();

        var cut = Render<AdminFederativeLicenseManagementView>();

        cut.WaitForAssertion(() => Assert.Contains("Pendiente", cut.Markup));

        cut.Find("select.select.select-bordered.select-sm").Change("Confirmada");
        cut.Find("button.btn.btn-sm.btn-outline").Click();

        cut.WaitForAssertion(() =>
        {
            Assert.NotNull(service.LastUpdateRequest);
            Assert.Equal(targetUserId, service.LastUpdateRequest!.UserId);
            Assert.Equal(requestId, service.LastUpdateRequest.SolicitudId);
            Assert.Equal("Confirmada", service.LastUpdateRequest.Estado);
            Assert.Contains("settings_federative_licenses_update_success", cut.Markup);
            Assert.Contains("Confirmada", cut.Markup);
        });
    }
}
