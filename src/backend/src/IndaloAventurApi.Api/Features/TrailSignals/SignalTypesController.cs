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

/// <summary>
/// Administra el catalogo de tipos de senal disponible en la aplicacion.
/// </summary>
[ApiController]
[Route("api/signal-types")]
[Authorize(Policy = AuthorizationPolicies.Authenticated)]
public sealed class SignalTypesController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Lista todos los tipos de senal disponibles.
    /// </summary>
    /// <param name="cancellationToken">Token para cancelar la consulta.</param>
    /// <returns>Coleccion completa de tipos de senal.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<SignalTypeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<SignalTypeDto>>> GetAll(CancellationToken cancellationToken)
    {
        var items = await mediator.Send(new GetAllSignalTypesQuery(), cancellationToken);
        return Ok(items);
    }

    /// <summary>
    /// Crea un nuevo tipo de senal.
    /// </summary>
    /// <param name="request">Nombre e icono del tipo de senal.</param>
    /// <param name="cancellationToken">Token para cancelar la operacion.</param>
    /// <returns>Identificador del tipo de senal creado.</returns>
    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.Admin)]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<int>> Create([FromBody] CreateSignalTypeRequest request, CancellationToken cancellationToken)
    {
        var id = await mediator.Send(new CreateSignalTypeCommand(request.Nombre, request.Icono), cancellationToken);
        return CreatedAtAction(nameof(Create), new { id }, id);
    }

    /// <summary>
    /// Actualiza un tipo de senal existente.
    /// </summary>
    /// <param name="id">Identificador del tipo de senal a modificar.</param>
    /// <param name="request">Nombre e icono actualizados.</param>
    /// <param name="cancellationToken">Token para cancelar la operacion.</param>
    /// <returns>Respuesta sin contenido cuando la actualizacion se completa correctamente.</returns>
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

    /// <summary>
    /// Elimina un tipo de senal del catalogo.
    /// </summary>
    /// <param name="id">Identificador del tipo de senal a eliminar.</param>
    /// <param name="cancellationToken">Token para cancelar la operacion.</param>
    /// <returns>Respuesta sin contenido cuando la eliminacion se completa correctamente.</returns>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = AuthorizationPolicies.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteSignalTypeCommand(id), cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Datos necesarios para crear un tipo de senal.
    /// </summary>
    /// <param name="Nombre">Nombre visible del tipo de senal.</param>
    /// <param name="Icono">Icono o clave grafica asociada al tipo.</param>
    public sealed record CreateSignalTypeRequest(string Nombre, string Icono);

    /// <summary>
    /// Datos editables de un tipo de senal existente.
    /// </summary>
    /// <param name="Nombre">Nombre visible actualizado del tipo de senal.</param>
    /// <param name="Icono">Icono o clave grafica actualizada.</param>
    public sealed record UpdateSignalTypeRequest(string Nombre, string Icono);
}
