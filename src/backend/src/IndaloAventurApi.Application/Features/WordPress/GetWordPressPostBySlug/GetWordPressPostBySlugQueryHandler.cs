using IndaloAventurApi.Application.Abstractions.WordPress;
using MediatR;

namespace IndaloAventurApi.Application.Features.WordPress.GetWordPressPostBySlug;

public sealed class GetWordPressPostBySlugQueryHandler(IWordPressService wordPressService)
    : IRequestHandler<GetWordPressPostBySlugQuery, WordPressPostDto>
{
    public Task<WordPressPostDto> Handle(GetWordPressPostBySlugQuery request, CancellationToken cancellationToken)
    {
        return wordPressService.GetPostBySlugAsync(request.Slug, cancellationToken);
    }
}
