using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IndaloAventurApi.Application.Abstractions.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace IndaloAventurApi.Infrastructure.Security;

public sealed class JwtTokenService(IOptions<JwtOptions> options, UserManager<Usuario> userManager) : IJwtTokenService
{
    private readonly JwtOptions _options = options.Value;

    public async Task<string> CreateTokenAsync(Guid userId, string email, IEnumerable<string> roles, bool isMember, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null || string.IsNullOrWhiteSpace(user.SecurityStamp))
        {
            throw new InvalidOperationException("No se ha podido emitir el token de acceso.");
        }

        var userIdText = userId.ToString();
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userIdText),
            new(JwtRegisteredClaimNames.Email, email),
            new(ClaimTypes.NameIdentifier, userIdText),
            new(ClaimTypes.Email, email),
            new(AuthClaimNames.IsMember, isMember.ToString().ToLowerInvariant()),
            new(AuthClaimNames.SecurityStamp, user.SecurityStamp),
            new(AuthClaimNames.AuthenticationTime, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(System.Globalization.CultureInfo.InvariantCulture))
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_options.AccessTokenMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
