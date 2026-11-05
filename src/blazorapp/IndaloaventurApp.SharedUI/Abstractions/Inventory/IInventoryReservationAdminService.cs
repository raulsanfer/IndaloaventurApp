namespace IndaloaventurApp.SharedUI.Abstractions.Inventory;

using IndaloaventurApp.SharedUI.Models.Common;
using IndaloaventurApp.SharedUI.Models.Inventory;

/// <summary>
/// Provides administrator-only operations for material reservations.
/// </summary>
public interface IInventoryReservationAdminService
{
    /// <summary>Gets all material reservations visible to administrators.</summary>
    Task<ServiceResult<IReadOnlyList<MaterialReservation>>> GetReservationsAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets an administrator-visible reservation by identifier.</summary>
    /// <param name="reservationId">Reservation identifier.</param>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    Task<ServiceResult<MaterialReservation>> GetReservationAsync(Guid reservationId, CancellationToken cancellationToken = default);

    /// <summary>Registers that material has been handed to the member.</summary>
    /// <param name="reservationId">Reservation identifier.</param>
    /// <param name="request">Effective pickup date.</param>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    Task<ServiceResult<bool>> RegisterPickupAsync(
        Guid reservationId,
        RegisterAdminMaterialReservationPickupRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Cancels a material reservation as an administrator.</summary>
    /// <param name="reservationId">Reservation identifier.</param>
    /// <param name="request">Administration cancellation observation.</param>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    Task<ServiceResult<bool>> CancelReservationAsync(
        Guid reservationId,
        CancelAdminMaterialReservationRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Confirms the material return as an administrator.</summary>
    /// <param name="reservationId">Reservation identifier.</param>
    /// <param name="request">Effective material return date.</param>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    Task<ServiceResult<bool>> ConfirmReturnAsync(
        Guid reservationId,
        ConfirmAdminMaterialReservationReturnRequest request,
        CancellationToken cancellationToken = default);
}
