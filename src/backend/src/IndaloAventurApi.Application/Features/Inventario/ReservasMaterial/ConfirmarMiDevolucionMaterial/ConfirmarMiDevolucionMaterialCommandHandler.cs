using IndaloAventurApi.Application.Abstractions.Inventario.ReservasMaterial;
using IndaloAventurApi.Application.Abstractions.Inventario;
using IndaloAventurApi.Application.Features.Inventario.ReservasMaterial.Notifications;
using IndaloAventurApi.Domain.Inventario.ReservasMaterial;
using MediatR;

namespace IndaloAventurApi.Application.Features.Inventario.ReservasMaterial.ConfirmarMiDevolucionMaterial;

public sealed class ConfirmarMiDevolucionMaterialCommandHandler(
    IReservaMaterialRepository reservaRepository,
    ISocioReservaReader socioReader,
    IInventarioMaterialRepository materialRepository,
    IReservaMaterialNotificationService notificationService)
    : IRequestHandler<ConfirmarMiDevolucionMaterialCommand, bool>
{
    public async Task<bool> Handle(ConfirmarMiDevolucionMaterialCommand request, CancellationToken cancellationToken)
    {
        var socio = await ReservaMaterialMemberAccess.RequireSocioAsync(
            request.AuthenticatedUserId,
            request.IsMember,
            socioReader,
            "confirmar la devolucion de",
            cancellationToken);

        var confirmed = await reservaRepository.ExecuteSerializableAsync(async transactionCancellationToken =>
        {
            var reserva = await reservaRepository.GetByIdForUpdateAsync(request.ReservaId, transactionCancellationToken)
                ?? throw new KeyNotFoundException("La reserva de material no existe.");
            ReservaMaterialMemberAccess.RequireOwner(reserva.UserId, request.AuthenticatedUserId);

            reserva.ConfirmarDevolucionSocio();
            var material = await materialRepository.GetByIdForUpdateAsync(reserva.InventarioMaterialId, transactionCancellationToken)
                ?? throw new KeyNotFoundException("El material de inventario asociado a la reserva no existe.");
            await reservaRepository.SaveChangesAsync(transactionCancellationToken);
            return (Reserva: reserva, MaterialDescripcion: material.Descripcion);
        }, cancellationToken);

        await notificationService.NotifyAsync(
            ReservaMaterialNotificationFactory.Create(
                TipoNotificacionReservaMaterial.ConfirmacionDevolucionSocio,
                confirmed.Reserva,
                confirmed.MaterialDescripcion,
                socio),
            cancellationToken);

        return true;
    }
}
