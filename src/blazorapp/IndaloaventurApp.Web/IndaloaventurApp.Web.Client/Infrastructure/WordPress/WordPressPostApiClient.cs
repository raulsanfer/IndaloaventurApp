namespace IndaloaventurApp.Web.Client.Infrastructure.WordPress;

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using IndaloaventurApp.SharedUI.Abstractions.Session;
using IndaloaventurApp.SharedUI.Abstractions.WordPress;
using IndaloaventurApp.SharedUI.Models.Common;
using IndaloaventurApp.SharedUI.Models.WordPress;

public sealed class WordPressPostApiClient(HttpClient httpClient, ISessionService sessionService) : IWordPressPostService
{
    private const string ListEndpoint = "/api/wordpress/posts?page=1&pageSize=10";
    private const string DetailEndpointTemplate = "/api/wordpress/posts/{0}";

    public async Task<ServiceResult<IReadOnlyList<WordPressPost>>> GetLatestPostsAsync(CancellationToken cancellationToken = default)
    {
        return await SendAsync(
            ListEndpoint,
            payload => ServiceResult<IReadOnlyList<WordPressPost>>.Success(
                payload.Select(Map).ToArray()),
            cancellationToken);
    }

    public async Task<ServiceResult<WordPressPost>> GetPostBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            return ServiceResult<WordPressPost>.Failure(new ServiceError("wordpress.invalid_slug", "Slug de noticia no válido."));
        }

        var endpoint = string.Format(
            System.Globalization.CultureInfo.InvariantCulture,
            DetailEndpointTemplate,
            Uri.EscapeDataString(slug));

        return await SendAsync(
            endpoint,
            payload => ServiceResult<WordPressPost>.Success(Map(payload)),
            cancellationToken);
    }

    private async Task<ServiceResult<T>> SendAsync<T>(
        string endpoint,
        Func<WordPressPostDto, ServiceResult<T>> mapSingle,
        CancellationToken cancellationToken)
    {
        try
        {
            using var request = CreateAuthorizedRequest(endpoint);
            using var response = await httpClient.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return ServiceResult<T>.Failure(new ServiceError("auth.session_invalid", "Sesión inválida."));
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return ServiceResult<T>.Failure(new ServiceError("wordpress.not_found", "La noticia solicitada no existe."));
            }

            if (!response.IsSuccessStatusCode)
            {
                return ServiceResult<T>.Failure(new ServiceError("wordpress.unavailable", $"Error HTTP {(int)response.StatusCode}."));
            }

            var payload = await response.Content.ReadFromJsonAsync<WordPressPostDto>(cancellationToken: cancellationToken);
            if (payload is null)
            {
                return ServiceResult<T>.Failure(new ServiceError("wordpress.empty", "Respuesta vacía."));
            }

            return mapSingle(payload);
        }
        catch (HttpRequestException)
        {
            return ServiceResult<T>.Failure(new ServiceError("wordpress.unavailable", "No se pudo conectar con las noticias."));
        }
        catch (TaskCanceledException)
        {
            return ServiceResult<T>.Failure(new ServiceError("wordpress.timeout", "Tiempo de espera agotado."));
        }
        catch (JsonException)
        {
            return ServiceResult<T>.Failure(new ServiceError("wordpress.invalid_payload", "La respuesta de noticias no tiene un formato válido."));
        }
        catch (NotSupportedException)
        {
            return ServiceResult<T>.Failure(new ServiceError("wordpress.invalid_payload", "La respuesta de noticias no tiene un formato válido."));
        }
    }

    private async Task<ServiceResult<T>> SendAsync<T>(
        string endpoint,
        Func<IReadOnlyList<WordPressPostDto>, ServiceResult<T>> mapMany,
        CancellationToken cancellationToken)
    {
        try
        {
            using var request = CreateAuthorizedRequest(endpoint);
            using var response = await httpClient.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return ServiceResult<T>.Failure(new ServiceError("auth.session_invalid", "Sesión inválida."));
            }

            if (!response.IsSuccessStatusCode)
            {
                return ServiceResult<T>.Failure(new ServiceError("wordpress.unavailable", $"Error HTTP {(int)response.StatusCode}."));
            }

            var payload = await response.Content.ReadFromJsonAsync<WordPressPostDto[]>(cancellationToken: cancellationToken);
            if (payload is null)
            {
                return ServiceResult<T>.Failure(new ServiceError("wordpress.empty", "Respuesta vacía."));
            }

            return mapMany(payload);
        }
        catch (HttpRequestException)
        {
            return ServiceResult<T>.Failure(new ServiceError("wordpress.unavailable", "No se pudo conectar con las noticias."));
        }
        catch (TaskCanceledException)
        {
            return ServiceResult<T>.Failure(new ServiceError("wordpress.timeout", "Tiempo de espera agotado."));
        }
        catch (JsonException)
        {
            return ServiceResult<T>.Failure(new ServiceError("wordpress.invalid_payload", "La respuesta de noticias no tiene un formato válido."));
        }
        catch (NotSupportedException)
        {
            return ServiceResult<T>.Failure(new ServiceError("wordpress.invalid_payload", "La respuesta de noticias no tiene un formato válido."));
        }
    }

    private HttpRequestMessage CreateAuthorizedRequest(string endpoint)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, endpoint);

        if (sessionService.IsAuthenticated && !string.IsNullOrWhiteSpace(sessionService.CurrentSession?.AccessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue(
                sessionService.CurrentSession!.TokenType,
                sessionService.CurrentSession.AccessToken);
        }

        return request;
    }

    private static WordPressPost Map(WordPressPostDto dto)
    {
        return new WordPressPost(
            dto.Id,
            dto.Slug ?? string.Empty,
            string.IsNullOrWhiteSpace(dto.Titulo) ? "Noticia IndaloAventura" : dto.Titulo,
            dto.Resumen,
            dto.Contenido,
            dto.ImagenDestacadaUrl,
            dto.Enlace,
            dto.FechaPublicacionUtc);
    }

    private sealed record WordPressPostDto(
        long Id,
        string? Slug,
        string? Titulo,
        string? Resumen,
        string? Contenido,
        string? ImagenDestacadaUrl,
        string? Enlace,
        DateTimeOffset FechaPublicacionUtc);
}
