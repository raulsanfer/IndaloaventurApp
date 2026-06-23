using IndaloAventurApi.Api.Security;
using IndaloAventurApi.Application.Abstractions.TrailSignals;
using IndaloAventurApi.Application.Features.TrailSignals.Signals.CreateSignal;
using IndaloAventurApi.Application.Features.TrailSignals.Signals.CreateSignalComment;
using IndaloAventurApi.Application.Features.TrailSignals.Signals.GetSignalById;
using IndaloAventurApi.Application.Features.TrailSignals.Signals.GetSignalImages;
using IndaloAventurApi.Application.Features.TrailSignals.Signals.GetSignalComments;
using IndaloAventurApi.Application.Features.TrailSignals.Signals.SearchSignals;
using IndaloAventurApi.Application.Features.TrailSignals.Signals.UpdateSignal;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IndaloAventurApi.Api.Features.TrailSignals;

/// <summary>
/// Gestiona la consulta y mantenimiento de senales de senderos.
/// </summary>
[ApiController]
[Route("api/signals")]
[Authorize(Policy = AuthorizationPolicies.Authenticated)]
public sealed class SignalsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Busca senales aplicando filtros opcionales sobre etiquetas, estado, descripcion y tipo.
    /// </summary>
    /// <param name="tags">Etiquetas a buscar en la senal.</param>
    /// <param name="activo">Filtra por estado activa o inactiva.</param>
    /// <param name="descripcion">Texto a buscar en la descripcion.</param>
    /// <param name="tipo">Identificador del tipo de senal.</param>
    /// <param name="cancellationToken">Token para cancelar la consulta.</param>
    /// <returns>Coleccion de senales que cumplen los filtros indicados.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<SignalDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<SignalDto>>> Search(
        [FromQuery] string? tags,
        [FromQuery] bool? activo,
        [FromQuery] string? descripcion,
        [FromQuery] int? tipo,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new SearchSignalsQuery(tags, activo, descripcion, tipo), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Recupera el detalle de una senal concreta.
    /// </summary>
    /// <param name="id">Identificador unico de la senal.</param>
    /// <param name="cancellationToken">Token para cancelar la consulta.</param>
    /// <returns>Detalle completo de la senal solicitada.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SignalDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SignalDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetSignalByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Recupera las imagenes asociadas a una senal.
    /// </summary>
    /// <param name="id">Identificador unico de la senal.</param>
    /// <param name="cancellationToken">Token para cancelar la consulta.</param>
    /// <returns>Imagen principal y secundaria asociadas a la senal.</returns>
    [HttpGet("{id:guid}/images")]
    [ProducesResponseType(typeof(SignalImagesDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SignalImagesDto>> GetImages(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetSignalImagesQuery(id), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Lista el historial de comentarios asociados a una senal.
    /// </summary>
    /// <param name="id">Identificador unico de la senal.</param>
    /// <param name="cancellationToken">Token para cancelar la consulta.</param>
    /// <returns>Coleccion cronologica de comentarios de la senal.</returns>
    [HttpGet("{id:guid}/comments")]
    [ProducesResponseType(typeof(IReadOnlyCollection<SignalCommentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyCollection<SignalCommentDto>>> GetComments(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetSignalCommentsQuery(id), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Crea una nueva senal de sendero.
    /// </summary>
    /// <param name="request">Datos descriptivos, geograficos y multimedia de la senal.</param>
    /// <param name="cancellationToken">Token para cancelar la operacion.</param>
    /// <returns>Identificador generado para la nueva senal.</returns>
    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.MemberOrAdmin)]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateSignalRequest request, CancellationToken cancellationToken)
    {
        var userId = User.GetRequiredUserId();
        var id = await mediator.Send(new CreateSignalCommand(
            request.Latitud,
            request.Longitud,
            request.Titulo,
            request.Descripcion,
            request.Foto1,
            request.Foto2,
            request.Activo,
            userId,
            request.Tipo,
            request.Tags), cancellationToken);

        return CreatedAtAction(nameof(Search), new { id }, id);
    }

    /// <summary>
    /// Registra un nuevo comentario sobre una senal existente.
    /// </summary>
    /// <param name="id">Identificador de la senal comentada.</param>
    /// <param name="request">Texto del comentario a registrar.</param>
    /// <param name="cancellationToken">Token para cancelar la operacion.</param>
    /// <returns>Identificador generado para el comentario.</returns>
    [HttpPost("{id:guid}/comments")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Guid>> CreateComment(Guid id, [FromBody] CreateSignalCommentRequest request, CancellationToken cancellationToken)
    {
        var userId = User.GetRequiredUserId();
        var commentId = await mediator.Send(new CreateSignalCommentCommand(id, userId, request.Texto), cancellationToken);
        return CreatedAtAction(nameof(GetComments), new { id }, commentId);
    }

    /// <summary>
    /// Actualiza los datos editables de una senal existente.
    /// </summary>
    /// <param name="id">Identificador de la senal a modificar.</param>
    /// <param name="request">Datos actualizados de la senal.</param>
    /// <param name="cancellationToken">Token para cancelar la operacion.</param>
    /// <returns>Respuesta sin contenido cuando la actualizacion se completa correctamente.</returns>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = AuthorizationPolicies.MemberOrAdmin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSignalRequest request, CancellationToken cancellationToken)
    {
        var userId = User.GetRequiredUserId();
        await mediator.Send(new UpdateSignalCommand(
            id,
            request.Titulo,
            request.Descripcion,
            request.Activo,
            userId), cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Indica explicitamente que la eliminacion fisica de senales no esta soportada.
    /// </summary>
    /// <param name="id">Identificador de la senal cuya eliminacion se solicita.</param>
    /// <returns>Siempre produce un error de operacion no valida.</returns>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = AuthorizationPolicies.MemberOrAdmin)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public IActionResult Delete(Guid id)
    {
        throw new InvalidOperationException("La eliminacion de senales no esta permitida.");
    }

    /// <summary>
    /// Datos necesarios para crear una nueva senal.
    /// </summary>
    /// <param name="Latitud">Latitud geografica de la senal.</param>
    /// <param name="Longitud">Longitud geografica de la senal.</param>
    /// <param name="Titulo">Titulo visible de la senal.</param>
    /// <param name="Descripcion">Descripcion funcional o contextual de la senal.</param>
    /// <param name="Foto1">Imagen principal codificada como bytes.</param>
    /// <param name="Foto2">Imagen secundaria opcional codificada como bytes.</param>
    /// <param name="Activo">Indica si la senal esta activa.</param>
    /// <param name="Tipo">Identificador del tipo de senal.</param>
    /// <param name="Tags">Etiquetas asociadas para busqueda o clasificacion.</param>
    public sealed record CreateSignalRequest(
        float Latitud,
        float Longitud,
        string Titulo,
        string Descripcion,
        byte[] Foto1,
        byte[]? Foto2,
        bool Activo,
        int Tipo,
        string Tags);

    /// <summary>
    /// Datos editables de una senal existente.
    /// </summary>
    /// <param name="Titulo">Titulo visible de la senal.</param>
    /// <param name="Descripcion">Descripcion funcional o contextual de la senal.</param>
    /// <param name="Activo">Indica si la senal debe quedar activa.</param>
    public sealed record UpdateSignalRequest(
        string Titulo,
        string Descripcion,
        bool Activo);

    /// <summary>
    /// Datos necesarios para registrar un comentario sobre una senal.
    /// </summary>
    /// <param name="Texto">Contenido textual del comentario.</param>
    public sealed record CreateSignalCommentRequest(string Texto);
}
