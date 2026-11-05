namespace IndaloaventurApp.SharedUI.Models.Inventory;

/// <summary>
/// Defines the server-authoritative lifecycle states of a material reservation.
/// </summary>
public enum MaterialReservationStatus
{
    /// <summary>The member has submitted the reservation and it awaits administration.</summary>
    Requested = 1,

    /// <summary>The material has been handed over to the member.</summary>
    PickedUp = 2,

    /// <summary>The return has been confirmed by both the member and administration.</summary>
    Returned = 3,

    /// <summary>The reservation has been cancelled.</summary>
    Cancelled = 4
}
