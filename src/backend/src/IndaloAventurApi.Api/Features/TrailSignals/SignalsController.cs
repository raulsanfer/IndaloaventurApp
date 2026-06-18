using System.Security.Claims;
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

[ApiController]
[Route("api/signals")]
[Authorize]
public sealed class SignalsController(IMediator mediator) : ControllerBase
{
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

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SignalDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SignalDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetSignalByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}/images")]
    [ProducesResponseType(typeof(SignalImagesDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SignalImagesDto>> GetImages(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetSignalImagesQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}/comments")]
    [ProducesResponseType(typeof(IReadOnlyCollection<SignalCommentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyCollection<SignalCommentDto>>> GetComments(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetSignalCommentsQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Member")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateSignalRequest request, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
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

    [HttpPost("{id:guid}/comments")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Guid>> CreateComment(Guid id, [FromBody] CreateSignalCommentRequest request, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var commentId = await mediator.Send(new CreateSignalCommentCommand(id, userId, request.Texto), cancellationToken);
        return CreatedAtAction(nameof(GetComments), new { id }, commentId);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Member")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSignalRequest request, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        await mediator.Send(new UpdateSignalCommand(
            id,
            request.Titulo,
            request.Descripcion,
            request.Activo,
            userId), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin,Member")]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public IActionResult Delete(Guid id)
    {
        throw new InvalidOperationException("La eliminacion de senales no esta permitida.");
    }

    private Guid GetCurrentUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(value, out var userId))
        {
            throw new UnauthorizedAccessException("No se pudo resolver el usuario autenticado.");
        }

        return userId;
    }

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

    public sealed record UpdateSignalRequest(
        string Titulo,
        string Descripcion,
        bool Activo);

    public sealed record CreateSignalCommentRequest(string Texto);
}
