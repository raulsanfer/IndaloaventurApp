using IndaloAventurApi.Application.Abstractions.Inventario;
using IndaloAventurApi.Application.Abstractions.Inventario.ReservasMaterial;
using IndaloAventurApi.Application.Common;
using IndaloAventurApi.Application.Features.Inventario.ReservasMaterial.Notifications;
using IndaloAventurApi.Domain.Inventario.ReservasMaterial;
using MediatR;

namespace IndaloAventurApi.Application.Features.Inventario.ReservasMaterial.CreateMiReservaMaterial;

public sealed class CreateMiReservaMaterialCommandHandler(
    IReservaMaterialRepository reservaRepository,
    IInventarioMaterialRepository materialRepository,
    ISocioReservaReader socioReader,
    IReservaMaterialNotificationService notificationService)
    : IRequestHandler<CreateMiReservaMaterialCommand, ReservaMaterialDto>
{
    public async Task<ReservaMaterialDto> Handle(CreateMiReservaMaterialCommand request, CancellationToken cancellationToken)
    {
        if (!request.IsMember)
        {
            throw new ForbiddenAccessException("Solo los socios del club pueden solicitar una reserva de material.");
        }

        var socio = await socioReader.GetByUserIdAsync(request.AuthenticatedUserId, cancellationToken);
        if (socio is null)
        {
            throw new ForbiddenAccessException("Para solicitar una reserva de material debes disponer de una ficha de socio.");
        }

        var created = await reservaRepository.ExecuteSerializableAsync(async transactionCancellationToken =>
        {
            var material = await materialRepository.GetByIdForUpdateAsync(request.InventarioMaterialId, transactionCancellationToken)
                ?? throw new KeyNotFoundException("El material de inventario no existe.");

            if (!material.Activo)
            {
                throw new InvalidOperationException("El material de inventario no esta activo para nuevas reservas.");
            }

            var cantidadComprometida = await reservaRepository.GetCantidadComprometidaAsync(
                material.Id,
                excludingReservaId: null,
                transactionCancellationToken);
            if (request.CantidadSolicitada > material.Cantidad - cantidadComprometida)
            {
                throw new InvalidOperationException("No hay unidades disponibles suficientes para esta reserva.");
            }

            var reserva = ReservaMaterial.Crear(
                request.AuthenticatedUserId,
                material.Id,
                request.CantidadSolicitada,
                request.FechaRetiradaPrevista,
                request.FechaDevolucionEstimada,
                request.Observaciones);

            await reservaRepository.AddAsync(reserva, transactionCancellationToken);
            await reservaRepository.SaveChangesAsync(transactionCancellationToken);

            return (Reserva: reserva, MaterialDescripcion: material.Descripcion);
        }, cancellationToken);

        await notificationService.NotifyAsync(
            ReservaMaterialNotificationFactory.Create(
                TipoNotificacionReservaMaterial.Solicitud,
                created.Reserva,
                created.MaterialDescripcion,
                socio),
            cancellationToken);

        return ReservaMaterialMapping.ToDto(created.Reserva, created.MaterialDescripcion, socio);
    }
}
