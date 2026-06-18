using FluentValidation;
using IndaloAventurApi.Application.Abstractions.Cqrs;
using IndaloAventurApi.Application.Abstractions.LicenciasFederativas;

namespace IndaloAventurApi.Application.Features.LicenciasFederativas.CreateSolicitudLicenciaFederativa;

public sealed record CreateSolicitudLicenciaFederativaCommand(
    Guid AuthenticatedUserId,
    string AuthenticatedUserEmail,
    bool IsMember,
    int Temporada,
    int TarifaLicenciaFederativaId) : ICommand<SolicitudLicenciaFederativaDto>;

public sealed class CreateSolicitudLicenciaFederativaCommandValidator : AbstractValidator<CreateSolicitudLicenciaFederativaCommand>
{
    public CreateSolicitudLicenciaFederativaCommandValidator()
    {
        RuleFor(x => x.AuthenticatedUserId).NotEmpty();
        RuleFor(x => x.AuthenticatedUserEmail).NotEmpty().EmailAddress().MaximumLength(254);
        RuleFor(x => x.Temporada).GreaterThan(0);
        RuleFor(x => x.TarifaLicenciaFederativaId).GreaterThan(0);
    }
}
