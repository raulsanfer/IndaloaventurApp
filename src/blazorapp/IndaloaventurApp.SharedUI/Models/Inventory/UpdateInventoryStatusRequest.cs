namespace IndaloaventurApp.SharedUI.Models.Inventory;

/// <summary>
/// Contains the values required to update an inventory status.
/// </summary>
/// <param name="Id">The status identifier.</param>
/// <param name="Name">The status name.</param>
public sealed record UpdateInventoryStatusRequest(int Id, string Name);
