using System.Text;
using IndaloAventurApi.Application.Abstractions.Identity;
using MediatR;

namespace IndaloAventurApi.Application.Features.Auth.PasswordRecovery;

public sealed class ResetPasswordCommandHandler(IIdentityService identityService)
    : IRequestHandler<ResetPasswordCommand, PasswordRecoveryResponse>
{
    public async Task<PasswordRecoveryResponse> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        string decodedToken;

        try
        {
            decodedToken = Encoding.UTF8.GetString(Base64UrlDecode(request.Token));
        }
        catch (Exception)
        {
            throw new InvalidOperationException("El token de recuperacion no es valido.");
        }

        var result = await identityService.ResetPasswordAsync(request.Email, decodedToken, request.NewPassword, cancellationToken);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(string.Join("; ", result.Errors));
        }

        return new PasswordRecoveryResponse("La contrasena se ha actualizado correctamente.");
    }

    private static byte[] Base64UrlDecode(string value)
    {
        var padded = value
            .Replace('-', '+')
            .Replace('_', '/');

        padded = (padded.Length % 4) switch
        {
            2 => padded + "==",
            3 => padded + "=",
            _ => padded
        };

        return Convert.FromBase64String(padded);
    }
}
