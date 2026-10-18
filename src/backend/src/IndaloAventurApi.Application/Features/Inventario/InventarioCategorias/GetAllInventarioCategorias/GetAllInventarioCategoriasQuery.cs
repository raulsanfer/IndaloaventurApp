using IndaloAventurApi.Application.Abstractions.Cqrs;
using IndaloAventurApi.Application.Abstractions.Inventario;

namespace IndaloAventurApi.Application.Features.Inventario.InventarioCategorias.GetAllInventarioCategorias;

public sealed record GetAllInventarioCategoriasQuery : IQuery<IReadOnlyCollection<InventarioCategoriaDto>>;
