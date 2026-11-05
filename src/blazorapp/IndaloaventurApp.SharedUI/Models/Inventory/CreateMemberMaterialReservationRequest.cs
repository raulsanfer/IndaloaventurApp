namespace IndaloaventurApp.SharedUI.Models.Inventory;

/// <summary>
/// Contains the values a member submits to request mountain material.
/// </summary>
/// <param name="InventoryMaterialId">Identifier of the selected active material.</param>
/// <param name="RequestedQuantity">Positive number of requested units.</param>
/// <param name="ScheduledPickupDate">Date planned for collecting the material.</param>
/// <param name="EstimatedReturnDate">Date planned for returning the material.</param>
/// <param name="MemberNotes">Optional notes supplied by the member.</param>
public sealed record CreateMemberMaterialReservationRequest(
    int InventoryMaterialId,
    int RequestedQuantity,
    DateOnly ScheduledPickupDate,
    DateOnly EstimatedReturnDate,
    string? MemberNotes);
