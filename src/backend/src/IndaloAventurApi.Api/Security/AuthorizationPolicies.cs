using IndaloAventurApi.Application.Abstractions.Security;
using IndaloAventurApi.Infrastructure.Security;
using Microsoft.AspNetCore.Authorization;

namespace IndaloAventurApi.Api.Security;

public static class AuthorizationPolicies
{
    public const string Authenticated = "Authenticated";
    public const string Admin = "Admin";
    public const string MemberOrAdmin = "MemberOrAdmin";
    public const string ClubMember = "ClubMember";

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
