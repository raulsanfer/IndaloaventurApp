using IndaloAventurApi.Api.Security;
using IndaloAventurApi.Application.Abstractions.ClubPositions;
using IndaloAventurApi.Application.Features.ClubPositions.Cargos.CreateCargo;
using IndaloAventurApi.Application.Features.ClubPositions.Cargos.DeleteCargo;
using IndaloAventurApi.Application.Features.ClubPositions.Cargos.GetAllCargos;
using IndaloAventurApi.Application.Features.ClubPositions.Cargos.UpdateCargo;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IndaloAventurApi.Api.Features.ClubPositions;

[ApiController]
[Route("api/cargos")]
[Authorize(Policy = AuthorizationPolicies.Admin)]
public sealed class CargosController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<CargoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<CargoDto>>> GetAll(CancellationToken cancellationToken)
    {
        var items = await mediator.Send(new GetAllCargosQuery(), cancellationToken);
        return Ok(items);
    }

    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<int>> Create([FromBody] CreateCargoRequest request, CancellationToken cancellationToken)
    {
        var id = await mediator.Send(new CreateCargoCommand(request.Descripcion), cancellationToken);
        return CreatedAtAction(nameof(Create), new { id }, id);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCargoRequest request, CancellationToken cancellationToken)
    {
        await mediator.Send(new UpdateCargoCommand(id, request.Descripcion), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteCargoCommand(id), cancellationToken);
        return NoContent();
    }

    public sealed record CreateCargoRequest(string Descripcion);
    public sealed record UpdateCargoRequest(string Descripcion);
}
