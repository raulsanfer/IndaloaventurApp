using FluentValidation;
using IndaloAventurApi.Application.Abstractions.Cqrs;
using IndaloAventurApi.Application.Abstractions.LicenciasFederativas;

namespace IndaloAventurApi.Application.Features.LicenciasFederativas.GetTarifasLicenciasFederativas;

public sealed record GetTarifasLicenciasFederativasQuery(int? Temporada, bool? MediaTemporada) : IQuery<IReadOnlyCollection<TarifaLicenciaFederativaDto>>;

public sealed class GetTarifasLicenciasFederativasQueryValidator : AbstractValidator<GetTarifasLicenciasFederativasQuery>
{
    public GetTarifasLicenciasFederativasQueryValidator()
    {
        RuleFor(x => x.Temporada).GreaterThan(0).When(x => x.Temporada.HasValue);
    }
}
