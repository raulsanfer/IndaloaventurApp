namespace IndaloaventurApp.SharedUI.Models.Inventory;

/// <summary>
/// Contains the values required to create an inventory category.
/// </summary>
/// <param name="Name">The category name.</param>
/// <param name="ParentCategoryId">The optional root category identifier used as parent.</param>
public sealed record CreateInventoryCategoryRequest(string Name, int? ParentCategoryId);
