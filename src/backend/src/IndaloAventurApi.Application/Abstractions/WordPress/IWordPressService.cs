namespace IndaloAventurApi.Application.Abstractions.WordPress;

public interface IWordPressService
{
    Task<IReadOnlyCollection<WordPressPostSummaryDto>> GetPostsAsync(
        int page,
        int pageSize,
        string? search,
        CancellationToken cancellationToken);

    Task<WordPressPostDto> GetPostBySlugAsync(
        string slug,
        CancellationToken cancellationToken);
}
