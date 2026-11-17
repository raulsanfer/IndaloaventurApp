using FluentValidation;
using IndaloAventurApi.Application.Abstractions.Cqrs;
using IndaloAventurApi.Application.Abstractions.LicenciasFederativas;

namespace IndaloAventurApi.Application.Features.LicenciasFederativas.CreateSolicitudLicenciaFederativa;

public sealed record CreateSolicitudLicenciaFederativaCommand(
    Guid AuthenticatedUserId,
    string AuthenticatedUserEmail,
    bool IsMember,
    int Temporada,
    int TarifaLicenciaFederativaId,
    DateOnly? FechaInicio = null,
    DateOnly? FechaFin = null) : ICommand<SolicitudLicenciaFederativaDto>;

public sealed class CreateSolicitudLicenciaFederativaCommandValidator : AbstractValidator<CreateSolicitudLicenciaFederativaCommand>
{
    public CreateSolicitudLicenciaFederativaCommandValidator()
    {
        RuleFor(x => x.FechaInicio).Must((request, inicio) => inicio.HasValue == request.FechaFin.HasValue)
            .WithMessage("La cobertura debe incluir fecha de inicio y fecha de fin.");
        RuleFor(x => x.FechaFin).Must((request, fin) => !request.FechaInicio.HasValue || !fin.HasValue || request.FechaInicio <= fin)
            .WithMessage("La fecha de inicio no puede ser posterior a la fecha de fin.");
        RuleFor(x => x.AuthenticatedUserId).NotEmpty();
        RuleFor(x => x.AuthenticatedUserEmail).NotEmpty().EmailAddress().MaximumLength(254);
        RuleFor(x => x.Temporada).GreaterThan(0);
        RuleFor(x => x.TarifaLicenciaFederativaId).GreaterThan(0);
    }
}
