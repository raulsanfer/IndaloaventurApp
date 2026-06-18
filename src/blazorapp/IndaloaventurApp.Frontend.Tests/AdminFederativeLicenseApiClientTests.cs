namespace IndaloaventurApp.Frontend.Tests;

using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using IndaloaventurApp.SharedUI.Models.Auth;
using IndaloaventurApp.SharedUI.Models.Licenses;
using IndaloaventurApp.Web.Client.Infrastructure.Licenses;

public sealed class AdminFederativeLicenseApiClientTests
{
    [Fact]
    public async Task GetFederativeLicensesAsync_UsesAdminEndpointAndFilters()
    {
        var sessionService = new RecordingSessionService();
        var userId = Guid.Parse("2a5c3c5a-6aa5-4bcc-b22e-ef6f6955366a");
        sessionService.SetSession(new AuthSession("token", "Bearer", 3600, false, new[] { "Admin" }, userId));

        var handler = new StubHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Get, request.Method);
            Assert.Equal("/api/licencias-federativas/admin/solicitudes", request.RequestUri?.AbsolutePath);
            Assert.Equal($"?userId={userId:D}&temporada=2026&estado=Pendiente", request.RequestUri?.Query);

            var responseBody = JsonSerializer.Serialize(new[]
            {
                new
                {
                    id = Guid.Parse("a8d9cb45-7b4d-43e1-a658-1fd144f6b4ee"),
                    userId,
                    userEmail = "admin@club.es",
                    temporada = 2026,
                    estado = "Pendiente",
                    fechaCreacionUtc = "2026-06-17T10:00:00Z",
                    tarifaLicenciaFederativaId = 18,
                    licencia = "B Plus",
                    categoria = "Adulto",
                    territorio = "Nacional",
                    precioClub = 88.5m,
                    precioIndependiente = 120m
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

        var sut = new AdminFederativeLicenseApiClient(httpClient, sessionService);

        var result = await sut.GetFederativeLicensesAsync(new AdminFederativeLicenseQuery(userId, 2026, "Pendiente"));

        Assert.True(result.IsSuccess);
        var item = Assert.Single(result.Value!);
        Assert.Equal(userId, item.UserId);
        Assert.Equal("admin@club.es", item.UserEmail);
        Assert.Equal("Pendiente", item.Estado);
        Assert.Equal("Nacional", item.AmbitoTerritorial);
    }

    [Fact]
    public async Task UpdateFederativeLicenseStatusAsync_PutsExpectedPayload()
    {
        var sessionService = new RecordingSessionService();
        var userId = Guid.Parse("2a5c3c5a-6aa5-4bcc-b22e-ef6f6955366a");
        var requestId = Guid.Parse("a8d9cb45-7b4d-43e1-a658-1fd144f6b4ee");
        sessionService.SetSession(new AuthSession("token", "Bearer", 3600, false, new[] { "Admin" }, userId));

        var handler = new StubHttpMessageHandler(async request =>
        {
            Assert.Equal(HttpMethod.Put, request.Method);
            Assert.Equal($"/api/licencias-federativas/admin/users/{userId:D}/solicitudes/{requestId:D}/estado", request.RequestUri?.AbsolutePath);

            var json = await request.Content!.ReadAsStringAsync();
            Assert.Contains("\"estado\":\"Confirmada\"", json);

            var responseBody = JsonSerializer.Serialize(new
            {
                id = requestId,
                userId,
                userEmail = "admin@club.es",
                temporada = 2026,
                estado = "Confirmada",
                fechaCreacionUtc = "2026-06-17T10:00:00Z",
                tarifaLicenciaFederativaId = 18,
                licencia = "B Plus",
                categoria = "Adulto",
                territorio = "Nacional",
                precioClub = 88.5m,
                precioIndependiente = 120m
            });

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseBody, Encoding.UTF8, "application/json")
            };
        });

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://localhost")
        };

        var sut = new AdminFederativeLicenseApiClient(httpClient, sessionService);

        var result = await sut.UpdateFederativeLicenseStatusAsync(new UpdateAdminFederativeLicenseStatusRequest(userId, requestId, "Confirmada"));

        Assert.True(result.IsSuccess);
        Assert.Equal("Confirmada", result.Value!.Estado);
        Assert.Equal(requestId, result.Value.Id);
    }

    [Fact]
    public async Task GetFederativeLicensesAsync_ReturnsForbiddenForNonAdminSession()
    {
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(new AuthSession("token", "Bearer", 3600, true, new[] { "Member" }));

        using var httpClient = new HttpClient(new StubHttpMessageHandler(_ => throw new InvalidOperationException("Should not call API")))
        {
            BaseAddress = new Uri("https://localhost")
        };

        var sut = new AdminFederativeLicenseApiClient(httpClient, sessionService);

        var result = await sut.GetFederativeLicensesAsync(new AdminFederativeLicenseQuery());

        Assert.False(result.IsSuccess);
        Assert.Equal("licenses.admin_forbidden", result.Error?.Code);
    }
}
