using IndaloAventurApi.Application.Abstractions.Cqrs;
using IndaloAventurApi.Application.Abstractions.Inventario;

namespace IndaloAventurApi.Application.Features.Inventario.InventarioEstados.GetAllInventarioEstados;

public sealed record GetAllInventarioEstadosQuery : IQuery<IReadOnlyCollection<InventarioEstadoDto>>;
