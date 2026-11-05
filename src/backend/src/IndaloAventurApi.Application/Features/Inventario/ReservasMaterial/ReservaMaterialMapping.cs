using IndaloAventurApi.Application.Abstractions.Inventario.ReservasMaterial;
using IndaloAventurApi.Domain.Inventario.ReservasMaterial;

namespace IndaloAventurApi.Application.Features.Inventario.ReservasMaterial;

internal static class ReservaMaterialMapping
{
    public static ReservaMaterialDto ToDto(ReservaMaterial reserva, string materialDescripcion, SocioReservaDto? socio = null)
        => new(
            reserva.Id,
            reserva.UserId,
            reserva.InventarioMaterialId,
            materialDescripcion,
            reserva.CantidadSolicitada,
            reserva.FechaSolicitudUtc,
            reserva.FechaRetiradaPrevista,
            reserva.FechaRetirada,
            reserva.FechaDevolucionEstimada,
            reserva.FechaDevolucionEfectiva,
            reserva.Observaciones,
            reserva.ObservacionAdministracion,
            reserva.Estado,
            reserva.ConfirmacionDevolucionSocio,
            reserva.ConfirmacionDevolucionAdmin,
            socio);
}
