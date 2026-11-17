namespace IndaloaventurApp.Frontend.Tests.Features.Licenses;

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

public sealed class FederativeLicenseCoverageTests : BunitContext
{
    private int Year => DateTime.UtcNow.Year;
    private FederativeLicenseRate Rate(int type, int id = 30) => new(id, Year, type, "Tipo " + type, "A", "Adulto", "Nacional", 40m, 55m);
    private IRenderedComponent<FederativeLicensesView> Open(RecordingFederativeLicenseService service)
    {
        var session = new RecordingSessionService();
        session.SetSession(new AuthSession("token", "Bearer", 3600, true, ["Member"]));
        Services.AddSingleton<ISessionService>(session);
        Services.AddSingleton<IFederativeLicenseService>(service);
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();
        var cut = Render<FederativeLicensesView>();
        _ = cut.Find("button.mi-club-licenses__request-btn").ClickAsync(new());
        return cut;
    }
    private static Task<ServiceResult<IReadOnlyList<FederativeLicenseRate>>> Rates(params FederativeLicenseRate[] rates)
        => Task.FromResult(ServiceResult<IReadOnlyList<FederativeLicenseRate>>.Success(rates));
    private static AngleSharp.Dom.IElement Confirm(IRenderedComponent<FederativeLicensesView> cut)
        => cut.Find(".modal-action .btn-primary");
    private void SelectDaily(IRenderedComponent<FederativeLicensesView> cut)
    {
        cut.FindAll("select")[0].Change(Year.ToString());
        cut.FindAll("select")[1].Change("3");
        cut.FindAll("select")[2].Change("A");
        cut.FindAll("select")[3].Change("Adulto");
    }

    /// <summary>Verifies daily coverage validates dates and keeps catalog price.</summary>
    [Theory]
    [InlineData(null, null, false)]
    [InlineData("01-01", null, false)]
    [InlineData("12-31", "01-01", false)]
    [InlineData("01-01", "01-01", true)]
    [InlineData("01-01", "12-31", true)]
    [InlineData("previous", "01-01", false)]
    [InlineData("01-01", "next", false)]
    public void DailyCoverage_ValidatesDates_AndKeepsCatalogPrice(string? start, string? end, bool valid)
    {
        var service = new RecordingFederativeLicenseService { GetAvailableRatesHandler = (_, type, _) => Rates(Rate(type)) };
        var cut = Open(service);
        Assert.Equal(new[] { "1", "2", "3" }, cut.FindAll("select")[1].QuerySelectorAll("option").Select(o => o.GetAttribute("value")));
        SelectDaily(cut);
        string Date(string value) => value switch { "previous" => $"{Year-1}-12-31", "next" => $"{Year+1}-01-01", _ => $"{Year}-{value}" };
        if (start is not null) cut.FindAll("input[type=date]")[0].Change(Date(start));
        if (end is not null) cut.FindAll("input[type=date]")[1].Change(Date(end));
        Assert.Equal(!valid, Confirm(cut).HasAttribute("disabled"));
        Assert.Contains(40m.ToString("C", System.Globalization.CultureInfo.CurrentCulture), cut.Find(".mi-club-licenses__price-value").TextContent);
        if (valid)
        {
            Confirm(cut).Click();
            Assert.Equal(DateOnly.Parse(Date(start!)), service.LastCreateRequest!.FechaInicio);
            Assert.Equal(DateOnly.Parse(Date(end!)), service.LastCreateRequest.FechaFin);
        }
        else Assert.Null(service.LastCreateRequest);
    }

