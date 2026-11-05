using IndaloAventurApi.Application.Abstractions.Inventario.ReservasMaterial;
using IndaloAventurApi.Domain.Inventario.ReservasMaterial;

namespace IndaloAventurApi.Application.Features.Inventario.ReservasMaterial.Notifications;

internal static class ReservaMaterialNotificationFactory
{
    public static ReservaMaterialNotification Create(
        TipoNotificacionReservaMaterial tipo,
        ReservaMaterial reserva,
        string materialDescripcion,
        SocioReservaDto socio)
        => new(
            tipo,
            reserva.Id,
            socio,
            materialDescripcion,
            reserva.CantidadSolicitada,
            reserva.Estado,
            reserva.FechaRetiradaPrevista,
            reserva.FechaDevolucionEstimada,
            reserva.FechaRetirada,
            reserva.FechaDevolucionEfectiva,
            reserva.ObservacionAdministracion);
}
