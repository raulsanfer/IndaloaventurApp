using IndaloAventurApi.Application.Features.Auth.Login;
using IndaloAventurApi.Application.Features.Auth.PasswordRecovery;
using IndaloAventurApi.Application.Features.Auth.Register;
using IndaloAventurApi.Application.Features.Auth.SocialLogin;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IndaloAventurApi.Api.Features.Auth;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(IMediator mediator) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command, CancellationToken cancellationToken)
    {
        await mediator.Send(command, cancellationToken);
        return Ok();
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginQuery query, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(query, cancellationToken);
        return Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("social-login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> SocialLogin([FromBody] SocialLoginCommand command, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(command, cancellationToken);
        return Ok(response);
    }

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