    /// <summary>Verifies changes clear dependencies and coverage and deduplicate options.</summary>
    [Fact]
    public void Changes_ClearDependenciesAndCoverage_AndDeduplicateOptions()
    {
        var cut = Open(new RecordingFederativeLicenseService { GetAvailableRatesHandler = (_, type, _) => Rates(Rate(type), Rate(type, 31) with { Categoria = "Infantil" }) });
        SelectDaily(cut);
        Assert.Equal(2, cut.FindAll("select")[2].QuerySelectorAll("option").Length); // placeholder and A
        Assert.Equal(3, cut.FindAll("select")[3].QuerySelectorAll("option").Length);
        cut.FindAll("input[type=date]")[0].Change($"{Year}-01-01");
        cut.FindAll("input[type=date]")[1].Change($"{Year}-01-01");
        cut.FindAll("select")[2].Change("A");
        Assert.Empty(cut.FindAll(".mi-club-licenses__price-value"));
        Assert.True(Confirm(cut).HasAttribute("disabled"));
        cut.FindAll("select")[0].Change((Year+1).ToString());
        Assert.All(cut.FindAll("input[type=date]"), input => Assert.Equal("", input.GetAttribute("value")));
        cut.FindAll("select")[1].Change("1");
        Assert.Empty(cut.FindAll("input[type=date]"));
        cut.FindAll("select")[1].Change("3");
        Assert.All(cut.FindAll("input[type=date]"), input => Assert.Equal("", input.GetAttribute("value")));
    }

    /// <summary>Verifies empty or ambiguous daily rates cannot be confirmed.</summary>
    [Fact]
    public void EmptyOrAmbiguousDailyRates_CannotBeConfirmed()
    {
        var cut = Open(new RecordingFederativeLicenseService { GetAvailableRatesHandler = (_, type, _) => type == 3 ? Rates() : Rates(Rate(type), Rate(type,31)) });
        cut.FindAll("select")[0].Change(Year.ToString());
        cut.FindAll("select")[2].Change("A");
        cut.FindAll("select")[3].Change("Adulto");
        Assert.Contains("mi_club_federative_licenses_request_catalog_invalid", cut.Markup);
        Assert.True(Confirm(cut).HasAttribute("disabled"));
        cut.FindAll("select")[1].Change("3");
        Assert.Contains("mi_club_federative_licenses_request_rates_empty", cut.Markup);
        Assert.True(Confirm(cut).HasAttribute("disabled"));
        Assert.Empty(cut.FindAll(".mi-club-licenses__price-value"));
    }

    /// <summary>Verifies failed daily request leaves form usable without rendering server details.</summary>
    [Theory]
    [InlineData("licenses.request_conflict", "licenses_request_non_daily_conflict")]
    [InlineData("licenses.request_invalid", "licenses_request_invalid")]
    public void FailedDailyRequest_LeavesFormUsable_WithoutRenderingServerDetails(string code, string key)
    {
        var cut = Open(new RecordingFederativeLicenseService {
            GetAvailableRatesHandler = (_, type, _) => Rates(Rate(type)),
            CreateFederativeLicenseRequestHandler = (_, _) => Task.FromResult(ServiceResult<FederativeLicenseRequest>.Failure(new(code, "<script>secret</script>"))) });
        SelectDaily(cut);
        cut.FindAll("input[type=date]")[0].Change($"{Year}-01-01");
        cut.FindAll("input[type=date]")[1].Change($"{Year}-01-01");
        Confirm(cut).Click();
        Assert.Contains(key, cut.Markup);
        Assert.DoesNotContain("secret", cut.Markup);
        Assert.False(Confirm(cut).HasAttribute("disabled"));
    }

    /// <summary>Verifies daily creation refreshes all requests of season and allows another daily request.</summary>
    [Fact]
    public void DailyCreation_RefreshesAllRequestsOfSeason_AndAllowsAnotherDailyRequest()
    {
        var requests = new List<FederativeLicenseRequest> { new(Guid.NewGuid(), Year, 1, "Completa", "A", "Adulto", "Nacional", "Cancelada") };
        var service = new RecordingFederativeLicenseService {
            GetMyFederativeLicensesHandler = _ => Task.FromResult(ServiceResult<IReadOnlyList<FederativeLicenseRequest>>.Success(requests.ToArray())),
            GetAvailableRatesHandler = (_, type, _) => Rates(Rate(type)),
            CreateFederativeLicenseRequestHandler = (request, _) => {
                var item = new FederativeLicenseRequest(Guid.NewGuid(), Year, 3, "Día", "A", "Adulto", "Nacional", "Pendiente", request.FechaInicio, request.FechaFin);
                requests.Add(item);
                return Task.FromResult(ServiceResult<FederativeLicenseRequest>.Success(item));
            } };
        var cut = Open(service);
        for (var i=0; i<2; i++)
        {
            SelectDaily(cut);
            cut.FindAll("input[type=date]")[0].Change($"{Year}-01-01");
            cut.FindAll("input[type=date]")[1].Change($"{Year}-01-01");
            Confirm(cut).Click();
            cut.WaitForAssertion(() => Assert.Equal(i+2, cut.FindAll(".mi-club-licenses__item").Count));
            if (i==0) cut.Find("button.mi-club-licenses__request-btn").Click();
        }
        Assert.Equal(2, cut.FindAll(".mi-club-licenses__item-coverage").Count);
        Assert.Single(cut.FindAll("details"));
    }

