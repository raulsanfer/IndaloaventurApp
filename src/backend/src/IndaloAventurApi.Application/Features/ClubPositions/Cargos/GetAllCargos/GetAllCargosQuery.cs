using IndaloAventurApi.Application.Abstractions.ClubPositions;
using IndaloAventurApi.Application.Abstractions.Cqrs;

namespace IndaloAventurApi.Application.Features.ClubPositions.Cargos.GetAllCargos;

public sealed record GetAllCargosQuery() : IQuery<IReadOnlyCollection<CargoDto>>;
