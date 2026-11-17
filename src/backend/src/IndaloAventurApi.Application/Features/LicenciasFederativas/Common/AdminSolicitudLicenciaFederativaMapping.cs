using IndaloAventurApi.Application.Abstractions.LicenciasFederativas;
using IndaloAventurApi.Domain.LicenciasFederativas;

namespace IndaloAventurApi.Application.Features.LicenciasFederativas.Common;

internal static class AdminSolicitudLicenciaFederativaMapping
{
    public static AdminSolicitudLicenciaFederativaDto ToAdminDto(SolicitudLicenciaFederativa solicitud)
    {
        if (solicitud.TarifaLicenciaFederativa is null)
        {
            throw new InvalidOperationException("La solicitud no tiene tarifa asociada cargada.");
        }

        return new AdminSolicitudLicenciaFederativaDto(
            solicitud.Id,
            solicitud.UserId,
            string.Empty,
            solicitud.Temporada,
            solicitud.Estado.ToString(),
            solicitud.FechaCreacionUtc,
            solicitud.TarifaLicenciaFederativaId,
            solicitud.TarifaLicenciaFederativa.Licencia,
            solicitud.TarifaLicenciaFederativa.Categoria,
            solicitud.TarifaLicenciaFederativa.Territorio,
            solicitud.TarifaLicenciaFederativa.TipoLicenciaId,
            solicitud.TarifaLicenciaFederativa.TipoLicencia.Nombre,
            solicitud.TarifaLicenciaFederativa.PrecioClub,
            solicitud.TarifaLicenciaFederativa.PrecioIndependiente,
            solicitud.FechaInicio,
            solicitud.FechaFin);
    }

    public static AdminSolicitudLicenciaFederativaDto WithUserEmail(
        AdminSolicitudLicenciaFederativaDto dto,
        string userEmail)
    {
        return dto with { UserEmail = userEmail };
    }
}
