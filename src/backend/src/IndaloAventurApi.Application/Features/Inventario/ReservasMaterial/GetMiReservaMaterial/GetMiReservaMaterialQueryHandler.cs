using IndaloAventurApi.Application.Abstractions.Inventario;
using IndaloAventurApi.Application.Abstractions.Inventario.ReservasMaterial;
using IndaloAventurApi.Application.Common;
using MediatR;

namespace IndaloAventurApi.Application.Features.Inventario.ReservasMaterial.GetMiReservaMaterial;

public sealed class GetMiReservaMaterialQueryHandler(
    IReservaMaterialRepository reservaRepository,
    IInventarioMaterialRepository materialRepository,
    ISocioReservaReader socioReader)
    : IRequestHandler<GetMiReservaMaterialQuery, ReservaMaterialDto>
{
    public async Task<ReservaMaterialDto> Handle(GetMiReservaMaterialQuery request, CancellationToken cancellationToken)
    {
        var socio = await ReservaMaterialMemberAccess.RequireSocioAsync(
            request.AuthenticatedUserId,
            request.IsMember,
            socioReader,
            "consultar",
            cancellationToken);

        var reserva = await reservaRepository.GetByIdAsync(request.ReservaId, cancellationToken)
            ?? throw new KeyNotFoundException("La reserva de material no existe.");
        if (reserva.UserId != request.AuthenticatedUserId)
        {
            throw new KeyNotFoundException("La reserva de material no existe.");
        }

        var material = await materialRepository.GetByIdAsync(reserva.InventarioMaterialId, cancellationToken)
            ?? throw new KeyNotFoundException("El material de inventario asociado a la reserva no existe.");
        return ReservaMaterialMapping.ToDto(reserva, material.Descripcion, socio);
    }
}
