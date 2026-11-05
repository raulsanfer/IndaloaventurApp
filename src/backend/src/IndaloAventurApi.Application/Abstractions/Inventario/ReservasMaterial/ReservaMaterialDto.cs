using IndaloAventurApi.Domain.Inventario.ReservasMaterial;

namespace IndaloAventurApi.Application.Abstractions.Inventario.ReservasMaterial;

public sealed record ReservaMaterialDto(
    Guid Id,
    Guid UserId,
    int InventarioMaterialId,
    string MaterialDescripcion,
    int CantidadSolicitada,
    DateTime FechaSolicitudUtc,
    DateOnly FechaRetiradaPrevista,
    DateOnly? FechaRetirada,
    DateOnly FechaDevolucionEstimada,
    DateOnly? FechaDevolucionEfectiva,
    string? Observaciones,
    string? ObservacionAdministracion,
    EstadoReservaMaterial Estado,
    bool ConfirmacionDevolucionSocio,
    bool ConfirmacionDevolucionAdmin,
    SocioReservaDto? Socio = null,
    DisponibilidadMaterialReservaDto? Disponibilidad = null);
