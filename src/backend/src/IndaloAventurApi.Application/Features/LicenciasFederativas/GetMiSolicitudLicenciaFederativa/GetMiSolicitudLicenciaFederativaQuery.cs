using FluentValidation;
using IndaloAventurApi.Application.Abstractions.Cqrs;
using IndaloAventurApi.Application.Abstractions.LicenciasFederativas;

namespace IndaloAventurApi.Application.Features.LicenciasFederativas.GetMiSolicitudLicenciaFederativa;

public sealed record GetMiSolicitudLicenciaFederativaQuery(Guid AuthenticatedUserId, Guid SolicitudId) : IQuery<SolicitudLicenciaFederativaDto>;

public sealed class GetMiSolicitudLicenciaFederativaQueryValidator : AbstractValidator<GetMiSolicitudLicenciaFederativaQuery>
{
    public GetMiSolicitudLicenciaFederativaQueryValidator()
    {
        RuleFor(x => x.AuthenticatedUserId).NotEmpty();
        RuleFor(x => x.SolicitudId).NotEmpty();
    }
}