    /// <summary>Verifies old rate responses cannot overwrite new selection.</summary>
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task OldRateResponses_CannotOverwriteNewSelection(bool oldFailure)
    {
        var pending = new TaskCompletionSource<ServiceResult<IReadOnlyList<FederativeLicenseRate>>>();
        var cut = Open(new RecordingFederativeLicenseService {
            GetAvailableRatesHandler = (_, type, _) => type == 1 ? pending.Task : Rates(Rate(type) with { Licencia = "Actual" }) });
        var load = cut.FindAll("select")[0].ChangeAsync(new() { Value = Year.ToString() });
        await cut.FindAll("select")[1].ChangeAsync(new() { Value = "3" });
        pending.SetResult(oldFailure ? ServiceResult<IReadOnlyList<FederativeLicenseRate>>.Failure(new("error", "Old")) : ServiceResult<IReadOnlyList<FederativeLicenseRate>>.Success([Rate(1) with { Licencia = "Obsoleta" }]));
        await load;
        cut.WaitForAssertion(() => {
            Assert.Contains("Actual", cut.Markup);
            Assert.DoesNotContain("Obsoleta", cut.Markup);
            Assert.Empty(cut.FindAll(".loading"));
            Assert.DoesNotContain("mi_club_federative_licenses_request_error", cut.Markup);
        });
    }

    /// <summary>Verifies closed modal ignores old type response after reopen.</summary>
    [Fact]
    public async Task ClosedModal_IgnoresOldTypeResponse_AfterReopen()
    {
        var pending = new TaskCompletionSource<ServiceResult<IReadOnlyList<FederativeLicenseType>>>();
        var calls=0;
        var service = new RecordingFederativeLicenseService { GetLicenseTypesHandler = _ => ++calls == 1 ? pending.Task : Task.FromResult(ServiceResult<IReadOnlyList<FederativeLicenseType>>.Success([new(1,"Actual")])) };
        var cut = Open(service);
        cut.Find(".mi-club-licenses__modal-header button").Click();
        await cut.Find("button.mi-club-licenses__request-btn").ClickAsync(new());
        await cut.InvokeAsync(() => pending.SetResult(ServiceResult<IReadOnlyList<FederativeLicenseType>>.Success([new(1,"Obsoleto")])));
        cut.WaitForAssertion(() => {
            Assert.Contains("Actual", cut.Markup);
            Assert.DoesNotContain("Obsoleto", cut.Markup);
            Assert.Empty(cut.FindAll(".loading"));
        });
    }

