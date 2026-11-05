namespace IndaloAventurApi.Application.Abstractions.Inventario;

public sealed record InventarioMaterialDto(int Id, string Descripcion, string? MarcaModelo, int Cantidad, int CategoriaId, string CategoriaNombre, int? CategoriaPadreId, string? CategoriaPadreNombre, int EstadoId, string EstadoNombre, DateOnly? FechaAdquisicion, string AdquiridoDonado, string? Observaciones, string? NumeroSerie, bool Activo, int CantidadDisponible);
