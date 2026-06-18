namespace IndaloaventurApp.SharedUI.Components.FoodAlerts;

using System.Globalization;
using IndaloaventurApp.SharedUI.Abstractions.FoodAlerts;
using IndaloaventurApp.SharedUI.Models.FoodAlerts;
using IndaloaventurApp.SharedUI.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

public partial class FoodAlertCategoryView
{
    [Inject]
    private IFoodAlertService FoodAlertService { get; set; } = default!;

    [Inject]
    private IStringLocalizer<SharedTexts> L { get; set; } = default!;

    [Parameter]
    public string CategoryCode { get; set; } = string.Empty;

    protected IReadOnlyList<FoodAlertListItem> Alerts { get; private set; } = Array.Empty<FoodAlertListItem>();

    protected bool IsLoading { get; private set; } = true;

    protected string? ErrorMessageKey { get; private set; }

    protected string CategoryTitle { get; private set; } = string.Empty;

    protected string CategorySubtitle { get; private set; } = string.Empty;

    protected override async Task OnParametersSetAsync()
    {
        IsLoading = true;
        ErrorMessageKey = null;
        Alerts = Array.Empty<FoodAlertListItem>();

        if (!FoodAlertCatalog.TryGetByCode(CategoryCode, out var category))
        {
            CategoryTitle = L["food_alerts_home_title"];
            CategorySubtitle = L["food_alerts_invalid_category"];
            ErrorMessageKey = "food_alerts_invalid_category";
            IsLoading = false;
            return;
        }

        CategoryTitle = L[category!.TitleResourceKey];
        CategorySubtitle = L[category.SubtitleResourceKey];

        var result = await FoodAlertService.GetAlertsAsync(category.Code);
        IsLoading = false;

        if (result.IsSuccess)
        {
            Alerts = result.Value ?? Array.Empty<FoodAlertListItem>();
            return;
        }

        ErrorMessageKey = MapErrorKey(result.Error?.Code);
    }

    protected string GetAlertHref(FoodAlertListItem alert)
        => $"/alertas-alimentarias/alerta/{Uri.EscapeDataString(alert.Id)}?category={Uri.EscapeDataString(CategoryCode)}";

    protected static string FormatDate(DateTime publishedAt)
        => publishedAt.ToLocalTime().ToString("d 'de' MMMM 'de' yyyy", CultureInfo.GetCultureInfo("es-ES"));

    private static string MapErrorKey(string? errorCode)
        => errorCode switch
        {
            "food_alerts.timeout" => "food_alerts_timeout",
            "food_alerts.invalid_category" => "food_alerts_invalid_category",
            _ => "food_alerts_error"
        };
}
