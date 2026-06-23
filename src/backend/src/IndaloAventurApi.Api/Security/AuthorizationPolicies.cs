using IndaloAventurApi.Application.Abstractions.Security;
using IndaloAventurApi.Infrastructure.Security;
using Microsoft.AspNetCore.Authorization;

namespace IndaloAventurApi.Api.Security;

/// <summary>
/// Define los nombres y el registro centralizado de las politicas de autorizacion de la API.
/// </summary>
public static class AuthorizationPolicies
{
    /// <summary>
    /// Politica para cualquier usuario autenticado.
    /// </summary>
    public const string Authenticated = "Authenticated";
    /// <summary>
    /// Politica reservada a administradores.
    /// </summary>
    public const string Admin = "Admin";
    /// <summary>
    /// Politica para usuarios con rol de miembro tecnico o administrador.
    /// </summary>
    public const string MemberOrAdmin = "MemberOrAdmin";
    /// <summary>
    /// Politica para usuarios autenticados marcados como socios.
    /// </summary>
    public const string ClubMember = "ClubMember";

    /// <summary>
    /// Construye la configuracion de politicas que se registra en ASP.NET Core.
    /// </summary>
    /// <returns>Accion que registra las politicas sobre <see cref="AuthorizationOptions"/>.</returns>
    public static Action<AuthorizationOptions> Configure()
    {
        return options =>
        {
            options.AddPolicy(Authenticated, policy => policy.RequireAuthenticatedUser());
            options.AddPolicy(Admin, policy => policy.RequireRole(IdentityRoles.Admin));
            options.AddPolicy(MemberOrAdmin, policy => policy.RequireRole(IdentityRoles.Admin, IdentityRoles.Member));
            options.AddPolicy(ClubMember, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(AuthClaimNames.IsMember, bool.TrueString.ToLowerInvariant());
            });
        };
    }
}
