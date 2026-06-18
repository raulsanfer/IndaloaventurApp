namespace IndaloaventurApp.Frontend.Tests;

using Bunit;
using IndaloaventurApp.SharedUI.Abstractions.Licenses;
using IndaloaventurApp.SharedUI.Abstractions.Session;
using IndaloaventurApp.SharedUI.Components.Club;
using IndaloaventurApp.SharedUI.Models.Auth;
using IndaloaventurApp.SharedUI.Models.Common;
using IndaloaventurApp.SharedUI.Models.Licenses;
using IndaloaventurApp.SharedUI.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

public sealed class FederativeLicensesViewTests : BunitContext
{
    [Fact]
    public void FederativeLicensesView_ShowsNonOperationalState_WhenSessionHasNoMemberAccess()
    {
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(new AuthSession("token", "Bearer", 3600, false, Array.Empty<string>()));

        Services.AddSingleton<ISessionService>(sessionService);
        Services.AddSingleton<IFederativeLicenseService>(new RecordingFederativeLicenseService());
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();

        var cut = Render<FederativeLicensesView>();

        Assert.Contains("mi_club_federative_licenses_not_available", cut.Markup);
        Assert.DoesNotContain("mi_club_federative_licenses_loading", cut.Markup);
    }

    [Fact]
    public void FederativeLicensesView_ShowsNonOperationalState_ForAdminRole()
    {
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(new AuthSession("token", "Bearer", 3600, false, new[] { "Admin" }));

        Services.AddSingleton<ISessionService>(sessionService);
        Services.AddSingleton<IFederativeLicenseService>(new RecordingFederativeLicenseService());
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();

        var cut = Render<FederativeLicensesView>();

        Assert.Contains("mi_club_federative_licenses_not_available", cut.Markup);
        Assert.Contains("mi_club_federative_licenses_request", cut.Markup);
        Assert.True(cut.Find("button.mi-club-licenses__request-btn").HasAttribute("disabled"));
    }

    [Fact]
    public void FederativeLicensesView_GroupsLicensesBySeason_AndRendersState()
    {
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(new AuthSession("token", "Bearer", 3600, true, new[] { "Member" }));

        var service = new RecordingFederativeLicenseService
        {
            GetMyFederativeLicensesHandler = _ => Task.FromResult(ServiceResult<IReadOnlyList<FederativeLicenseRequest>>.Success(new[]
            {
                new FederativeLicenseRequest(Guid.NewGuid(), 2025, false, "Basica", "Adulto", "Andalucia", "Aprobada"),
                new FederativeLicenseRequest(Guid.NewGuid(), 2026, true, "B Plus", "Infantil", "Nacional", "Pendiente"),
                new FederativeLicenseRequest(Guid.NewGuid(), 2026, false, "A", "Adulto", "Autonomico", "En revision")
            }))
        };

        Services.AddSingleton<ISessionService>(sessionService);
        Services.AddSingleton<IFederativeLicenseService>(service);
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();

        var cut = Render<FederativeLicensesView>();

        cut.WaitForAssertion(() =>
        {
            Assert.Contains("Temporada 2026", cut.Markup);
            Assert.Contains("Temporada 2025", cut.Markup);
            Assert.Contains("Temporada 2026 · Media Temporada", cut.Markup);
            Assert.Contains("Temporada 2026 · Temporada Completa", cut.Markup);
            Assert.Contains("Pendiente", cut.Markup);
            Assert.Contains("En revision", cut.Markup);
            Assert.Contains("Andalucia", cut.Markup);
            Assert.Contains("mi_club_federative_licenses_request", cut.Markup);
        });
    }

    [Fact]
    public void FederativeLicensesView_LoadsRates_WhenSeasonIsSelected()
    {
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(new AuthSession("token", "Bearer", 3600, true, new[] { "Member" }));

        var service = new RecordingFederativeLicenseService
        {
            GetMyFederativeLicensesHandler = _ => Task.FromResult(ServiceResult<IReadOnlyList<FederativeLicenseRequest>>.Success(Array.Empty<FederativeLicenseRequest>())),
            GetAvailableRatesHandler = (temporada, mediaTemporada, _) =>
            {
                Assert.Equal(2026, temporada);
                Assert.False(mediaTemporada);

                return Task.FromResult(ServiceResult<IReadOnlyList<FederativeLicenseRate>>.Success(new[]
                {
                    new FederativeLicenseRate(18, 2026, false, "B Plus", "Adulto", "Nacional", 85.5m, null)
                }));
            }
        };

        Services.AddSingleton<ISessionService>(sessionService);
        Services.AddSingleton<IFederativeLicenseService>(service);
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();

        var cut = Render<FederativeLicensesView>();

        cut.Find("button.btn.btn-primary.mi-club-licenses__request-btn").Click();
        cut.FindAll("select")[1].Change("2026");

        cut.WaitForAssertion(() =>
        {
            Assert.Contains("mi_club_federative_licenses_request_title", cut.Markup);
            Assert.Contains("B Plus", cut.Markup);
        });
    }

