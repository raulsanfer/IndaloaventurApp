namespace IndaloaventurApp.Frontend.Tests;

using Bunit;
using IndaloaventurApp.SharedUI.Abstractions.FoodAlerts;
using IndaloaventurApp.SharedUI.Components.FoodAlerts;
using IndaloaventurApp.SharedUI.Models.Common;
using IndaloaventurApp.SharedUI.Models.FoodAlerts;
using IndaloaventurApp.SharedUI.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

public sealed class FoodAlertViewsTests : BunitContext
{
    [Fact]
    public void FoodAlertsHomeView_RendersThreeCategoryCards()
    {
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();

        var cut = Render<FoodAlertsHomeView>();

        Assert.Contains("/alertas-alimentarias/categoria/general", cut.Markup);
        Assert.Contains("/alertas-alimentarias/categoria/complementos", cut.Markup);
        Assert.Contains("/alertas-alimentarias/categoria/alergenos", cut.Markup);
    }

    [Fact]
    public void FoodAlertCategoryView_ShowsEmptyState_WhenCategoryHasNoAlerts()
    {
        var service = new RecordingFoodAlertService
        {
            GetAlertsHandler = (_, _) => Task.FromResult(ServiceResult<IReadOnlyList<FoodAlertListItem>>.Success(Array.Empty<FoodAlertListItem>()))
        };

        Services.AddSingleton<IFoodAlertService>(service);
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();

        var cut = Render<FoodAlertCategoryView>(parameters => parameters.Add(x => x.CategoryCode, "general"));

        cut.WaitForAssertion(() => Assert.Contains("food_alerts_empty", cut.Markup));
    }

    [Fact]
    public void FoodAlertCategoryView_ShowsAlertLinkAndSummary()
    {
        var service = new RecordingFoodAlertService
        {
            GetAlertsHandler = (_, _) => Task.FromResult(ServiceResult<IReadOnlyList<FoodAlertListItem>>.Success(
                new[]
                {
                    new FoodAlertListItem(
                        "alert-1",
                        "general",
                        "Titulo alerta",
                        "Resumen de la alerta.",
                        new DateTime(2026, 6, 13, 10, 0, 0, DateTimeKind.Utc),
                        "https://example.test/1")
                }))
        };

        Services.AddSingleton<IFoodAlertService>(service);
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();

        var cut = Render<FoodAlertCategoryView>(parameters => parameters.Add(x => x.CategoryCode, "general"));

        cut.WaitForAssertion(() =>
        {
            Assert.Contains("/alertas-alimentarias/alerta/alert-1?category=general", cut.Markup);
            Assert.Contains("Titulo alerta", cut.Markup);
            Assert.Contains("Resumen de la alerta.", cut.Markup);
        });
    }

    [Fact]
    public void FoodAlertDetailView_ShowsBackLinkAndNormalizedDescription()
    {
        var service = new RecordingFoodAlertService
        {
            GetAlertHandler = (_, _) => Task.FromResult(ServiceResult<FoodAlertDetailItem>.Success(
                new FoodAlertDetailItem(
                    "alert-9",
                    null,
                    "Detalle alerta",
                    "Primer bloque." + Environment.NewLine + Environment.NewLine + "Segundo bloque.",
                    new DateTime(2026, 6, 13, 10, 0, 0, DateTimeKind.Utc),
                    "https://example.test/9")))
        };

        Services.AddSingleton<IFoodAlertService>(service);
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();

        var cut = Render<FoodAlertDetailView>(parameters => parameters
            .Add(x => x.AlertId, "alert-9")
            .Add(x => x.CategoryCode, "general"));

        cut.WaitForAssertion(() =>
        {
            Assert.Contains("/alertas-alimentarias/categoria/general", cut.Markup);
            Assert.Contains("Detalle alerta", cut.Markup);
            Assert.Contains("Primer bloque.", cut.Markup);
            Assert.Contains("Segundo bloque.", cut.Markup);
        });
    }

    [Fact]
    public void FoodAlertDetailView_ShowsErrorState_WhenServiceFails()
    {
        var service = new RecordingFoodAlertService
        {
            GetAlertHandler = (_, _) => Task.FromResult(ServiceResult<FoodAlertDetailItem>.Failure(
                new ServiceError("food_alerts.not_found", "No encontrada.")))
        };

        Services.AddSingleton<IFoodAlertService>(service);
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();

        var cut = Render<FoodAlertDetailView>(parameters => parameters.Add(x => x.AlertId, "missing"));

        cut.WaitForAssertion(() => Assert.Contains("food_alerts_detail_not_found", cut.Markup));
    }
}
