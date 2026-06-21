using IndaloAventurApi.Api.Security;
using IndaloAventurApi.Application.Abstractions.Phonebook;
using IndaloAventurApi.Application.Features.AgendaTelefonica.CreateFichaContacto;
using IndaloAventurApi.Application.Features.AgendaTelefonica.DeleteFichaContacto;
using IndaloAventurApi.Application.Features.AgendaTelefonica.GetFichaContactoById;
using IndaloAventurApi.Application.Features.AgendaTelefonica.ListFichasContacto;
using IndaloAventurApi.Application.Features.AgendaTelefonica.UpdateFichaContacto;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IndaloAventurApi.Api.Features.AgendaTelefonica;

[ApiController]
[Route("api/agenda-telefonica")]
[Authorize(Policy = AuthorizationPolicies.Authenticated)]
public sealed class AgendaTelefonicaController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<FichaContactoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<FichaContactoDto>>> List(CancellationToken cancellationToken)
    {
        var fichas = await mediator.Send(new ListFichasContactoQuery(), cancellationToken);
        return Ok(fichas);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(FichaContactoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FichaContactoDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var ficha = await mediator.Send(new GetFichaContactoByIdQuery(id), cancellationToken);
        return Ok(ficha);
    }

    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.Admin)]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateFichaContactoCommand command, CancellationToken cancellationToken)
    {
        var id = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = AuthorizationPolicies.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateFichaContactoRequest request, CancellationToken cancellationToken)
    {
        await mediator.Send(new UpdateFichaContactoCommand(id, request.Nombre, request.Telefono1, request.Telefono2, request.Email, request.Direccion, request.Observaciones), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = AuthorizationPolicies.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteFichaContactoCommand(id), cancellationToken);
        return NoContent();
    }

    public sealed record UpdateFichaContactoRequest(string Nombre, string Telefono1, string? Telefono2, string? Email, string? Direccion, string? Observaciones);
}
