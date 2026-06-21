using IndaloAventurApi.Api.Security;
using IndaloAventurApi.Application.Abstractions.TrailSignals;
using IndaloAventurApi.Application.Features.TrailSignals.SignalTypes.CreateSignalType;
using IndaloAventurApi.Application.Features.TrailSignals.SignalTypes.DeleteSignalType;
using IndaloAventurApi.Application.Features.TrailSignals.SignalTypes.GetAllSignalTypes;
using IndaloAventurApi.Application.Features.TrailSignals.SignalTypes.UpdateSignalType;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IndaloAventurApi.Api.Features.TrailSignals;

[ApiController]
[Route("api/signal-types")]
[Authorize(Policy = AuthorizationPolicies.Authenticated)]
public sealed class SignalTypesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<SignalTypeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<SignalTypeDto>>> GetAll(CancellationToken cancellationToken)
    {
        var items = await mediator.Send(new GetAllSignalTypesQuery(), cancellationToken);
        return Ok(items);
    }

    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.Admin)]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<int>> Create([FromBody] CreateSignalTypeRequest request, CancellationToken cancellationToken)
    {
        var id = await mediator.Send(new CreateSignalTypeCommand(request.Nombre, request.Icono), cancellationToken);
        return CreatedAtAction(nameof(Create), new { id }, id);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = AuthorizationPolicies.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateSignalTypeRequest request, CancellationToken cancellationToken)
    {
        await mediator.Send(new UpdateSignalTypeCommand(id, request.Nombre, request.Icono), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = AuthorizationPolicies.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteSignalTypeCommand(id), cancellationToken);
        return NoContent();
    }

    public sealed record CreateSignalTypeRequest(string Nombre, string Icono);
    public sealed record UpdateSignalTypeRequest(string Nombre, string Icono);
}
