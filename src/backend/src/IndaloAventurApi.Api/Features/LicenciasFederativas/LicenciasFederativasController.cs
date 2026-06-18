using System.Security.Claims;
using System.Text.Json.Serialization;
using IndaloAventurApi.Application.Abstractions.LicenciasFederativas;
using IndaloAventurApi.Application.Abstractions.Security;
using IndaloAventurApi.Application.Features.LicenciasFederativas.CreateSolicitudLicenciaFederativa;
using IndaloAventurApi.Application.Features.LicenciasFederativas.GetAdminSolicitudesLicenciaFederativa;
using IndaloAventurApi.Application.Features.LicenciasFederativas.GetMiSolicitudLicenciaFederativa;
using IndaloAventurApi.Application.Features.LicenciasFederativas.GetMisSolicitudesLicenciaFederativa;
using IndaloAventurApi.Application.Features.LicenciasFederativas.GetTarifasLicenciasFederativas;
using IndaloAventurApi.Application.Features.LicenciasFederativas.UpdateAdminSolicitudLicenciaFederativaEstado;
using IndaloAventurApi.Domain.LicenciasFederativas;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IndaloAventurApi.Api.Features.LicenciasFederativas;

[ApiController]
[Route("api/licencias-federativas")]
[Authorize]
public sealed class LicenciasFederativasController(IMediator mediator) : ControllerBase
{
    [HttpGet("tarifas")]
    [ProducesResponseType(typeof(IReadOnlyCollection<TarifaLicenciaFederativaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyCollection<TarifaLicenciaFederativaDto>>> GetRates(
        [FromQuery] int? temporada,
        [FromQuery] bool? mediaTemporada,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetTarifasLicenciasFederativasQuery(temporada, mediaTemporada), cancellationToken);
        return Ok(result);
    }

    [HttpPost("me/solicitudes")]
    [ProducesResponseType(typeof(SolicitudLicenciaFederativaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<SolicitudLicenciaFederativaDto>> CreateMyRequest(
        [FromBody] CreateSolicitudLicenciaFederativaRequest request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateSolicitudLicenciaFederativaCommand(
            GetCurrentUserId(),
            GetCurrentUserEmail(),
            GetCurrentUserIsMember(),
            request.Temporada,
            request.TarifaLicenciaFederativaId), cancellationToken);

        return CreatedAtAction(nameof(GetMyRequestById), new { solicitudId = result.Id }, result);
    }

    [HttpGet("me/solicitudes")]
    [ProducesResponseType(typeof(IReadOnlyCollection<SolicitudLicenciaFederativaDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<SolicitudLicenciaFederativaDto>>> GetMyRequests(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetMisSolicitudesLicenciaFederativaQuery(GetCurrentUserId()), cancellationToken);
        return Ok(result);
    }

    [HttpGet("me/solicitudes/{solicitudId:guid}")]
    [ProducesResponseType(typeof(SolicitudLicenciaFederativaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SolicitudLicenciaFederativaDto>> GetMyRequestById(Guid solicitudId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetMiSolicitudLicenciaFederativaQuery(GetCurrentUserId(), solicitudId), cancellationToken);
        return Ok(result);
    }

    [HttpGet("admin/solicitudes")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(IReadOnlyCollection<AdminSolicitudLicenciaFederativaDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<AdminSolicitudLicenciaFederativaDto>>> GetAdminRequests(
        [FromQuery] Guid? userId,
        [FromQuery] int? temporada,
        [FromQuery] EstadoSolicitudLicenciaFederativa? estado,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetAdminSolicitudesLicenciaFederativaQuery(userId, temporada, estado), cancellationToken);
        return Ok(result);
    }

    [HttpPut("admin/users/{userId:guid}/solicitudes/{solicitudId:guid}/estado")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(AdminSolicitudLicenciaFederativaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AdminSolicitudLicenciaFederativaDto>> UpdateAdminRequestStatus(
        Guid userId,
        Guid solicitudId,
        [FromBody] UpdateAdminSolicitudLicenciaFederativaEstadoRequest request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UpdateAdminSolicitudLicenciaFederativaEstadoCommand(userId, solicitudId, request.Estado), cancellationToken);
        return Ok(result);
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

    private string GetCurrentUserEmail()
    {
        var value = User.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new UnauthorizedAccessException("No se pudo resolver el email del usuario autenticado.");
        }

        return value;
    }

    private bool GetCurrentUserIsMember()
    {
        var claimValue = User.FindFirstValue(AuthClaimNames.IsMember);
        if (!bool.TryParse(claimValue, out var isMember))
        {
            throw new UnauthorizedAccessException("No se pudo resolver el estado de socio del usuario autenticado.");
        }

        return isMember;
    }

    public sealed record CreateSolicitudLicenciaFederativaRequest(int Temporada, int TarifaLicenciaFederativaId);
    public sealed record UpdateAdminSolicitudLicenciaFederativaEstadoRequest(
        [property: JsonConverter(typeof(JsonStringEnumConverter))]
        EstadoSolicitudLicenciaFederativa Estado);
}
