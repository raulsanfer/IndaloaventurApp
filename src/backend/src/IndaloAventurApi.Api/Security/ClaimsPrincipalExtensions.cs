using System.Security.Claims;
using IndaloAventurApi.Application.Abstractions.Security;
using IndaloAventurApi.Infrastructure.Security;

namespace IndaloAventurApi.Api.Security;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetRequiredUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(value, out var userId))
        {
            throw new UnauthorizedAccessException("No se pudo resolver el usuario autenticado.");
        }

        return userId;
    }

    public static string GetRequiredEmail(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new UnauthorizedAccessException("No se pudo resolver el email del usuario autenticado.");
        }

        return value;
    }

    public static bool GetRequiredIsMember(this ClaimsPrincipal user)
    {
        var claimValue = user.FindFirstValue(AuthClaimNames.IsMember);
        if (!bool.TryParse(claimValue, out var isMember))
        {
            throw new UnauthorizedAccessException("No se pudo resolver el estado de socio del usuario autenticado.");
        }

        return isMember;
    }

    public static bool HasAdminRole(this ClaimsPrincipal user)
    {
        return user.IsInRole(IdentityRoles.Admin);
    }
}
