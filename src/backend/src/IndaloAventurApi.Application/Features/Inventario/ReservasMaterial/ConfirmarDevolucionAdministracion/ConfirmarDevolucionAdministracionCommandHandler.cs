using IndaloAventurApi.Application.Abstractions.Inventario.ReservasMaterial;
using IndaloAventurApi.Application.Abstractions.Inventario;
using IndaloAventurApi.Application.Features.Inventario.ReservasMaterial.Notifications;
using IndaloAventurApi.Domain.Inventario.ReservasMaterial;
using MediatR;

namespace IndaloAventurApi.Application.Features.Inventario.ReservasMaterial.ConfirmarDevolucionAdministracion;

public sealed class ConfirmarDevolucionAdministracionCommandHandler(
    IReservaMaterialRepository reservaRepository,
    IInventarioMaterialRepository materialRepository,
    ISocioReservaReader socioReader,
    IReservaMaterialNotificationService notificationService)
    : IRequestHandler<ConfirmarDevolucionAdministracionCommand, bool>
{
    public async Task<bool> Handle(ConfirmarDevolucionAdministracionCommand request, CancellationToken cancellationToken)
    {
        ReservaMaterialAdminAccess.RequireAdmin(request.IsAdmin);

        var confirmation = await reservaRepository.ExecuteSerializableAsync(async transactionCancellationToken =>
        {
            var reserva = await reservaRepository.GetByIdForUpdateAsync(request.ReservaId, transactionCancellationToken)
                ?? throw new KeyNotFoundException("La reserva de material no existe.");

            reserva.ConfirmarDevolucionAdministracion(request.FechaDevolucionEfectiva);
            var material = await materialRepository.GetByIdForUpdateAsync(reserva.InventarioMaterialId, transactionCancellationToken)
                ?? throw new KeyNotFoundException("El material de inventario asociado a la reserva no existe.");
            var socio = await socioReader.GetByUserIdAsync(reserva.UserId, transactionCancellationToken)
                ?? throw new KeyNotFoundException("El socio asociado a la reserva no existe.");
            await reservaRepository.SaveChangesAsync(transactionCancellationToken);
            return (Reserva: reserva, MaterialDescripcion: material.Descripcion, Socio: socio);
        }, cancellationToken);

        await notificationService.NotifyAsync(
            ReservaMaterialNotificationFactory.Create(
                TipoNotificacionReservaMaterial.ConfirmacionDevolucionAdministracion,
                confirmation.Reserva,
                confirmation.MaterialDescripcion,
                confirmation.Socio),
            cancellationToken);

        return true;
    }
}
