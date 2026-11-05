namespace IndaloaventurApp.SharedUI.Models.Inventory;

/// <summary>
/// Identifies the member associated with an administrator-visible material reservation.
/// </summary>
/// <param name="UserId">Unique identifier of the member user.</param>
/// <param name="FullName">Full name supplied by the reservation API.</param>
/// <param name="Email">Email address supplied by the reservation API.</param>
public sealed record MaterialReservationMember(Guid UserId, string FullName, string Email);
