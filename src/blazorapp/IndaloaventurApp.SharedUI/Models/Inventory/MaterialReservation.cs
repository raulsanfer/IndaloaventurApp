namespace IndaloaventurApp.SharedUI.Models.Inventory;

/// <summary>
/// Represents a material reservation returned by the administrator reservation API.
/// </summary>
/// <param name="Id">Unique reservation identifier.</param>
/// <param name="UserId">Identifier of the member that owns the reservation.</param>
/// <param name="InventoryMaterialId">Identifier of the reserved inventory material.</param>
/// <param name="MaterialDescription">Description of the reserved material.</param>
/// <param name="RequestedQuantity">Number of requested material units.</param>
/// <param name="RequestedAtUtc">UTC date and time when the member submitted the request.</param>
/// <param name="ScheduledPickupDate">Date planned for the material pickup.</param>
/// <param name="PickupDate">Effective pickup date when it has been registered.</param>
/// <param name="EstimatedReturnDate">Date planned for the material return.</param>
/// <param name="ActualReturnDate">Effective return date when it has been registered.</param>
/// <param name="MemberNotes">Notes supplied by the member.</param>
/// <param name="AdministrationNote">Notes supplied by administration.</param>
/// <param name="Status">Server-authoritative lifecycle state.</param>
/// <param name="MemberReturnConfirmed">Whether the member has confirmed the return.</param>
/// <param name="AdministrationReturnConfirmed">Whether administration has confirmed the return.</param>
/// <param name="Member">Member identity returned for administrator views.</param>
/// <param name="Availability">Server-calculated availability information for the material.</param>
public sealed record MaterialReservation(
    Guid Id,
    Guid UserId,
    int InventoryMaterialId,
    string MaterialDescription,
    int RequestedQuantity,
    DateTime RequestedAtUtc,
    DateOnly ScheduledPickupDate,
    DateOnly? PickupDate,
    DateOnly EstimatedReturnDate,
    DateOnly? ActualReturnDate,
    string? MemberNotes,
    string? AdministrationNote,
    MaterialReservationStatus Status,
    bool MemberReturnConfirmed,
    bool AdministrationReturnConfirmed,
    MaterialReservationMember? Member,
    MaterialReservationAvailability? Availability);
