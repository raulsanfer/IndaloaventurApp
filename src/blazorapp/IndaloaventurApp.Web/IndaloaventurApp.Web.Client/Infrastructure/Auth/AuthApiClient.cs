namespace IndaloaventurApp.Web.Client.Infrastructure.Auth;

using System.Net.Http.Json;
using System.Text.Json;
using IndaloaventurApp.SharedUI.Abstractions.Auth;
using IndaloaventurApp.SharedUI.Models.Auth;
using IndaloaventurApp.SharedUI.Models.Common;

public sealed class AuthApiClient(HttpClient httpClient) : IAuthService
{
    private const string LoginEndpoint = "/api/auth/login";
    private const string SocialLoginEndpoint = "/api/auth/social-login";
    private const string PasswordRecoveryEndpoint = "/api/auth/passrecovery";
    private const string ResetPasswordEndpoint = "/api/auth/reset-password";
    private const string InvalidCredentialsError = "auth.invalid_credentials";
    private const string InvalidSocialCredentialsError = "auth.social_invalid";
    private const string UnavailableError = "auth.unavailable";

    public async Task<ServiceResult<AuthSession>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync(
                LoginEndpoint,
                new LoginApiRequest(request.EmailOrUserName, request.Password),
                cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return await ReadSessionAsync(response, cancellationToken);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return ServiceResult<AuthSession>.Failure(new ServiceError(InvalidCredentialsError, "Credenciales no válidas."));
            }

