using IndaloAventurApi.Application.Abstractions.Email;
using IndaloAventurApi.Application.Abstractions.Inventario.ReservasMaterial;
using IndaloAventurApi.Application.Features.Inventario.ReservasMaterial.Notifications;
using IndaloAventurApi.Domain.Inventario.ReservasMaterial;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace IndaloAventurApi.Application.Tests.Inventario;

public sealed class ReservaMaterialNotificationServiceTests
{
    [Fact]
    public async Task Notify_EnviaAlSocioYAdministracion_LasCuatroPlantillasTrazables()
    {
        var emailSender = new FakeEmailSender();
        var service = new ReservaMaterialNotificationService(
            emailSender,
            Options.Create(new ReservaMaterialNotificationOptions { InventoryReservationNotificationsTo = "administracion@club.test" }),
            NullLogger<ReservaMaterialNotificationService>.Instance);
        var reservaId = Guid.NewGuid();
        var notificationBase = new ReservaMaterialNotification(
            TipoNotificacionReservaMaterial.Solicitud,
            reservaId,
            new SocioReservaDto(Guid.NewGuid(), "Ana Socia", "ana@club.test"),
            "Casco & linterna",
            2,
            EstadoReservaMaterial.Retirada,
            new DateOnly(2026, 11, 4),
            new DateOnly(2026, 11, 6),
            new DateOnly(2026, 11, 4),
            null,
            "Observacion administrativa");

        foreach (var type in Enum.GetValues<TipoNotificacionReservaMaterial>())
        {
            await service.NotifyAsync(notificationBase with { Tipo = type }, CancellationToken.None);
        }

        Assert.Equal(8, emailSender.Messages.Count);
        Assert.All(emailSender.Messages, message =>
        {
            Assert.Contains(reservaId.ToString(), message.HtmlBody, StringComparison.Ordinal);
            Assert.Contains("Casco &amp; linterna", message.HtmlBody, StringComparison.Ordinal);
            Assert.Contains("Ana Socia", message.PlainTextBody, StringComparison.Ordinal);
        });
        Assert.Contains(emailSender.Messages, x => x.Subject.StartsWith($"Solicitud de reserva #{reservaId}: Casco & linterna", StringComparison.Ordinal));
        Assert.Contains(emailSender.Messages, x => x.Subject.StartsWith($"Actualizacion de reserva #{reservaId}: Retirada", StringComparison.Ordinal));
        Assert.Contains(emailSender.Messages, x => x.Subject == $"Confirmacion de devolucion del socio #{reservaId}");
        Assert.Contains(emailSender.Messages, x => x.Subject == $"Confirmacion administrativa de devolucion #{reservaId}");
        Assert.Contains(emailSender.Messages.Select(x => x.To), x => x == "ana@club.test");
        Assert.Contains(emailSender.Messages.Select(x => x.To), x => x == "administracion@club.test");
    }

    [Fact]
    public async Task Notify_SiElCorreoFalla_NoPropagaElFallo()
    {
        var service = new ReservaMaterialNotificationService(
            new FakeEmailSender(throwOnSend: true),
            Options.Create(new ReservaMaterialNotificationOptions { InventoryReservationNotificationsTo = "administracion@club.test" }),
            NullLogger<ReservaMaterialNotificationService>.Instance);

        var exception = await Record.ExceptionAsync(() => service.NotifyAsync(CrearNotificacion(), CancellationToken.None));

        Assert.Null(exception);
    }

    private static ReservaMaterialNotification CrearNotificacion()
        => new(
            TipoNotificacionReservaMaterial.Solicitud,
            Guid.NewGuid(),
            new SocioReservaDto(Guid.NewGuid(), "Ana Socia", "ana@club.test"),
            "Casco",
            1,
            EstadoReservaMaterial.Solicitada,
            new DateOnly(2026, 11, 4),
            new DateOnly(2026, 11, 6),
            null,
            null,
            null);

    private sealed class FakeEmailSender(bool throwOnSend = false) : IEmailSender
    {
        public List<EmailMessage> Messages { get; } = [];

        public Task SendAsync(EmailMessage message, CancellationToken cancellationToken)
        {
            if (throwOnSend)
            {
                throw new InvalidOperationException("Proveedor no disponible.");
            }

            Messages.Add(message);
            return Task.CompletedTask;
        }
    }
}
