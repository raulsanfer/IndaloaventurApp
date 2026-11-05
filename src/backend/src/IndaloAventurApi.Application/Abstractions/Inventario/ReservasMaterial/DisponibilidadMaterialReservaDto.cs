namespace IndaloAventurApi.Application.Abstractions.Inventario.ReservasMaterial;

public sealed record DisponibilidadMaterialReservaDto(
    int InventarioMaterialId,
    int CantidadTotal,
    int CantidadComprometida,
    int CantidadDisponible,
    bool Activo);