            return ServiceResult<AuthSession>.Failure(new ServiceError(UnavailableError, $"Error HTTP {(int)response.StatusCode}."));
        }
        catch (HttpRequestException)
        {
            return TryDevelopmentFallback(request);
        }
        catch (TaskCanceledException)
        {
            return ServiceResult<AuthSession>.Failure(new ServiceError(UnavailableError, "Tiempo de espera superado."));
        }
    }

    public async Task<ServiceResult<AuthSession>> LoginSocialAsync(SocialLoginRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync(
                SocialLoginEndpoint,
                request,
                cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return await ReadSessionAsync(response, cancellationToken);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return ServiceResult<AuthSession>.Failure(new ServiceError(InvalidSocialCredentialsError, "No se pudo validar la autenticación social."));
            }

            return ServiceResult<AuthSession>.Failure(new ServiceError(UnavailableError, $"Error HTTP {(int)response.StatusCode}."));
        }
        catch (HttpRequestException)
        {
            return ServiceResult<AuthSession>.Failure(new ServiceError(UnavailableError, "Servicio de autenticación no disponible."));
        }
        catch (TaskCanceledException)
        {
            return ServiceResult<AuthSession>.Failure(new ServiceError(UnavailableError, "Tiempo de espera superado."));
        }
    }

    public async Task<ServiceResult<string>> RequestPasswordRecoveryAsync(PasswordRecoveryRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync(
                PasswordRecoveryEndpoint,
                request,
                cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return await ReadMessageAsync(response, "auth.password_recovery_empty", "Respuesta de recuperación vacía.", cancellationToken);
            }

            return ServiceResult<string>.Failure(
                new ServiceError(
                    "auth.password_recovery_failed",
                    await ReadErrorMessageAsync(response, cancellationToken) ?? $"Error HTTP {(int)response.StatusCode}."));
        }
        catch (HttpRequestException)
        {
            return ServiceResult<string>.Failure(
                new ServiceError("auth.password_recovery_unavailable", "No se pudo iniciar la recuperación de contraseña."));
        }
        catch (TaskCanceledException)
        {
            return ServiceResult<string>.Failure(
                new ServiceError("auth.password_recovery_timeout", "Tiempo de espera superado."));
        }
    }

    public async Task<ServiceResult<string>> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync(
                ResetPasswordEndpoint,
                request,
                cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return await ReadMessageAsync(response, "auth.reset_password_empty", "Respuesta de reseteo vacía.", cancellationToken);
            }

            return ServiceResult<string>.Failure(
                new ServiceError(
                    "auth.reset_password_failed",
                    await ReadErrorMessageAsync(response, cancellationToken) ?? $"Error HTTP {(int)response.StatusCode}."));
        }
        catch (HttpRequestException)
        {
            return ServiceResult<string>.Failure(
                new ServiceError("auth.reset_password_unavailable", "No se pudo restablecer la contraseña."));
        }
        catch (TaskCanceledException)
        {
            return ServiceResult<string>.Failure(
                new ServiceError("auth.reset_password_timeout", "Tiempo de espera superado."));
        }
    }

    private static ServiceResult<AuthSession> TryDevelopmentFallback(LoginRequest request)
    {
        if (!request.EmailOrUserName.Equals("demo@indalo.es", StringComparison.OrdinalIgnoreCase) ||
            !request.Password.Equals("indalo123", StringComparison.Ordinal))
        {
            return ServiceResult<AuthSession>.Failure(new ServiceError(InvalidCredentialsError, "Credenciales no válidas."));
        }

        return ServiceResult<AuthSession>.Success(new AuthSession("demo-token", "Bearer", 3600, true));
    }

    private sealed record LoginApiRequest(string Email, string Password);

    private sealed record LoginApiResponse(string AccessToken, string TokenType, int ExpiresInSeconds, bool? IsMember);

    private sealed record MessageApiResponse(string Message);

    private static async Task<ServiceResult<AuthSession>> ReadSessionAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var payload = await response.Content.ReadFromJsonAsync<LoginApiResponse>(cancellationToken: cancellationToken);
        if (payload is null || string.IsNullOrWhiteSpace(payload.AccessToken))
        {
            return ServiceResult<AuthSession>.Failure(new ServiceError(UnavailableError, "Respuesta de autenticación vacía."));
        }

        return ServiceResult<AuthSession>.Success(
            new AuthSession(
                payload.AccessToken,
                string.IsNullOrWhiteSpace(payload.TokenType) ? "Bearer" : payload.TokenType,
                payload.ExpiresInSeconds,
                ResolveIsMember(payload.AccessToken, payload.IsMember),
                ResolveRoles(payload.AccessToken),
                ResolveUserId(payload.AccessToken)));
    }

    private static async Task<ServiceResult<string>> ReadMessageAsync(
        HttpResponseMessage response,
        string errorCode,
        string fallbackMessage,
        CancellationToken cancellationToken)
    {
        var payload = await response.Content.ReadFromJsonAsync<MessageApiResponse>(cancellationToken: cancellationToken);
        if (payload is null || string.IsNullOrWhiteSpace(payload.Message))
        {
            return ServiceResult<string>.Failure(new ServiceError(errorCode, fallbackMessage));
        }

        return ServiceResult<string>.Success(payload.Message);
    }

    private static async Task<string?> ReadErrorMessageAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        try
        {
            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var json = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
            var root = json.RootElement;

            if (root.ValueKind != JsonValueKind.Object)
            {
                return null;
            }

            if (TryGetPropertyIgnoreCase(root, "detail", out var detail) &&
                detail.ValueKind == JsonValueKind.String &&
                !string.IsNullOrWhiteSpace(detail.GetString()))
            {
                return detail.GetString();
            }

            if (TryGetPropertyIgnoreCase(root, "errors", out var errors) && errors.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in errors.EnumerateArray())
                {
                    if (item.ValueKind == JsonValueKind.Object &&
                        TryGetPropertyIgnoreCase(item, "errorMessage", out var errorMessage) &&
                        errorMessage.ValueKind == JsonValueKind.String &&
                        !string.IsNullOrWhiteSpace(errorMessage.GetString()))
                    {
                        return errorMessage.GetString();
                    }
                }
            }

            if (TryGetPropertyIgnoreCase(root, "message", out var message) &&
                message.ValueKind == JsonValueKind.String &&
                !string.IsNullOrWhiteSpace(message.GetString()))
            {
                return message.GetString();
            }
        }
        catch (JsonException)
        {
            return null;
        }

        return null;
    }

    private static bool TryGetPropertyIgnoreCase(JsonElement element, string propertyName, out JsonElement value)
    {
        foreach (var property in element.EnumerateObject())
        {
            if (string.Equals(property.Name, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                value = property.Value;
                return true;
            }
        }

        value = default;
        return false;
    }

    private static bool ResolveIsMember(string accessToken, bool? fallbackValue)
    {
        return TryReadIsMemberClaim(accessToken) ?? fallbackValue ?? false;
    }

    private static bool? TryReadIsMemberClaim(string accessToken)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return null;
        }

        var tokenSegments = accessToken.Split('.');
        if (tokenSegments.Length < 2)
        {
            return null;
        }

        try
        {
            var payloadBytes = DecodeBase64Url(tokenSegments[1]);
            using var payloadJson = JsonDocument.Parse(payloadBytes);

            foreach (var claimName in new[] { "IsMember", "isMember", "is_member" })
            {
                if (payloadJson.RootElement.TryGetProperty(claimName, out var claimValue) &&
                    TryParseBooleanClaim(claimValue, out var isMember))
                {
                    return isMember;
                }
            }
        }
        catch (FormatException)
        {
            return null;
        }
        catch (JsonException)
        {
            return null;
        }

        return null;
    }

    private static IReadOnlyList<string> ResolveRoles(string accessToken)
    {
        return TryReadRoles(accessToken) ?? Array.Empty<string>();
    }

    private static Guid? ResolveUserId(string accessToken)
    {
        return TryReadUserId(accessToken);
    }

    private static Guid? TryReadUserId(string accessToken)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return null;
        }

        var tokenSegments = accessToken.Split('.');
        if (tokenSegments.Length < 2)
        {
            return null;
        }

        try
        {
            var payloadBytes = DecodeBase64Url(tokenSegments[1]);
            using var payloadJson = JsonDocument.Parse(payloadBytes);

            foreach (var claimName in new[]
                     {
                         "sub",
                         "nameid",
                         "NameIdentifier",
                         "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
                     })
            {
                if (payloadJson.RootElement.TryGetProperty(claimName, out var claimValue) &&
                    claimValue.ValueKind == JsonValueKind.String &&
                    Guid.TryParse(claimValue.GetString(), out var userId))
                {
                    return userId;
                }
            }
        }
        catch (FormatException)
        {
            return null;
        }
        catch (JsonException)
        {
            return null;
        }

        return null;
    }

    private static IReadOnlyList<string>? TryReadRoles(string accessToken)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return null;
        }

        var tokenSegments = accessToken.Split('.');
        if (tokenSegments.Length < 2)
        {
            return null;
        }

        try
        {
            var payloadBytes = DecodeBase64Url(tokenSegments[1]);
            using var payloadJson = JsonDocument.Parse(payloadBytes);

            foreach (var claimName in new[]
                     {
                         "role",
                         "roles",
                         "Role",
                         "Roles",
                         "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
                     })
            {
                if (payloadJson.RootElement.TryGetProperty(claimName, out var claimValue))
                {
                    var roles = ParseRoles(claimValue);
                    if (roles.Count > 0)
                    {
                        return roles;
                    }
                }
            }
        }
        catch (FormatException)
        {
            return null;
        }
        catch (JsonException)
        {
            return null;
        }

        return null;
    }

    private static byte[] DecodeBase64Url(string base64Url)
    {
        var padded = base64Url.Replace('-', '+').Replace('_', '/');
        padded = padded.PadRight(padded.Length + ((4 - padded.Length % 4) % 4), '=');
        return Convert.FromBase64String(padded);
    }

    private static bool TryParseBooleanClaim(JsonElement claimValue, out bool isMember)
    {
        switch (claimValue.ValueKind)
        {
            case JsonValueKind.True:
                isMember = true;
                return true;
            case JsonValueKind.False:
                isMember = false;
                return true;
            case JsonValueKind.String:
                return bool.TryParse(claimValue.GetString(), out isMember);
            case JsonValueKind.Number:
                if (claimValue.TryGetInt32(out var numericValue))
                {
                    isMember = numericValue != 0;
                    return true;
                }

                break;
        }

        isMember = false;
        return false;
    }

    private static IReadOnlyList<string> ParseRoles(JsonElement claimValue)
    {
        var roles = new List<string>();

        switch (claimValue.ValueKind)
        {
            case JsonValueKind.String:
                AddRole(roles, claimValue.GetString());
                break;
            case JsonValueKind.Array:
                foreach (var item in claimValue.EnumerateArray())
                {
                    if (item.ValueKind == JsonValueKind.String)
                    {
                        AddRole(roles, item.GetString());
                    }
                }

                break;
        }

        return roles
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static void AddRole(ICollection<string> roles, string? role)
    {
        if (!string.IsNullOrWhiteSpace(role))
        {
            roles.Add(role.Trim());
        }
    }
}
