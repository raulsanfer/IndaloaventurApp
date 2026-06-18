using IndaloAventurApi.Application.Abstractions.Cqrs;
using IndaloAventurApi.Application.Abstractions.WordPress;

namespace IndaloAventurApi.Application.Features.WordPress.GetWordPressPosts;

public sealed record GetWordPressPostsQuery(
    int Page = 1,
    int? PageSize = null,
    string? Search = null) : IQuery<IReadOnlyCollection<WordPressPostSummaryDto>>;
