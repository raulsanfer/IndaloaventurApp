using System.Text.Json.Serialization;
using IndaloAventurApi.Api.Security;
using IndaloAventurApi.Application.Abstractions.LicenciasFederativas;
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
[Authorize(Policy = AuthorizationPolicies.Authenticated)]
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
    [Authorize(Policy = AuthorizationPolicies.ClubMember)]
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
            User.GetRequiredUserId(),
            User.GetRequiredEmail(),
            User.GetRequiredIsMember(),
            request.Temporada,
            request.TarifaLicenciaFederativaId), cancellationToken);

        return CreatedAtAction(nameof(GetMyRequestById), new { solicitudId = result.Id }, result);
    }

    [HttpGet("me/solicitudes")]
    [ProducesResponseType(typeof(IReadOnlyCollection<SolicitudLicenciaFederativaDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<SolicitudLicenciaFederativaDto>>> GetMyRequests(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetMisSolicitudesLicenciaFederativaQuery(User.GetRequiredUserId()), cancellationToken);
        return Ok(result);
    }

    [HttpGet("me/solicitudes/{solicitudId:guid}")]
    [ProducesResponseType(typeof(SolicitudLicenciaFederativaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SolicitudLicenciaFederativaDto>> GetMyRequestById(Guid solicitudId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetMiSolicitudLicenciaFederativaQuery(User.GetRequiredUserId(), solicitudId), cancellationToken);
        return Ok(result);
    }

    [HttpGet("admin/solicitudes")]
    [Authorize(Policy = AuthorizationPolicies.Admin)]
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
    [Authorize(Policy = AuthorizationPolicies.Admin)]
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

    public sealed record CreateSolicitudLicenciaFederativaRequest(int Temporada, int TarifaLicenciaFederativaId);
    public sealed record UpdateAdminSolicitudLicenciaFederativaEstadoRequest(
        [property: JsonConverter(typeof(JsonStringEnumConverter))]
        EstadoSolicitudLicenciaFederativa Estado);
}
