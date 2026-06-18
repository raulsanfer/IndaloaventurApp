using System.Net.Http.Json;
using IndaloAventurApi.Application.Abstractions.WordPress;
using Microsoft.Extensions.Options;

namespace IndaloAventurApi.Infrastructure.WordPress;

public sealed class WordPressService(HttpClient httpClient, IOptions<WordPressOptions> options) : IWordPressService
{
    private readonly WordPressOptions _options = options.Value;

    public async Task<IReadOnlyCollection<WordPressPostSummaryDto>> GetPostsAsync(int page, int pageSize, string? search, CancellationToken cancellationToken)
    {
        var query = new List<string>
        {
            $"page={page}",
            $"per_page={pageSize}",
            "status=publish",
            "orderby=date",
            "order=desc",
            "_embed=wp:featuredmedia",
            "_fields=id,slug,date_gmt,title.rendered,excerpt.rendered,_links,_embedded"
        };

        if (!string.IsNullOrWhiteSpace(search))
        {
            query.Add($"search={Uri.EscapeDataString(search)}");
        }

        var payload = await GetPostsPayloadAsync(query, cancellationToken);
        return payload.Select(ToSummaryDto).ToArray();
    }

    public async Task<WordPressPostDto> GetPostBySlugAsync(string slug, CancellationToken cancellationToken)
    {
        var payload = await GetPostsPayloadAsync([$"slug={Uri.EscapeDataString(slug)}", "_embed=true"], cancellationToken);
        var post = payload.FirstOrDefault(x => string.Equals(x.Slug, slug, StringComparison.OrdinalIgnoreCase));
        if (post is null)
        {
            throw new KeyNotFoundException("El post de WordPress no existe.");
        }

        return ToDetailDto(post);
    }

    private async Task<List<WordPressPostPayload>> GetPostsPayloadAsync(IEnumerable<string> query, CancellationToken cancellationToken)
    {
        var uri = $"{_options.PostsEndpoint}?{string.Join("&", query)}";

        HttpResponseMessage response;
        try
        {
            response = await httpClient.GetAsync(uri, cancellationToken);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            throw new InvalidOperationException("La consulta a WordPress ha superado el tiempo de espera.");
        }
        catch (HttpRequestException)
        {
            throw new InvalidOperationException("No se pudo conectar con WordPress.");
        }

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"WordPress devolvio un error remoto ({(int)response.StatusCode}).");
        }

        return await response.Content.ReadFromJsonAsync<List<WordPressPostPayload>>(cancellationToken: cancellationToken) ?? [];
    }

    private static WordPressPostSummaryDto ToSummaryDto(WordPressPostPayload payload)
    {
        var title = payload.Title?.Rendered?.Trim() ?? string.Empty;
        var excerpt = payload.Excerpt?.Rendered?.Trim() ?? string.Empty;
        var featuredImageUrl = payload.Embedded?.FeaturedMedia?.FirstOrDefault()?.SourceUrl?.Trim();
        return new WordPressPostSummaryDto(
            payload.Id,
            payload.Slug ?? string.Empty,
            title,
            excerpt,
            string.IsNullOrWhiteSpace(featuredImageUrl) ? null : featuredImageUrl,
            payload.DateGmt);
    }

    private static WordPressPostDto ToDetailDto(WordPressPostPayload payload)
    {
        var title = payload.Title?.Rendered?.Trim() ?? string.Empty;
        var excerpt = payload.Excerpt?.Rendered?.Trim() ?? string.Empty;
        var content = payload.Content?.Rendered?.Trim() ?? string.Empty;
        var featuredImageUrl = payload.Embedded?.FeaturedMedia?.FirstOrDefault()?.SourceUrl?.Trim();
        return new WordPressPostDto(
            payload.Id,
            payload.Slug ?? string.Empty,
            title,
            excerpt,
            content,
            string.IsNullOrWhiteSpace(featuredImageUrl) ? null : featuredImageUrl,
            payload.Link ?? string.Empty,
            payload.DateGmt);
    }

    private sealed class WordPressPostPayload
    {
        public long Id { get; init; }
        public string? Slug { get; init; }
        public string? Link { get; init; }
        [System.Text.Json.Serialization.JsonPropertyName("date_gmt")]
        public DateTime DateGmt { get; init; }
        public RenderedField? Title { get; init; }
        public RenderedField? Excerpt { get; init; }
        public RenderedField? Content { get; init; }
        [System.Text.Json.Serialization.JsonPropertyName("_embedded")]
        public EmbeddedPayload? Embedded { get; init; }
    }

    private sealed class EmbeddedPayload
    {
        [System.Text.Json.Serialization.JsonPropertyName("wp:featuredmedia")]
        public List<FeaturedMediaPayload>? FeaturedMedia { get; init; }
    }

    private sealed class FeaturedMediaPayload
    {
        [System.Text.Json.Serialization.JsonPropertyName("source_url")]
        public string? SourceUrl { get; init; }
    }

    private sealed class RenderedField
    {
        public string? Rendered { get; init; }
    }
}
