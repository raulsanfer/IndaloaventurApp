using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using IndaloAventurApi.Domain.LicenciasFederativas;
using IndaloAventurApi.Infrastructure.Persistence.DbContext;
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
    public async Task Tipos_ConsultaAutenticadaOrdenadaYSinMantenimiento()
    {
        _httpClient.DefaultRequestHeaders.Authorization = null;
        Assert.Equal(HttpStatusCode.Unauthorized, (await _httpClient.GetAsync("/api/licencias-federativas/tipos")).StatusCode);
        await EnsureRolesAndAdminAsync();
        await AuthenticateAsMemberAsync(isMember: false);
        var tipos = await _httpClient.GetFromJsonAsync<TipoLicenciaPayload[]>("/api/licencias-federativas/tipos");
        Assert.Equal(new[] { 1, 2, 3 }, tipos!.Select(x => x.Id));
        Assert.Equal(new[] { "Completa", "Media 01/07 - 31/12", "Día" }, tipos!.Select(x => x.Nombre));
        await AuthenticateAsAdminAsync();
        Assert.Equal(HttpStatusCode.OK, (await _httpClient.GetAsync("/api/licencias-federativas/tipos")).StatusCode);
        Assert.Equal(HttpStatusCode.MethodNotAllowed, (await _httpClient.PostAsJsonAsync("/api/licencias-federativas/tipos", new { Nombre = "Otro" })).StatusCode);
    }

    [Theory]
    [InlineData("tipoLicenciaId=0")]
    [InlineData("tipoLicenciaId=-1")]
    [InlineData("tipoLicenciaId=abc")]
    [InlineData("mediaTemporada=true")]
    [InlineData("mediaTemporada=false&tipoLicenciaId=1")]
    public async Task Tarifas_RechazaFiltroInvalidoOAntiguo(string filtro)
    {
        await EnsureRolesAndAdminAsync();
        await AuthenticateAsMemberAsync(isMember: false);
        var response = await _httpClient.GetAsync("/api/licencias-federativas/tarifas?" + filtro);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("application/problem+json", response.Content.Headers.ContentType!.ToString());
        if (filtro.Contains("mediaTemporada"))
            Assert.Contains("tipoLicenciaId", await response.Content.ReadAsStringAsync());
    }

    [Theory]
    [InlineData("temporada=2999")]
    [InlineData("temporada=2999&tipoLicenciaId=3")]
    [InlineData("tipoLicenciaId=9999")]
    public async Task Tarifas_SinCoincidenciasDevuelveColeccionVacia(string filtro)
    {
        await EnsureRolesAndAdminAsync();
        await AuthenticateAsMemberAsync(isMember: false);
        var tarifas = await _httpClient.GetFromJsonAsync<TarifaLicenciaFederativaPayload[]>("/api/licencias-federativas/tarifas?" + filtro);
        Assert.Empty(tarifas!);
    }

    [Theory]
    [InlineData(3, null, null)]
    [InlineData(3, "2026-05-01", null)]
    [InlineData(3, null, "2026-05-01")]
    [InlineData(3, "2026-05-02", "2026-05-01")]
    [InlineData(3, "2025-12-31", "2026-01-01")]
    [InlineData(3, "2026-12-31", "2027-01-01")]
    [InlineData(1, "2026-05-01", null)]
    [InlineData(2, "2026-05-02", "2026-05-01")]
    [InlineData(1, "2027-01-01", "2027-01-02")]
    [InlineData(3, "no-es-fecha", "2026-05-01")]
    public async Task Alta_CoberturaInvalidaDevuelve400SinPersistir(int tipo, string? inicio, string? fin)
    {
        await EnsureRolesAndAdminAsync();
        await AuthenticateAsMemberAsync(isMember: true);
        var tarifaId = tipo == 3 ? await CrearTarifaDiariaAsync() : tipo == 1 ? 1 : 26;
        var response = await _httpClient.PostAsJsonAsync("/api/licencias-federativas/me/solicitudes", new
        {
            Temporada = 2026, TarifaLicenciaFederativaId = tarifaId, FechaInicio = inicio, FechaFin = fin
        });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("application/problem+json", response.Content.Headers.ContentType!.ToString());
        Assert.Empty((await _httpClient.GetFromJsonAsync<SolicitudLicenciaFederativaPayload[]>("/api/licencias-federativas/me/solicitudes"))!);
    }

    [Fact]
    public async Task Diarias_ConvivenConCompletaYConservanCoberturaEnConsultasYCambioAdministrativo()
    {
        await EnsureRolesAndAdminAsync();
        var email = $"diaria-{Guid.NewGuid():N}@club.test";
        await AuthenticateAsMemberAsync(email, true);
        var tarifaId = await CrearTarifaDiariaAsync();
        var inicio = new DateOnly(2026, 5, 1);
        var response = await _httpClient.PostAsJsonAsync("/api/licencias-federativas/me/solicitudes", new
        {
            Temporada = 2026, TarifaLicenciaFederativaId = tarifaId, FechaInicio = inicio, FechaFin = inicio
        });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var primera = (await response.Content.ReadFromJsonAsync<SolicitudLicenciaFederativaPayload>())!;
        Assert.Equal("Día", primera.TipoLicenciaNombre);
        Assert.Equal(inicio, primera.FechaInicio);
        Assert.Equal(inicio, primera.FechaFin);
        var segundaResponse = await _httpClient.PostAsJsonAsync("/api/licencias-federativas/me/solicitudes", new
        {
            Temporada = 2026, TarifaLicenciaFederativaId = tarifaId, FechaInicio = "2026-06-01", FechaFin = "2026-06-04"
        });
        Assert.Equal(HttpStatusCode.Created, segundaResponse.StatusCode);
        var segunda = (await segundaResponse.Content.ReadFromJsonAsync<SolicitudLicenciaFederativaPayload>())!;
        Assert.Equal(7.5m, segunda.PrecioClub);
        Assert.NotEqual(primera.Id, segunda.Id);
        Assert.Equal(HttpStatusCode.Created, (await _httpClient.PostAsJsonAsync("/api/licencias-federativas/me/solicitudes", new
        { Temporada = 2026, TarifaLicenciaFederativaId = 1 })).StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, (await _httpClient.PostAsJsonAsync("/api/licencias-federativas/me/solicitudes", new
        { Temporada = 2026, TarifaLicenciaFederativaId = 26 })).StatusCode);

        var propias = (await _httpClient.GetFromJsonAsync<SolicitudLicenciaFederativaPayload[]>("/api/licencias-federativas/me/solicitudes"))!;
        Assert.Equal(3, propias.Length);
        Assert.Equal(2, propias.Count(x => x.TipoLicenciaId == 3));
        Assert.Null(Assert.Single(propias, x => x.TipoLicenciaId == 1).FechaInicio);
        Assert.Equal(primera, await _httpClient.GetFromJsonAsync<SolicitudLicenciaFederativaPayload>($"/api/licencias-federativas/me/solicitudes/{primera.Id}"));

        Guid userId;
        using (var scope = _services.CreateScope())
            userId = (await scope.ServiceProvider.GetRequiredService<UserManager<Usuario>>().FindByEmailAsync(email))!.Id;
        await AuthenticateAsAdminAsync();
        var actualizadaResponse = await _httpClient.PutAsJsonAsync($"/api/licencias-federativas/admin/users/{userId}/solicitudes/{primera.Id}/estado", new { Estado = "Confirmada" });
        Assert.Equal(HttpStatusCode.OK, actualizadaResponse.StatusCode);
        var actualizada = (await actualizadaResponse.Content.ReadFromJsonAsync<AdminSolicitudLicenciaFederativaPayload>())!;
        Assert.Equal("Confirmada", actualizada.Estado);
        Assert.Equal(primera.FechaInicio, actualizada.FechaInicio);
        Assert.Equal(primera.FechaFin, actualizada.FechaFin);
        Assert.Equal(primera.TipoLicenciaId, actualizada.TipoLicenciaId);
        Assert.Equal(primera.FechaCreacionUtc, actualizada.FechaCreacionUtc);
        var administrativas = (await _httpClient.GetFromJsonAsync<AdminSolicitudLicenciaFederativaPayload[]>($"/api/licencias-federativas/admin/solicitudes?userId={userId}&temporada=2026"))!;
        Assert.Equal(3, administrativas.Length);
        Assert.Equal("Pendiente", Assert.Single(administrativas, x => x.Id == segunda.Id).Estado);
    }

    [Theory]
    [InlineData(1, "2026-01-01", "2026-12-31")]
    [InlineData(26, null, null)]
    public async Task NoDiaria_AdmiteCoberturaOpcional(int tarifa, string? inicio, string? fin)
    {
        await EnsureRolesAndAdminAsync();
        await AuthenticateAsMemberAsync(isMember: true);
        var response = await _httpClient.PostAsJsonAsync("/api/licencias-federativas/me/solicitudes", new
        { Temporada = 2026, TarifaLicenciaFederativaId = tarifa, FechaInicio = inicio, FechaFin = fin });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var dto = (await response.Content.ReadFromJsonAsync<SolicitudLicenciaFederativaPayload>())!;
        Assert.Equal(inicio is null ? (DateOnly?)null : DateOnly.Parse(inicio), dto.FechaInicio);
        Assert.Equal(fin is null ? (DateOnly?)null : DateOnly.Parse(fin), dto.FechaFin);
    }

    [Theory]
    [InlineData(999999, 2026, HttpStatusCode.NotFound)]
    [InlineData(1, 2027, HttpStatusCode.Conflict)]
    public async Task Alta_RechazaTarifaInexistenteOTemporadaIncoherente(int tarifa, int temporada, HttpStatusCode esperado)
    {
        await EnsureRolesAndAdminAsync();
        await AuthenticateAsMemberAsync(isMember: true);
        Assert.Equal(esperado, (await _httpClient.PostAsJsonAsync("/api/licencias-federativas/me/solicitudes", new
        { Temporada = temporada, TarifaLicenciaFederativaId = tarifa })).StatusCode);
    }

    private async Task<int> CrearTarifaDiariaAsync()
    {
        using var scope = _services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var tarifa = TarifaLicenciaFederativa.Crear(2026, "D-" + Guid.NewGuid().ToString("N"), "Mayores", 7.5m, null, "Andalucía", TipoLicencia.DiaId);
        db.Add(tarifa);
        await db.SaveChangesAsync();
        return tarifa.Id;
    }

    private sealed record TipoLicenciaPayload(int Id, string Nombre);

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
        Assert.Contains(tarifas, x => x.Id == 1 && x.Temporada == 2026 && x.Licencia == "A" && x.TipoLicenciaId == TipoLicencia.CompletaId);
        Assert.Contains(tarifas, x => x.Id == 26 && x.Temporada == 2026 && x.Licencia == "A" && x.TipoLicenciaId == TipoLicencia.MediaId);
    }

    [Fact]
    public async Task Tarifas_WithAuthenticatedAdmin_AndTemporadaFilter_ShouldReturnOnlyMatchingSeason()
    {
        await EnsureRolesAndAdminAsync();
        await AddTarifaAsync(TarifaLicenciaFederativa.Crear(2027, "B", "Juvenil", 55m, 75m, "Nacional", TipoLicencia.CompletaId));
        await AuthenticateAsAdminAsync();

        var response = await _httpClient.GetAsync("/api/licencias-federativas/tarifas?temporada=2027");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var tarifas = await response.Content.ReadFromJsonAsync<IReadOnlyCollection<TarifaLicenciaFederativaPayload>>();
        Assert.NotNull(tarifas);
        Assert.Single(tarifas!);
        Assert.All(tarifas!, x => Assert.Equal(2027, x.Temporada));
        Assert.Equal("B", tarifas.Single().Licencia);
        Assert.Equal(TipoLicencia.CompletaId, tarifas.Single().TipoLicenciaId);
        Assert.Equal(55m, tarifas.Single().PrecioClub);
    }

    [Fact]
    public async Task Tarifas_WithTipoLicenciaFilter_ShouldReturnOnlyHalfSeasonRates()
    {
        await EnsureRolesAndAdminAsync();
        await AuthenticateAsMemberAsync(isMember: false);

        var response = await _httpClient.GetAsync("/api/licencias-federativas/tarifas?tipoLicenciaId=2");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var tarifas = await response.Content.ReadFromJsonAsync<IReadOnlyCollection<TarifaLicenciaFederativaPayload>>();
        Assert.NotNull(tarifas);
        Assert.NotEmpty(tarifas!);
        Assert.All(tarifas!, x => Assert.Equal(TipoLicencia.MediaId, x.TipoLicenciaId));
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
        Assert.Equal(TipoLicencia.CompletaId, created.TipoLicenciaId);
        Assert.Equal(45m, created.PrecioClub);

        var sentMessage = Assert.Single(_factory.EmailSender.Messages);
        Assert.Equal("club@indaloaventura.com", sentMessage.To);
        Assert.Equal("Nueva solicitud de licencia federativa", sentMessage.Subject);
        Assert.Contains(email, sentMessage.HtmlBody, StringComparison.Ordinal);
        Assert.Contains(email, sentMessage.PlainTextBody, StringComparison.Ordinal);

        var listResponse = await _httpClient.GetAsync("/api/licencias-federativas/me/solicitudes");
        Assert.True(listResponse.IsSuccessStatusCode, await listResponse.Content.ReadAsStringAsync());
        var list = await listResponse.Content.ReadFromJsonAsync<IReadOnlyCollection<SolicitudLicenciaFederativaPayload>>();
        Assert.NotNull(list);
        Assert.Contains(list!, x => x.Id == created.Id && x.Categoria == "Mayores" && x.PrecioClub == 45m && x.TipoLicenciaId == TipoLicencia.CompletaId);

        var detailResponse = await _httpClient.GetAsync($"/api/licencias-federativas/me/solicitudes/{created.Id:D}");
        Assert.Equal(HttpStatusCode.OK, detailResponse.StatusCode);
        var detail = await detailResponse.Content.ReadFromJsonAsync<SolicitudLicenciaFederativaPayload>();
        Assert.NotNull(detail);
        Assert.Equal(created.Id, detail!.Id);
        Assert.Equal("Andalucia, Ceuta y Melilla", detail.Territorio);
        Assert.Equal(TipoLicencia.CompletaId, detail.TipoLicenciaId);
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
        Assert.Contains(allItems, x => x.Id == firstUser.Solicitud.Id && x.TipoLicenciaId == firstUser.Solicitud.TipoLicenciaId);

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
        await _factory.AuthenticateAsAdminAsync(_httpClient);
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
        await _factory.EnsureRolesAndAdminAsync();
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
        int TipoLicenciaId,
        string TipoLicenciaNombre,
        decimal PrecioClub,
        decimal? PrecioIndependiente,
        DateOnly? FechaInicio,
        DateOnly? FechaFin);

    private sealed record TarifaLicenciaFederativaPayload(
        int Id,
        int Temporada,
        string Licencia,
        string Categoria,
        string Territorio,
        int TipoLicenciaId,
        string TipoLicenciaNombre,
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
        int TipoLicenciaId,
        string TipoLicenciaNombre,
        decimal PrecioClub,
        decimal? PrecioIndependiente,
        DateOnly? FechaInicio,
        DateOnly? FechaFin);
}
