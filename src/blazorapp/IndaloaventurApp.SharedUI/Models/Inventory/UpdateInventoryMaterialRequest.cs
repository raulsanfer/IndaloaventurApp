namespace IndaloaventurApp.SharedUI.Models.Inventory;

/// <summary>
/// Contains the complete editable values required to update an inventory material.
/// </summary>
/// <param name="Id">The material identifier.</param>
/// <param name="Material">The material values to persist.</param>
public sealed record UpdateInventoryMaterialRequest(int Id, SaveInventoryMaterialRequest Material);
