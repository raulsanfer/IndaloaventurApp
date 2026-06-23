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

/// <summary>
/// Administra el catalogo de cargos del club.
/// </summary>
[ApiController]
[Route("api/cargos")]
[Authorize(Policy = AuthorizationPolicies.Admin)]
public sealed class CargosController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Lista todos los cargos disponibles.
    /// </summary>
    /// <param name="cancellationToken">Token para cancelar la consulta.</param>
    /// <returns>Coleccion completa de cargos.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<CargoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<CargoDto>>> GetAll(CancellationToken cancellationToken)
    {
        var items = await mediator.Send(new GetAllCargosQuery(), cancellationToken);
        return Ok(items);
    }

    /// <summary>
    /// Crea un nuevo cargo en el catalogo del club.
    /// </summary>
    /// <param name="request">Descripcion del cargo que se desea crear.</param>
    /// <param name="cancellationToken">Token para cancelar la operacion.</param>
    /// <returns>Identificador del cargo creado.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<int>> Create([FromBody] CreateCargoRequest request, CancellationToken cancellationToken)
    {
        var id = await mediator.Send(new CreateCargoCommand(request.Descripcion), cancellationToken);
        return CreatedAtAction(nameof(Create), new { id }, id);
    }

    /// <summary>
    /// Actualiza la descripcion de un cargo existente.
    /// </summary>
    /// <param name="id">Identificador del cargo a modificar.</param>
    /// <param name="request">Datos editables del cargo.</param>
    /// <param name="cancellationToken">Token para cancelar la operacion.</param>
    /// <returns>Respuesta sin contenido cuando la actualizacion se completa correctamente.</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCargoRequest request, CancellationToken cancellationToken)
    {
        await mediator.Send(new UpdateCargoCommand(id, request.Descripcion), cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Elimina un cargo del catalogo del club.
    /// </summary>
    /// <param name="id">Identificador del cargo a eliminar.</param>
    /// <param name="cancellationToken">Token para cancelar la operacion.</param>
    /// <returns>Respuesta sin contenido cuando la eliminacion se completa correctamente.</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteCargoCommand(id), cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Datos necesarios para crear un cargo.
    /// </summary>
    /// <param name="Descripcion">Nombre o descripcion del cargo.</param>
    public sealed record CreateCargoRequest(string Descripcion);

    /// <summary>
    /// Datos editables de un cargo existente.
    /// </summary>
    /// <param name="Descripcion">Nombre o descripcion actualizada del cargo.</param>
    public sealed record UpdateCargoRequest(string Descripcion);
}
