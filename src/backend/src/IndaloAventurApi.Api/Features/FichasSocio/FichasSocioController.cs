using IndaloAventurApi.Api.Security;
using IndaloAventurApi.Application.Abstractions.FichasSocio;
using IndaloAventurApi.Application.Features.FichasSocio.CreateFichaSocio;
using IndaloAventurApi.Application.Features.FichasSocio.GetFichaSocio;
using IndaloAventurApi.Application.Features.FichasSocio.UpdateFichaSocio;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IndaloAventurApi.Api.Features.FichasSocio;

/// <summary>
/// Gestiona la ficha de socio asociada a cada usuario.
/// </summary>
[ApiController]
[Route("api/fichas-socio")]
[Authorize(Policy = AuthorizationPolicies.Authenticated)]
public sealed class FichasSocioController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Crea la ficha de socio de un usuario indicado por su identificador.
    /// </summary>
    /// <param name="userId">Identificador del usuario al que se asociara la ficha.</param>
    /// <param name="request">Datos completos de la ficha de socio.</param>
    /// <param name="cancellationToken">Token para cancelar la operacion.</param>
    /// <returns>Ficha de socio creada para el usuario indicado.</returns>
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

    /// <summary>
    /// Recupera la ficha de socio del usuario autenticado.
    /// </summary>
    /// <param name="cancellationToken">Token para cancelar la consulta.</param>
    /// <returns>Ficha de socio asociada al usuario autenticado.</returns>
    [HttpGet("me")]
    [ProducesResponseType(typeof(FichaSocioDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FichaSocioDto>> GetMe(CancellationToken cancellationToken)
    {
        var userId = User.GetRequiredUserId();
        var result = await mediator.Send(new GetFichaSocioQuery(userId, userId, User.HasAdminRole()), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Recupera la ficha de socio de un usuario concreto.
    /// </summary>
    /// <param name="userId">Identificador del usuario cuya ficha se desea consultar.</param>
    /// <param name="cancellationToken">Token para cancelar la consulta.</param>
    /// <returns>Ficha de socio del usuario solicitado.</returns>
    [HttpGet("{userId:guid}")]
    [ProducesResponseType(typeof(FichaSocioDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FichaSocioDto>> GetByUserId(Guid userId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetFichaSocioQuery(User.GetRequiredUserId(), userId, User.HasAdminRole()), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Actualiza la ficha de socio del usuario autenticado.
    /// </summary>
    /// <param name="request">Datos de la ficha que se desean guardar.</param>
    /// <param name="cancellationToken">Token para cancelar la operacion.</param>
    /// <returns>Ficha de socio actualizada del usuario autenticado.</returns>
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

    /// <summary>
    /// Actualiza la ficha de socio de un usuario concreto.
    /// </summary>
    /// <param name="userId">Identificador del usuario cuya ficha se desea actualizar.</param>
    /// <param name="request">Datos de la ficha que se desean guardar.</param>
    /// <param name="cancellationToken">Token para cancelar la operacion.</param>
    /// <returns>Ficha de socio actualizada del usuario solicitado.</returns>
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

    /// <summary>
    /// Datos editables de la ficha de socio.
    /// </summary>
    /// <param name="CargoId">Identificador del cargo del socio, si aplica.</param>
    /// <param name="Nombre">Nombre del socio.</param>
    /// <param name="Apellidos">Apellidos del socio.</param>
    /// <param name="Dni">Documento identificativo del socio.</param>
    /// <param name="FechaNacimiento">Fecha de nacimiento del socio.</param>
    /// <param name="Direccion">Direccion postal del socio.</param>
    /// <param name="CodigoPostal">Codigo postal del domicilio.</param>
    /// <param name="Poblacion">Poblacion del domicilio.</param>
    /// <param name="Provincia">Provincia del domicilio.</param>
    /// <param name="Tlf">Telefono principal del socio.</param>
    /// <param name="Email">Correo electronico de contacto.</param>
    /// <param name="Alergias">Alergias u observaciones medicas relevantes.</param>
    /// <param name="AceptaPoliticaPrivacidad">Indica si acepta la politica de privacidad.</param>
    /// <param name="AceptaUsoImagenes">Indica si autoriza el uso de imagenes.</param>
    /// <param name="AceptaCobroCuenta">Indica si autoriza el cobro por cuenta bancaria.</param>
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
