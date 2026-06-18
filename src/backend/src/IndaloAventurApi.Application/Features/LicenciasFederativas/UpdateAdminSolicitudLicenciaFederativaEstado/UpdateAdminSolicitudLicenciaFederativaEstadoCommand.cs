using FluentValidation;
using IndaloAventurApi.Application.Abstractions.Cqrs;
using IndaloAventurApi.Application.Abstractions.LicenciasFederativas;
using IndaloAventurApi.Domain.LicenciasFederativas;

namespace IndaloAventurApi.Application.Features.LicenciasFederativas.UpdateAdminSolicitudLicenciaFederativaEstado;

public sealed record UpdateAdminSolicitudLicenciaFederativaEstadoCommand(
    Guid UserId,
    Guid SolicitudId,
    EstadoSolicitudLicenciaFederativa Estado) : ICommand<AdminSolicitudLicenciaFederativaDto>;

public sealed class UpdateAdminSolicitudLicenciaFederativaEstadoCommandValidator : AbstractValidator<UpdateAdminSolicitudLicenciaFederativaEstadoCommand>
{
    public UpdateAdminSolicitudLicenciaFederativaEstadoCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.SolicitudId).NotEmpty();
        RuleFor(x => x.Estado).IsInEnum();
    }
}
