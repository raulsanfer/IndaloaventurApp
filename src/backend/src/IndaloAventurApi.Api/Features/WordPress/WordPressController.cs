using IndaloAventurApi.Api.Security;
using IndaloAventurApi.Application.Abstractions.WordPress;
using IndaloAventurApi.Application.Features.WordPress.GetWordPressPostBySlug;
using IndaloAventurApi.Application.Features.WordPress.GetWordPressPosts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IndaloAventurApi.Api.Features.WordPress;

/// <summary>
/// Expone contenido publicado en WordPress para su consumo desde la aplicacion.
/// </summary>
[ApiController]
[Route("api/wordpress")]
[Authorize(Policy = AuthorizationPolicies.Authenticated)]
public sealed class WordPressController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Recupera un listado paginado de entradas de WordPress.
    /// </summary>
    /// <param name="page">Numero de pagina solicitado.</param>
    /// <param name="pageSize">Tamano de pagina deseado.</param>
    /// <param name="search">Texto opcional para filtrar entradas.</param>
    /// <param name="cancellationToken">Token para cancelar la consulta.</param>
    /// <returns>Coleccion paginada de entradas resumidas.</returns>
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

    /// <summary>
    /// Recupera el detalle completo de una entrada de WordPress por su slug.
    /// </summary>
    /// <param name="slug">Slug publico de la entrada a consultar.</param>
    /// <param name="cancellationToken">Token para cancelar la consulta.</param>
    /// <returns>Entrada de WordPress correspondiente al slug indicado.</returns>
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
