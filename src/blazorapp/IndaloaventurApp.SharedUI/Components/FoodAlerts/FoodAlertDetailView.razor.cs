namespace IndaloaventurApp.SharedUI.Components.FoodAlerts;

using System.Globalization;
using IndaloaventurApp.SharedUI.Abstractions.FoodAlerts;
using IndaloaventurApp.SharedUI.Models.FoodAlerts;
using IndaloaventurApp.SharedUI.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

public partial class FoodAlertDetailView
{
    [Inject]
    private IFoodAlertService FoodAlertService { get; set; } = default!;

    [Inject]
    private IStringLocalizer<SharedTexts> L { get; set; } = default!;

    [Parameter]
    public string AlertId { get; set; } = string.Empty;

    [Parameter]
    public string? CategoryCode { get; set; }

    protected FoodAlertDetailItem? Alert { get; private set; }

    protected bool IsLoading { get; private set; } = true;

    protected string? ErrorMessageKey { get; private set; }

    protected string CategoryTitle { get; private set; } = string.Empty;

    protected string BackHref { get; private set; } = "/alertas-alimentarias";

    protected string BackText { get; private set; } = string.Empty;

    protected IReadOnlyList<string> DescriptionParagraphs =>
        string.IsNullOrWhiteSpace(Alert?.Description)
            ? Array.Empty<string>()
            : Alert.Description
                .Split(Environment.NewLine + Environment.NewLine, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    protected override async Task OnParametersSetAsync()
    {
        IsLoading = true;
        ErrorMessageKey = null;
        Alert = null;
        ResolveCategoryContext();

        var result = await FoodAlertService.GetAlertAsync(AlertId);
        IsLoading = false;

        if (result.IsSuccess)
        {
            Alert = result.Value;
            return;
        }

        ErrorMessageKey = result.Error?.Code switch
        {
            "food_alerts.not_found" => "food_alerts_detail_not_found",
            "food_alerts.timeout" => "food_alerts_timeout",
            _ => "food_alerts_detail_error"
        };
    }

    protected static string FormatDate(DateTime publishedAt)
        => publishedAt.ToLocalTime().ToString("d 'de' MMMM 'de' yyyy", CultureInfo.GetCultureInfo("es-ES"));

    private void ResolveCategoryContext()
    {
        if (FoodAlertCatalog.TryGetByCode(CategoryCode, out var category))
        {
            CategoryTitle = L[category!.TitleResourceKey];
            BackHref = $"/alertas-alimentarias/categoria/{Uri.EscapeDataString(category.Code)}";
            BackText = L["food_alerts_back_category"];
            return;
        }

        CategoryTitle = string.Empty;
        BackHref = "/alertas-alimentarias";
        BackText = L["food_alerts_back_home"];
    }
}
