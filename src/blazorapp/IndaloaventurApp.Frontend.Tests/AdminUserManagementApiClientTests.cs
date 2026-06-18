namespace IndaloaventurApp.Frontend.Tests;

using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using IndaloaventurApp.SharedUI.Models.Member;
using IndaloaventurApp.Web.Client.Infrastructure.Member;

public sealed class AdminUserManagementApiClientTests
{
    [Fact]
    public async Task GetUsersAsync_UsesUsersEndpointAndReturnsMatchingUsers()
    {
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(TestSessions.AdminSession);

        var handler = new StubHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Get, request.Method);
            Assert.Equal("/api/users", request.RequestUri?.AbsolutePath);
            Assert.Equal("?email=ana%40club.es", request.RequestUri?.Query);

            var responseBody = JsonSerializer.Serialize(new[]
            {
                new
                {
                    userId = Guid.Parse("68ec4927-bb02-4ff5-aaf1-1df912513e62"),
                    email = "ana@club.es",
                    isMember = false,
                    isActive = false,
                    roles = new[] { "Member" }
                },
                new
                {
                    userId = Guid.Parse("a9f2f781-8319-422e-8a84-41393c5702ba"),
                    email = "otro@club.es",
                    isMember = true,
                    isActive = true,
                    roles = new[] { "Admin" }
                }
            });

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseBody, Encoding.UTF8, "application/json")
            });
        });

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://localhost")
        };

        var sut = new AdminUserManagementApiClient(httpClient, sessionService);

        var result = await sut.GetUsersAsync("ANA@CLUB.ES");

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.Count);
        Assert.Equal("ana@club.es", result.Value![0].Email);
        Assert.False(result.Value![0].IsActive);
    }

    [Fact]
    public async Task GetUsersAsync_WithoutEmail_UsesUsersEndpointWithoutQuery()
    {
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(TestSessions.AdminSession);

        var handler = new StubHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Get, request.Method);
            Assert.Equal("/api/users", request.RequestUri?.AbsolutePath);
            Assert.True(string.IsNullOrEmpty(request.RequestUri?.Query));

            var responseBody = JsonSerializer.Serialize(new[]
            {
                new
                {
                    userId = Guid.Parse("68ec4927-bb02-4ff5-aaf1-1df912513e62"),
                    email = "ana@club.es",
                    isMember = false,
                    roles = new[] { "Member" }
                }
            });

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseBody, Encoding.UTF8, "application/json")
            });
        });

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://localhost")
        };

        var sut = new AdminUserManagementApiClient(httpClient, sessionService);

        var result = await sut.GetUsersAsync();

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!);
        Assert.True(result.Value![0].IsActive);
    }

    [Fact]
    public async Task CreateMemberFileAsync_PostsCompatiblePayloadForCurrentBackend()
    {
        var userId = Guid.NewGuid();
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(TestSessions.AdminSession);

        var handler = new StubHttpMessageHandler(async request =>
        {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.Equal($"/api/fichas-socio/{userId}", request.RequestUri?.AbsolutePath);

            var rawJson = await request.Content!.ReadAsStringAsync();
            var payload = JsonSerializer.Deserialize<UpdateMemberFileApiPayload>(rawJson, new JsonSerializerOptions(JsonSerializerDefaults.Web));

            Assert.NotNull(payload);
            Assert.Equal("Pendiente", payload.Nombre);
            Assert.Equal("Pendiente", payload.Apellidos);
            Assert.Matches(@"^\d{8}A$", payload.Dni!);
            Assert.Equal("ana@club.es", payload.Email);
            Assert.Equal("Pendiente de completar", payload.Direccion);
            Assert.Equal("00000", payload.CodigoPostal);
            Assert.Equal("Pendiente", payload.Poblacion);
            Assert.Equal("Pendiente", payload.Provincia);
            Assert.Equal("+34000000000", payload.Tlf);
            Assert.Equal(new DateOnly(1900, 1, 1), payload.FechaNacimiento);
            Assert.True(payload.AceptaPoliticaPrivacidad);
            Assert.DoesNotContain("isMember", rawJson, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("roles", rawJson, StringComparison.OrdinalIgnoreCase);

            return new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent(JsonSerializer.Serialize(new
                {
                    userId,
                    cargoId = (int?)null,
                    nombre = (string?)null,
                    apellidos = (string?)null,
                    dni = (string?)null,
                    fechaNacimiento = "1900-01-01",
                    direccion = (string?)null,
                    codigoPostal = (string?)null,
                    poblacion = (string?)null,
                    provincia = (string?)null,
                    tlf = (string?)null,
                    email = "ana@club.es",
                    alergias = (string?)null,
                    aceptaPoliticaPrivacidad = false,
                    aceptaUsoImagenes = false,
                    aceptaCobroCuenta = false
                }), Encoding.UTF8, "application/json")
            };
        });

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://localhost")
        };

        var sut = new AdminUserManagementApiClient(httpClient, sessionService);

        var result = await sut.CreateMemberFileAsync(userId, "ANA@CLUB.ES");

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("ana@club.es", result.Value.Email);
    }

    [Fact]
    public async Task UpdateMemberFileAsync_PutsAdminPayload()
    {
        var userId = Guid.NewGuid();
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(TestSessions.AdminSession);

        var handler = new StubHttpMessageHandler(async request =>
        {
            Assert.Equal(HttpMethod.Put, request.Method);
            Assert.Equal($"/api/fichas-socio/{userId}", request.RequestUri?.AbsolutePath);

            var rawJson = await request.Content!.ReadAsStringAsync();
            var payload = JsonSerializer.Deserialize<UpdateMemberFileApiPayload>(rawJson, new JsonSerializerOptions(JsonSerializerDefaults.Web));

            Assert.NotNull(payload);
            Assert.Equal("Ana", payload.Nombre);
            Assert.Equal("ana@club.es", payload.Email);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(rawJson, Encoding.UTF8, "application/json")
            };
        });

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://localhost")
        };

        var sut = new AdminUserManagementApiClient(httpClient, sessionService);

        var result = await sut.UpdateMemberFileAsync(userId, new UpdateMemberSelfProfileRequest(
            2,
            "Ana",
            "Montes",
            "12345678A",
            new DateOnly(1990, 4, 18),
            "Calle Sierra 5",
            "04001",
            "Almeria",
            "Almeria",
            "600123123",
            "ana@club.es",
            "Polen",
            true,
            false,
            true));

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Ana", result.Value.Nombre);
    }

    [Fact]
    public async Task DeactivateUserAsync_PostsToDeactivateEndpoint()
    {
        var userId = Guid.NewGuid();
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(TestSessions.AdminSession);

        var handler = new StubHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.Equal($"/api/users/{userId}/deactivate", request.RequestUri?.AbsolutePath);

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NoContent));
        });

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://localhost")
        };

        var sut = new AdminUserManagementApiClient(httpClient, sessionService);

        var result = await sut.DeactivateUserAsync(userId);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
    }

    private sealed record UpdateMemberFileApiPayload(
        int? CargoId,
        string? Nombre,
        string? Apellidos,
        string? Dni,
        DateOnly FechaNacimiento,
        string? Direccion,
        string? CodigoPostal,
        string? Poblacion,
        string? Provincia,
        string? Tlf,
        string? Email,
        string? Alergias,
        bool AceptaPoliticaPrivacidad,
        bool AceptaUsoImagenes,
        bool AceptaCobroCuenta);
}
