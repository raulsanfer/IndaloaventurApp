namespace IndaloaventurApp.Web.Client.Infrastructure.Licenses;

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using IndaloaventurApp.SharedUI.Abstractions.Licenses;
using IndaloaventurApp.SharedUI.Abstractions.Session;
using IndaloaventurApp.SharedUI.Models.Common;
using IndaloaventurApp.SharedUI.Models.Licenses;

public sealed class AdminFederativeLicenseApiClient(HttpClient httpClient, ISessionService sessionService) : IAdminFederativeLicenseService
{
    private const string AdminRequestsEndpoint = "/api/licencias-federativas/admin/solicitudes";

    public async Task<ServiceResult<IReadOnlyList<AdminFederativeLicenseRequest>>> GetFederativeLicensesAsync(
        AdminFederativeLicenseQuery query,
        CancellationToken cancellationToken = default)
    {
        var sessionError = ValidateAdminSession();
        if (sessionError is not null)
        {
            return ServiceResult<IReadOnlyList<AdminFederativeLicenseRequest>>.Failure(sessionError);
        }

        try
        {
            using var request = CreateAuthorizedRequest(HttpMethod.Get, BuildRequestsEndpoint(query));
            using var response = await httpClient.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return ServiceResult<IReadOnlyList<AdminFederativeLicenseRequest>>.Failure(new ServiceError("auth.session_invalid", "Sesion invalida."));
            }

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                return ServiceResult<IReadOnlyList<AdminFederativeLicenseRequest>>.Failure(new ServiceError("licenses.admin_forbidden", "Acceso denegado a la gestión administrativa de licencias."));
            }

            if (!response.IsSuccessStatusCode)
            {
                return ServiceResult<IReadOnlyList<AdminFederativeLicenseRequest>>.Failure(new ServiceError("licenses.admin_unavailable", $"Error HTTP {(int)response.StatusCode}."));
            }

            var payload = await response.Content.ReadFromJsonAsync<AdminSolicitudLicenciaFederativaDto[]>(cancellationToken: cancellationToken);
            if (payload is null)
            {
                return ServiceResult<IReadOnlyList<AdminFederativeLicenseRequest>>.Failure(new ServiceError("licenses.admin_empty", "Respuesta vacia."));
            }

