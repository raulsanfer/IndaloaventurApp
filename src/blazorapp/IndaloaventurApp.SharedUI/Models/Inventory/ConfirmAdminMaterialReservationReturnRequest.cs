namespace IndaloaventurApp.SharedUI.Models.Inventory;

/// <summary>
/// Supplies the effective return date for an administrator material reservation decision.
/// </summary>
/// <param name="ActualReturnDate">Date on which the material was returned to the club.</param>
public sealed record ConfirmAdminMaterialReservationReturnRequest(DateOnly ActualReturnDate);
