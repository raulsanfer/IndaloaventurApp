using System.Net;
using System.Net.Mail;
using IndaloAventurApi.Application.Abstractions.Email;
using Microsoft.Extensions.Options;

namespace IndaloAventurApi.Infrastructure.Email;

public sealed class SmtpEmailSender(IOptions<SmtpEmailSenderOptions> options) : IEmailSender
{
    public async Task SendAsync(EmailMessage message, CancellationToken cancellationToken)
    {
        var smtpOptions = options.Value;
        if (string.IsNullOrWhiteSpace(smtpOptions.Host) || string.IsNullOrWhiteSpace(smtpOptions.FromAddress))
        {
            throw new InvalidOperationException("La configuracion de email SMTP requiere Host y FromAddress.");
        }

        using var mailMessage = new MailMessage
        {
            From = new MailAddress(smtpOptions.FromAddress, smtpOptions.FromName),
            Subject = message.Subject,
            Body = message.HtmlBody,
            IsBodyHtml = true
        };

        mailMessage.To.Add(message.To);
        if (!string.IsNullOrWhiteSpace(message.PlainTextBody))
        {
            mailMessage.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(message.PlainTextBody, null, "text/plain"));
        }

        using var client = new SmtpClient(smtpOptions.Host, smtpOptions.Port)
        {
            EnableSsl = smtpOptions.EnableSsl
        };

        if (!string.IsNullOrWhiteSpace(smtpOptions.Username))
        {
            client.Credentials = new NetworkCredential(smtpOptions.Username, smtpOptions.Password ?? string.Empty);
        }

        await client.SendMailAsync(mailMessage, cancellationToken);
    }
}
