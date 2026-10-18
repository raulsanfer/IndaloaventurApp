using IndaloAventurApi.Api.Security;
using IndaloAventurApi.Application.Abstractions.Inventario;
using IndaloAventurApi.Application.Features.Inventario.InventarioCategorias.CreateInventarioCategoria;
using IndaloAventurApi.Application.Features.Inventario.InventarioCategorias.DeleteInventarioCategoria;
using IndaloAventurApi.Application.Features.Inventario.InventarioCategorias.GetAllInventarioCategorias;
using IndaloAventurApi.Application.Features.Inventario.InventarioCategorias.GetInventarioCategoriaById;
using IndaloAventurApi.Application.Features.Inventario.InventarioCategorias.UpdateInventarioCategoria;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

#pragma warning disable CS1591

namespace IndaloAventurApi.Api.Features.Inventario;

/// <summary>
/// Administra el catalogo jerarquico de categorias de inventario.
/// </summary>
[ApiController]
[Route("api/inventario-categorias")]
[Authorize(Policy = AuthorizationPolicies.Authenticated)]
public sealed class InventarioCategoriasController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<InventarioCategoriaDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<InventarioCategoriaDto>>> GetAll(CancellationToken cancellationToken)
        => Ok(await mediator.Send(new GetAllInventarioCategoriasQuery(), cancellationToken));

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(InventarioCategoriaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<InventarioCategoriaDto>> GetById(int id, CancellationToken cancellationToken)
        => Ok(await mediator.Send(new GetInventarioCategoriaByIdQuery(id), cancellationToken));

    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.Admin)]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<int>> Create([FromBody] CreateInventarioCategoriaRequest request, CancellationToken cancellationToken)
    {
        var id = await mediator.Send(new CreateInventarioCategoriaCommand(request.Nombre, request.CategoriaPadreId), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = AuthorizationPolicies.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateInventarioCategoriaRequest request, CancellationToken cancellationToken)
    {
        await mediator.Send(new UpdateInventarioCategoriaCommand(id, request.Nombre, request.CategoriaPadreId), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = AuthorizationPolicies.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteInventarioCategoriaCommand(id), cancellationToken);
        return NoContent();
    }

    public sealed record CreateInventarioCategoriaRequest(string Nombre, int? CategoriaPadreId);
    public sealed record UpdateInventarioCategoriaRequest(string Nombre, int? CategoriaPadreId);
}

#pragma warning restore CS1591
