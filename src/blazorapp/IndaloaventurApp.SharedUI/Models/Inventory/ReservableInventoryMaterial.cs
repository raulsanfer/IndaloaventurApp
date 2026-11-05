namespace IndaloaventurApp.SharedUI.Models.Inventory;

/// <summary>
/// Represents the minimum inventory data a member can use to request material.
/// </summary>
/// <param name="Id">Identifier of the inventory material.</param>
/// <param name="Description">Display description of the material.</param>
/// <param name="IsActive">Whether the material accepts new reservation requests.</param>
/// <param name="AvailableQuantity">Current availability snapshot calculated by the server.</param>
public sealed record ReservableInventoryMaterial(int Id, string Description, bool IsActive, int AvailableQuantity);
