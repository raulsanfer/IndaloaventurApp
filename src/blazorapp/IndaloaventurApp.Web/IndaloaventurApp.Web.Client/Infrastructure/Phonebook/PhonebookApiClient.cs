namespace IndaloaventurApp.Web.Client.Infrastructure.Phonebook;

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using IndaloaventurApp.SharedUI.Abstractions.Phonebook;
using IndaloaventurApp.SharedUI.Abstractions.Session;
using IndaloaventurApp.SharedUI.Models.Common;
using IndaloaventurApp.SharedUI.Models.Phonebook;

public sealed class PhonebookApiClient(HttpClient httpClient, ISessionService sessionService) : IPhonebookService
{
    private const string ListEndpoint = "/api/agenda-telefonica";

    public async Task<ServiceResult<IReadOnlyList<PhonebookContact>>> GetContactsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using var request = CreateAuthorizedRequest();
            using var response = await httpClient.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return ServiceResult<IReadOnlyList<PhonebookContact>>.Failure(new ServiceError("auth.session_invalid", "Sesión inválida."));
            }

            if (!response.IsSuccessStatusCode)
            {
                return ServiceResult<IReadOnlyList<PhonebookContact>>.Failure(new ServiceError("phonebook.unavailable", $"Error HTTP {(int)response.StatusCode}."));
            }

            var payload = await response.Content.ReadFromJsonAsync<FichaContactoDto[]>(cancellationToken: cancellationToken);
            if (payload is null)
            {
                return ServiceResult<IReadOnlyList<PhonebookContact>>.Failure(new ServiceError("phonebook.empty", "Respuesta vacía."));
            }

            return ServiceResult<IReadOnlyList<PhonebookContact>>.Success(
                payload.Select(Map).ToArray());
        }
        catch (HttpRequestException)
        {
            return ServiceResult<IReadOnlyList<PhonebookContact>>.Failure(new ServiceError("phonebook.unavailable", "No se pudo conectar con la agenda telefónica."));
        }
        catch (TaskCanceledException)
        {
            return ServiceResult<IReadOnlyList<PhonebookContact>>.Failure(new ServiceError("phonebook.timeout", "Tiempo de espera agotado."));
        }
        catch (JsonException)
        {
            return ServiceResult<IReadOnlyList<PhonebookContact>>.Failure(new ServiceError("phonebook.invalid_payload", "La respuesta de la agenda telefónica no tiene un formato válido."));
        }
        catch (NotSupportedException)
        {
            return ServiceResult<IReadOnlyList<PhonebookContact>>.Failure(new ServiceError("phonebook.invalid_payload", "La respuesta de la agenda telefónica no tiene un formato válido."));
        }
    }

    private HttpRequestMessage CreateAuthorizedRequest()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, ListEndpoint);

        if (sessionService.IsAuthenticated && !string.IsNullOrWhiteSpace(sessionService.CurrentSession?.AccessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue(
                sessionService.CurrentSession!.TokenType,
                sessionService.CurrentSession.AccessToken);
        }

        return request;
    }

    private static PhonebookContact Map(FichaContactoDto dto)
    {
        return new PhonebookContact(
            dto.Id,
            dto.FechaAlta,
            string.IsNullOrWhiteSpace(dto.Nombre) ? "Contacto del club" : dto.Nombre,
            dto.Telefono1,
            dto.Telefono2,
            dto.Email,
            dto.Direccion,
            dto.Observaciones);
    }

    private sealed record FichaContactoDto(
        Guid Id,
        DateTime FechaAlta,
        string? Nombre,
        string? Telefono1,
        string? Telefono2,
        string? Email,
        string? Direccion,
        string? Observaciones);
}
