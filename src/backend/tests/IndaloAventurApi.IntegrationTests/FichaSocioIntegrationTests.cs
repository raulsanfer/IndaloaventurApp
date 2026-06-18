using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using IndaloAventurApi.Infrastructure.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace IndaloAventurApi.IntegrationTests;

public sealed class FichaSocioIntegrationTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _httpClient = factory.CreateClient();
    private readonly IServiceProvider _services = factory.Services;

    [Fact]
    public async Task FichaSocio_MeFlow_WithMember_ShouldGetAndUpdateOwnProfile_WhenAdminCreatedIt()
    {
        await EnsureRolesAndAdminAsync();
        var memberEmail = $"member-{Guid.NewGuid():N}@club.test";
        var memberId = await CreateAndGetMemberUserIdAsync(memberEmail);
        await AuthenticateAsAdminAsync();

        var cargoId = await CreateCargoAsync("Presidente");
        var createResponse = await _httpClient.PostAsJsonAsync($"/api/fichas-socio/{memberId:D}", CreateRequest(cargoId, "12345678Z", "04001", "+34600111222", memberEmail));
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        await AuthenticateAsUserAsync(memberEmail, "Test1234A");
        var putResponse = await _httpClient.PutAsJsonAsync("/api/fichas-socio/me", CreateRequest(cargoId, "87654321X", "04001", "+34600111222", memberEmail));
        Assert.Equal(HttpStatusCode.OK, putResponse.StatusCode);

        var getResponse = await _httpClient.GetAsync("/api/fichas-socio/me");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var payload = await getResponse.Content.ReadFromJsonAsync<FichaSocioPayload>();
        Assert.NotNull(payload);
        Assert.Equal("87654321X", payload!.Dni);
    }

    [Fact]
    public async Task FichaSocio_Member_ShouldGetForbidden_ForThirdPartyAccess()
    {
        await EnsureRolesAndAdminAsync();
        await AuthenticateAsMemberAsync();

        var response = await _httpClient.GetAsync($"/api/fichas-socio/{Guid.NewGuid():D}");
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task FichaSocio_Admin_ShouldManageThirdPartyProfile()
    {
        await EnsureRolesAndAdminAsync();
        var memberId = await CreateAndGetMemberUserIdAsync();
        await AuthenticateAsAdminAsync();

        var cargoId = await CreateCargoAsync("Secretario");
        var createResponse = await _httpClient.PostAsJsonAsync($"/api/fichas-socio/{memberId:D}", CreateRequest(cargoId, "12345678Z", "04001", "+34600111222", "member-admin@club.test"));
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var putResponse = await _httpClient.PutAsJsonAsync($"/api/fichas-socio/{memberId:D}", CreateRequest(cargoId, "12345678Z", "04001", "+34600999000", "member-admin@club.test"));
        Assert.Equal(HttpStatusCode.OK, putResponse.StatusCode);

        var getResponse = await _httpClient.GetAsync($"/api/fichas-socio/{memberId:D}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
    }

    [Fact]
    public async Task FichaSocio_InvalidData_ShouldReturnBadRequest()
    {
        await EnsureRolesAndAdminAsync();
        var memberEmail = $"member-invalid-{Guid.NewGuid():N}@club.test";
        var memberId = await CreateAndGetMemberUserIdAsync(memberEmail);
        await AuthenticateAsAdminAsync();

        var cargoId = await CreateCargoAsync("Tesorero");
        var createResponse = await _httpClient.PostAsJsonAsync($"/api/fichas-socio/{memberId:D}", CreateRequest(cargoId, "12345678Z", "04001", "+34600111222", memberEmail));
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        await AuthenticateAsUserAsync(memberEmail, "Test1234A");

        var response = await _httpClient.PutAsJsonAsync("/api/fichas-socio/me", CreateRequest(cargoId, "ABC", "12", "55", "correo-invalido"));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task FichaSocio_Member_ShouldGetForbidden_WhenCreatingFicha()
    {
        await EnsureRolesAndAdminAsync();
        await AuthenticateAsMemberAsync();

        var response = await _httpClient.PostAsJsonAsync($"/api/fichas-socio/{Guid.NewGuid():D}", CreateRequest(1, "12345678Z", "04001", "+34600111222", "x@club.test"));
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task FichaSocio_ShouldReturnConflict_WhenCargoDoesNotExist()
    {
        await EnsureRolesAndAdminAsync();
        var memberId = await CreateAndGetMemberUserIdAsync();
        await AuthenticateAsAdminAsync();

        var response = await _httpClient.PostAsJsonAsync($"/api/fichas-socio/{memberId:D}", CreateRequest(99999, "12345678Z", "04001", "+34600111222", "x@club.test"));
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    private object CreateRequest(int? cargoId, string dni, string codigoPostal, string tlf, string email)
        => new
        {
            CargoId = cargoId,
            Nombre = "Ana",
            Apellidos = "Perez",
            Dni = dni,
            FechaNacimiento = new DateOnly(1990, 1, 1),
            Direccion = "Calle A",
            CodigoPostal = codigoPostal,
            Poblacion = "Almeria",
            Provincia = "Almeria",
            Tlf = tlf,
            Email = email,
            Alergias = "",
            AceptaPoliticaPrivacidad = true,
            AceptaUsoImagenes = false,
            AceptaCobroCuenta = false
        };

    private async Task<int> CreateCargoAsync(string descripcion)
    {
        var createResponse = await _httpClient.PostAsJsonAsync("/api/cargos", new { Descripcion = descripcion });
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        return (await createResponse.Content.ReadFromJsonAsync<int>())!;
    }

    private async Task<Guid> CreateAndGetMemberUserIdAsync(string? email = null)
    {
        email ??= $"member-managed-{Guid.NewGuid():N}@club.test";
        const string password = "Test1234A";
        var register = await _httpClient.PostAsJsonAsync("/api/auth/register", new { Email = email, Password = password });
        register.EnsureSuccessStatusCode();

        using var scope = _services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Usuario>>();
        var user = await userManager.FindByEmailAsync(email);
        return user!.Id;
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

    private async Task AuthenticateAsUserAsync(string email, string password)
    {
        var loginResponse = await _httpClient.PostAsJsonAsync("/api/auth/login", new { Email = email, Password = password });
        var payload = await loginResponse.Content.ReadFromJsonAsync<LoginPayload>();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", payload!.AccessToken);
    }

    private async Task AuthenticateAsAdminAsync()
    {
        await EnsureRolesAndAdminAsync();

        var loginResponse = await _httpClient.PostAsJsonAsync("/api/auth/login", new
        {
            Email = "admin@indaloaventura.local",
            Password = "Admin1234A"
        });

        var payload = await loginResponse.Content.ReadFromJsonAsync<LoginPayload>();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", payload!.AccessToken);
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

    private sealed record FichaSocioPayload(
        Guid UserId,
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