            return ServiceResult<IReadOnlyList<AdminFederativeLicenseRequest>>.Success(payload.Select(MapAdminRequest).ToArray());
        }
        catch (HttpRequestException)
        {
            return ServiceResult<IReadOnlyList<AdminFederativeLicenseRequest>>.Failure(new ServiceError("licenses.admin_unavailable", "No se pudo conectar con la gestión administrativa de licencias."));
        }
        catch (TaskCanceledException)
        {
            return ServiceResult<IReadOnlyList<AdminFederativeLicenseRequest>>.Failure(new ServiceError("licenses.admin_timeout", "Tiempo de espera agotado."));
        }
        catch (JsonException)
        {
            return ServiceResult<IReadOnlyList<AdminFederativeLicenseRequest>>.Failure(new ServiceError("licenses.admin_invalid_payload", "La respuesta administrativa de licencias no tiene un formato valido."));
        }
        catch (NotSupportedException)
        {
            return ServiceResult<IReadOnlyList<AdminFederativeLicenseRequest>>.Failure(new ServiceError("licenses.admin_invalid_payload", "La respuesta administrativa de licencias no tiene un formato valido."));
        }
    }

    public async Task<ServiceResult<AdminFederativeLicenseRequest>> UpdateFederativeLicenseStatusAsync(
        UpdateAdminFederativeLicenseStatusRequest requestModel,
        CancellationToken cancellationToken = default)
    {
        var sessionError = ValidateAdminSession();
        if (sessionError is not null)
        {
            return ServiceResult<AdminFederativeLicenseRequest>.Failure(sessionError);
        }

        try
        {
            using var request = CreateAuthorizedRequest(
                HttpMethod.Put,
                $"{AdminRequestsEndpoint.Replace("/solicitudes", string.Empty)}/users/{requestModel.UserId:D}/solicitudes/{requestModel.SolicitudId:D}/estado");

            request.Content = JsonContent.Create(new UpdateEstadoRequest(requestModel.Estado));

            using var response = await httpClient.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return ServiceResult<AdminFederativeLicenseRequest>.Failure(new ServiceError("auth.session_invalid", "Sesion invalida."));
            }

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                return ServiceResult<AdminFederativeLicenseRequest>.Failure(new ServiceError("licenses.admin_forbidden", "Acceso denegado a la gestión administrativa de licencias."));
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var detail = await TryReadProblemDetailsAsync(response, cancellationToken);
                return ServiceResult<AdminFederativeLicenseRequest>.Failure(new ServiceError("licenses.admin_validation", detail ?? "La actualización del estado no es valida."));
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return ServiceResult<AdminFederativeLicenseRequest>.Failure(new ServiceError("licenses.admin_not_found", "No se encontró la solicitud de licencia indicada."));
            }

            if (!response.IsSuccessStatusCode)
            {
                return ServiceResult<AdminFederativeLicenseRequest>.Failure(new ServiceError("licenses.admin_update_failed", $"Error HTTP {(int)response.StatusCode}."));
            }

            var payload = await response.Content.ReadFromJsonAsync<AdminSolicitudLicenciaFederativaDto>(cancellationToken: cancellationToken);
            if (payload is null)
            {
                return ServiceResult<AdminFederativeLicenseRequest>.Failure(new ServiceError("licenses.admin_empty", "Respuesta vacia."));
            }

            return ServiceResult<AdminFederativeLicenseRequest>.Success(MapAdminRequest(payload));
        }
        catch (HttpRequestException)
        {
            return ServiceResult<AdminFederativeLicenseRequest>.Failure(new ServiceError("licenses.admin_update_failed", "No se pudo actualizar el estado de la licencia."));
        }
        catch (TaskCanceledException)
        {
            return ServiceResult<AdminFederativeLicenseRequest>.Failure(new ServiceError("licenses.admin_timeout", "Tiempo de espera agotado."));
        }
        catch (JsonException)
        {
            return ServiceResult<AdminFederativeLicenseRequest>.Failure(new ServiceError("licenses.admin_invalid_payload", "La respuesta administrativa de licencias no tiene un formato valido."));
        }
        catch (NotSupportedException)
        {
            return ServiceResult<AdminFederativeLicenseRequest>.Failure(new ServiceError("licenses.admin_invalid_payload", "La respuesta administrativa de licencias no tiene un formato valido."));
        }
    }

    private ServiceError? ValidateAdminSession()
    {
        if (!sessionService.IsAuthenticated || string.IsNullOrWhiteSpace(sessionService.CurrentSession?.AccessToken))
        {
            return new ServiceError("auth.missing_session", "No hay sesion activa.");
        }

        if (sessionService.CurrentSession?.CanAdministerFederativeLicenses() != true)
        {
            return new ServiceError("licenses.admin_forbidden", "La gestión administrativa de licencias solo está disponible para administradores.");
        }

        return null;
    }

    private HttpRequestMessage CreateAuthorizedRequest(HttpMethod method, string endpoint)
    {
        var request = new HttpRequestMessage(method, endpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue(
            sessionService.CurrentSession!.TokenType,
            sessionService.CurrentSession.AccessToken);
        return request;
    }

    private static string BuildRequestsEndpoint(AdminFederativeLicenseQuery query)
    {
        var queryParts = new List<string>();

        if (query.UserId.HasValue)
        {
            queryParts.Add($"userId={query.UserId.Value:D}");
        }

        if (query.Temporada.HasValue)
        {
            queryParts.Add($"temporada={query.Temporada.Value}");
        }

        if (!string.IsNullOrWhiteSpace(query.Estado))
        {
            queryParts.Add($"estado={Uri.EscapeDataString(query.Estado.Trim())}");
        }

        return queryParts.Count == 0
            ? AdminRequestsEndpoint
            : $"{AdminRequestsEndpoint}?{string.Join("&", queryParts)}";
    }

    private static async Task<string?> TryReadProblemDetailsAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        try
        {
            var payload = await response.Content.ReadFromJsonAsync<ProblemDetailsDto>(cancellationToken: cancellationToken);
            return string.IsNullOrWhiteSpace(payload?.Detail)
                ? payload?.Title
                : payload.Detail;
        }
        catch (Exception ex) when (ex is JsonException or NotSupportedException)
        {
            return null;
        }
    }

    private static AdminFederativeLicenseRequest MapAdminRequest(AdminSolicitudLicenciaFederativaDto dto)
    {
        return new AdminFederativeLicenseRequest(
            dto.Id,
            dto.UserId,
            string.IsNullOrWhiteSpace(dto.UserEmail) ? $"user-{dto.UserId:D}" : dto.UserEmail.Trim(),
            dto.Temporada,
            string.IsNullOrWhiteSpace(dto.Estado) ? "Sin estado" : dto.Estado.Trim(),
            dto.FechaCreacionUtc,
            dto.TarifaLicenciaFederativaId,
            string.IsNullOrWhiteSpace(dto.Licencia) ? "Licencia federativa" : dto.Licencia.Trim(),
            string.IsNullOrWhiteSpace(dto.Categoria) ? "Sin categoria" : dto.Categoria.Trim(),
            string.IsNullOrWhiteSpace(dto.Territorio) ? "Sin ambito" : dto.Territorio.Trim(),
            dto.PrecioClub,
            dto.PrecioIndependiente);
    }

    private sealed record AdminSolicitudLicenciaFederativaDto(
        Guid Id,
        Guid UserId,
        string? UserEmail,
        int Temporada,
        string? Estado,
        DateTime FechaCreacionUtc,
        int TarifaLicenciaFederativaId,
        string? Licencia,
        string? Categoria,
        string? Territorio,
        decimal PrecioClub,
        decimal? PrecioIndependiente);

    private sealed record UpdateEstadoRequest(string Estado);

    private sealed record ProblemDetailsDto(string? Title, string? Detail);
}
