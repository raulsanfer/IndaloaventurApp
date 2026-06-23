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

/// <summary>
/// Expone operaciones para consultar y mantener la agenda telefonica del club.
/// </summary>
[ApiController]
[Route("api/agenda-telefonica")]
[Authorize(Policy = AuthorizationPolicies.Authenticated)]
public sealed class AgendaTelefonicaController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Devuelve todas las fichas de contacto visibles para el usuario autenticado.
    /// </summary>
    /// <param name="cancellationToken">Token para cancelar la consulta.</param>
    /// <returns>Coleccion de fichas de contacto registradas en la agenda.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<FichaContactoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<FichaContactoDto>>> List(CancellationToken cancellationToken)
    {
        var fichas = await mediator.Send(new ListFichasContactoQuery(), cancellationToken);
        return Ok(fichas);
    }

    /// <summary>
    /// Obtiene una ficha de contacto concreta a partir de su identificador.
    /// </summary>
    /// <param name="id">Identificador unico de la ficha de contacto.</param>
    /// <param name="cancellationToken">Token para cancelar la consulta.</param>
    /// <returns>La ficha de contacto encontrada.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(FichaContactoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FichaContactoDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var ficha = await mediator.Send(new GetFichaContactoByIdQuery(id), cancellationToken);
        return Ok(ficha);
    }

    /// <summary>
    /// Crea una nueva ficha de contacto en la agenda.
    /// </summary>
    /// <param name="command">Datos necesarios para crear la ficha de contacto.</param>
    /// <param name="cancellationToken">Token para cancelar la operacion.</param>
    /// <returns>Identificador generado para la nueva ficha.</returns>
    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.Admin)]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateFichaContactoCommand command, CancellationToken cancellationToken)
    {
        var id = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    /// <summary>
    /// Actualiza los datos de una ficha de contacto existente.
    /// </summary>
    /// <param name="id">Identificador unico de la ficha que se desea modificar.</param>
    /// <param name="request">Campos editables de la ficha de contacto.</param>
    /// <param name="cancellationToken">Token para cancelar la operacion.</param>
    /// <returns>Respuesta sin contenido cuando la actualizacion se completa correctamente.</returns>
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

    /// <summary>
    /// Elimina una ficha de contacto de la agenda.
    /// </summary>
    /// <param name="id">Identificador unico de la ficha que se desea eliminar.</param>
    /// <param name="cancellationToken">Token para cancelar la operacion.</param>
    /// <returns>Respuesta sin contenido cuando la eliminacion se completa correctamente.</returns>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = AuthorizationPolicies.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteFichaContactoCommand(id), cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Datos editables de una ficha de contacto.
    /// </summary>
    /// <param name="Nombre">Nombre visible del contacto.</param>
    /// <param name="Telefono1">Telefono principal del contacto.</param>
    /// <param name="Telefono2">Telefono secundario opcional.</param>
    /// <param name="Email">Direccion de correo electronico opcional.</param>
    /// <param name="Direccion">Direccion postal opcional.</param>
    /// <param name="Observaciones">Observaciones adicionales opcionales.</param>
    public sealed record UpdateFichaContactoRequest(string Nombre, string Telefono1, string? Telefono2, string? Email, string? Direccion, string? Observaciones);
}
