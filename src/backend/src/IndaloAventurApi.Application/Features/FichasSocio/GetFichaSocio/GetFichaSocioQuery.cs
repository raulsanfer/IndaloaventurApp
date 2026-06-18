using IndaloAventurApi.Application.Abstractions.Cqrs;
using IndaloAventurApi.Application.Abstractions.FichasSocio;

namespace IndaloAventurApi.Application.Features.FichasSocio.GetFichaSocio;

public sealed record GetFichaSocioQuery(Guid AuthenticatedUserId, Guid TargetUserId, bool IsAdmin) : IQuery<FichaSocioDto>;
