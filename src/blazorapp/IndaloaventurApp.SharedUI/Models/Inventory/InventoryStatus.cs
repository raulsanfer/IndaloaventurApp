namespace IndaloaventurApp.SharedUI.Models.Inventory;

/// <summary>
/// Represents a material condition in the inventory catalog.
/// </summary>
/// <param name="Id">The status identifier.</param>
/// <param name="Name">The display name.</param>
public sealed record InventoryStatus(int Id, string Name);
