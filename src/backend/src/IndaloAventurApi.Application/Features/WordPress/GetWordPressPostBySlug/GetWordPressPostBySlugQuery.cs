using IndaloAventurApi.Application.Abstractions.Cqrs;
using IndaloAventurApi.Application.Abstractions.WordPress;

namespace IndaloAventurApi.Application.Features.WordPress.GetWordPressPostBySlug;

public sealed record GetWordPressPostBySlugQuery(string Slug) : IQuery<WordPressPostDto>;
