namespace IndaloaventurApp.SharedUI.Models.Inventory;

/// <summary>
/// Supplies the effective pickup date for an administrator material reservation decision.
/// </summary>
/// <param name="PickupDate">Date on which the material was handed to the member.</param>
public sealed record RegisterAdminMaterialReservationPickupRequest(DateOnly PickupDate);
