using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using IndaloAventurApi.Domain.LicenciasFederativas;
using IndaloAventurApi.Infrastructure.Persistence;
using IndaloAventurApi.Infrastructure.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace IndaloAventurApi.IntegrationTests;

public sealed class LicenciasFederativasIntegrationTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory = factory;
    private readonly HttpClient _httpClient = factory.CreateClient();
    private readonly IServiceProvider _services = factory.Services;

    [Fact]
    public async Task MisSolicitudes_WithoutToken_ShouldReturnUnauthorized()
    {
        var response = await _httpClient.GetAsync("/api/licencias-federativas/me/solicitudes");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Tarifas_WithoutToken_ShouldReturnUnauthorized()
    {
        _httpClient.DefaultRequestHeaders.Authorization = null;

        var response = await _httpClient.GetAsync("/api/licencias-federativas/tarifas");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Tarifas_WithAuthenticatedMember_ShouldReturnCatalog()
    {
        await EnsureRolesAndAdminAsync();
        await AuthenticateAsMemberAsync(isMember: false);

        var response = await _httpClient.GetAsync("/api/licencias-federativas/tarifas");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var tarifas = await response.Content.ReadFromJsonAsync<IReadOnlyCollection<TarifaLicenciaFederativaPayload>>();
        Assert.NotNull(tarifas);
        Assert.True(tarifas!.Count >= 50);
        Assert.Contains(tarifas, x => x.Id == 1 && x.Temporada == 2026 && x.Licencia == "A" && !x.MediaTemporada);
        Assert.Contains(tarifas, x => x.Id == 26 && x.Temporada == 2026 && x.Licencia == "A" && x.MediaTemporada);
    }

    [Fact]
    public async Task Tarifas_WithAuthenticatedAdmin_AndTemporadaFilter_ShouldReturnOnlyMatchingSeason()
    {
        await EnsureRolesAndAdminAsync();
        await AddTarifaAsync(TarifaLicenciaFederativa.Crear(2027, "B", "Juvenil", 55m, 75m, "Nacional"));
        await AuthenticateAsAdminAsync();

        var response = await _httpClient.GetAsync("/api/licencias-federativas/tarifas?temporada=2027");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var tarifas = await response.Content.ReadFromJsonAsync<IReadOnlyCollection<TarifaLicenciaFederativaPayload>>();
        Assert.NotNull(tarifas);
        Assert.Single(tarifas!);
        Assert.All(tarifas!, x => Assert.Equal(2027, x.Temporada));
        Assert.Equal("B", tarifas.Single().Licencia);
        Assert.False(tarifas.Single().MediaTemporada);
        Assert.Equal(55m, tarifas.Single().PrecioClub);
    }

    [Fact]
    public async Task Tarifas_WithMediaTemporadaFilter_ShouldReturnOnlyHalfSeasonRates()
    {
        await EnsureRolesAndAdminAsync();
        await AuthenticateAsMemberAsync(isMember: false);

        var response = await _httpClient.GetAsync("/api/licencias-federativas/tarifas?mediaTemporada=true");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var tarifas = await response.Content.ReadFromJsonAsync<IReadOnlyCollection<TarifaLicenciaFederativaPayload>>();
        Assert.NotNull(tarifas);
        Assert.NotEmpty(tarifas!);
        Assert.All(tarifas!, x => Assert.True(x.MediaTemporada));
        Assert.Contains(tarifas!, x => x.Id == 26 && x.Licencia == "A");
    }

    [Fact]
    public async Task CrearSolicitud_WithNonMemberClaim_ShouldReturnForbidden()
    {
        await EnsureRolesAndAdminAsync();
        await AuthenticateAsMemberAsync(isMember: false);

        var response = await _httpClient.PostAsJsonAsync("/api/licencias-federativas/me/solicitudes", new
        {
            Temporada = 2026,
            TarifaLicenciaFederativaId = 1
        });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task CrearYConsultarMisSolicitudes_WithMemberClaim_ShouldReturnOwnData()
    {
        await EnsureRolesAndAdminAsync();
        var email = $"licencia-member-{Guid.NewGuid():N}@club.test";
        await AuthenticateAsMemberAsync(email, isMember: true);
        _factory.EmailSender.Clear();

        var createResponse = await _httpClient.PostAsJsonAsync("/api/licencias-federativas/me/solicitudes", new
        {
            Temporada = 2026,
            TarifaLicenciaFederativaId = 1
        });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await createResponse.Content.ReadFromJsonAsync<SolicitudLicenciaFederativaPayload>();
        Assert.NotNull(created);
        Assert.Equal(2026, created!.Temporada);
        Assert.Equal("Pendiente", created.Estado);
        Assert.Equal(1, created.TarifaLicenciaFederativaId);
        Assert.Equal("A", created.Licencia);
        Assert.False(created.MediaTemporada);
        Assert.Equal(45m, created.PrecioClub);

        var sentMessage = Assert.Single(_factory.EmailSender.Messages);
        Assert.Equal("club@indaloaventura.com", sentMessage.To);
        Assert.Equal("Nueva solicitud de licencia federativa", sentMessage.Subject);
        Assert.Contains(email, sentMessage.HtmlBody, StringComparison.Ordinal);
        Assert.Contains(email, sentMessage.PlainTextBody, StringComparison.Ordinal);

        var listResponse = await _httpClient.GetAsync("/api/licencias-federativas/me/solicitudes");
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);
        var list = await listResponse.Content.ReadFromJsonAsync<IReadOnlyCollection<SolicitudLicenciaFederativaPayload>>();
        Assert.NotNull(list);
        Assert.Contains(list!, x => x.Id == created.Id && x.Categoria == "Mayores" && x.PrecioClub == 45m && !x.MediaTemporada);

        var detailResponse = await _httpClient.GetAsync($"/api/licencias-federativas/me/solicitudes/{created.Id:D}");
        Assert.Equal(HttpStatusCode.OK, detailResponse.StatusCode);
        var detail = await detailResponse.Content.ReadFromJsonAsync<SolicitudLicenciaFederativaPayload>();
        Assert.NotNull(detail);
        Assert.Equal(created.Id, detail!.Id);
        Assert.Equal("Andalucia, Ceuta y Melilla", detail.Territorio);
        Assert.False(detail.MediaTemporada);
        Assert.Equal(45m, detail.PrecioClub);
        Assert.True(detail.FechaCreacionUtc <= DateTime.UtcNow);
    }

    [Fact]
    public async Task CrearSolicitudDuplicadaParaMismaTemporada_ShouldReturnConflict()
    {
        await EnsureRolesAndAdminAsync();
        await AuthenticateAsMemberAsync(isMember: true);

        var firstResponse = await _httpClient.PostAsJsonAsync("/api/licencias-federativas/me/solicitudes", new
        {
            Temporada = 2026,
            TarifaLicenciaFederativaId = 1
        });
        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);

        var duplicateResponse = await _httpClient.PostAsJsonAsync("/api/licencias-federativas/me/solicitudes", new
        {
            Temporada = 2026,
            TarifaLicenciaFederativaId = 1
        });

        Assert.Equal(HttpStatusCode.Conflict, duplicateResponse.StatusCode);
    }

    [Fact]
    public async Task MisSolicitudes_ShouldKeepIsolationBetweenUsers()
    {
        await EnsureRolesAndAdminAsync();

        var ownerEmail = $"licencia-owner-{Guid.NewGuid():N}@club.test";
        await AuthenticateAsMemberAsync(ownerEmail, isMember: true);

        var createResponse = await _httpClient.PostAsJsonAsync("/api/licencias-federativas/me/solicitudes", new
        {
            Temporada = 2026,
            TarifaLicenciaFederativaId = 1
        });
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await createResponse.Content.ReadFromJsonAsync<SolicitudLicenciaFederativaPayload>();
        Assert.NotNull(created);

        await AuthenticateAsMemberAsync($"licencia-other-{Guid.NewGuid():N}@club.test", isMember: true);

        var listResponse = await _httpClient.GetAsync("/api/licencias-federativas/me/solicitudes");
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);
        var list = await listResponse.Content.ReadFromJsonAsync<IReadOnlyCollection<SolicitudLicenciaFederativaPayload>>();
        Assert.NotNull(list);
        Assert.DoesNotContain(list!, x => x.Id == created!.Id);

        var detailResponse = await _httpClient.GetAsync($"/api/licencias-federativas/me/solicitudes/{created!.Id:D}");
        Assert.Equal(HttpStatusCode.NotFound, detailResponse.StatusCode);
    }

    [Fact]
    public async Task AdminSolicitudes_WithMemberRole_ShouldReturnForbidden()
    {
        await EnsureRolesAndAdminAsync();
        await AuthenticateAsMemberAsync(isMember: true);

        var listResponse = await _httpClient.GetAsync("/api/licencias-federativas/admin/solicitudes");
        Assert.Equal(HttpStatusCode.Forbidden, listResponse.StatusCode);

        var updateResponse = await _httpClient.PutAsJsonAsync($"/api/licencias-federativas/admin/users/{Guid.NewGuid():D}/solicitudes/{Guid.NewGuid():D}/estado", new
        {
            Estado = "Confirmada"
        });
        Assert.Equal(HttpStatusCode.Forbidden, updateResponse.StatusCode);
    }

    [Fact]
    public async Task AdminSolicitudes_ShouldListAndFilterAllRequests()
    {
        await EnsureRolesAndAdminAsync();

        var firstUser = await CreateSolicitudAsMemberAsync($"admin-list-1-{Guid.NewGuid():N}@club.test", 2026, 1);
        var secondUser = await CreateSolicitudAsMemberAsync($"admin-list-2-{Guid.NewGuid():N}@club.test", 2026, 2);

        await AuthenticateAsAdminAsync();

        var allResponse = await _httpClient.GetAsync("/api/licencias-federativas/admin/solicitudes");
        Assert.Equal(HttpStatusCode.OK, allResponse.StatusCode);
        var allItems = await allResponse.Content.ReadFromJsonAsync<IReadOnlyCollection<AdminSolicitudLicenciaFederativaPayload>>();
        Assert.NotNull(allItems);
        Assert.Contains(allItems!, x => x.Id == firstUser.Solicitud.Id && x.UserId == firstUser.UserId && x.UserEmail == firstUser.Email);
        Assert.Contains(allItems, x => x.Id == secondUser.Solicitud.Id && x.UserId == secondUser.UserId && x.UserEmail == secondUser.Email);
        Assert.Contains(allItems, x => x.Id == firstUser.Solicitud.Id && x.MediaTemporada == firstUser.Solicitud.MediaTemporada);

        var byEstadoResponse = await _httpClient.GetAsync("/api/licencias-federativas/admin/solicitudes?estado=Pendiente");
        Assert.Equal(HttpStatusCode.OK, byEstadoResponse.StatusCode);
        var byEstado = await byEstadoResponse.Content.ReadFromJsonAsync<IReadOnlyCollection<AdminSolicitudLicenciaFederativaPayload>>();
        Assert.NotNull(byEstado);
        Assert.Contains(byEstado!, x => x.Id == firstUser.Solicitud.Id);
        Assert.Contains(byEstado, x => x.Id == secondUser.Solicitud.Id);

        var byUserTemporadaResponse = await _httpClient.GetAsync($"/api/licencias-federativas/admin/solicitudes?userId={firstUser.UserId:D}&temporada=2026");
        Assert.Equal(HttpStatusCode.OK, byUserTemporadaResponse.StatusCode);
        var byUserTemporada = await byUserTemporadaResponse.Content.ReadFromJsonAsync<IReadOnlyCollection<AdminSolicitudLicenciaFederativaPayload>>();
        Assert.NotNull(byUserTemporada);
        Assert.Single(byUserTemporada!);
        Assert.Equal(firstUser.Solicitud.Id, byUserTemporada.Single().Id);
    }

    [Fact]
    public async Task AdminSolicitudes_ShouldUpdateEstado_ForMatchingUserAndSolicitud()
    {
        await EnsureRolesAndAdminAsync();
        var created = await CreateSolicitudAsMemberAsync($"admin-update-{Guid.NewGuid():N}@club.test", 2026, 1);

        await AuthenticateAsAdminAsync();

        var response = await _httpClient.PutAsJsonAsync(
            $"/api/licencias-federativas/admin/users/{created.UserId:D}/solicitudes/{created.Solicitud.Id:D}/estado",
            new { Estado = "Confirmada" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updated = await response.Content.ReadFromJsonAsync<AdminSolicitudLicenciaFederativaPayload>();
        Assert.NotNull(updated);
        Assert.Equal(created.Solicitud.Id, updated!.Id);
        Assert.Equal("Confirmada", updated.Estado);
        Assert.Equal(created.UserId, updated.UserId);
        Assert.Equal(created.Email, updated.UserEmail);
        Assert.Equal(created.Solicitud.Temporada, updated.Temporada);
        Assert.Equal(created.Solicitud.TarifaLicenciaFederativaId, updated.TarifaLicenciaFederativaId);
    }

    [Fact]
    public async Task AdminSolicitudes_ShouldReturnNotFound_ForMissingOrMismatchedSolicitud()
    {
        await EnsureRolesAndAdminAsync();
        var created = await CreateSolicitudAsMemberAsync($"admin-mismatch-{Guid.NewGuid():N}@club.test", 2026, 1);

        await AuthenticateAsAdminAsync();

        var missingResponse = await _httpClient.PutAsJsonAsync(
            $"/api/licencias-federativas/admin/users/{created.UserId:D}/solicitudes/{Guid.NewGuid():D}/estado",
            new { Estado = "Cancelada" });
        Assert.Equal(HttpStatusCode.NotFound, missingResponse.StatusCode);

        var mismatchedResponse = await _httpClient.PutAsJsonAsync(
            $"/api/licencias-federativas/admin/users/{Guid.NewGuid():D}/solicitudes/{created.Solicitud.Id:D}/estado",
            new { Estado = "Cancelada" });
        Assert.Equal(HttpStatusCode.NotFound, mismatchedResponse.StatusCode);
    }

    private async Task AuthenticateAsMemberAsync(bool isMember)
    {
        await AuthenticateAsMemberAsync($"member-{Guid.NewGuid():N}@club.test", isMember);
    }

    private async Task AuthenticateAsMemberAsync(string email, bool isMember)
    {
        const string password = "Test1234A";
        await _httpClient.PostAsJsonAsync("/api/auth/register", new { Email = email, Password = password });
        await SetUserMembershipAsync(email, isMember);

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

    private async Task<(Guid UserId, string Email, SolicitudLicenciaFederativaPayload Solicitud)> CreateSolicitudAsMemberAsync(string email, int temporada, int tarifaLicenciaFederativaId)
    {
        await AuthenticateAsMemberAsync(email, isMember: true);

        var createResponse = await _httpClient.PostAsJsonAsync("/api/licencias-federativas/me/solicitudes", new
        {
            Temporada = temporada,
            TarifaLicenciaFederativaId = tarifaLicenciaFederativaId
        });
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var solicitud = await createResponse.Content.ReadFromJsonAsync<SolicitudLicenciaFederativaPayload>();
        Assert.NotNull(solicitud);

        using var scope = _services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Usuario>>();
        var user = await userManager.FindByEmailAsync(email);
        Assert.NotNull(user);

        return (user!.Id, email, solicitud!);
    }

    private async Task SetUserMembershipAsync(string email, bool isMember)
    {
        using var scope = _services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Usuario>>();
        var user = await userManager.FindByEmailAsync(email);
        Assert.NotNull(user);

        user!.IsMember = isMember;
        var result = await userManager.UpdateAsync(user);
        Assert.True(result.Succeeded, string.Join("; ", result.Errors.Select(x => x.Description)));
    }

    private async Task AddTarifaAsync(TarifaLicenciaFederativa tarifa)
    {
        using var scope = _services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.TarifasLicenciasFederativas.AddAsync(tarifa);
        await dbContext.SaveChangesAsync();
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

    private sealed record SolicitudLicenciaFederativaPayload(
        Guid Id,
        int Temporada,
        string Estado,
        DateTime FechaCreacionUtc,
        int TarifaLicenciaFederativaId,
        string Licencia,
        string Categoria,
        string Territorio,
        bool MediaTemporada,
        decimal PrecioClub,
        decimal? PrecioIndependiente);

    private sealed record TarifaLicenciaFederativaPayload(
        int Id,
        int Temporada,
        string Licencia,
        string Categoria,
        string Territorio,
        bool MediaTemporada,
        decimal PrecioClub,
        decimal? PrecioIndependiente);

    private sealed record AdminSolicitudLicenciaFederativaPayload(
        Guid Id,
        Guid UserId,
        string UserEmail,
        int Temporada,
        string Estado,
        DateTime FechaCreacionUtc,
        int TarifaLicenciaFederativaId,
        string Licencia,
        string Categoria,
        string Territorio,
        bool MediaTemporada,
        decimal PrecioClub,
        decimal? PrecioIndependiente);
}
