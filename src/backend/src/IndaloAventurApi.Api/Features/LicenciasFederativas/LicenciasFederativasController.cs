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

/// <summary>
/// Gestiona tarifas y solicitudes de licencias federativas.
/// </summary>
[ApiController]
[Route("api/licencias-federativas")]
[Authorize(Policy = AuthorizationPolicies.Authenticated)]
public sealed class LicenciasFederativasController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Devuelve las tarifas disponibles de licencias federativas.
    /// </summary>
    /// <param name="temporada">Temporada a consultar; si se omite se aplica el criterio por defecto del sistema.</param>
    /// <param name="mediaTemporada">Filtra tarifas de media temporada cuando se indica.</param>
    /// <param name="cancellationToken">Token para cancelar la consulta.</param>
    /// <returns>Coleccion de tarifas que cumplen los filtros indicados.</returns>
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

    /// <summary>
    /// Crea una nueva solicitud de licencia federativa para el usuario autenticado.
    /// </summary>
    /// <param name="request">Temporada y tarifa solicitadas.</param>
    /// <param name="cancellationToken">Token para cancelar la operacion.</param>
    /// <returns>Solicitud creada con su estado inicial.</returns>
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

    /// <summary>
    /// Lista todas las solicitudes de licencia del usuario autenticado.
    /// </summary>
    /// <param name="cancellationToken">Token para cancelar la consulta.</param>
    /// <returns>Coleccion de solicitudes del usuario autenticado.</returns>
    [HttpGet("me/solicitudes")]
    [ProducesResponseType(typeof(IReadOnlyCollection<SolicitudLicenciaFederativaDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<SolicitudLicenciaFederativaDto>>> GetMyRequests(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetMisSolicitudesLicenciaFederativaQuery(User.GetRequiredUserId()), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Obtiene una solicitud de licencia concreta del usuario autenticado.
    /// </summary>
    /// <param name="solicitudId">Identificador de la solicitud.</param>
    /// <param name="cancellationToken">Token para cancelar la consulta.</param>
    /// <returns>Solicitud de licencia encontrada.</returns>
    [HttpGet("me/solicitudes/{solicitudId:guid}")]
    [ProducesResponseType(typeof(SolicitudLicenciaFederativaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SolicitudLicenciaFederativaDto>> GetMyRequestById(Guid solicitudId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetMiSolicitudLicenciaFederativaQuery(User.GetRequiredUserId(), solicitudId), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Lista solicitudes de licencia con filtros administrativos.
    /// </summary>
    /// <param name="userId">Filtra por usuario propietario de la solicitud.</param>
    /// <param name="temporada">Filtra por temporada de la licencia.</param>
    /// <param name="estado">Filtra por estado actual de la solicitud.</param>
    /// <param name="cancellationToken">Token para cancelar la consulta.</param>
    /// <returns>Coleccion de solicitudes visibles para administracion.</returns>
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

    /// <summary>
    /// Actualiza el estado administrativo de una solicitud de licencia.
    /// </summary>
    /// <param name="userId">Identificador del usuario propietario de la solicitud.</param>
    /// <param name="solicitudId">Identificador de la solicitud a actualizar.</param>
    /// <param name="request">Nuevo estado administrativo que se desea aplicar.</param>
    /// <param name="cancellationToken">Token para cancelar la operacion.</param>
    /// <returns>Solicitud actualizada con el nuevo estado.</returns>
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

    /// <summary>
    /// Datos necesarios para registrar una solicitud de licencia federativa.
    /// </summary>
    /// <param name="Temporada">Temporada federativa solicitada.</param>
    /// <param name="TarifaLicenciaFederativaId">Identificador de la tarifa seleccionada.</param>
    public sealed record CreateSolicitudLicenciaFederativaRequest(int Temporada, int TarifaLicenciaFederativaId);

    /// <summary>
    /// Datos necesarios para cambiar el estado de una solicitud desde administracion.
    /// </summary>
    /// <param name="Estado">Nuevo estado que se desea asignar a la solicitud.</param>
    public sealed record UpdateAdminSolicitudLicenciaFederativaEstadoRequest(
        [property: JsonConverter(typeof(JsonStringEnumConverter))]
        EstadoSolicitudLicenciaFederativa Estado);
}
