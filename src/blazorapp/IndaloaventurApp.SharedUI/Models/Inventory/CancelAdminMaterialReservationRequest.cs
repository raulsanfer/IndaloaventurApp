namespace IndaloaventurApp.SharedUI.Models.Inventory;

/// <summary>
/// Supplies the administrator observation for a material reservation cancellation.
/// </summary>
/// <param name="AdministrationNote">Optional or required cancellation observation according to the reservation state.</param>
public sealed record CancelAdminMaterialReservationRequest(string? AdministrationNote);
