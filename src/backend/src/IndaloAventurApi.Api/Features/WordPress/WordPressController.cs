using IndaloAventurApi.Api.Security;
using IndaloAventurApi.Application.Abstractions.WordPress;
using IndaloAventurApi.Application.Features.WordPress.GetWordPressPostBySlug;
using IndaloAventurApi.Application.Features.WordPress.GetWordPressPosts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IndaloAventurApi.Api.Features.WordPress;

[ApiController]
[Route("api/wordpress")]
[Authorize(Policy = AuthorizationPolicies.Authenticated)]
public sealed class WordPressController(IMediator mediator) : ControllerBase
{
    [HttpGet("posts")]
    [ProducesResponseType(typeof(IReadOnlyCollection<WordPressPostSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyCollection<WordPressPostSummaryDto>>> GetPosts(
        [FromQuery] int page = 1,
        [FromQuery] int? pageSize = null,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
    {
        var items = await mediator.Send(new GetWordPressPostsQuery(page, pageSize, search), cancellationToken);
        return Ok(items);
    }

    [HttpGet("posts/{slug}")]
    [ProducesResponseType(typeof(WordPressPostDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WordPressPostDto>> GetPostBySlug(
        string slug,
        CancellationToken cancellationToken = default)
    {
        var item = await mediator.Send(new GetWordPressPostBySlugQuery(slug), cancellationToken);
        return Ok(item);
    }
}
