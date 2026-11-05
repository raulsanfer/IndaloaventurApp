namespace IndaloaventurApp.SharedUI.Abstractions.Inventory;

using IndaloaventurApp.SharedUI.Models.Common;
using IndaloaventurApp.SharedUI.Models.Inventory;

/// <summary>
/// Provides the authenticated member operations for the member's own material reservations.
/// </summary>
public interface IMemberMaterialReservationService
{
    /// <summary>Gets reservations owned by the authenticated member.</summary>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    /// <returns>The current member reservation list.</returns>
    Task<ServiceResult<IReadOnlyList<MaterialReservation>>> GetMyReservationsAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets one reservation owned by the authenticated member.</summary>
    /// <param name="reservationId">Identifier of the member's reservation.</param>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    /// <returns>The requested reservation when it is available to the current member.</returns>
    Task<ServiceResult<MaterialReservation>> GetMyReservationAsync(Guid reservationId, CancellationToken cancellationToken = default);

    /// <summary>Gets inventory materials that may be selected in a new member request.</summary>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    /// <returns>Active inventory materials suitable for the selector.</returns>
    Task<ServiceResult<IReadOnlyList<ReservableInventoryMaterial>>> GetReservableMaterialsAsync(CancellationToken cancellationToken = default);

    /// <summary>Creates a material reservation for the authenticated member.</summary>
    /// <param name="request">Values for the member's new material request.</param>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    /// <returns>The server-confirmed reservation.</returns>
    Task<ServiceResult<MaterialReservation>> CreateReservationAsync(
        CreateMemberMaterialReservationRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Cancels a requested reservation owned by the authenticated member.</summary>
    /// <param name="reservationId">Identifier of the reservation to cancel.</param>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    /// <returns>A result that succeeds only after the server accepts the cancellation.</returns>
    Task<ServiceResult<bool>> CancelReservationAsync(Guid reservationId, CancellationToken cancellationToken = default);

    /// <summary>Confirms that the authenticated member returned picked-up material.</summary>
    /// <param name="reservationId">Identifier of the reservation whose return is confirmed.</param>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    /// <returns>A result that succeeds only after the server accepts the confirmation.</returns>
    Task<ServiceResult<bool>> ConfirmReturnAsync(Guid reservationId, CancellationToken cancellationToken = default);
}
