namespace IndaloaventurApp.SharedUI.Models.Inventory;

/// <summary>
/// Represents an inventory category and its immediate subcategories.
/// </summary>
/// <param name="Id">The category identifier.</param>
/// <param name="Name">The display name.</param>
/// <param name="ParentCategoryId">The parent category identifier, when the category is a subcategory.</param>
/// <param name="Subcategories">The immediate child categories.</param>
public sealed record InventoryCategory(
    int Id,
    string Name,
    int? ParentCategoryId,
    IReadOnlyList<InventoryCategory> Subcategories);
