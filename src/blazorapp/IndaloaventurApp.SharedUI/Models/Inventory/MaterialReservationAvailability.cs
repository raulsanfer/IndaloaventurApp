namespace IndaloaventurApp.SharedUI.Models.Inventory;

/// <summary>
/// Describes the server-calculated availability of material associated with a reservation.
/// </summary>
/// <param name="InventoryMaterialId">Identifier of the inventory material.</param>
/// <param name="TotalQuantity">Total number of material units.</param>
/// <param name="CommittedQuantity">Units committed by active reservations.</param>
/// <param name="AvailableQuantity">Units currently available according to the server.</param>
/// <param name="IsActive">Whether the material is currently active for reservations.</param>
public sealed record MaterialReservationAvailability(
    int InventoryMaterialId,
    int TotalQuantity,
    int CommittedQuantity,
    int AvailableQuantity,
    bool IsActive);
