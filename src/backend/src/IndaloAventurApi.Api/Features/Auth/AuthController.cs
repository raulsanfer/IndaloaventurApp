using IndaloAventurApi.Application.Features.Auth.Login;
using IndaloAventurApi.Application.Features.Auth.PasswordRecovery;
using IndaloAventurApi.Application.Features.Auth.Register;
using IndaloAventurApi.Application.Features.Auth.SocialLogin;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IndaloAventurApi.Api.Features.Auth;

/// <summary>
/// Gestiona el registro, autenticacion y recuperacion de credenciales de usuarios.
/// </summary>
[ApiController]
[Route("api/auth")]
public sealed class AuthController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Registra un nuevo usuario local en la plataforma.
    /// </summary>
    /// <param name="command">Datos de alta del usuario.</param>
    /// <param name="cancellationToken">Token para cancelar la operacion.</param>
    /// <returns>Respuesta correcta cuando el registro se completa.</returns>
    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command, CancellationToken cancellationToken)
    {
        await mediator.Send(command, cancellationToken);
        return Ok();
    }

    /// <summary>
    /// Autentica a un usuario con credenciales locales y devuelve un token JWT.
    /// </summary>
    /// <param name="query">Credenciales necesarias para iniciar sesion.</param>
    /// <param name="cancellationToken">Token para cancelar la operacion.</param>
    /// <returns>Token de acceso y datos de sesion del usuario autenticado.</returns>
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginQuery query, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(query, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Autentica a un usuario mediante un proveedor social externo.
    /// </summary>
    /// <param name="command">Token o datos emitidos por el proveedor social.</param>
    /// <param name="cancellationToken">Token para cancelar la operacion.</param>
    /// <returns>Token de acceso y datos de sesion del usuario autenticado.</returns>
    [AllowAnonymous]
    [HttpPost("social-login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> SocialLogin([FromBody] SocialLoginCommand command, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(command, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Inicia el proceso de recuperacion de contrasena para un usuario.
    /// </summary>
    /// <param name="command">Datos necesarios para solicitar la recuperacion.</param>
    /// <param name="cancellationToken">Token para cancelar la operacion.</param>
    /// <returns>Resultado de la solicitud de recuperacion.</returns>
    [AllowAnonymous]
    [HttpPost("passrecovery")]
    [ProducesResponseType(typeof(PasswordRecoveryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PasswordRecoveryResponse>> RequestPasswordRecovery(
        [FromBody] RequestPasswordRecoveryCommand command,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(command, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Restablece la contrasena de un usuario a partir de un token de recuperacion valido.
    /// </summary>
    /// <param name="command">Nueva contrasena y token de recuperacion.</param>
    /// <param name="cancellationToken">Token para cancelar la operacion.</param>
    /// <returns>Resultado del restablecimiento de contrasena.</returns>
    [AllowAnonymous]
    [HttpPost("reset-password")]
    [ProducesResponseType(typeof(PasswordRecoveryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PasswordRecoveryResponse>> ResetPassword(
        [FromBody] ResetPasswordCommand command,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(command, cancellationToken);
        return Ok(response);
    }
}
