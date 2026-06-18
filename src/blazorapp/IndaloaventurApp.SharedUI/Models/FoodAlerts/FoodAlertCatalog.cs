namespace IndaloaventurApp.SharedUI.Models.FoodAlerts;

public static class FoodAlertCatalog
{
    private static readonly IReadOnlyList<FoodAlertCategoryItem> Categories =
    [
        new("general", "food_alerts_category_general_title", "food_alerts_category_general_subtitle"),
        new("complementos", "food_alerts_category_supplements_title", "food_alerts_category_supplements_subtitle"),
        new("alergenos", "food_alerts_category_allergies_title", "food_alerts_category_allergies_subtitle")
    ];

    public static IReadOnlyList<FoodAlertCategoryItem> All => Categories;

    public static bool IsSupportedCode(string? categoryCode)
        => TryGetByCode(categoryCode, out _);

    public static bool TryGetByCode(string? categoryCode, out FoodAlertCategoryItem? category)
    {
        category = Categories.FirstOrDefault(item =>
            string.Equals(item.Code, categoryCode, StringComparison.OrdinalIgnoreCase));

        return category is not null;
    }
}
