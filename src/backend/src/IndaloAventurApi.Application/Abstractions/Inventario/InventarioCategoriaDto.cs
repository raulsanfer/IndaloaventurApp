namespace IndaloAventurApi.Application.Abstractions.Inventario;

public sealed record InventarioCategoriaDto(int Id, string Nombre, int? CategoriaPadreId, IReadOnlyCollection<InventarioSubcategoriaDto> Subcategorias);

public sealed record InventarioSubcategoriaDto(int Id, string Nombre);
