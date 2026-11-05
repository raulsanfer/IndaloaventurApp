using IndaloAventurApi.Domain.Inventario.ReservasMaterial;

namespace IndaloAventurApi.Application.Abstractions.Inventario.ReservasMaterial;

public sealed record ReservaMaterialNotification(
    TipoNotificacionReservaMaterial Tipo,
    Guid ReservaId,
    SocioReservaDto Socio,
    string MaterialDescripcion,
    int CantidadSolicitada,
    EstadoReservaMaterial Estado,
    DateOnly FechaRetiradaPrevista,
    DateOnly FechaDevolucionEstimada,
    DateOnly? FechaRetirada,
    DateOnly? FechaDevolucionEfectiva,
    string? ObservacionAdministracion);
