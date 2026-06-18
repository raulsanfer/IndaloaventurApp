using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IndaloAventurApi.Application.Abstractions.Security;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace IndaloAventurApi.Infrastructure.Security;

public sealed class JwtTokenService(IOptions<JwtOptions> options) : IJwtTokenService
{
    private readonly JwtOptions _options = options.Value;

    public string CreateToken(Guid userId, string email, IEnumerable<string> roles, bool isMember)
    {
        var userIdText = userId.ToString();
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userIdText),
            new(JwtRegisteredClaimNames.Email, email),
            new(ClaimTypes.NameIdentifier, userIdText),
            new(ClaimTypes.Email, email),
            new(AuthClaimNames.IsMember, isMember.ToString().ToLowerInvariant())
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
