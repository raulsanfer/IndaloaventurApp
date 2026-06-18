using FluentValidation;
using IndaloAventurApi.Application.Abstractions.Cqrs;
using IndaloAventurApi.Application.Abstractions.LicenciasFederativas;
using IndaloAventurApi.Domain.LicenciasFederativas;

namespace IndaloAventurApi.Application.Features.LicenciasFederativas.GetAdminSolicitudesLicenciaFederativa;

public sealed record GetAdminSolicitudesLicenciaFederativaQuery(
    Guid? UserId,
    int? Temporada,
    EstadoSolicitudLicenciaFederativa? Estado) : IQuery<IReadOnlyCollection<AdminSolicitudLicenciaFederativaDto>>;

public sealed class GetAdminSolicitudesLicenciaFederativaQueryValidator : AbstractValidator<GetAdminSolicitudesLicenciaFederativaQuery>
{
    public GetAdminSolicitudesLicenciaFederativaQueryValidator()
    {
        RuleFor(x => x.Temporada).GreaterThan(0).When(x => x.Temporada.HasValue);
    }
}
