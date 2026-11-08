using IndaloAventurApi.Application.Abstractions.Inventario.ReservasMaterial;
using IndaloAventurApi.Application.Abstractions.Inventario;
using IndaloAventurApi.Application.Features.Inventario.ReservasMaterial.Notifications;
using IndaloAventurApi.Domain.Inventario.ReservasMaterial;
using MediatR;

namespace IndaloAventurApi.Application.Features.Inventario.ReservasMaterial.RegistrarRetiradaReservaMaterial;

public sealed class RegistrarRetiradaReservaMaterialCommandHandler(
    IReservaMaterialRepository reservaRepository,
    IInventarioMaterialRepository materialRepository,
    ISocioReservaReader socioReader,
    IReservaMaterialNotificationService notificationService)
    : IRequestHandler<RegistrarRetiradaReservaMaterialCommand, bool>
{
    public async Task<bool> Handle(RegistrarRetiradaReservaMaterialCommand request, CancellationToken cancellationToken)
    {
        ReservaMaterialInventoryManagementAccess.RequireInventoryManagement(request.CanManageInventory);

        var retirada = await reservaRepository.ExecuteSerializableAsync(async transactionCancellationToken =>
        {
            var reserva = await reservaRepository.GetByIdForUpdateAsync(request.ReservaId, transactionCancellationToken)
                ?? throw new KeyNotFoundException("La reserva de material no existe.");

            reserva.RegistrarRetirada(request.FechaRetirada);
            var material = await materialRepository.GetByIdForUpdateAsync(reserva.InventarioMaterialId, transactionCancellationToken)
                ?? throw new KeyNotFoundException("El material de inventario asociado a la reserva no existe.");
            var socio = await socioReader.GetByUserIdAsync(reserva.UserId, transactionCancellationToken)
                ?? throw new KeyNotFoundException("El socio asociado a la reserva no existe.");
            await reservaRepository.SaveChangesAsync(transactionCancellationToken);
            return (Reserva: reserva, MaterialDescripcion: material.Descripcion, Socio: socio);
        }, cancellationToken);

        await notificationService.NotifyAsync(
            ReservaMaterialNotificationFactory.Create(
                TipoNotificacionReservaMaterial.CambioDeEstado,
                retirada.Reserva,
                retirada.MaterialDescripcion,
                retirada.Socio),
            cancellationToken);

        return true;
    }
}
