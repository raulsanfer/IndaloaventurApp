using System.Security.Claims;
using IndaloAventurApi.Application.Abstractions.Security;
using IndaloAventurApi.Infrastructure.Security;

namespace IndaloAventurApi.Api.Security;

/// <summary>
/// Proporciona accesos tipados a los claims requeridos por la API.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Obtiene el identificador del usuario autenticado desde sus claims.
    /// </summary>
    /// <param name="user">Principal autenticado de la solicitud actual.</param>
    /// <returns>Identificador del usuario autenticado.</returns>
    public static Guid GetRequiredUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(value, out var userId))
        {
            throw new UnauthorizedAccessException("No se pudo resolver el usuario autenticado.");
        }

        return userId;
    }

    /// <summary>
    /// Obtiene el correo electronico del usuario autenticado desde sus claims.
    /// </summary>
    /// <param name="user">Principal autenticado de la solicitud actual.</param>
    /// <returns>Correo electronico del usuario autenticado.</returns>
    public static string GetRequiredEmail(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new UnauthorizedAccessException("No se pudo resolver el email del usuario autenticado.");
        }

        return value;
    }

    /// <summary>
    /// Obtiene el indicador de pertenencia al club desde los claims del usuario autenticado.
    /// </summary>
    /// <param name="user">Principal autenticado de la solicitud actual.</param>
    /// <returns>
    /// <see langword="true"/> cuando el usuario esta marcado como socio; en caso contrario, <see langword="false"/>.
    /// </returns>
    public static bool GetRequiredIsMember(this ClaimsPrincipal user)
    {
        var claimValue = user.FindFirstValue(AuthClaimNames.IsMember);
        if (!bool.TryParse(claimValue, out var isMember))
        {
            throw new UnauthorizedAccessException("No se pudo resolver el estado de socio del usuario autenticado.");
        }

        return isMember;
    }

    /// <summary>
    /// Indica si el usuario autenticado dispone del rol de administrador.
    /// </summary>
    /// <param name="user">Principal autenticado de la solicitud actual.</param>
    /// <returns><see langword="true"/> si el usuario es administrador; en caso contrario, <see langword="false"/>.</returns>
    public static bool HasAdminRole(this ClaimsPrincipal user)
    {
        return user.IsInRole(IdentityRoles.Admin);
    }
}
