using System.Net;
using IndaloAventurApi.Application.Abstractions.Email;
using IndaloAventurApi.Application.Abstractions.Inventario.ReservasMaterial;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IndaloAventurApi.Application.Features.Inventario.ReservasMaterial.Notifications;

public sealed class ReservaMaterialNotificationService(
    IEmailSender emailSender,
    IOptions<ReservaMaterialNotificationOptions> options,
    ILogger<ReservaMaterialNotificationService> logger)
    : IReservaMaterialNotificationService
{
    public async Task NotifyAsync(ReservaMaterialNotification notification, CancellationToken cancellationToken)
    {
        var template = BuildTemplate(notification);
        var recipients = new[]
            {
                notification.Socio.Email?.Trim(),
                options.Value.InventoryReservationNotificationsTo?.Trim()
            }
            .Where(email => !string.IsNullOrWhiteSpace(email))
            .Select(email => email!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (recipients.Length == 0)
        {
            logger.LogWarning("No hay destinatarios configurados para la notificacion de la reserva {ReservaId}.", notification.ReservaId);
            return;
        }

        foreach (var recipient in recipients)
        {
            try
            {
                await emailSender.SendAsync(
                    new EmailMessage(recipient, template.Subject, template.HtmlBody, template.PlainTextBody),
                    cancellationToken);
            }
            catch (Exception exception)
            {
                logger.LogWarning(
                    exception,
                    "No se pudo enviar la notificacion {NotificationType} de la reserva {ReservaId} a {Recipient}.",
                    notification.Tipo,
                    notification.ReservaId,
                    recipient);
            }
        }
    }

    private static NotificationTemplate BuildTemplate(ReservaMaterialNotification notification)
    {
        var subject = notification.Tipo switch
        {
            TipoNotificacionReservaMaterial.Solicitud => $"Solicitud de reserva #{notification.ReservaId}: {notification.MaterialDescripcion}",
            TipoNotificacionReservaMaterial.CambioDeEstado => $"Actualizacion de reserva #{notification.ReservaId}: {notification.Estado}",
            TipoNotificacionReservaMaterial.ConfirmacionDevolucionSocio => $"Confirmacion de devolucion del socio #{notification.ReservaId}",
            TipoNotificacionReservaMaterial.ConfirmacionDevolucionAdministracion => $"Confirmacion administrativa de devolucion #{notification.ReservaId}",
            _ => $"Actualizacion de reserva #{notification.ReservaId}: {notification.Estado}"
        };

        var adminObservation = string.IsNullOrWhiteSpace(notification.ObservacionAdministracion)
            ? string.Empty
            : $"\nObservacion administrativa: {notification.ObservacionAdministracion}";
        var plainTextBody = $"""
            Reserva: {notification.ReservaId}
            Socio: {notification.Socio.NombreCompleto} ({notification.Socio.Email})
            Material: {notification.MaterialDescripcion}
            Cantidad: {notification.CantidadSolicitada}
            Estado: {notification.Estado}
            Retirada prevista: {FormatDate(notification.FechaRetiradaPrevista)}
            Retirada efectiva: {FormatDate(notification.FechaRetirada)}
            Devolucion estimada: {FormatDate(notification.FechaDevolucionEstimada)}
            Devolucion efectiva: {FormatDate(notification.FechaDevolucionEfectiva)}{adminObservation}
            """;
        var htmlBody = $"""
            <html>
            <body style="font-family: Arial, sans-serif; color: #1f2937;">
                <p>Se ha registrado una actualizacion de reserva de material.</p>
                <ul>
                    <li><strong>Reserva:</strong> {Encode(notification.ReservaId)}</li>
                    <li><strong>Socio:</strong> {Encode(notification.Socio.NombreCompleto)} ({Encode(notification.Socio.Email)})</li>
                    <li><strong>Material:</strong> {Encode(notification.MaterialDescripcion)}</li>
                    <li><strong>Cantidad:</strong> {notification.CantidadSolicitada}</li>
                    <li><strong>Estado:</strong> {Encode(notification.Estado)}</li>
                    <li><strong>Retirada prevista:</strong> {Encode(FormatDate(notification.FechaRetiradaPrevista))}</li>
                    <li><strong>Retirada efectiva:</strong> {Encode(FormatDate(notification.FechaRetirada))}</li>
                    <li><strong>Devolucion estimada:</strong> {Encode(FormatDate(notification.FechaDevolucionEstimada))}</li>
                    <li><strong>Devolucion efectiva:</strong> {Encode(FormatDate(notification.FechaDevolucionEfectiva))}</li>
                </ul>
                {FormatAdminObservation(notification.ObservacionAdministracion)}
            </body>
            </html>
            """;

        return new NotificationTemplate(subject, htmlBody, plainTextBody);
    }

    private static string FormatDate(DateOnly date) => date.ToString("dd/MM/yyyy");
    private static string FormatDate(DateOnly? date) => date?.ToString("dd/MM/yyyy") ?? "Pendiente";
    private static string Encode(object value) => WebUtility.HtmlEncode(Convert.ToString(value) ?? string.Empty);
    private static string FormatAdminObservation(string? observation)
        => string.IsNullOrWhiteSpace(observation)
            ? string.Empty
            : $"<p><strong>Observacion administrativa:</strong> {Encode(observation)}</p>";

    private sealed record NotificationTemplate(string Subject, string HtmlBody, string PlainTextBody);
}
