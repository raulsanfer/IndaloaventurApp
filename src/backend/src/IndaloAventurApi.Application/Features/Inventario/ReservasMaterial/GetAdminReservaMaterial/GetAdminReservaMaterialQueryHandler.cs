using IndaloAventurApi.Application.Abstractions.Inventario;
using IndaloAventurApi.Application.Abstractions.Inventario.ReservasMaterial;
using MediatR;

namespace IndaloAventurApi.Application.Features.Inventario.ReservasMaterial.GetAdminReservaMaterial;

public sealed class GetAdminReservaMaterialQueryHandler(
    IReservaMaterialRepository reservaRepository,
    IInventarioMaterialRepository materialRepository,
    ISocioReservaReader socioReader)
    : IRequestHandler<GetAdminReservaMaterialQuery, ReservaMaterialDto>
{
    public async Task<ReservaMaterialDto> Handle(GetAdminReservaMaterialQuery request, CancellationToken cancellationToken)
    {
        ReservaMaterialAdminAccess.RequireAdmin(request.IsAdmin);

        var reserva = await reservaRepository.GetByIdAsync(request.ReservaId, cancellationToken)
            ?? throw new KeyNotFoundException("La reserva de material no existe.");
        var material = await materialRepository.GetByIdAsync(reserva.InventarioMaterialId, cancellationToken)
            ?? throw new KeyNotFoundException("El material de inventario asociado a la reserva no existe.");
        var socio = await socioReader.GetByUserIdAsync(reserva.UserId, cancellationToken)
            ?? throw new KeyNotFoundException("El socio asociado a la reserva no existe.");

        return ReservaMaterialMapping.ToDto(reserva, material.Descripcion, socio);
    }
}