    /// <summary>Verifies old rate response cannot clear loading of new request or affect reopened modal.</summary>
    [Fact]
    public async Task OldRateResponse_CannotClearLoadingOfNewRequest_OrAffectReopenedModal()
    {
        var old = new TaskCompletionSource<ServiceResult<IReadOnlyList<FederativeLicenseRate>>>();
        var current = new TaskCompletionSource<ServiceResult<IReadOnlyList<FederativeLicenseRate>>>();
        var cut = Open(new RecordingFederativeLicenseService { GetAvailableRatesHandler = (_,type,_) => type==1 ? old.Task : current.Task });
        var first = cut.FindAll("select")[0].ChangeAsync(new() { Value=Year.ToString() });
        var second = cut.FindAll("select")[1].ChangeAsync(new() { Value="3" });
        old.SetResult(ServiceResult<IReadOnlyList<FederativeLicenseRate>>.Failure(new("error","Old")));
        await first;
        cut.WaitForAssertion(() => Assert.Single(cut.FindAll(".modal .loading")));
        Assert.True(Confirm(cut).HasAttribute("disabled"));
        cut.Find(".mi-club-licenses__modal-header button").Click();
        await cut.Find("button.mi-club-licenses__request-btn").ClickAsync(new());
        current.SetResult(ServiceResult<IReadOnlyList<FederativeLicenseRate>>.Success([Rate(3) with { Licencia="Obsoleta" }]));
        await second;
        cut.WaitForAssertion(() => {
            Assert.Empty(cut.FindAll(".modal .loading"));
            Assert.DoesNotContain("Obsoleta",cut.Markup);
            Assert.True(Confirm(cut).HasAttribute("disabled"));
        });
    }

    /// <summary>Verifies submit disables confirmation until completion and cannot be opened again.</summary>
    [Fact]
    public async Task Submit_DisablesConfirmationUntilCompletion_AndCannotBeOpenedAgain()
    {
        var pending = new TaskCompletionSource<ServiceResult<FederativeLicenseRequest>>();
        var calls=0;
        var cut=Open(new RecordingFederativeLicenseService {
            GetAvailableRatesHandler=(_,type,_)=>Rates(Rate(type)),
            CreateFederativeLicenseRequestHandler=(_,_)=> { calls++; return pending.Task; } });
        SelectDaily(cut);
        cut.FindAll("input[type=date]")[0].Change($"{Year}-01-01");
        cut.FindAll("input[type=date]")[1].Change($"{Year}-01-01");
        var submitting=Confirm(cut).ClickAsync(new());
        Assert.True(Confirm(cut).HasAttribute("disabled"));
        await cut.Find("button.mi-club-licenses__request-btn").ClickAsync(new());
        Assert.Single(cut.FindAll(".modal"));
        Assert.Equal(1,calls);
        pending.SetResult(ServiceResult<FederativeLicenseRequest>.Failure(new("error","Failed")));
        await submitting;
        cut.WaitForAssertion(()=>Assert.False(Confirm(cut).HasAttribute("disabled")));
    }

    /// <summary>Verifies untrusted catalog and history names are rendered as text.</summary>
    [Fact]
    public void UntrustedCatalogAndHistoryNames_AreRenderedAsText()
    {
        const string untrusted="<script>alert('x')</script>";
        var cut=Open(new RecordingFederativeLicenseService {
            GetLicenseTypesHandler=_=>Task.FromResult(ServiceResult<IReadOnlyList<FederativeLicenseType>>.Success([new(1,untrusted)])),
            GetMyFederativeLicensesHandler=_=>Task.FromResult(ServiceResult<IReadOnlyList<FederativeLicenseRequest>>.Success([new(Guid.NewGuid(),Year,1,untrusted,untrusted,"Adulto","Nacional","Pendiente")])) });
        Assert.Empty(cut.FindAll("script"));
        Assert.Contains(untrusted,cut.FindAll("select")[1].TextContent);
        Assert.Contains(untrusted,cut.Find(".mi-club-licenses__item-season").TextContent);
    }

    /// <summary>Verifies failed or empty type catalog does not invent choices.</summary>
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void FailedOrEmptyTypeCatalog_DoesNotInventChoices(bool empty)
    {
        var cut=Open(new RecordingFederativeLicenseService {
            GetLicenseTypesHandler=_=>Task.FromResult(empty
                ? ServiceResult<IReadOnlyList<FederativeLicenseType>>.Success([])
                : ServiceResult<IReadOnlyList<FederativeLicenseType>>.Failure(new("error","internal secret"))) });
        Assert.Contains("licenses_types_error",cut.Markup);
        Assert.DoesNotContain("internal secret",cut.Markup);
        Assert.True(Confirm(cut).HasAttribute("disabled"));
        Assert.True(cut.FindAll("select")[1].HasAttribute("disabled"));
    }
}
