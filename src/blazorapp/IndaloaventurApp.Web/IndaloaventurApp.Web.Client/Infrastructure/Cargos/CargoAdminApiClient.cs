namespace IndaloaventurApp.Web.Client.Infrastructure.Cargos;

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using IndaloaventurApp.SharedUI.Abstractions.Cargos;
using IndaloaventurApp.SharedUI.Abstractions.Session;
using IndaloaventurApp.SharedUI.Models.Cargos;
using IndaloaventurApp.SharedUI.Models.Common;

public sealed class CargoAdminApiClient(HttpClient httpClient, ISessionService sessionService) : ICargoAdminService
{
    private const string CargosEndpoint = "/api/cargos";

    public async Task<ServiceResult<IReadOnlyList<CargoItem>>> GetCargosAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using var request = CreateAuthorizedRequest(HttpMethod.Get, CargosEndpoint);
            using var response = await httpClient.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return ServiceResult<IReadOnlyList<CargoItem>>.Failure(new ServiceError("auth.session_invalid", "Sesion invalida."));
            }

            if (!response.IsSuccessStatusCode)
            {
                return ServiceResult<IReadOnlyList<CargoItem>>.Failure(new ServiceError("cargos.unavailable", $"Error HTTP {(int)response.StatusCode}."));
            }

            var payload = await response.Content.ReadFromJsonAsync<CargoDto[]>(cancellationToken: cancellationToken);
            if (payload is null)
            {
                return ServiceResult<IReadOnlyList<CargoItem>>.Failure(new ServiceError("cargos.empty", "Respuesta vacia."));
            }

            return ServiceResult<IReadOnlyList<CargoItem>>.Success(payload.Select(Map).ToArray());
        }
        catch (HttpRequestException)
        {
            return ServiceResult<IReadOnlyList<CargoItem>>.Failure(new ServiceError("cargos.unavailable", "No se pudo conectar con cargos."));
        }
        catch (TaskCanceledException)
        {
            return ServiceResult<IReadOnlyList<CargoItem>>.Failure(new ServiceError("cargos.timeout", "Tiempo de espera agotado."));
        }
        catch (JsonException)
        {
            return ServiceResult<IReadOnlyList<CargoItem>>.Failure(new ServiceError("cargos.invalid_payload", "La respuesta de cargos no es valida."));
        }
        catch (NotSupportedException)
        {
            return ServiceResult<IReadOnlyList<CargoItem>>.Failure(new ServiceError("cargos.invalid_payload", "La respuesta de cargos no es valida."));
        }
    }

    public async Task<ServiceResult<CargoItem>> CreateCargoAsync(CreateCargoRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            using var message = CreateAuthorizedRequest(HttpMethod.Post, CargosEndpoint);
            message.Content = JsonContent.Create(new CreateCargoApiRequest(request.Description));

            using var response = await httpClient.SendAsync(message, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return ServiceResult<CargoItem>.Failure(new ServiceError("auth.session_invalid", "Sesion invalida."));
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return ServiceResult<CargoItem>.Failure(new ServiceError("cargos.validation", "Solicitud invalida."));
            }

            if (!response.IsSuccessStatusCode)
            {
                return ServiceResult<CargoItem>.Failure(new ServiceError("cargos.unavailable", $"Error HTTP {(int)response.StatusCode}."));
            }

            var id = await response.Content.ReadFromJsonAsync<int>(cancellationToken: cancellationToken);
            return ServiceResult<CargoItem>.Success(new CargoItem(id, request.Description.Trim()));
        }
        catch (HttpRequestException)
        {
            return ServiceResult<CargoItem>.Failure(new ServiceError("cargos.unavailable", "No se pudo conectar con cargos."));
        }
        catch (TaskCanceledException)
        {
            return ServiceResult<CargoItem>.Failure(new ServiceError("cargos.timeout", "Tiempo de espera agotado."));
        }
    }

    public async Task<ServiceResult<CargoItem>> UpdateCargoAsync(UpdateCargoRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            using var message = CreateAuthorizedRequest(HttpMethod.Put, $"{CargosEndpoint}/{request.Id}");
            message.Content = JsonContent.Create(new UpdateCargoCommand(request.Description));

            using var response = await httpClient.SendAsync(message, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return ServiceResult<CargoItem>.Failure(new ServiceError("auth.session_invalid", "Sesion invalida."));
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return ServiceResult<CargoItem>.Failure(new ServiceError("cargos.validation", "Solicitud invalida."));
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return ServiceResult<CargoItem>.Failure(new ServiceError("cargos.not_found", "Cargo no encontrado."));
            }

            if (!response.IsSuccessStatusCode)
            {
                return ServiceResult<CargoItem>.Failure(new ServiceError("cargos.unavailable", $"Error HTTP {(int)response.StatusCode}."));
            }

            return ServiceResult<CargoItem>.Success(new CargoItem(request.Id, request.Description.Trim()));
        }
        catch (HttpRequestException)
        {
            return ServiceResult<CargoItem>.Failure(new ServiceError("cargos.unavailable", "No se pudo conectar con cargos."));
        }
        catch (TaskCanceledException)
        {
            return ServiceResult<CargoItem>.Failure(new ServiceError("cargos.timeout", "Tiempo de espera agotado."));
        }
    }

    public async Task<ServiceResult<bool>> DeleteCargoAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            using var message = CreateAuthorizedRequest(HttpMethod.Delete, $"{CargosEndpoint}/{id}");
            using var response = await httpClient.SendAsync(message, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return ServiceResult<bool>.Failure(new ServiceError("auth.session_invalid", "Sesion invalida."));
            }

            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                return ServiceResult<bool>.Failure(new ServiceError("cargos.conflict", "El cargo esta en uso."));
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return ServiceResult<bool>.Failure(new ServiceError("cargos.not_found", "Cargo no encontrado."));
            }

            if (!response.IsSuccessStatusCode)
            {
                return ServiceResult<bool>.Failure(new ServiceError("cargos.unavailable", $"Error HTTP {(int)response.StatusCode}."));
            }

            return ServiceResult<bool>.Success(true);
        }
        catch (HttpRequestException)
        {
            return ServiceResult<bool>.Failure(new ServiceError("cargos.unavailable", "No se pudo conectar con cargos."));
        }
        catch (TaskCanceledException)
        {
            return ServiceResult<bool>.Failure(new ServiceError("cargos.timeout", "Tiempo de espera agotado."));
        }
    }

    private HttpRequestMessage CreateAuthorizedRequest(HttpMethod method, string endpoint)
    {
        var request = new HttpRequestMessage(method, endpoint);

        if (sessionService.IsAuthenticated && !string.IsNullOrWhiteSpace(sessionService.CurrentSession?.AccessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue(
                sessionService.CurrentSession!.TokenType,
                sessionService.CurrentSession.AccessToken);
        }

        return request;
    }

    private static CargoItem Map(CargoDto dto)
    {
        var description = string.IsNullOrWhiteSpace(dto.Descripcion)
            ? $"Cargo #{dto.Id}"
            : dto.Descripcion.Trim();

        return new CargoItem(dto.Id, description);
    }

    private sealed record CargoDto(int Id, string? Descripcion);

    private sealed record CreateCargoApiRequest
    {
        public CreateCargoApiRequest(string descripcion)
        {
            Descripcion = descripcion;
        }

        public string Descripcion { get; init; }
    }

    private sealed record UpdateCargoCommand
    {
        public UpdateCargoCommand(string descripcion)
        {
            Descripcion = descripcion;
        }

        public string Descripcion { get; init; }
    }
}
