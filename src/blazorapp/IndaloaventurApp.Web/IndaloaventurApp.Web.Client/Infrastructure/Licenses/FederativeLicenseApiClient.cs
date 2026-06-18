namespace IndaloaventurApp.Web.Client.Infrastructure.Licenses;

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Globalization;
using IndaloaventurApp.SharedUI.Abstractions.Licenses;
using IndaloaventurApp.SharedUI.Abstractions.Session;
using IndaloaventurApp.SharedUI.Models.Common;
using IndaloaventurApp.SharedUI.Models.Licenses;

public sealed class FederativeLicenseApiClient(HttpClient httpClient, ISessionService sessionService) : IFederativeLicenseService
{
    private const string RatesEndpoint = "/api/licencias-federativas/tarifas";
    private const string RequestsEndpoint = "/api/licencias-federativas/me/solicitudes";

    public async Task<ServiceResult<IReadOnlyList<FederativeLicenseRequest>>> GetMyFederativeLicensesAsync(CancellationToken cancellationToken = default)
    {
        var sessionError = ValidateMemberSession();
        if (sessionError is not null)
        {
            return ServiceResult<IReadOnlyList<FederativeLicenseRequest>>.Failure(sessionError);
        }

        try
        {
            using var request = CreateAuthorizedRequest(HttpMethod.Get, RequestsEndpoint);
            using var response = await httpClient.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return ServiceResult<IReadOnlyList<FederativeLicenseRequest>>.Failure(new ServiceError("auth.session_invalid", "Sesion invalida."));
            }

            if (!response.IsSuccessStatusCode)
            {
                return ServiceResult<IReadOnlyList<FederativeLicenseRequest>>.Failure(new ServiceError("licenses.unavailable", $"Error HTTP {(int)response.StatusCode}."));
            }

            var payload = await response.Content.ReadFromJsonAsync<SolicitudLicenciaFederativaDto[]>(cancellationToken: cancellationToken);
            if (payload is null)
            {
                return ServiceResult<IReadOnlyList<FederativeLicenseRequest>>.Failure(new ServiceError("licenses.empty", "Respuesta vacia."));
            }

            return ServiceResult<IReadOnlyList<FederativeLicenseRequest>>.Success(payload.Select(MapRequest).ToArray());
        }
        catch (HttpRequestException)
        {
            return ServiceResult<IReadOnlyList<FederativeLicenseRequest>>.Failure(new ServiceError("licenses.unavailable", "No se pudo conectar con las licencias federativas."));
        }
        catch (TaskCanceledException)
        {
            return ServiceResult<IReadOnlyList<FederativeLicenseRequest>>.Failure(new ServiceError("licenses.timeout", "Tiempo de espera agotado."));
        }
        catch (JsonException)
        {
            return ServiceResult<IReadOnlyList<FederativeLicenseRequest>>.Failure(new ServiceError("licenses.invalid_payload", "La respuesta de licencias federativas no tiene un formato valido."));
        }
        catch (NotSupportedException)
        {
            return ServiceResult<IReadOnlyList<FederativeLicenseRequest>>.Failure(new ServiceError("licenses.invalid_payload", "La respuesta de licencias federativas no tiene un formato valido."));
        }
    }

    public async Task<ServiceResult<IReadOnlyList<FederativeLicenseRate>>> GetAvailableRatesAsync(int temporada, bool mediaTemporada = false, CancellationToken cancellationToken = default)
    {
        var sessionError = ValidateMemberSession();
        if (sessionError is not null)
        {
            return ServiceResult<IReadOnlyList<FederativeLicenseRate>>.Failure(sessionError);
        }

        try
        {
            var query = string.Create(
                CultureInfo.InvariantCulture,
                $"{RatesEndpoint}?temporada={temporada}&mediaTemporada={mediaTemporada.ToString().ToLowerInvariant()}");

            using var request = CreateAuthorizedRequest(HttpMethod.Get, query);
            using var response = await httpClient.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return ServiceResult<IReadOnlyList<FederativeLicenseRate>>.Failure(new ServiceError("auth.session_invalid", "Sesion invalida."));
            }

            if (!response.IsSuccessStatusCode)
            {
                return ServiceResult<IReadOnlyList<FederativeLicenseRate>>.Failure(new ServiceError("licenses.rates_unavailable", $"Error HTTP {(int)response.StatusCode}."));
            }

            var payload = await response.Content.ReadFromJsonAsync<TarifaLicenciaFederativaDto[]>(cancellationToken: cancellationToken);
            if (payload is null)
            {
                return ServiceResult<IReadOnlyList<FederativeLicenseRate>>.Failure(new ServiceError("licenses.rates_empty", "Respuesta vacia."));
            }

            return ServiceResult<IReadOnlyList<FederativeLicenseRate>>.Success(payload.Select(MapRate).ToArray());
        }
        catch (HttpRequestException)
        {
            return ServiceResult<IReadOnlyList<FederativeLicenseRate>>.Failure(new ServiceError("licenses.rates_unavailable", "No se pudo cargar el catalogo de licencias federativas."));
        }
        catch (TaskCanceledException)
        {
            return ServiceResult<IReadOnlyList<FederativeLicenseRate>>.Failure(new ServiceError("licenses.rates_timeout", "Tiempo de espera agotado."));
        }
        catch (JsonException)
        {
            return ServiceResult<IReadOnlyList<FederativeLicenseRate>>.Failure(new ServiceError("licenses.rates_invalid_payload", "La respuesta del catalogo de licencias no tiene un formato valido."));
        }
        catch (NotSupportedException)
        {
            return ServiceResult<IReadOnlyList<FederativeLicenseRate>>.Failure(new ServiceError("licenses.rates_invalid_payload", "La respuesta del catalogo de licencias no tiene un formato valido."));
        }
    }

    public async Task<ServiceResult<FederativeLicenseRequest>> CreateFederativeLicenseRequestAsync(CreateFederativeLicenseRequest requestPayload, CancellationToken cancellationToken = default)
    {
        var sessionError = ValidateMemberSession();
        if (sessionError is not null)
        {
            return ServiceResult<FederativeLicenseRequest>.Failure(sessionError);
        }

        try
        {
            using var request = CreateAuthorizedRequest(HttpMethod.Post, RequestsEndpoint);
            request.Content = JsonContent.Create(new CreateSolicitudLicenciaFederativaRequest(
                requestPayload.Temporada,
                requestPayload.TarifaLicenciaFederativaId));

            using var response = await httpClient.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return ServiceResult<FederativeLicenseRequest>.Failure(new ServiceError("auth.session_invalid", "Sesion invalida."));
            }

            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                var detail = await TryReadProblemDetailsAsync(response, cancellationToken);
                return ServiceResult<FederativeLicenseRequest>.Failure(new ServiceError("licenses.request_conflict", detail ?? "Ya existe una solicitud para la temporada seleccionada."));
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var detail = await TryReadProblemDetailsAsync(response, cancellationToken);
                return ServiceResult<FederativeLicenseRequest>.Failure(new ServiceError("licenses.request_invalid", detail ?? "La solicitud de licencia no es valida."));
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                var detail = await TryReadProblemDetailsAsync(response, cancellationToken);
                return ServiceResult<FederativeLicenseRequest>.Failure(new ServiceError("licenses.request_rate_not_found", detail ?? "La tarifa seleccionada ya no esta disponible."));
            }

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                var detail = await TryReadProblemDetailsAsync(response, cancellationToken);
                return ServiceResult<FederativeLicenseRequest>.Failure(new ServiceError("licenses.not_member", detail ?? "Las licencias federativas solo estan disponibles para socios member."));
            }

            if (!response.IsSuccessStatusCode)
            {
                return ServiceResult<FederativeLicenseRequest>.Failure(new ServiceError("licenses.request_failed", $"Error HTTP {(int)response.StatusCode}."));
            }

            var payload = await response.Content.ReadFromJsonAsync<SolicitudLicenciaFederativaDto>(cancellationToken: cancellationToken);
            if (payload is null)
            {
                return ServiceResult<FederativeLicenseRequest>.Failure(new ServiceError("licenses.request_empty", "Respuesta vacia."));
            }

            return ServiceResult<FederativeLicenseRequest>.Success(MapRequest(payload));
        }
        catch (HttpRequestException)
        {
            return ServiceResult<FederativeLicenseRequest>.Failure(new ServiceError("licenses.request_failed", "No se pudo registrar la solicitud de licencia federativa."));
        }
        catch (TaskCanceledException)
        {
            return ServiceResult<FederativeLicenseRequest>.Failure(new ServiceError("licenses.request_timeout", "Tiempo de espera agotado."));
        }
        catch (JsonException)
        {
            return ServiceResult<FederativeLicenseRequest>.Failure(new ServiceError("licenses.request_invalid_payload", "La respuesta de creacion de licencia no tiene un formato valido."));
        }
        catch (NotSupportedException)
        {
            return ServiceResult<FederativeLicenseRequest>.Failure(new ServiceError("licenses.request_invalid_payload", "La respuesta de creacion de licencia no tiene un formato valido."));
        }
    }

    private ServiceError? ValidateMemberSession()
    {
        if (!sessionService.IsAuthenticated || string.IsNullOrWhiteSpace(sessionService.CurrentSession?.AccessToken))
        {
            return new ServiceError("auth.missing_session", "No hay sesion activa.");
        }

        if (sessionService.CurrentSession?.CanAccessOwnFederativeLicenses() != true)
        {
            return new ServiceError("licenses.not_member", "Las licencias federativas solo estan disponibles para socios member.");
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

    private static FederativeLicenseRequest MapRequest(SolicitudLicenciaFederativaDto dto)
    {
        return new FederativeLicenseRequest(
            dto.Id,
            dto.Temporada,
            dto.MediaTemporada,
            string.IsNullOrWhiteSpace(dto.Licencia) ? "Licencia federativa" : dto.Licencia.Trim(),
            string.IsNullOrWhiteSpace(dto.Categoria) ? "Sin categoria" : dto.Categoria.Trim(),
            string.IsNullOrWhiteSpace(dto.Territorio) ? "Sin ambito" : dto.Territorio.Trim(),
            string.IsNullOrWhiteSpace(dto.Estado) ? "Sin estado" : dto.Estado.Trim());
    }

    private static FederativeLicenseRate MapRate(TarifaLicenciaFederativaDto dto)
    {
        return new FederativeLicenseRate(
            dto.Id,
            dto.Temporada,
            dto.MediaTemporada,
            string.IsNullOrWhiteSpace(dto.Licencia) ? "Licencia federativa" : dto.Licencia.Trim(),
            string.IsNullOrWhiteSpace(dto.Categoria) ? "Sin categoria" : dto.Categoria.Trim(),
            string.IsNullOrWhiteSpace(dto.Territorio) ? "Sin ambito" : dto.Territorio.Trim(),
            dto.PrecioClub,
            dto.PrecioIndependiente);
    }

    private sealed record SolicitudLicenciaFederativaDto(
        Guid Id,
        int Temporada,
        string? Estado,
        DateTime FechaCreacionUtc,
        int TarifaLicenciaFederativaId,
        string? Licencia,
        string? Categoria,
        string? Territorio,
        bool MediaTemporada,
        double PrecioClub,
        double? PrecioIndependiente);

    private sealed record TarifaLicenciaFederativaDto(
        int Id,
        int Temporada,
        bool MediaTemporada,
        string? Licencia,
        string? Categoria,
        string? Territorio,
        decimal PrecioClub,
        decimal? PrecioIndependiente);

    private sealed record CreateSolicitudLicenciaFederativaRequest(
        int Temporada,
        int TarifaLicenciaFederativaId);

    private sealed record ProblemDetailsDto(
        string? Title,
        string? Detail);
}
