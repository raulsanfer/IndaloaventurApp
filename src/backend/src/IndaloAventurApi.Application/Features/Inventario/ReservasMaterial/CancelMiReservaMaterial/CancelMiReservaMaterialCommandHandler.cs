using IndaloAventurApi.Application.Abstractions.Inventario.ReservasMaterial;
using IndaloAventurApi.Application.Abstractions.Inventario;
using IndaloAventurApi.Application.Features.Inventario.ReservasMaterial.Notifications;
using IndaloAventurApi.Domain.Inventario.ReservasMaterial;
using MediatR;

namespace IndaloAventurApi.Application.Features.Inventario.ReservasMaterial.CancelMiReservaMaterial;

public sealed class CancelMiReservaMaterialCommandHandler(
    IReservaMaterialRepository reservaRepository,
    ISocioReservaReader socioReader,
    IInventarioMaterialRepository materialRepository,
    IReservaMaterialNotificationService notificationService)
    : IRequestHandler<CancelMiReservaMaterialCommand, bool>
{
    public async Task<bool> Handle(CancelMiReservaMaterialCommand request, CancellationToken cancellationToken)
    {
        var socio = await ReservaMaterialMemberAccess.RequireSocioAsync(
            request.AuthenticatedUserId,
            request.IsMember,
            socioReader,
            "cancelar",
            cancellationToken);

        var cancelled = await reservaRepository.ExecuteSerializableAsync(async transactionCancellationToken =>
        {
            var reserva = await reservaRepository.GetByIdForUpdateAsync(request.ReservaId, transactionCancellationToken)
                ?? throw new KeyNotFoundException("La reserva de material no existe.");
            ReservaMaterialMemberAccess.RequireOwner(reserva.UserId, request.AuthenticatedUserId);

            reserva.CancelarPorSocio();
            var material = await materialRepository.GetByIdForUpdateAsync(reserva.InventarioMaterialId, transactionCancellationToken)
                ?? throw new KeyNotFoundException("El material de inventario asociado a la reserva no existe.");
            await reservaRepository.SaveChangesAsync(transactionCancellationToken);
            return (Reserva: reserva, MaterialDescripcion: material.Descripcion);
        }, cancellationToken);

        await notificationService.NotifyAsync(
            ReservaMaterialNotificationFactory.Create(
                TipoNotificacionReservaMaterial.CambioDeEstado,
                cancelled.Reserva,
                cancelled.MaterialDescripcion,
                socio),
            cancellationToken);

        return true;
    }
}
