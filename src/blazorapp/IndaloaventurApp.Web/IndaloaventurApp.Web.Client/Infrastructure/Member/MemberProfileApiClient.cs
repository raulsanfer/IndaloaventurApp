namespace IndaloaventurApp.Web.Client.Infrastructure.Member;

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using IndaloaventurApp.SharedUI.Abstractions.Member;
using IndaloaventurApp.SharedUI.Abstractions.Session;
using IndaloaventurApp.SharedUI.Models.Common;
using IndaloaventurApp.SharedUI.Models.Member;

public sealed class MemberProfileApiClient(HttpClient httpClient, ISessionService sessionService) : IMemberProfileService
{
    private const string MyProfileEndpoint = "/api/fichas-socio/me";

    public async Task<ServiceResult<MemberProfile>> GetMyProfileAsync(CancellationToken cancellationToken = default)
    {
        if (!sessionService.IsAuthenticated || string.IsNullOrWhiteSpace(sessionService.CurrentSession?.AccessToken))
        {
            return ServiceResult<MemberProfile>.Failure(new ServiceError("auth.missing_session", "No hay sesión activa."));
        }

        var sessionIsMember = sessionService.CurrentSession?.IsMember ?? false;
        var currentSession = sessionService.CurrentSession!;

        if (!sessionIsMember)
        {
            return ServiceResult<MemberProfile>.Success(null!);
        }

        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, MyProfileEndpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue(
                currentSession.TokenType,
                currentSession.AccessToken);

            using var response = await httpClient.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return ServiceResult<MemberProfile>.Failure(new ServiceError("auth.session_invalid", "Sesión inválida."));
            }

            if (!response.IsSuccessStatusCode)
            {
                return ServiceResult<MemberProfile>.Failure(new ServiceError("profile.unavailable", $"Error HTTP {(int)response.StatusCode}."));
            }

            var dto = await response.Content.ReadFromJsonAsync<FichaSocioDto>(cancellationToken: cancellationToken);
            if (dto is null)
            {
                return ServiceResult<MemberProfile>.Failure(new ServiceError("profile.empty", "Perfil vacío."));
            }

            return ServiceResult<MemberProfile>.Success(Map(dto, sessionIsMember));
        }
        catch (HttpRequestException)
        {
            return ServiceResult<MemberProfile>.Failure(new ServiceError("profile.unavailable", "No se pudo conectar con la ficha de socio."));
        }
        catch (TaskCanceledException)
        {
            return ServiceResult<MemberProfile>.Failure(new ServiceError("profile.timeout", "Tiempo de espera agotado."));
        }
    }

    public async Task<ServiceResult<MemberSelfProfile>> GetMyMemberFileAsync(CancellationToken cancellationToken = default)
    {
        if (!sessionService.IsAuthenticated || string.IsNullOrWhiteSpace(sessionService.CurrentSession?.AccessToken))
        {
            return ServiceResult<MemberSelfProfile>.Failure(new ServiceError("auth.missing_session", "No hay sesión activa."));
        }

        if (sessionService.CurrentSession?.IsMember != true)
        {
            return ServiceResult<MemberSelfProfile>.Failure(new ServiceError("profile.not_member", "La ficha solo está disponible para socios."));
        }

        try
        {
            using var request = CreateAuthorizedRequest(HttpMethod.Get, MyProfileEndpoint);
            using var response = await httpClient.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return ServiceResult<MemberSelfProfile>.Failure(new ServiceError("auth.session_invalid", "Sesión inválida."));
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return ServiceResult<MemberSelfProfile>.Failure(new ServiceError("profile.not_found", "No se encontró la ficha de socio."));
            }

            if (!response.IsSuccessStatusCode)
            {
                return ServiceResult<MemberSelfProfile>.Failure(new ServiceError("profile.unavailable", $"Error HTTP {(int)response.StatusCode}."));
            }

            var dto = await response.Content.ReadFromJsonAsync<MemberSelfProfileDto>(cancellationToken: cancellationToken);
            if (dto is null)
            {
                return ServiceResult<MemberSelfProfile>.Failure(new ServiceError("profile.empty", "Ficha vacía."));
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

    public async Task<ServiceResult<MemberSelfProfile>> UpdateMyMemberFileAsync(UpdateMemberSelfProfileRequest request, CancellationToken cancellationToken = default)
    {
        if (!sessionService.IsAuthenticated || string.IsNullOrWhiteSpace(sessionService.CurrentSession?.AccessToken))
        {
            return ServiceResult<MemberSelfProfile>.Failure(new ServiceError("auth.missing_session", "No hay sesión activa."));
        }

        if (sessionService.CurrentSession?.IsMember != true)
        {
            return ServiceResult<MemberSelfProfile>.Failure(new ServiceError("profile.not_member", "La ficha solo está disponible para socios."));
        }

        try
        {
            using var message = CreateAuthorizedRequest(HttpMethod.Put, MyProfileEndpoint);
            message.Content = JsonContent.Create(new UpdateMemberSelfProfileApiRequest(
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
                return ServiceResult<MemberSelfProfile>.Failure(new ServiceError("auth.session_invalid", "Sesión inválida."));
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return ServiceResult<MemberSelfProfile>.Failure(new ServiceError("profile.validation", "La ficha contiene valores no válidos."));
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return ServiceResult<MemberSelfProfile>.Failure(new ServiceError("profile.not_found", "No se encontró la ficha de socio."));
            }

            if (!response.IsSuccessStatusCode)
            {
                return ServiceResult<MemberSelfProfile>.Failure(new ServiceError("profile.unavailable", $"Error HTTP {(int)response.StatusCode}."));
            }

            var dto = await response.Content.ReadFromJsonAsync<MemberSelfProfileDto>(cancellationToken: cancellationToken);
            if (dto is null)
            {
                return ServiceResult<MemberSelfProfile>.Failure(new ServiceError("profile.empty", "Ficha vacía."));
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

    private HttpRequestMessage CreateAuthorizedRequest(HttpMethod method, string endpoint)
    {
        var currentSession = sessionService.CurrentSession!;
        var request = new HttpRequestMessage(method, endpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue(
            currentSession.TokenType,
            currentSession.AccessToken);
        return request;
    }

    private static MemberProfile Map(FichaSocioDto dto, bool sessionIsMember)
    {
        var fullName = string.Join(' ', new[] { dto.Nombre, dto.Apellidos }.Where(value => !string.IsNullOrWhiteSpace(value))).Trim();
        var resolvedName = string.IsNullOrWhiteSpace(fullName) ? "Socio Indalo" : fullName;
        var isMember = dto.IsMember ?? sessionIsMember;

        var cargoLabel = string.IsNullOrWhiteSpace(dto.CargoLabel)
            ? (dto.CargoId.HasValue ? $"Cargo #{dto.CargoId.Value}" : null)
            : dto.CargoLabel;

        return new MemberProfile(
            dto.UserId,
            resolvedName,
            dto.Email,
            isMember,
            dto.CargoId,
            cargoLabel,
            "SOCIO PREMIUM");
    }

    private static MemberSelfProfile MapMemberFile(MemberSelfProfileDto dto)
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

    private sealed record FichaSocioDto(
        Guid UserId,
        bool? IsMember,
        int? CargoId,
        string? CargoLabel,
        string? Nombre,
        string? Apellidos,
        string? Email);

    private sealed record MemberSelfProfileDto(
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

    private sealed record UpdateMemberSelfProfileApiRequest(
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
