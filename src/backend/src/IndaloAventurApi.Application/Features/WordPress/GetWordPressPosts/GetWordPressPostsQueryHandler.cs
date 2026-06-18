using IndaloAventurApi.Application.Abstractions.WordPress;
using MediatR;
using Microsoft.Extensions.Options;

namespace IndaloAventurApi.Application.Features.WordPress.GetWordPressPosts;

public sealed class GetWordPressPostsQueryHandler(
    IWordPressService wordPressService,
    IOptions<WordPressOptions> options)
    : IRequestHandler<GetWordPressPostsQuery, IReadOnlyCollection<WordPressPostSummaryDto>>
{
    private readonly WordPressOptions _options = options.Value;

    public Task<IReadOnlyCollection<WordPressPostSummaryDto>> Handle(GetWordPressPostsQuery request, CancellationToken cancellationToken)
    {
        var pageSize = request.PageSize ?? _options.DefaultPostsPageSize;
        return wordPressService.GetPostsAsync(request.Page, pageSize, request.Search, cancellationToken);
    }
}
