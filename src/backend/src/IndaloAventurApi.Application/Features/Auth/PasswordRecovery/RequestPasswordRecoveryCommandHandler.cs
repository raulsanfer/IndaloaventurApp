using System.Net;
using System.Text;
using IndaloAventurApi.Application.Abstractions.Email;
using IndaloAventurApi.Application.Abstractions.Identity;
using MediatR;
using Microsoft.Extensions.Options;

namespace IndaloAventurApi.Application.Features.Auth.PasswordRecovery;

public sealed class RequestPasswordRecoveryCommandHandler(
    IIdentityService identityService,
    IEmailSender emailSender,
    IOptions<PasswordRecoveryOptions> options)
    : IRequestHandler<RequestPasswordRecoveryCommand, PasswordRecoveryResponse>
{
    private const string NeutralMessage = "Si existe una cuenta asociada al email indicado, te hemos enviado instrucciones para recuperar tu contrasena.";

    public async Task<PasswordRecoveryResponse> Handle(RequestPasswordRecoveryCommand request, CancellationToken cancellationToken)
    {
        var token = await identityService.GeneratePasswordResetTokenAsync(request.Email, cancellationToken);
        if (string.IsNullOrWhiteSpace(token))
        {
            return new PasswordRecoveryResponse(NeutralMessage);
        }

        var recoveryOptions = options.Value;
        if (string.IsNullOrWhiteSpace(recoveryOptions.FrontendResetPasswordUrl))
        {
            throw new InvalidOperationException("La configuracion de recuperacion de contrasena requiere una URL publica de frontend.");
        }

        var encodedToken = Base64UrlEncode(token);
        var separator = recoveryOptions.FrontendResetPasswordUrl.Contains('?', StringComparison.Ordinal) ? '&' : '?';
        var resetUrl = $"{recoveryOptions.FrontendResetPasswordUrl}{separator}email={Uri.EscapeDataString(request.Email)}&token={Uri.EscapeDataString(encodedToken)}";

        var htmlBody = $$"""
            <html>
            <body style="font-family: Arial, sans-serif; color: #1f2937;">
                <h2 style="color: #0f172a;">Recuperacion de contrasena</h2>
                <p>Hola,</p>
                <p>Hemos recibido una solicitud para restablecer la contrasena de tu cuenta en IndaloAventura.</p>
                <p>Pulsa en el siguiente enlace para definir una nueva contrasena:</p>
                <p>
                    <a href="{{WebUtility.HtmlEncode(resetUrl)}}" style="display:inline-block;padding:12px 18px;background:#2563eb;color:#ffffff;text-decoration:none;border-radius:6px;">
                        Definir nueva contrasena
                    </a>
                </p>
                <p>Si no has solicitado este cambio, puedes ignorar este correo.</p>
                <p>Gracias,<br />Equipo IndaloAventura</p>
            </body>
            </html>
            """;

        var plainTextBody = $"""
            Hemos recibido una solicitud para restablecer la contrasena de tu cuenta en IndaloAventura.
            Abre este enlace para definir una nueva contrasena:
            {resetUrl}

            Si no has solicitado este cambio, puedes ignorar este correo.
            """;

        await emailSender.SendAsync(
            new EmailMessage(request.Email, "Recuperacion de contrasena de IndaloAventura", htmlBody, plainTextBody),
            cancellationToken);

        return new PasswordRecoveryResponse(NeutralMessage);
    }

    private static string Base64UrlEncode(string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value);
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
}
