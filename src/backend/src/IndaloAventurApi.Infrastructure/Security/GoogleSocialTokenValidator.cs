using Google.Apis.Auth;
using Microsoft.Extensions.Options;

namespace IndaloAventurApi.Infrastructure.Security;

public sealed class GoogleSocialTokenValidator(IOptions<SocialAuthOptions> options) : ISocialTokenValidator
{
    public async Task<SocialTokenValidationResult> ValidateAsync(string provider, string token, CancellationToken cancellationToken)
    {
        if (!string.Equals(provider, "google", StringComparison.OrdinalIgnoreCase))
        {
            return SocialTokenValidationResult.Failed("El proveedor no es compatible.");
        }

        var audience = options.Value.GoogleAudience;
        if (string.IsNullOrWhiteSpace(audience))
        {
            return SocialTokenValidationResult.Failed("La audiencia de Google no esta configurada.");
        }

        try
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(token, new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = [audience]
            });

            if (string.IsNullOrWhiteSpace(payload.Email) || string.IsNullOrWhiteSpace(payload.Subject))
            {
                return SocialTokenValidationResult.Failed("El contenido del token social es incompleto.");
            }

            return new SocialTokenValidationResult(true, "google", payload.Subject, payload.Email, []);
        }
        catch (InvalidJwtException)
        {
            return SocialTokenValidationResult.Failed("El token social no es valido.");
        }
    }
}
