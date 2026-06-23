using FluentValidation;
using IndaloAventurApi.Application.Common;
using IndaloAventurApi.Domain.Abstractions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace IndaloAventurApi.Api.Common;

/// <summary>
/// Convierte excepciones de la aplicacion en respuestas HTTP con formato Problem Details.
/// </summary>
public sealed class ProblemDetailsExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<ProblemDetailsExceptionHandler> logger) : IExceptionHandler
{
    /// <summary>
    /// Traduce la excepcion recibida a un codigo HTTP y un cuerpo estandarizado de error.
    /// </summary>
    /// <param name="httpContext">Contexto HTTP de la solicitud fallida.</param>
    /// <param name="exception">Excepcion capturada durante el procesamiento.</param>
    /// <param name="cancellationToken">Token para cancelar la escritura de la respuesta.</param>
    /// <returns>
    /// <see langword="true"/> cuando se ha escrito la respuesta de error; en caso contrario, <see langword="false"/>.
    /// </returns>
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "La solicitud ha fallado");
        var problemDetails = new ProblemDetails
        {
            Instance = httpContext.Request.Path
        };

        switch (exception)
        {
            case ValidationException validationException:
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                problemDetails.Title = "Error de validacion";
                problemDetails.Detail = "Una o mas validaciones han fallado.";
                problemDetails.Extensions["errors"] = validationException.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });
                break;
            case DomainException:
            case InvalidOperationException:
                httpContext.Response.StatusCode = StatusCodes.Status409Conflict;
                problemDetails.Title = "Conflicto de regla de negocio";
                problemDetails.Detail = exception.Message;
                break;
            case UnauthorizedAccessException:
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                problemDetails.Title = "No autorizado";
                problemDetails.Detail = exception.Message;
                break;
            case ForbiddenAccessException:
                httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
                problemDetails.Title = "Acceso denegado";
                problemDetails.Detail = exception.Message;
                break;
            case KeyNotFoundException:
                httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                problemDetails.Title = "No encontrado";
                problemDetails.Detail = exception.Message;
                break;
            default:
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                problemDetails.Title = "Error del servidor";
                problemDetails.Detail = "Error inesperado.";
                break;
        }

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails
        });
    }
}
