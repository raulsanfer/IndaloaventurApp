using IndaloAventurApi.Application.Abstractions.Inventario;
using IndaloAventurApi.Application.Abstractions.Inventario.ReservasMaterial;
using IndaloAventurApi.Application.Common;
using MediatR;

namespace IndaloAventurApi.Application.Features.Inventario.ReservasMaterial.UpdateMiReservaMaterial;

public sealed class UpdateMiReservaMaterialCommandHandler(
    IReservaMaterialRepository reservaRepository,
    IInventarioMaterialRepository materialRepository,
    ISocioReservaReader socioReader)
    : IRequestHandler<UpdateMiReservaMaterialCommand, ReservaMaterialDto>
{
    public async Task<ReservaMaterialDto> Handle(UpdateMiReservaMaterialCommand request, CancellationToken cancellationToken)
    {
        if (!request.IsMember)
        {
            throw new ForbiddenAccessException("Solo los socios del club pueden modificar una reserva de material.");
        }

        var socio = await socioReader.GetByUserIdAsync(request.AuthenticatedUserId, cancellationToken);
        if (socio is null)
        {
            throw new ForbiddenAccessException("Para modificar una reserva de material debes disponer de una ficha de socio.");
        }

        return await reservaRepository.ExecuteSerializableAsync(async transactionCancellationToken =>
        {
            var reserva = await reservaRepository.GetByIdForUpdateAsync(request.ReservaId, transactionCancellationToken)
                ?? throw new KeyNotFoundException("La reserva de material no existe.");
            if (reserva.UserId != request.AuthenticatedUserId)
            {
                throw new KeyNotFoundException("La reserva de material no existe.");
            }

            var material = await materialRepository.GetByIdForUpdateAsync(request.InventarioMaterialId, transactionCancellationToken)
                ?? throw new KeyNotFoundException("El material de inventario no existe.");
            if (!material.Activo)
            {
                throw new InvalidOperationException("El material de inventario no esta activo para nuevas reservas.");
            }

            var cantidadComprometida = await reservaRepository.GetCantidadComprometidaAsync(
                material.Id,
                reserva.Id,
                transactionCancellationToken);
            if (request.CantidadSolicitada > material.Cantidad - cantidadComprometida)
            {
                throw new InvalidOperationException("No hay unidades disponibles suficientes para esta reserva.");
            }

            reserva.ActualizarSolicitud(
                material.Id,
                request.CantidadSolicitada,
                request.FechaRetiradaPrevista,
                request.FechaDevolucionEstimada,
                request.Observaciones);
            await reservaRepository.SaveChangesAsync(transactionCancellationToken);

            return ReservaMaterialMapping.ToDto(reserva, material.Descripcion, socio);
        }, cancellationToken);
    }
}
