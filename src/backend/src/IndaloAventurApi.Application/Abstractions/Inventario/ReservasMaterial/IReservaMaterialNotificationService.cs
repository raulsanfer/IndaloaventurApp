namespace IndaloAventurApi.Application.Abstractions.Inventario.ReservasMaterial;

public interface IReservaMaterialNotificationService
{
    Task NotifyAsync(ReservaMaterialNotification notification, CancellationToken cancellationToken);
}
