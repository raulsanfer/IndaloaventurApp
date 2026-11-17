using System.Net;
using FluentValidation;
using FluentValidation.Results;
using IndaloAventurApi.Domain.Abstractions;
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

        SolicitudLicenciaFederativa solicitud;
        try
        {
            solicitud = SolicitudLicenciaFederativa.Crear(request.AuthenticatedUserId, request.Temporada, tarifa, request.FechaInicio, request.FechaFin);
        }
        catch (DomainException exception)
        {
            throw new ValidationException([new ValidationFailure("Cobertura", exception.Message)]);
        }

        await solicitudRepository.ExecuteSerializableAsync(async token =>
        {
            var existentes = await solicitudRepository.ListByUserIdAndTemporadaForUpdateAsync(request.AuthenticatedUserId, request.Temporada, token);
            if (tarifa.TipoLicenciaId != TipoLicencia.DiaId && existentes.Any(x => x.TarifaLicenciaFederativa!.TipoLicenciaId != TipoLicencia.DiaId))
                throw new InvalidOperationException("Ya has realizado una solicitud de licencia completa o media para esa temporada.");
            await solicitudRepository.AddAsync(solicitud, token);
            await solicitudRepository.SaveChangesAsync(token);
            return solicitud;
        }, cancellationToken);

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
            tarifa.TipoLicenciaId,
            tarifa.TipoLicencia.Nombre,
            tarifa.PrecioClub,
            tarifa.PrecioIndependiente,
            solicitud.FechaInicio,
            solicitud.FechaFin);
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
