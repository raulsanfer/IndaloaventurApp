using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using IndaloAventurApi.Infrastructure.Persistence.DbContext;
using IndaloAventurApi.Infrastructure.Security;
using IndaloAventurApi.Domain.FichasSocio;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace IndaloAventurApi.IntegrationTests;

public sealed class InventarioReservasIntegrationTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory = factory;

    [Fact]
    public async Task ReservasMaterial_FlujoHttpCompleto_DesdeSolicitudHastaDevolucion()
    {
        using var adminClient = _factory.CreateClient();
        await _factory.AuthenticateAsAdminAsync(adminClient);
        var material = await CrearMaterialAsync(adminClient, activo: true);

        using var socioClient = _factory.CreateClient();
        await AutenticarSocioConFichaAsync(socioClient);
        var createResponse = await socioClient.PostAsJsonAsync("/api/inventario-reservas/me", CrearSolicitud(material.Id, cantidad: 2));

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await createResponse.Content.ReadFromJsonAsync<ReservaMaterialPayload>();
        Assert.NotNull(created);
        Assert.Equal(1, created!.Estado);

        var pickupResponse = await adminClient.PostAsJsonAsync(
            $"/api/inventario-reservas/admin/{created.Id:D}/retirada",
            new { FechaRetirada = new DateOnly(2026, 11, 4) });
        Assert.Equal(HttpStatusCode.NoContent, pickupResponse.StatusCode);

        var memberConfirmation = await socioClient.PostAsync($"/api/inventario-reservas/me/{created.Id:D}/confirmacion-devolucion", null);
        Assert.Equal(HttpStatusCode.NoContent, memberConfirmation.StatusCode);

        var adminConfirmation = await adminClient.PostAsJsonAsync(
            $"/api/inventario-reservas/admin/{created.Id:D}/confirmacion-devolucion",
            new { FechaDevolucionEfectiva = new DateOnly(2026, 11, 6) });
        Assert.Equal(HttpStatusCode.NoContent, adminConfirmation.StatusCode);

        var detailResponse = await adminClient.GetAsync($"/api/inventario-reservas/admin/{created.Id:D}");
        Assert.Equal(HttpStatusCode.OK, detailResponse.StatusCode);
        var detail = await detailResponse.Content.ReadFromJsonAsync<ReservaMaterialPayload>();
        Assert.NotNull(detail);
        Assert.Equal(3, detail!.Estado);
        Assert.True(detail.ConfirmacionDevolucionSocio);
        Assert.True(detail.ConfirmacionDevolucionAdmin);
        Assert.Equal(new DateOnly(2026, 11, 6), detail.FechaDevolucionEfectiva);
    }

    [Fact]
    public async Task ReservasMaterial_RutasProtegidasExigenSocioRealFichaYTitularidad()
    {
        using var anonymousClient = _factory.CreateClient();
        Assert.Equal(HttpStatusCode.Unauthorized, (await anonymousClient.GetAsync("/api/inventario-reservas/me")).StatusCode);

        using var adminClient = _factory.CreateClient();
        await _factory.AuthenticateAsAdminAsync(adminClient);
        var material = await CrearMaterialAsync(adminClient, activo: true);

        using var technicalMemberClient = _factory.CreateClient();
        await RegistrarYAutenticarAsync(technicalMemberClient, isMember: false, crearFicha: false);
        Assert.Equal(HttpStatusCode.Forbidden, (await technicalMemberClient.PostAsJsonAsync("/api/inventario-reservas/me", CrearSolicitud(material.Id, 1))).StatusCode);

        using var sinFichaClient = _factory.CreateClient();
        await RegistrarYAutenticarAsync(sinFichaClient, isMember: true, crearFicha: false);
        Assert.Equal(HttpStatusCode.Forbidden, (await sinFichaClient.PostAsJsonAsync("/api/inventario-reservas/me", CrearSolicitud(material.Id, 1))).StatusCode);

        using var ownerClient = _factory.CreateClient();
        await AutenticarSocioConFichaAsync(ownerClient);
        var createResponse = await ownerClient.PostAsJsonAsync("/api/inventario-reservas/me", CrearSolicitud(material.Id, 1));
        var created = await createResponse.Content.ReadFromJsonAsync<ReservaMaterialPayload>();
        Assert.NotNull(created);

        using var otherMemberClient = _factory.CreateClient();
        await AutenticarSocioConFichaAsync(otherMemberClient);
        Assert.Equal(HttpStatusCode.NotFound, (await otherMemberClient.GetAsync($"/api/inventario-reservas/me/{created!.Id:D}")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await otherMemberClient.GetAsync("/api/inventario-reservas/admin")).StatusCode);
    }

    [Fact]
    public async Task ReservasMaterial_MaterialInactivoBloqueaNuevasReservasYConservaHistorico()
    {
        using var adminClient = _factory.CreateClient();
        await _factory.AuthenticateAsAdminAsync(adminClient);
        var materialInactivo = await CrearMaterialAsync(adminClient, activo: false);

        using var socioClient = _factory.CreateClient();
        await AutenticarSocioConFichaAsync(socioClient);
        var inactiveResponse = await socioClient.PostAsJsonAsync("/api/inventario-reservas/me", CrearSolicitud(materialInactivo.Id, 1));
        Assert.Equal(HttpStatusCode.Conflict, inactiveResponse.StatusCode);

        var materialActivo = await CrearMaterialAsync(adminClient, activo: true);
        var createdResponse = await socioClient.PostAsJsonAsync("/api/inventario-reservas/me", CrearSolicitud(materialActivo.Id, 1));
        Assert.Equal(HttpStatusCode.Created, createdResponse.StatusCode);
        var reserva = await createdResponse.Content.ReadFromJsonAsync<ReservaMaterialPayload>();
        Assert.NotNull(reserva);

        var deactivateResponse = await adminClient.PutAsJsonAsync($"/api/inventario-materiales/{materialActivo.Id}", new
        {
            materialActivo.Descripcion,
            MarcaModelo = (string?)null,
            Cantidad = materialActivo.Cantidad,
            materialActivo.CategoriaId,
            materialActivo.EstadoId,
            FechaAdquisicion = (DateOnly?)null,
            AdquiridoDonado = "Comprado",
            Observaciones = (string?)null,
            NumeroSerie = (string?)null,
            Activo = false
        });
        Assert.Equal(HttpStatusCode.NoContent, deactivateResponse.StatusCode);

        Assert.Equal(HttpStatusCode.OK, (await socioClient.GetAsync($"/api/inventario-reservas/me/{reserva!.Id:D}")).StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, (await socioClient.PostAsJsonAsync("/api/inventario-reservas/me", CrearSolicitud(materialActivo.Id, 1))).StatusCode);
    }

    private async Task<MaterialCreado> CrearMaterialAsync(HttpClient adminClient, bool activo)
    {
        var suffix = Guid.NewGuid().ToString("N");
        var categoriaResponse = await adminClient.PostAsJsonAsync("/api/inventario-categorias", new { Nombre = $"Categoria {suffix}", CategoriaPadreId = (int?)null });
        Assert.Equal(HttpStatusCode.Created, categoriaResponse.StatusCode);
        var categoriaId = await categoriaResponse.Content.ReadFromJsonAsync<int>();

        var estadoResponse = await adminClient.PostAsJsonAsync("/api/inventario-estados", new { Nombre = $"Disponible {suffix}" });
        Assert.Equal(HttpStatusCode.Created, estadoResponse.StatusCode);
        var estadoId = await estadoResponse.Content.ReadFromJsonAsync<int>();
        var descripcion = $"Casco {suffix}";

        var materialResponse = await adminClient.PostAsJsonAsync("/api/inventario-materiales", new
        {
            Descripcion = descripcion,
            MarcaModelo = (string?)null,
            Cantidad = 5,
            CategoriaId = categoriaId,
            EstadoId = estadoId,
            FechaAdquisicion = (DateOnly?)null,
            AdquiridoDonado = "Comprado",
            Observaciones = (string?)null,
            NumeroSerie = (string?)null,
            Activo = activo
        });
        Assert.Equal(HttpStatusCode.Created, materialResponse.StatusCode);
        var materialId = await materialResponse.Content.ReadFromJsonAsync<int>();

        return new MaterialCreado(materialId, descripcion, 5, categoriaId, estadoId);
    }

    private async Task AutenticarSocioConFichaAsync(HttpClient client)
        => await RegistrarYAutenticarAsync(client, isMember: true, crearFicha: true);

    private async Task RegistrarYAutenticarAsync(HttpClient client, bool isMember, bool crearFicha)
    {
        await _factory.EnsureRolesAndAdminAsync();
        var email = $"reserva-{Guid.NewGuid():N}@club.test";
        const string password = "Test1234A";
        Assert.Equal(HttpStatusCode.OK, (await client.PostAsJsonAsync("/api/auth/register", new { Email = email, Password = password })).StatusCode);

        using (var scope = _factory.Services.CreateScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Usuario>>();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var user = await userManager.FindByEmailAsync(email);
            Assert.NotNull(user);
            user!.IsMember = isMember;
            Assert.True((await userManager.UpdateAsync(user)).Succeeded);

            if (crearFicha)
            {
                dbContext.FichasSocio.Add(FichaSocio.Crear(
                    user.Id,
                    null,
                    "Socio",
                    Guid.NewGuid().ToString("N")[..8],
                    "00000000T",
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
        }

        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new { Email = email, Password = password });
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        var login = await loginResponse.Content.ReadFromJsonAsync<LoginPayload>();
        Assert.NotNull(login);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login!.AccessToken);
    }

    private static object CrearSolicitud(int materialId, int cantidad)
        => new
        {
            InventarioMaterialId = materialId,
            CantidadSolicitada = cantidad,
            FechaRetiradaPrevista = new DateOnly(2026, 11, 4),
            FechaDevolucionEstimada = new DateOnly(2026, 11, 6),
            Observaciones = "Prueba de reserva"
        };

    private sealed record LoginPayload(string AccessToken);
    private sealed record ReservaMaterialPayload(
        Guid Id,
        int Estado,
        bool ConfirmacionDevolucionSocio,
        bool ConfirmacionDevolucionAdmin,
        DateOnly? FechaDevolucionEfectiva);
    private sealed record MaterialCreado(int Id, string Descripcion, int Cantidad, int CategoriaId, int EstadoId);
}
