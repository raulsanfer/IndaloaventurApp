namespace IndaloaventurApp.SharedUI.Models.Inventory;

/// <summary>
/// Contains the values required to update an inventory category.
/// </summary>
/// <param name="Id">The category identifier.</param>
/// <param name="Name">The category name.</param>
/// <param name="ParentCategoryId">The optional root category identifier used as parent.</param>
public sealed record UpdateInventoryCategoryRequest(int Id, string Name, int? ParentCategoryId);
