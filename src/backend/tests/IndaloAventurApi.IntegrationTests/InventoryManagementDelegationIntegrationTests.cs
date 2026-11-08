using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using IndaloAventurApi.Domain.FichasSocio;
using IndaloAventurApi.Infrastructure.Persistence.DbContext;
using IndaloAventurApi.Infrastructure.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace IndaloAventurApi.IntegrationTests;

public sealed class InventoryManagementDelegationIntegrationTests(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory = factory;

    [Fact]
    public async Task InventoryManagerMember_CanManageInventoryButNotUserAdministration()
    {
        using var adminClient = _factory.CreateClient();
        await _factory.AuthenticateAsAdminAsync(adminClient);

        var (delegateUserId, delegateEmail, delegatePassword) = await CreateAndGrantInventoryManagerAsync(adminClient);
        await AddFichaSocioAsync(delegateUserId, delegateEmail);

        using var delegateClient = _factory.CreateClient();
        var delegateToken = await LoginAsync(delegateClient, delegateEmail, delegatePassword);
        Assert.Contains(
            new JwtSecurityTokenHandler().ReadJwtToken(delegateToken).Claims,
            claim => claim.Type == ClaimTypes.Role && claim.Value == IdentityRoles.InventoryManager);

        var managedUsers = await adminClient.GetFromJsonAsync<IReadOnlyCollection<ManagedUserPayload>>("/api/users");
        var delegateUser = Assert.Single(managedUsers!, user => user.UserId == delegateUserId);
        Assert.True(delegateUser.IsMember);
        Assert.Contains(IdentityRoles.InventoryManager, delegateUser.Roles);

        Assert.Equal(HttpStatusCode.Forbidden, (await delegateClient.GetAsync("/api/users")).StatusCode);

        var suffix = Guid.NewGuid().ToString("N");
        var categoryId = await CreateCategoryAsync(delegateClient, $"Categoria {suffix}");
        Assert.Equal(HttpStatusCode.NoContent, (await delegateClient.PutAsJsonAsync(
            $"/api/inventario-categorias/{categoryId}",
            new { Nombre = $"Categoria actualizada {suffix}", CategoriaPadreId = (int?)null })).StatusCode);

        var disposableCategoryId = await CreateCategoryAsync(delegateClient, $"Categoria temporal {suffix}");
        Assert.Equal(HttpStatusCode.NoContent, (await delegateClient.DeleteAsync($"/api/inventario-categorias/{disposableCategoryId}")).StatusCode);

        var stateId = await CreateStateAsync(delegateClient, $"Disponible {suffix}");
        Assert.Equal(HttpStatusCode.NoContent, (await delegateClient.PutAsJsonAsync(
            $"/api/inventario-estados/{stateId}",
            new { Nombre = $"Disponible actualizado {suffix}" })).StatusCode);

        var disposableStateId = await CreateStateAsync(delegateClient, $"Estado temporal {suffix}");
        Assert.Equal(HttpStatusCode.NoContent, (await delegateClient.DeleteAsync($"/api/inventario-estados/{disposableStateId}")).StatusCode);

        var materialId = await CreateMaterialAsync(delegateClient, $"Casco {suffix}", categoryId, stateId);
        Assert.Equal(HttpStatusCode.NoContent, (await delegateClient.PutAsJsonAsync(
            $"/api/inventario-materiales/{materialId}",
            CreateMaterialRequest($"Casco actualizado {suffix}", categoryId, stateId))).StatusCode);

        var disposableMaterialId = await CreateMaterialAsync(delegateClient, $"Material temporal {suffix}", categoryId, stateId);
        Assert.Equal(HttpStatusCode.NoContent, (await delegateClient.DeleteAsync($"/api/inventario-materiales/{disposableMaterialId}")).StatusCode);

        using var memberClient = _factory.CreateClient();
        var memberEmail = await RegisterClubMemberWithFichaAsync(memberClient);
        var reservationId = await CreateReservationAsync(memberClient, materialId);

        Assert.Equal(HttpStatusCode.OK, (await delegateClient.GetAsync("/api/inventario-reservas/admin")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await delegateClient.GetAsync($"/api/inventario-reservas/admin/{reservationId:D}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await delegateClient.GetAsync($"/api/inventario-reservas/me/{reservationId:D}")).StatusCode);

        Assert.Equal(HttpStatusCode.NoContent, (await delegateClient.PostAsJsonAsync(
            $"/api/inventario-reservas/admin/{reservationId:D}/retirada",
            new { FechaRetirada = new DateOnly(2026, 11, 4) })).StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, (await memberClient.PostAsync(
            $"/api/inventario-reservas/me/{reservationId:D}/confirmacion-devolucion", null)).StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, (await delegateClient.PostAsJsonAsync(
            $"/api/inventario-reservas/admin/{reservationId:D}/confirmacion-devolucion",
            new { FechaDevolucionEfectiva = new DateOnly(2026, 11, 6) })).StatusCode);

        var cancellationReservationId = await CreateReservationAsync(memberClient, materialId);
        Assert.Equal(HttpStatusCode.NoContent, (await delegateClient.PostAsJsonAsync(
            $"/api/inventario-reservas/admin/{cancellationReservationId:D}/cancelacion",
            new { ObservacionAdministracion = "Cancelada por el gestor de inventario" })).StatusCode);

        Assert.False(string.IsNullOrWhiteSpace(memberEmail));
    }

    [Fact]
    public async Task InventoryManager_DelegationAndMembershipChanges_InvalidatePreviousTokens()
    {
        using var adminClient = _factory.CreateClient();
        await _factory.AuthenticateAsAdminAsync(adminClient);
        var (userId, email, password) = await CreateAndGrantInventoryManagerAsync(adminClient);

        using var delegateClient = _factory.CreateClient();
        var initialToken = await LoginAsync(delegateClient, email, password);

        var unchangedUpdate = await adminClient.PutAsJsonAsync($"/api/users/{userId:D}", new
        {
            Email = email,
            IsMember = true,
            Roles = new[] { IdentityRoles.Member, IdentityRoles.InventoryManager }
        });
        Assert.Equal(HttpStatusCode.NoContent, unchangedUpdate.StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await delegateClient.GetAsync("/api/inventario-categorias")).StatusCode);

        var removeRole = await adminClient.PutAsJsonAsync($"/api/users/{userId:D}", new
        {
            Email = email,
            IsMember = true,
            Roles = new[] { IdentityRoles.Member }
        });
        Assert.Equal(HttpStatusCode.NoContent, removeRole.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await delegateClient.PostAsJsonAsync(
            "/api/inventario-estados", new { Nombre = $"No autorizado {Guid.NewGuid():N}" })).StatusCode);

        await LoginAsync(delegateClient, email, password);
        Assert.Equal(HttpStatusCode.Forbidden, (await delegateClient.PostAsJsonAsync(
            "/api/inventario-estados", new { Nombre = $"Sin rol {Guid.NewGuid():N}" })).StatusCode);

        var restoreRole = await adminClient.PutAsJsonAsync($"/api/users/{userId:D}", new
        {
            Email = email,
            IsMember = true,
            Roles = new[] { IdentityRoles.Member, IdentityRoles.InventoryManager }
        });
        Assert.Equal(HttpStatusCode.NoContent, restoreRole.StatusCode);
        await LoginAsync(delegateClient, email, password);

        var removeMembership = await adminClient.PutAsJsonAsync($"/api/users/{userId:D}", new
        {
            Email = email,
            IsMember = false,
            Roles = new[] { IdentityRoles.Member, IdentityRoles.InventoryManager }
        });
        Assert.Equal(HttpStatusCode.NoContent, removeMembership.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await delegateClient.PostAsJsonAsync(
            "/api/inventario-estados", new { Nombre = $"Token revocado {Guid.NewGuid():N}" })).StatusCode);

        await LoginAsync(delegateClient, email, password);
        Assert.Equal(HttpStatusCode.Forbidden, (await delegateClient.PostAsJsonAsync(
            "/api/inventario-estados", new { Nombre = $"No socio {Guid.NewGuid():N}" })).StatusCode);

        Assert.False(string.IsNullOrWhiteSpace(initialToken));
    }

    [Fact]
    public async Task FichaSocioMembershipAssignment_InvalidatesPreviousToken()
    {
        using var memberClient = _factory.CreateClient();
        const string password = "Test1234A";
        var email = $"ficha-security-stamp-{Guid.NewGuid():N}@club.test";
        Assert.Equal(HttpStatusCode.OK, (await memberClient.PostAsJsonAsync("/api/auth/register", new { Email = email, Password = password })).StatusCode);
        var previousToken = await LoginAsync(memberClient, email, password);
        var userId = await GetUserIdAsync(email);

        using var adminClient = _factory.CreateClient();
        await _factory.AuthenticateAsAdminAsync(adminClient);
        var createFicha = await adminClient.PostAsJsonAsync($"/api/fichas-socio/{userId:D}", CreateFichaRequest(email));
        Assert.Equal(HttpStatusCode.Created, createFicha.StatusCode);

        Assert.Equal(HttpStatusCode.Unauthorized, (await memberClient.GetAsync("/api/inventario-categorias")).StatusCode);
        Assert.False(string.IsNullOrWhiteSpace(previousToken));
    }

    private async Task<(Guid UserId, string Email, string Password)> CreateAndGrantInventoryManagerAsync(HttpClient adminClient)
    {
        var email = $"inventory-manager-{Guid.NewGuid():N}@club.test";
        const string password = "Test1234A";
        var createResponse = await adminClient.PostAsJsonAsync("/api/users", new
        {
            Email = email,
            Password = password,
            Roles = new[] { IdentityRoles.Member }
        });
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var userId = await createResponse.Content.ReadFromJsonAsync<Guid>();
        Assert.NotEqual(Guid.Empty, userId);

        var updateResponse = await adminClient.PutAsJsonAsync($"/api/users/{userId:D}", new
        {
            Email = email,
            IsMember = true,
            Roles = new[] { IdentityRoles.Member, IdentityRoles.InventoryManager }
        });
        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

        return (userId, email, password);
    }

    private async Task AddFichaSocioAsync(Guid userId, string email)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.FichasSocio.Add(FichaSocio.Crear(
            userId,
            null,
            "Gestor",
            "Inventario",
            $"{Guid.NewGuid():N}"[..8],
            new DateOnly(1990, 1, 1),
            "Calle Prueba 1",
            "04001",
            "Almeria",
            "Almeria",
            "600000000",
            email,
            null,
            true,
            true,
            false));
        await dbContext.SaveChangesAsync();
    }

    private async Task<string> RegisterClubMemberWithFichaAsync(HttpClient client)
    {
        var email = $"inventory-member-{Guid.NewGuid():N}@club.test";
        const string password = "Test1234A";
        Assert.Equal(HttpStatusCode.OK, (await client.PostAsJsonAsync("/api/auth/register", new { Email = email, Password = password })).StatusCode);
        var userId = await GetUserIdAsync(email);

        using (var scope = _factory.Services.CreateScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Usuario>>();
            var user = await userManager.FindByEmailAsync(email);
            Assert.NotNull(user);
            user!.IsMember = true;
            Assert.True((await userManager.UpdateAsync(user)).Succeeded);
        }

        await AddFichaSocioAsync(userId, email);
        await LoginAsync(client, email, password);
        return email;
    }

    private static async Task<string> LoginAsync(HttpClient client, string email, string password)
    {
        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new { Email = email, Password = password });
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        var login = await loginResponse.Content.ReadFromJsonAsync<LoginPayload>();
        Assert.NotNull(login);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login!.AccessToken);
        return login.AccessToken;
    }

    private async Task<Guid> GetUserIdAsync(string email)
    {
        using var scope = _factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Usuario>>();
        var user = await userManager.FindByEmailAsync(email);
        Assert.NotNull(user);
        return user!.Id;
    }

    private static async Task<int> CreateCategoryAsync(HttpClient client, string name)
    {
        var response = await client.PostAsJsonAsync("/api/inventario-categorias", new { Nombre = name, CategoriaPadreId = (int?)null });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return await response.Content.ReadFromJsonAsync<int>();
    }

    private static async Task<int> CreateStateAsync(HttpClient client, string name)
    {
        var response = await client.PostAsJsonAsync("/api/inventario-estados", new { Nombre = name });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return await response.Content.ReadFromJsonAsync<int>();
    }

    private static async Task<int> CreateMaterialAsync(HttpClient client, string description, int categoryId, int stateId)
    {
        var response = await client.PostAsJsonAsync("/api/inventario-materiales", CreateMaterialRequest(description, categoryId, stateId));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return await response.Content.ReadFromJsonAsync<int>();
    }

    private static object CreateMaterialRequest(string description, int categoryId, int stateId) => new
    {
        Descripcion = description,
        MarcaModelo = (string?)null,
        Cantidad = 5,
        CategoriaId = categoryId,
        EstadoId = stateId,
        FechaAdquisicion = (DateOnly?)null,
        AdquiridoDonado = "Comprado",
        Observaciones = (string?)null,
        NumeroSerie = (string?)null,
        Activo = true
    };

    private static async Task<Guid> CreateReservationAsync(HttpClient client, int materialId)
    {
        var response = await client.PostAsJsonAsync("/api/inventario-reservas/me", new
        {
            InventarioMaterialId = materialId,
            CantidadSolicitada = 1,
            FechaRetiradaPrevista = new DateOnly(2026, 11, 4),
            FechaDevolucionEstimada = new DateOnly(2026, 11, 6),
            Observaciones = "Prueba de reserva"
        });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var reservation = await response.Content.ReadFromJsonAsync<ReservationPayload>();
        Assert.NotNull(reservation);
        return reservation!.Id;
    }

    private static object CreateFichaRequest(string email) => new
    {
        CargoId = (int?)null,
        Nombre = "Socio",
        Apellidos = "Prueba",
        Dni = "00000000T",
        FechaNacimiento = new DateOnly(1990, 1, 1),
        Direccion = "Calle Prueba 1",
        CodigoPostal = "04001",
        Poblacion = "Almeria",
        Provincia = "Almeria",
        Tlf = "600000000",
        Email = email,
        Alergias = (string?)null,
        AceptaPoliticaPrivacidad = true,
        AceptaUsoImagenes = true,
        AceptaCobroCuenta = false
    };

    private sealed record LoginPayload(string AccessToken);
    private sealed record ManagedUserPayload(Guid UserId, string Email, bool IsMember, IReadOnlyCollection<string> Roles);
    private sealed record ReservationPayload(Guid Id);
}
