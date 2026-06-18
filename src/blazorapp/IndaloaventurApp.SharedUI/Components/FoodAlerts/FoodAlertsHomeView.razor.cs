namespace IndaloaventurApp.SharedUI.Components.FoodAlerts;

using IndaloaventurApp.SharedUI.Models.FoodAlerts;
using IndaloaventurApp.SharedUI.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

public partial class FoodAlertsHomeView
{
    [Inject]
    private IStringLocalizer<SharedTexts> L { get; set; } = default!;

    protected IReadOnlyList<FoodAlertCategoryItem> Categories { get; } = FoodAlertCatalog.All;

    protected static string GetCategoryHref(string categoryCode)
        => $"/alertas-alimentarias/categoria/{Uri.EscapeDataString(categoryCode)}";
}
