using IndaloAventurApi.Application.Abstractions.Cqrs;
using IndaloAventurApi.Application.Abstractions.LicenciasFederativas;

namespace IndaloAventurApi.Application.Features.LicenciasFederativas.GetTiposLicencia;

public sealed record GetTiposLicenciaQuery : IQuery<IReadOnlyCollection<TipoLicenciaDto>>;
