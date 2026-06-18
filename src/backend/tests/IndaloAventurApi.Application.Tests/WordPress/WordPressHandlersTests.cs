using IndaloAventurApi.Application.Abstractions.WordPress;
using IndaloAventurApi.Application.Features.WordPress.GetWordPressPostBySlug;
using IndaloAventurApi.Application.Features.WordPress.GetWordPressPosts;
using Microsoft.Extensions.Options;

namespace IndaloAventurApi.Application.Tests.WordPress;

public sealed class WordPressHandlersTests
{
    [Fact]
    public async Task GetWordPressPosts_ShouldUseConfiguredDefaultPageSize_WhenRequestOmitsIt()
    {
        var wordPressService = new FakeWordPressService();
        var handler = new GetWordPressPostsQueryHandler(
            wordPressService,
            Options.Create(new WordPressOptions { DefaultPostsPageSize = 7 }));

        await handler.Handle(new GetWordPressPostsQuery(1, null, "montana"), CancellationToken.None);

        Assert.Equal(7, wordPressService.LastPageSize);
        Assert.Equal("montana", wordPressService.LastSearch);
    }

    [Fact]
    public async Task GetWordPressPosts_ShouldUseRequestPageSize_WhenProvided()
    {
        var wordPressService = new FakeWordPressService();
        var handler = new GetWordPressPostsQueryHandler(
            wordPressService,
            Options.Create(new WordPressOptions { DefaultPostsPageSize = 7 }));

        await handler.Handle(new GetWordPressPostsQuery(1, 3, null), CancellationToken.None);

        Assert.Equal(3, wordPressService.LastPageSize);
    }

    [Fact]
    public async Task GetWordPressPostBySlug_ShouldDelegateToWordPressService()
    {
        var wordPressService = new FakeWordPressService();
        var handler = new GetWordPressPostBySlugQueryHandler(wordPressService);

        var post = await handler.Handle(new GetWordPressPostBySlugQuery("ruta-de-prueba"), CancellationToken.None);

        Assert.Equal("ruta-de-prueba", post.Slug);
    }

    private sealed class FakeWordPressService : IWordPressService
    {
        public int LastPageSize { get; private set; }
        public string? LastSearch { get; private set; }

        public Task<IReadOnlyCollection<WordPressPostSummaryDto>> GetPostsAsync(int page, int pageSize, string? search, CancellationToken cancellationToken)
        {
            LastPageSize = pageSize;
            LastSearch = search;
            return Task.FromResult<IReadOnlyCollection<WordPressPostSummaryDto>>(
            [
                new WordPressPostSummaryDto(1, "ruta-de-prueba", "Ruta de prueba", "Resumen 1", "https://example.com/ruta-de-prueba.jpg", DateTime.UtcNow)
            ]);
        }

        public Task<WordPressPostDto> GetPostBySlugAsync(string slug, CancellationToken cancellationToken)
            => Task.FromResult(new WordPressPostDto(1, slug, "Ruta de prueba", "Resumen 1", "Contenido 1", "https://example.com/ruta-de-prueba.jpg", "https://example.com/ruta-de-prueba", DateTime.UtcNow));
    }
}
