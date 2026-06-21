using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using IndaloAventurApi.Api.Security;
using IndaloAventurApi.Application.Abstractions.Security;
using IndaloAventurApi.Infrastructure.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.DependencyInjection;

namespace IndaloAventurApi.IntegrationTests;

public sealed class AuthorizationSecurityIntegrationTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory = factory;
    private readonly HttpClient _httpClient = factory.CreateClient();
    private readonly IServiceProvider _services = factory.Services;

    [Fact]
    public void AuthorizationMatrix_ShouldCoverEveryControllerEndpoint()
    {
        var documentedEndpoints = ApiAuthorizationMatrix.Rules
            .Select(rule => $"{rule.HttpMethod} {rule.Route}")
            .OrderBy(x => x, StringComparer.Ordinal)
            .ToArray();

        var discoveredEndpoints = DiscoverControllerEndpoints()
            .Select(endpoint => $"{endpoint.HttpMethod} {endpoint.Route}")
            .OrderBy(x => x, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(discoveredEndpoints, documentedEndpoints);
    }

    [Theory]
    [InlineData("/api/agenda-telefonica")]
    [InlineData("/api/cargos")]
    [InlineData("/api/fichas-socio/me")]
    [InlineData("/api/licencias-federativas/tarifas")]
    [InlineData("/api/signals")]
    [InlineData("/api/signal-types")]
    [InlineData("/api/users")]
    [InlineData("/api/wordpress/posts")]
    public async Task ProtectedEndpointGroups_ShouldRejectAnonymousRequests(string route)
    {
        _httpClient.DefaultRequestHeaders.Authorization = null;

        var response = await _httpClient.GetAsync(route);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task AuthorizationFailures_ShouldReturnSpanishProblemDetails()
    {
        _httpClient.DefaultRequestHeaders.Authorization = null;

        var unauthorizedResponse = await _httpClient.GetAsync("/api/users");
        Assert.Equal(HttpStatusCode.Unauthorized, unauthorizedResponse.StatusCode);
        var unauthorizedProblem = await unauthorizedResponse.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(unauthorizedProblem);
        Assert.Equal("No autorizado", unauthorizedProblem!.Title);
        Assert.Equal("Se requiere autenticacion valida.", unauthorizedProblem.Detail);

        await EnsureRolesAndAdminAsync();
        await AuthenticateAsMemberAsync();

        var forbiddenResponse = await _httpClient.GetAsync("/api/users");
        Assert.Equal(HttpStatusCode.Forbidden, forbiddenResponse.StatusCode);
        var forbiddenProblem = await forbiddenResponse.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(forbiddenProblem);
        Assert.Equal("Acceso denegado", forbiddenProblem!.Title);
        Assert.Contains("permisos suficientes", forbiddenProblem.Detail, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ProtectedEndpoint_WithExpiredToken_ShouldReturnSpanishUnauthorizedProblemDetails()
    {
        await EnsureRolesAndAdminAsync();

        var email = $"expired-token-{Guid.NewGuid():N}@club.test";
        const string password = "Test1234A";
        await _httpClient.PostAsJsonAsync("/api/auth/register", new { Email = email, Password = password });
        var userId = await GetUserIdByEmailAsync(email);

        var expiredToken = CreateToken(
            userId,
            email,
            [IdentityRoles.Member],
            isMember: false,
            expiresUtc: DateTime.UtcNow.AddMinutes(-5));

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", expiredToken);

        var response = await _httpClient.GetAsync("/api/agenda-telefonica");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(problem);
        Assert.Equal("No autorizado", problem!.Title);
        Assert.Contains("expirado", problem.Detail, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Login_ShouldTemporarilyLockUser_AfterRepeatedFailedAttempts()
    {
        await EnsureRolesAndAdminAsync();

        var email = $"lockout-{Guid.NewGuid():N}@club.test";
        const string password = "Test1234A";
        const string invalidPassword = "Wrong1234A";

        var registerResponse = await _httpClient.PostAsJsonAsync("/api/auth/register", new { Email = email, Password = password });
        Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);

        for (var attempt = 0; attempt < 5; attempt++)
        {
            var invalidLoginResponse = await _httpClient.PostAsJsonAsync("/api/auth/login", new { Email = email, Password = invalidPassword });
            Assert.Equal(HttpStatusCode.Unauthorized, invalidLoginResponse.StatusCode);
        }

        using (var scope = _services.CreateScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Usuario>>();
            var user = await userManager.FindByEmailAsync(email);
            Assert.NotNull(user);
            Assert.True(await userManager.IsLockedOutAsync(user!));
        }

        var validLoginResponse = await _httpClient.PostAsJsonAsync("/api/auth/login", new { Email = email, Password = password });
        Assert.Equal(HttpStatusCode.Unauthorized, validLoginResponse.StatusCode);
    }

    private static IReadOnlyCollection<(string HttpMethod, string Route)> DiscoverControllerEndpoints()
    {
        return typeof(Program).Assembly
            .GetTypes()
            .Where(type => type.IsClass && !type.IsAbstract && typeof(ControllerBase).IsAssignableFrom(type))
            .SelectMany(type =>
            {
                var controllerRoute = type.GetCustomAttribute<RouteAttribute>(inherit: true)?.Template
                    ?? throw new InvalidOperationException($"Controller {type.Name} has no route attribute.");

                return type
                    .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                    .SelectMany(method => method.GetCustomAttributes<HttpMethodAttribute>(inherit: true)
                        .SelectMany(attribute => attribute.HttpMethods.Select(httpMethod =>
                            (HttpMethod: httpMethod.ToUpperInvariant(), Route: CombineRoute(controllerRoute, attribute.Template)))));
            })
            .OrderBy(endpoint => endpoint.HttpMethod, StringComparer.Ordinal)
            .ThenBy(endpoint => endpoint.Route, StringComparer.Ordinal)
            .ToArray();
    }

    private static string CombineRoute(string controllerRoute, string? actionTemplate)
    {
        var controller = controllerRoute.Trim('/');
        var action = actionTemplate?.Trim('/');

        return string.IsNullOrWhiteSpace(action)
            ? $"/{controller}"
            : $"/{controller}/{action}";
    }

    private string CreateToken(Guid userId, string email, IEnumerable<string> roles, bool isMember, DateTime expiresUtc)
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

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(CustomWebApplicationFactory.TestJwtKey)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: CustomWebApplicationFactory.TestJwtIssuer,
            audience: CustomWebApplicationFactory.TestJwtAudience,
            claims: claims,
            notBefore: expiresUtc.AddHours(-1),
            expires: expiresUtc,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task AuthenticateAsMemberAsync()
    {
        var email = $"member-{Guid.NewGuid():N}@club.test";
        const string password = "Test1234A";
        await _httpClient.PostAsJsonAsync("/api/auth/register", new { Email = email, Password = password });

        var loginResponse = await _httpClient.PostAsJsonAsync("/api/auth/login", new { Email = email, Password = password });
        var payload = await loginResponse.Content.ReadFromJsonAsync<LoginPayload>();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", payload!.AccessToken);
    }

    private async Task<Guid> GetUserIdByEmailAsync(string email)
    {
        using var scope = _services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Usuario>>();
        var user = await userManager.FindByEmailAsync(email);
        Assert.NotNull(user);
        return user!.Id;
    }

    private async Task EnsureRolesAndAdminAsync()
    {
        using var scope = _services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Usuario>>();

        if (!await roleManager.RoleExistsAsync(IdentityRoles.Admin))
        {
            await roleManager.CreateAsync(new IdentityRole<Guid>(IdentityRoles.Admin));
        }

        if (!await roleManager.RoleExistsAsync(IdentityRoles.Member))
        {
            await roleManager.CreateAsync(new IdentityRole<Guid>(IdentityRoles.Member));
        }

        var admin = await userManager.FindByEmailAsync("admin@indaloaventura.local");
        if (admin is null)
        {
            admin = new Usuario
            {
                UserName = "admin@indaloaventura.local",
                Email = "admin@indaloaventura.local",
                EmailConfirmed = true,
                LockoutEnabled = true,
                IsMember = false
            };
            await userManager.CreateAsync(admin, "Admin1234A");
        }

        if (!await userManager.IsInRoleAsync(admin, IdentityRoles.Admin))
        {
            await userManager.AddToRoleAsync(admin, IdentityRoles.Admin);
        }
    }

    private sealed record LoginPayload(string AccessToken, string TokenType, int ExpiresInSeconds, bool IsMember);
}
