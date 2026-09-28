using IndaloAventurApi.Api.Security;
using IndaloAventurApi.Application.Abstractions.Identity;
using IndaloAventurApi.Application.Features.Users.CreateManagedUser;
using IndaloAventurApi.Application.Features.Users.ChangeManagedUserPassword;
using IndaloAventurApi.Application.Features.Users.DeactivateManagedUser;
using IndaloAventurApi.Application.Features.Users.ListManagedUsers;
using IndaloAventurApi.Application.Features.Users.ReactivateManagedUser;
using IndaloAventurApi.Application.Features.Users.UpdateManagedUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using System.Text.Json.Serialization;

namespace IndaloAventurApi.Api.Features.Users;

/// <summary>
/// Administra usuarios gestionados por el panel interno.
/// </summary>
[ApiController]
[Route("api/users")]
[Authorize(Policy = AuthorizationPolicies.Admin)]
public sealed class UsersController(IMediator mediator, ILogger<UsersController> logger) : ControllerBase
{
    /// <summary>
    /// Lista usuarios, opcionalmente filtrados por correo electronico.
    /// </summary>
    /// <param name="email">Filtro opcional por direccion de correo.</param>
    /// <param name="cancellationToken">Token para cancelar la consulta.</param>
    /// <returns>Coleccion de usuarios que cumplen el filtro indicado.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<ManagedUserDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<ManagedUserDto>>> List([FromQuery] string? email, CancellationToken cancellationToken)
    {
        var users = await mediator.Send(new ListManagedUsersQuery(email), cancellationToken);
        return Ok(users);
    }

    /// <summary>
    /// Crea un nuevo usuario gestionado desde administracion.
    /// </summary>
    /// <param name="command">Datos de alta y roles iniciales del usuario.</param>
    /// <param name="cancellationToken">Token para cancelar la operacion.</param>
    /// <returns>Identificador del usuario creado.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateManagedUserCommand command, CancellationToken cancellationToken)
    {
        var userId = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(List), new { id = userId }, userId);
    }

    /// <summary>
    /// Actualiza el correo, estado de socio y roles de un usuario.
    /// </summary>
    /// <param name="userId">Identificador del usuario a modificar.</param>
    /// <param name="request">Datos actualizados del usuario.</param>
    /// <param name="cancellationToken">Token para cancelar la operacion.</param>
    /// <returns>Respuesta sin contenido cuando la actualizacion se completa correctamente.</returns>
    [HttpPut("{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid userId, [FromBody] UpdateManagedUserRequest request, CancellationToken cancellationToken)
    {
        await mediator.Send(new UpdateManagedUserCommand(userId, request.Email, request.IsMember, request.Roles), cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Restablece administrativamente la contrasena de un usuario.
    /// </summary>
    /// <param name="userId">Identificador del usuario cuya contrasena se modifica.</param>
    /// <param name="request">Nueva contrasena, que no se devuelve ni registra.</param>
    /// <param name="cancellationToken">Token para cancelar la operacion.</param>
    /// <returns>Respuesta sin contenido cuando el cambio se completa.</returns>
    [HttpPut("{userId:guid}/password")]
    [Authorize(Policy = AuthorizationPolicies.AdminWithRecentAuthentication)]
    [EnableRateLimiting(ApiRateLimitPolicies.AdministrativePasswordChange)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> ChangePassword(Guid userId, [FromBody] ChangeManagedUserPasswordRequest request, CancellationToken cancellationToken)
    {
        var outcome = await mediator.Send(new ChangeManagedUserPasswordCommand(userId, request.NewPassword), cancellationToken);
        var actorId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "desconocido";

        switch (outcome)
        {
            case PasswordChangeOutcome.Succeeded:
                logger.LogInformation("Cambio de contrasena administrativo correcto. ActorId={ActorId} TargetUserId={TargetUserId} StatusCode={StatusCode} CorrelationId={CorrelationId}", actorId, userId, StatusCodes.Status204NoContent, HttpContext.TraceIdentifier);
                return NoContent();
            case PasswordChangeOutcome.UserNotFound:
                logger.LogWarning("Cambio de contrasena administrativo rechazado. ActorId={ActorId} TargetUserId={TargetUserId} StatusCode={StatusCode} CorrelationId={CorrelationId}", actorId, userId, StatusCodes.Status404NotFound, HttpContext.TraceIdentifier);
                return NotFound(new ProblemDetails { Status = StatusCodes.Status404NotFound, Title = "No encontrado", Detail = "No se ha encontrado el usuario solicitado." });
            default:
                logger.LogWarning("Cambio de contrasena administrativo rechazado. ActorId={ActorId} TargetUserId={TargetUserId} StatusCode={StatusCode} CorrelationId={CorrelationId}", actorId, userId, StatusCodes.Status400BadRequest, HttpContext.TraceIdentifier);
                return BadRequest(new ProblemDetails { Status = StatusCodes.Status400BadRequest, Title = "Solicitud no valida", Detail = "No se ha podido cambiar la contrasena." });
        }
    }

    /// <summary>
    /// Desactiva un usuario gestionado sin eliminar su cuenta.
    /// </summary>
    /// <param name="userId">Identificador del usuario que se desea desactivar.</param>
    /// <param name="cancellationToken">Token para cancelar la operacion.</param>
    /// <returns>Respuesta sin contenido cuando la desactivacion se completa correctamente.</returns>
    [HttpPost("{userId:guid}/deactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Deactivate(Guid userId, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeactivateManagedUserCommand(userId), cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Reactiva un usuario previamente desactivado.
    /// </summary>
    /// <param name="userId">Identificador del usuario que se desea reactivar.</param>
    /// <param name="cancellationToken">Token para cancelar la operacion.</param>
    /// <returns>Respuesta sin contenido cuando la reactivacion se completa correctamente.</returns>
    [HttpPost("{userId:guid}/reactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Reactivate(Guid userId, CancellationToken cancellationToken)
    {
        await mediator.Send(new ReactivateManagedUserCommand(userId), cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Datos editables de un usuario gestionado.
    /// </summary>
    /// <param name="Email">Correo electronico principal del usuario.</param>
    /// <param name="IsMember">Indica si el usuario es socio del club.</param>
    /// <param name="Roles">Lista completa de roles asignados al usuario.</param>
    public sealed record UpdateManagedUserRequest(string Email, bool IsMember, IReadOnlyCollection<string> Roles);

    /// <summary>
    /// Cuerpo estricto del cambio administrativo de contraseña.
    /// </summary>
    /// <param name="NewPassword">Nueva contraseña que se procesa sin registrarla ni devolverla.</param>
    [JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
    public sealed record ChangeManagedUserPasswordRequest(string? NewPassword);
}
