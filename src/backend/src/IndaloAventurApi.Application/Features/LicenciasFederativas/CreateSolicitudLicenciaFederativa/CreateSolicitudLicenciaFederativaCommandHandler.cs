using System.Net;
using IndaloAventurApi.Application.Abstractions.Email;
using IndaloAventurApi.Application.Abstractions.LicenciasFederativas;
using IndaloAventurApi.Application.Common;
using IndaloAventurApi.Domain.LicenciasFederativas;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IndaloAventurApi.Application.Features.LicenciasFederativas.CreateSolicitudLicenciaFederativa;

public sealed class CreateSolicitudLicenciaFederativaCommandHandler(
    ITarifaLicenciaFederativaRepository tarifaRepository,
    ISolicitudLicenciaFederativaRepository solicitudRepository,
    IEmailSender emailSender,
    IOptions<FederativeLicenseRequestNotificationOptions> notificationOptions,
    ILogger<CreateSolicitudLicenciaFederativaCommandHandler> logger)
    : IRequestHandler<CreateSolicitudLicenciaFederativaCommand, SolicitudLicenciaFederativaDto>
{
    public async Task<SolicitudLicenciaFederativaDto> Handle(CreateSolicitudLicenciaFederativaCommand request, CancellationToken cancellationToken)
    {
        if (!request.IsMember)
        {
            throw new ForbiddenAccessException("Solo los socios del club pueden solicitar una licencia federativa.");
        }

        var tarifa = await tarifaRepository.GetByIdAsync(request.TarifaLicenciaFederativaId, cancellationToken);
        if (tarifa is null)
        {
            throw new KeyNotFoundException("La tarifa de licencia federativa indicada no existe.");
        }

        if (tarifa.Temporada != request.Temporada)
        {
            throw new InvalidOperationException("La tarifa seleccionada no pertenece a la temporada indicada.");
        }

        var existing = await solicitudRepository.GetByUserIdAndTemporadaAsync(request.AuthenticatedUserId, request.Temporada, cancellationToken);
        if (existing is not null)
        {
            throw new InvalidOperationException("Ya has realizado una solicitud de licencia federativa para esa temporada.");
        }

        var solicitud = SolicitudLicenciaFederativa.Crear(request.AuthenticatedUserId, request.Temporada, tarifa);

        await solicitudRepository.AddAsync(solicitud, cancellationToken);
        await solicitudRepository.SaveChangesAsync(cancellationToken);

        await TrySendNotificationAsync(request.AuthenticatedUserEmail, cancellationToken);

        return new SolicitudLicenciaFederativaDto(
            solicitud.Id,
            solicitud.Temporada,
            solicitud.Estado.ToString(),
            solicitud.FechaCreacionUtc,
            tarifa.Id,
            tarifa.Licencia,
            tarifa.Categoria,
            tarifa.Territorio,
            tarifa.MediaTemporada,
            tarifa.PrecioClub,
            tarifa.PrecioIndependiente);
    }

    private async Task TrySendNotificationAsync(string requesterEmail, CancellationToken cancellationToken)
    {
        var recipient = notificationOptions.Value.FederativeLicenseRequestsTo?.Trim();
        if (string.IsNullOrWhiteSpace(recipient))
        {
            logger.LogWarning("No se ha configurado destinatario para avisos de solicitudes de licencias federativas.");
            return;
        }

        var htmlBody = $$"""
            <html>
            <body style="font-family: Arial, sans-serif; color: #1f2937;">
                <p>Se ha recibido una solicitud de licencia federativa.</p>
                <p>Email del solicitante: <strong>{{WebUtility.HtmlEncode(requesterEmail)}}</strong></p>
            </body>
            </html>
            """;

        var plainTextBody = $"""
            Se ha recibido una solicitud de licencia federativa.
            Email del solicitante: {requesterEmail}
            """;

        try
        {
            await emailSender.SendAsync(
                new EmailMessage(recipient, "Nueva solicitud de licencia federativa", htmlBody, plainTextBody),
                cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "No se pudo enviar el correo informativo de solicitud de licencia federativa para {RequesterEmail}.", requesterEmail);
        }
    }
}
