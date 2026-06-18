namespace IndaloaventurApp.Web.Client.Infrastructure.Member;

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using IndaloaventurApp.SharedUI.Abstractions.Member;
using IndaloaventurApp.SharedUI.Abstractions.Session;
using IndaloaventurApp.SharedUI.Models.Common;
using IndaloaventurApp.SharedUI.Models.Member;

public sealed class AdminUserManagementApiClient(HttpClient httpClient, ISessionService sessionService) : IAdminUserManagementService
{
    private const string UsersEndpoint = "/api/users";
    private const string MemberFileEndpointBase = "/api/fichas-socio";

    public async Task<ServiceResult<IReadOnlyList<ManagedUserItem>>> GetUsersAsync(string? email = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var requestedEmail = MemberSelfProfileMapper.NormalizeEmail(email);
            var endpoint = string.IsNullOrWhiteSpace(requestedEmail)
                ? UsersEndpoint
                : $"{UsersEndpoint}?email={Uri.EscapeDataString(requestedEmail)}";

            using var request = CreateAuthorizedRequest(HttpMethod.Get, endpoint);
            using var response = await httpClient.SendAsync(request, cancellationToken);

            var payloadResult = await ReadUsersResponseAsync(response, cancellationToken);
            if (!payloadResult.IsSuccess || payloadResult.Value is null)
            {
                return ServiceResult<IReadOnlyList<ManagedUserItem>>.Failure(payloadResult.Error!);
            }

            var users = payloadResult.Value
                .Select(MapUser)
                .ToArray();

            return ServiceResult<IReadOnlyList<ManagedUserItem>>.Success(users);
        }
        catch (HttpRequestException)
        {
            return ServiceResult<IReadOnlyList<ManagedUserItem>>.Failure(new ServiceError("users.unavailable", "No se pudo conectar con usuarios."));
        }
        catch (TaskCanceledException)
        {
            return ServiceResult<IReadOnlyList<ManagedUserItem>>.Failure(new ServiceError("users.timeout", "Tiempo de espera agotado."));
        }
        catch (JsonException)
        {
            return ServiceResult<IReadOnlyList<ManagedUserItem>>.Failure(new ServiceError("users.invalid_payload", "La respuesta de usuarios no es vÃ¡lida."));
        }
        catch (NotSupportedException)
        {
            return ServiceResult<IReadOnlyList<ManagedUserItem>>.Failure(new ServiceError("users.invalid_payload", "La respuesta de usuarios no es vÃ¡lida."));
        }
    }

    public async Task<ServiceResult<ManagedUserItem>> GetUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var usersResult = await GetUsersAsync(cancellationToken: cancellationToken);
        if (!usersResult.IsSuccess || usersResult.Value is null)
        {
            return ServiceResult<ManagedUserItem>.Failure(usersResult.Error!);
        }

        var user = usersResult.Value.FirstOrDefault(currentUser => currentUser.UserId == userId);
        if (user is null)
        {
            return ServiceResult<ManagedUserItem>.Failure(new ServiceError("users.not_found", "No se encontrÃ³ el usuario solicitado."));
        }

        return ServiceResult<ManagedUserItem>.Success(user);
    }

    public async Task<ServiceResult<MemberSelfProfile>> GetMemberFileAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            using var request = CreateAuthorizedRequest(HttpMethod.Get, $"{MemberFileEndpointBase}/{userId}");
            using var response = await httpClient.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return ServiceResult<MemberSelfProfile>.Failure(new ServiceError("auth.session_invalid", "SesiÃ³n invÃ¡lida."));
            }

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                return ServiceResult<MemberSelfProfile>.Failure(new ServiceError("users.forbidden", "Acceso denegado."));
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return ServiceResult<MemberSelfProfile>.Failure(new ServiceError("profile.not_found", "No se encontrÃ³ la ficha de socio."));
            }

            if (!response.IsSuccessStatusCode)
            {
                return ServiceResult<MemberSelfProfile>.Failure(new ServiceError("profile.unavailable", $"Error HTTP {(int)response.StatusCode}."));
            }

            var dto = await response.Content.ReadFromJsonAsync<MemberFileDto>(cancellationToken: cancellationToken);
            if (dto is null)
            {
                return ServiceResult<MemberSelfProfile>.Failure(new ServiceError("profile.empty", "Ficha vacÃ­a."));
            }

            return ServiceResult<MemberSelfProfile>.Success(MapMemberFile(dto));
        }
        catch (HttpRequestException)
        {
            return ServiceResult<MemberSelfProfile>.Failure(new ServiceError("profile.unavailable", "No se pudo conectar con la ficha de socio."));
        }
        catch (TaskCanceledException)
        {
            return ServiceResult<MemberSelfProfile>.Failure(new ServiceError("profile.timeout", "Tiempo de espera agotado."));
        }
    }

    public async Task<ServiceResult<MemberSelfProfile>> CreateMemberFileAsync(Guid userId, string email, CancellationToken cancellationToken = default)
    {
        try
        {
            var normalizedEmail = MemberSelfProfileMapper.NormalizeEmail(email) ?? $"member-{userId:N}@club.local";
            using var message = CreateAuthorizedRequest(HttpMethod.Post, $"{MemberFileEndpointBase}/{userId}");
            message.Content = JsonContent.Create(new MemberFileUpsertApiRequest(
                null,
                "Pendiente",
                "Pendiente",
                BuildPlaceholderDni(userId),
                new DateOnly(1900, 1, 1),
                "Pendiente de completar",
                "00000",
                "Pendiente",
                "Pendiente",
                "+34000000000",
                normalizedEmail,
                null,
                true,
                false,
                false));

            using var response = await httpClient.SendAsync(message, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return ServiceResult<MemberSelfProfile>.Failure(new ServiceError("auth.session_invalid", "SesiÃ³n invÃ¡lida."));
            }

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                return ServiceResult<MemberSelfProfile>.Failure(new ServiceError("users.forbidden", "Acceso denegado."));
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return ServiceResult<MemberSelfProfile>.Failure(new ServiceError("profile.validation", "La ficha contiene valores no vÃ¡lidos."));
            }

            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                return ServiceResult<MemberSelfProfile>.Failure(new ServiceError("profile.conflict", "La ficha ya existe."));
            }

            if (!response.IsSuccessStatusCode)
            {
                return ServiceResult<MemberSelfProfile>.Failure(new ServiceError("profile.unavailable", $"Error HTTP {(int)response.StatusCode}."));
            }

            var dto = await response.Content.ReadFromJsonAsync<MemberFileDto>(cancellationToken: cancellationToken);
            if (dto is null)
            {
                return ServiceResult<MemberSelfProfile>.Failure(new ServiceError("profile.empty", "Ficha vacÃ­a."));
            }

            return ServiceResult<MemberSelfProfile>.Success(MapMemberFile(dto));
        }
        catch (HttpRequestException)
        {
            return ServiceResult<MemberSelfProfile>.Failure(new ServiceError("profile.unavailable", "No se pudo crear la ficha de socio."));
        }
        catch (TaskCanceledException)
        {
            return ServiceResult<MemberSelfProfile>.Failure(new ServiceError("profile.timeout", "Tiempo de espera agotado."));
        }
    }

    public async Task<ServiceResult<MemberSelfProfile>> UpdateMemberFileAsync(Guid userId, UpdateMemberSelfProfileRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            using var message = CreateAuthorizedRequest(HttpMethod.Put, $"{MemberFileEndpointBase}/{userId}");
            message.Content = JsonContent.Create(new MemberFileUpsertApiRequest(
                request.CargoId,
                request.Nombre,
                request.Apellidos,
                request.Dni,
                request.FechaNacimiento,
                request.Direccion,
                request.CodigoPostal,
                request.Poblacion,
                request.Provincia,
                request.Tlf,
                request.Email,
                request.Alergias,
                request.AceptaPoliticaPrivacidad,
                request.AceptaUsoImagenes,
                request.AceptaCobroCuenta));

            using var response = await httpClient.SendAsync(message, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return ServiceResult<MemberSelfProfile>.Failure(new ServiceError("auth.session_invalid", "SesiÃ³n invÃ¡lida."));
            }

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                return ServiceResult<MemberSelfProfile>.Failure(new ServiceError("users.forbidden", "Acceso denegado."));
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return ServiceResult<MemberSelfProfile>.Failure(new ServiceError("profile.validation", "La ficha contiene valores no vÃ¡lidos."));
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return ServiceResult<MemberSelfProfile>.Failure(new ServiceError("profile.not_found", "No se encontrÃ³ la ficha de socio."));
            }

            if (!response.IsSuccessStatusCode)
            {
                return ServiceResult<MemberSelfProfile>.Failure(new ServiceError("profile.unavailable", $"Error HTTP {(int)response.StatusCode}."));
            }

            var dto = await response.Content.ReadFromJsonAsync<MemberFileDto>(cancellationToken: cancellationToken);
            if (dto is null)
            {
                return ServiceResult<MemberSelfProfile>.Failure(new ServiceError("profile.empty", "Ficha vacÃ­a."));
            }

            return ServiceResult<MemberSelfProfile>.Success(MapMemberFile(dto));
        }
        catch (HttpRequestException)
        {
            return ServiceResult<MemberSelfProfile>.Failure(new ServiceError("profile.unavailable", "No se pudo actualizar la ficha de socio."));
        }
        catch (TaskCanceledException)
        {
            return ServiceResult<MemberSelfProfile>.Failure(new ServiceError("profile.timeout", "Tiempo de espera agotado."));
        }
    }

    public async Task<ServiceResult<bool>> DeactivateUserAsync(Guid userId, CancellationToken cancellationToken = default)
        => await ChangeUserStateAsync(userId, "deactivate", cancellationToken);

    public async Task<ServiceResult<bool>> ReactivateUserAsync(Guid userId, CancellationToken cancellationToken = default)
        => await ChangeUserStateAsync(userId, "reactivate", cancellationToken);

    private HttpRequestMessage CreateAuthorizedRequest(HttpMethod method, string endpoint)
    {
        var currentSession = sessionService.CurrentSession!;
        var request = new HttpRequestMessage(method, endpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue(currentSession.TokenType, currentSession.AccessToken);
        return request;
    }

    private static async Task<ServiceResult<ManagedUserDto[]>> ReadUsersResponseAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            return ServiceResult<ManagedUserDto[]>.Failure(new ServiceError("auth.session_invalid", "SesiÃ³n invÃ¡lida."));
        }

        if (response.StatusCode == HttpStatusCode.Forbidden)
        {
            return ServiceResult<ManagedUserDto[]>.Failure(new ServiceError("users.forbidden", "Acceso denegado."));
        }

        if (!response.IsSuccessStatusCode)
        {
            return ServiceResult<ManagedUserDto[]>.Failure(new ServiceError("users.unavailable", $"Error HTTP {(int)response.StatusCode}."));
        }

        var payload = await response.Content.ReadFromJsonAsync<ManagedUserDto[]>(cancellationToken: cancellationToken);
        if (payload is null)
        {
            return ServiceResult<ManagedUserDto[]>.Failure(new ServiceError("users.empty", "Respuesta vacÃ­a."));
        }

        return ServiceResult<ManagedUserDto[]>.Success(payload);
    }

    private async Task<ServiceResult<bool>> ChangeUserStateAsync(Guid userId, string action, CancellationToken cancellationToken)
    {
        try
        {
            using var request = CreateAuthorizedRequest(HttpMethod.Post, $"{UsersEndpoint}/{userId}/{action}");
            using var response = await httpClient.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return ServiceResult<bool>.Failure(new ServiceError("auth.session_invalid", "SesiÃ³n invÃ¡lida."));
            }

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                return ServiceResult<bool>.Failure(new ServiceError("users.forbidden", "Acceso denegado."));
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return ServiceResult<bool>.Failure(new ServiceError("users.not_found", "No se encontrÃ³ el usuario solicitado."));
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return ServiceResult<bool>.Failure(new ServiceError("users.invalid_state", "La operaciÃ³n solicitada no es vÃ¡lida para el usuario."));
            }

            if (!response.IsSuccessStatusCode)
            {
                return ServiceResult<bool>.Failure(new ServiceError("users.unavailable", $"Error HTTP {(int)response.StatusCode}."));
            }

            return ServiceResult<bool>.Success(true);
        }
        catch (HttpRequestException)
        {
            return ServiceResult<bool>.Failure(new ServiceError("users.unavailable", "No se pudo conectar con usuarios."));
        }
        catch (TaskCanceledException)
        {
            return ServiceResult<bool>.Failure(new ServiceError("users.timeout", "Tiempo de espera agotado."));
        }
    }

    private static ManagedUserItem MapUser(ManagedUserDto dto)
    {
        var email = string.IsNullOrWhiteSpace(dto.Email)
            ? "sin-email@desconocido.local"
            : dto.Email.Trim();

        return new ManagedUserItem(
            dto.UserId,
            email,
            dto.IsMember,
            dto.IsActive ?? true,
            dto.Roles?
                .Where(role => !string.IsNullOrWhiteSpace(role))
                .Select(role => role.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray()
            ?? Array.Empty<string>());
    }

    private static MemberSelfProfile MapMemberFile(MemberFileDto dto)
    {
        return new MemberSelfProfile(
            dto.UserId,
            dto.CargoId,
            dto.CargoLabel?.Trim(),
            dto.Nombre?.Trim(),
            dto.Apellidos?.Trim(),
            dto.Dni?.Trim(),
            dto.FechaNacimiento,
            dto.Direccion?.Trim(),
            dto.CodigoPostal?.Trim(),
            dto.Poblacion?.Trim(),
            dto.Provincia?.Trim(),
            dto.Tlf?.Trim(),
            dto.Email?.Trim(),
            dto.Alergias?.Trim(),
            dto.AceptaPoliticaPrivacidad,
            dto.AceptaUsoImagenes,
            dto.AceptaCobroCuenta);
    }

    private static string BuildPlaceholderDni(Guid userId)
    {
        var digits = Math.Abs(userId.GetHashCode()) % 100_000_000;
        return $"{digits:D8}A";
    }

    private sealed record ManagedUserDto(Guid UserId, string? Email, bool IsMember, bool? IsActive, string[]? Roles);

    private sealed record MemberFileDto(
        Guid UserId,
        int? CargoId,
        string? CargoLabel,
        string? Nombre,
        string? Apellidos,
        string? Dni,
        DateOnly FechaNacimiento,
        string? Direccion,
        string? CodigoPostal,
        string? Poblacion,
        string? Provincia,
        string? Tlf,
        string? Email,
        string? Alergias,
        bool AceptaPoliticaPrivacidad,
        bool AceptaUsoImagenes,
        bool AceptaCobroCuenta);

    private sealed record MemberFileUpsertApiRequest(
        int? CargoId,
        string? Nombre,
        string? Apellidos,
        string? Dni,
        DateOnly FechaNacimiento,
        string? Direccion,
        string? CodigoPostal,
        string? Poblacion,
        string? Provincia,
        string? Tlf,
        string? Email,
        string? Alergias,
        bool AceptaPoliticaPrivacidad,
        bool AceptaUsoImagenes,
        bool AceptaCobroCuenta);
}
