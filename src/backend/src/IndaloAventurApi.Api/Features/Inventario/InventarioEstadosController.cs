using IndaloAventurApi.Api.Security;
using IndaloAventurApi.Application.Abstractions.Inventario;
using IndaloAventurApi.Application.Features.Inventario.InventarioEstados.CreateInventarioEstado;
using IndaloAventurApi.Application.Features.Inventario.InventarioEstados.DeleteInventarioEstado;
using IndaloAventurApi.Application.Features.Inventario.InventarioEstados.GetAllInventarioEstados;
using IndaloAventurApi.Application.Features.Inventario.InventarioEstados.GetInventarioEstadoById;
using IndaloAventurApi.Application.Features.Inventario.InventarioEstados.UpdateInventarioEstado;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

#pragma warning disable CS1591

namespace IndaloAventurApi.Api.Features.Inventario;

/// <summary>
/// Administra el catalogo de estados de inventario.
/// </summary>
[ApiController]
[Route("api/inventario-estados")]
[Authorize(Policy = AuthorizationPolicies.Authenticated)]
public sealed class InventarioEstadosController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<InventarioEstadoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<InventarioEstadoDto>>> GetAll(CancellationToken cancellationToken)
        => Ok(await mediator.Send(new GetAllInventarioEstadosQuery(), cancellationToken));

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(InventarioEstadoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<InventarioEstadoDto>> GetById(int id, CancellationToken cancellationToken)
        => Ok(await mediator.Send(new GetInventarioEstadoByIdQuery(id), cancellationToken));

    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.InventoryManagement)]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<int>> Create([FromBody] CreateInventarioEstadoRequest request, CancellationToken cancellationToken)
    {
        var id = await mediator.Send(new CreateInventarioEstadoCommand(request.Nombre), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = AuthorizationPolicies.InventoryManagement)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateInventarioEstadoRequest request, CancellationToken cancellationToken)
    {
        await mediator.Send(new UpdateInventarioEstadoCommand(id, request.Nombre), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = AuthorizationPolicies.InventoryManagement)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteInventarioEstadoCommand(id), cancellationToken);
        return NoContent();
    }

    public sealed record CreateInventarioEstadoRequest(string Nombre);
    public sealed record UpdateInventarioEstadoRequest(string Nombre);
}

#pragma warning restore CS1591