    [Fact]
    public void FederativeLicensesView_ShowsEmptyRatesMessage_WhenSelectedSeasonHasNoRates()
    {
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(new AuthSession("token", "Bearer", 3600, true, new[] { "Member" }));

        var service = new RecordingFederativeLicenseService
        {
            GetMyFederativeLicensesHandler = _ => Task.FromResult(ServiceResult<IReadOnlyList<FederativeLicenseRequest>>.Success(Array.Empty<FederativeLicenseRequest>())),
            GetAvailableRatesHandler = (_, _, _) => Task.FromResult(ServiceResult<IReadOnlyList<FederativeLicenseRate>>.Success(Array.Empty<FederativeLicenseRate>()))
        };

        Services.AddSingleton<ISessionService>(sessionService);
        Services.AddSingleton<IFederativeLicenseService>(service);
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();

        var cut = Render<FederativeLicensesView>();

        cut.Find("button.btn.btn-primary.mi-club-licenses__request-btn").Click();
        cut.FindAll("select")[1].Change("2027");

        cut.WaitForAssertion(() =>
        {
            Assert.Contains("mi_club_federative_licenses_request_rates_empty", cut.Markup);
        });
    }

    [Fact]
    public void FederativeLicensesView_ReloadsRates_WhenModalityChanges()
    {
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(new AuthSession("token", "Bearer", 3600, true, new[] { "Member" }));

        var requestedModalities = new List<bool>();

        var service = new RecordingFederativeLicenseService
        {
            GetMyFederativeLicensesHandler = _ => Task.FromResult(ServiceResult<IReadOnlyList<FederativeLicenseRequest>>.Success(Array.Empty<FederativeLicenseRequest>())),
            GetAvailableRatesHandler = (temporada, mediaTemporada, _) =>
            {
                Assert.Equal(2026, temporada);
                requestedModalities.Add(mediaTemporada);

                var rates = mediaTemporada
                    ? new[]
                    {
                        new FederativeLicenseRate(28, 2026, true, "A", "Cadete", "Autonomico", 32m, null)
                    }
                    : new[]
                    {
                        new FederativeLicenseRate(18, 2026, false, "B Plus", "Adulto", "Nacional", 85.5m, null)
                    };

                return Task.FromResult(ServiceResult<IReadOnlyList<FederativeLicenseRate>>.Success(rates));
            }
        };

        Services.AddSingleton<ISessionService>(sessionService);
        Services.AddSingleton<IFederativeLicenseService>(service);
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();

        var cut = Render<FederativeLicensesView>();

        cut.Find("button.btn.btn-primary.mi-club-licenses__request-btn").Click();

        var selects = cut.FindAll("select");
        selects[1].Change("2026");

        cut.WaitForAssertion(() => Assert.Contains("B Plus", cut.Markup));

        selects = cut.FindAll("select");
        selects[0].Change("true");

        cut.WaitForAssertion(() =>
        {
            Assert.Equal(new[] { false, true }, requestedModalities);
            Assert.Contains(">A</option>", cut.FindAll("select")[2].InnerHtml);
            Assert.DoesNotContain(">B Plus</option>", cut.FindAll("select")[2].InnerHtml);
        });
    }

    [Fact]
    public void FederativeLicensesView_CreatesRequest_AndRefreshesList()
    {
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(new AuthSession("token", "Bearer", 3600, true, new[] { "Member" }));

        IReadOnlyList<FederativeLicenseRequest> currentRequests = Array.Empty<FederativeLicenseRequest>();

        var service = new RecordingFederativeLicenseService
        {
            GetMyFederativeLicensesHandler = _ => Task.FromResult(ServiceResult<IReadOnlyList<FederativeLicenseRequest>>.Success(currentRequests)),
            GetAvailableRatesHandler = (_, mediaTemporada, _) =>
            {
                Assert.False(mediaTemporada);

                return Task.FromResult(ServiceResult<IReadOnlyList<FederativeLicenseRate>>.Success(new[]
                {
                    new FederativeLicenseRate(30, 2026, false, "A", "Adulto", "Autonomico", 40m, null)
                }));
            },
            CreateFederativeLicenseRequestHandler = (request, _) =>
            {
                currentRequests = new[]
                {
                    new FederativeLicenseRequest(Guid.NewGuid(), request.Temporada, false, "A", "Adulto", "Autonomico", "Pendiente")
                };

                return Task.FromResult(ServiceResult<FederativeLicenseRequest>.Success(currentRequests[0]));
            }
        };

        Services.AddSingleton<ISessionService>(sessionService);
        Services.AddSingleton<IFederativeLicenseService>(service);
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();

        var cut = Render<FederativeLicensesView>();

        cut.Find("button.btn.btn-primary.mi-club-licenses__request-btn").Click();

        var selects = cut.FindAll("select");
        selects[1].Change("2026");

        cut.WaitForAssertion(() => Assert.Contains("A", cut.Markup));

        selects = cut.FindAll("select");
        selects[2].Change("A");
        selects[3].Change("Adulto");

        var confirmButton = cut.FindAll("button")
            .Single(button => button.TextContent.Contains("mi_club_federative_licenses_request_confirm"));

        confirmButton.Click();

        cut.WaitForAssertion(() =>
        {
            Assert.NotNull(service.LastCreateRequest);
            Assert.Equal(2026, service.LastCreateRequest!.Temporada);
            Assert.Contains("Pendiente", cut.Markup);
            Assert.Contains("Autonomico", cut.Markup);
            Assert.Contains("Temporada 2026 · Temporada Completa", cut.Markup);
        });
    }
}
