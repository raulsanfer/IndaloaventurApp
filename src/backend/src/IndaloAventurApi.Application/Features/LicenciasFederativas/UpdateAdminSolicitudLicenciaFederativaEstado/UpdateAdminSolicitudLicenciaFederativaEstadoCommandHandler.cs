using IndaloAventurApi.Application.Abstractions.LicenciasFederativas;
using IndaloAventurApi.Application.Abstractions.Identity;
using IndaloAventurApi.Application.Features.LicenciasFederativas.Common;
using IndaloAventurApi.Domain.LicenciasFederativas;
using MediatR;

namespace IndaloAventurApi.Application.Features.LicenciasFederativas.UpdateAdminSolicitudLicenciaFederativaEstado;

public sealed class UpdateAdminSolicitudLicenciaFederativaEstadoCommandHandler(
    ISolicitudLicenciaFederativaRepository solicitudRepository,
    IIdentityService identityService)
    : IRequestHandler<UpdateAdminSolicitudLicenciaFederativaEstadoCommand, AdminSolicitudLicenciaFederativaDto>
{
    public async Task<AdminSolicitudLicenciaFederativaDto> Handle(UpdateAdminSolicitudLicenciaFederativaEstadoCommand request, CancellationToken cancellationToken)
    {
        var solicitud = await solicitudRepository.GetByIdAsync(request.SolicitudId, cancellationToken);
        if (solicitud is null || solicitud.UserId != request.UserId)
        {
            throw new KeyNotFoundException("La solicitud de licencia federativa no existe para el usuario indicado.");
        }

        ApplyEstado(solicitud, request.Estado);
        await solicitudRepository.SaveChangesAsync(cancellationToken);

        var adminDto = AdminSolicitudLicenciaFederativaMapping.ToAdminDto(solicitud);
        var user = await identityService.ListUsersAsync(null, cancellationToken);
        var userEmail = user.FirstOrDefault(x => x.UserId == solicitud.UserId)?.Email ?? string.Empty;
        return AdminSolicitudLicenciaFederativaMapping.WithUserEmail(adminDto, userEmail);
    }

    private static void ApplyEstado(SolicitudLicenciaFederativa solicitud, EstadoSolicitudLicenciaFederativa estado)
    {
        switch (estado)
        {
            case EstadoSolicitudLicenciaFederativa.Pendiente:
                solicitud.MarcarPendiente();
                break;
            case EstadoSolicitudLicenciaFederativa.Confirmada:
                solicitud.Confirmar();
                break;
            case EstadoSolicitudLicenciaFederativa.Cancelada:
                solicitud.Cancelar();
                break;
            default:
                throw new InvalidOperationException("El estado de licencia federativa indicado no es valido.");
        }
    }
}
