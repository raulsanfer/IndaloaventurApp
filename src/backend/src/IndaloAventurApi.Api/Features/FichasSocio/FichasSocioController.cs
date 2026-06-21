using IndaloAventurApi.Api.Security;
using IndaloAventurApi.Application.Abstractions.FichasSocio;
using IndaloAventurApi.Application.Features.FichasSocio.CreateFichaSocio;
using IndaloAventurApi.Application.Features.FichasSocio.GetFichaSocio;
using IndaloAventurApi.Application.Features.FichasSocio.UpdateFichaSocio;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IndaloAventurApi.Api.Features.FichasSocio;

[ApiController]
[Route("api/fichas-socio")]
[Authorize(Policy = AuthorizationPolicies.Authenticated)]
public sealed class FichasSocioController(IMediator mediator) : ControllerBase
{
    [HttpPost("{userId:guid}")]
    [Authorize(Policy = AuthorizationPolicies.Admin)]
    [ProducesResponseType(typeof(FichaSocioDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<FichaSocioDto>> CreateByUserId(Guid userId, [FromBody] UpdateFichaSocioRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateFichaSocioCommand(
            User.GetRequiredUserId(),
            userId,
            User.HasAdminRole(),
            request.CargoId,
            request.Nombre,
            request.Apellidos,
            request.Dni,
            request.FechaNacimiento,
            request.Direccion,
            request.CodigoPostal,
            request.Poblacion,
            request.Provincia,
            request.Tlf,
            request.Email,
            request.Alergias,
            request.AceptaPoliticaPrivacidad,
            request.AceptaUsoImagenes,
            request.AceptaCobroCuenta), cancellationToken);

        return CreatedAtAction(nameof(GetByUserId), new { userId }, result);
    }

    [HttpGet("me")]
    [ProducesResponseType(typeof(FichaSocioDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FichaSocioDto>> GetMe(CancellationToken cancellationToken)
    {
        var userId = User.GetRequiredUserId();
        var result = await mediator.Send(new GetFichaSocioQuery(userId, userId, User.HasAdminRole()), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{userId:guid}")]
    [ProducesResponseType(typeof(FichaSocioDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FichaSocioDto>> GetByUserId(Guid userId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetFichaSocioQuery(User.GetRequiredUserId(), userId, User.HasAdminRole()), cancellationToken);
        return Ok(result);
    }

    [HttpPut("me")]
    [ProducesResponseType(typeof(FichaSocioDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FichaSocioDto>> UpdateMe([FromBody] UpdateFichaSocioRequest request, CancellationToken cancellationToken)
    {
        var userId = User.GetRequiredUserId();
        var result = await mediator.Send(new UpdateFichaSocioCommand(
            userId,
            userId,
            User.HasAdminRole(),
            request.CargoId,
            request.Nombre,
            request.Apellidos,
            request.Dni,
            request.FechaNacimiento,
            request.Direccion,
            request.CodigoPostal,
            request.Poblacion,
            request.Provincia,
            request.Tlf,
            request.Email,
            request.Alergias,
            request.AceptaPoliticaPrivacidad,
            request.AceptaUsoImagenes,
            request.AceptaCobroCuenta), cancellationToken);

        return Ok(result);
    }

    [HttpPut("{userId:guid}")]
    [ProducesResponseType(typeof(FichaSocioDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FichaSocioDto>> UpdateByUserId(Guid userId, [FromBody] UpdateFichaSocioRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UpdateFichaSocioCommand(
            User.GetRequiredUserId(),
            userId,
            User.HasAdminRole(),
            request.CargoId,
            request.Nombre,
            request.Apellidos,
            request.Dni,
            request.FechaNacimiento,
            request.Direccion,
            request.CodigoPostal,
            request.Poblacion,
            request.Provincia,
            request.Tlf,
            request.Email,
            request.Alergias,
            request.AceptaPoliticaPrivacidad,
            request.AceptaUsoImagenes,
            request.AceptaCobroCuenta), cancellationToken);

        return Ok(result);
    }

    public sealed record UpdateFichaSocioRequest(
        int? CargoId,
        string Nombre,
        string Apellidos,
        string Dni,
        DateOnly FechaNacimiento,
        string Direccion,
        string CodigoPostal,
        string Poblacion,
        string Provincia,
        string Tlf,
        string Email,
        string? Alergias,
        bool AceptaPoliticaPrivacidad,
        bool AceptaUsoImagenes,
        bool AceptaCobroCuenta);
}
