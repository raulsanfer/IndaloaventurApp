namespace IndaloaventurApp.Frontend.Tests;

using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using IndaloaventurApp.SharedUI.Models.Auth;
using IndaloaventurApp.SharedUI.Models.Licenses;
using IndaloaventurApp.Web.Client.Infrastructure.Licenses;

public sealed class FederativeLicenseApiClientTests
{
    [Fact]
    public async Task GetMyFederativeLicensesAsync_GetsLicensesFromMeEndpoint()
    {
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(new AuthSession("token", "Bearer", 3600, true, new[] { "Member" }));

        var handler = new StubHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Get, request.Method);
            Assert.Equal("/api/licencias-federativas/me/solicitudes", request.RequestUri?.AbsolutePath);

            var responseBody = JsonSerializer.Serialize(new[]
            {
                new
                {
                    id = Guid.Parse("2af7d8c8-4112-4a6e-b48f-65821b4f2914"),
                    temporada = 2026,
                    estado = "Pendiente",
                    fechaCreacionUtc = "2026-05-21T14:30:00Z",
                    tarifaLicenciaFederativaId = 12,
                    licencia = "B Plus",
                    categoria = "Adulto",
                    territorio = "Nacional",
                    mediaTemporada = true,
                    precioClub = 95.50,
                    precioIndependiente = 120.00
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

        var sut = new FederativeLicenseApiClient(httpClient, sessionService);

        var result = await sut.GetMyFederativeLicensesAsync();

        Assert.True(result.IsSuccess);
        var item = Assert.Single(result.Value!);
        Assert.Equal(2026, item.Temporada);
        Assert.True(item.MediaTemporada);
        Assert.Equal("B Plus", item.Licencia);
        Assert.Equal("Pendiente", item.Estado);
        Assert.Equal("Nacional", item.AmbitoTerritorial);
    }

    [Fact]
    public async Task GetAvailableRatesAsync_UsesSeasonAndModalityFilters()
    {
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(new AuthSession("token", "Bearer", 3600, true, new[] { "Member" }));

        var handler = new StubHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Get, request.Method);
            Assert.Equal("/api/licencias-federativas/tarifas", request.RequestUri?.AbsolutePath);
            Assert.Equal("?temporada=2027&mediaTemporada=true", request.RequestUri?.Query);

            var responseBody = JsonSerializer.Serialize(new[]
            {
                new
                {
                    id = 14,
                    temporada = 2027,
                    mediaTemporada = true,
                    licencia = "A",
                    categoria = "Juvenil",
                    territorio = "Autonomico",
                    precioClub = 42.15,
                    precioIndependiente = 57.25
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

        var sut = new FederativeLicenseApiClient(httpClient, sessionService);

        var result = await sut.GetAvailableRatesAsync(2027, true);

        Assert.True(result.IsSuccess);
        var item = Assert.Single(result.Value!);
        Assert.Equal(14, item.Id);
        Assert.Equal(2027, item.Temporada);
        Assert.True(item.MediaTemporada);
        Assert.Equal("A", item.Licencia);
        Assert.Equal(42.15m, item.PrecioClub);
    }

    [Fact]
    public async Task CreateFederativeLicenseRequestAsync_PostsExpectedPayload()
    {
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(new AuthSession("token", "Bearer", 3600, true, new[] { "Member" }));

        var handler = new StubHttpMessageHandler(async request =>
        {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.Equal("/api/licencias-federativas/me/solicitudes", request.RequestUri?.AbsolutePath);

            var json = await request.Content!.ReadAsStringAsync();
            Assert.Contains("\"temporada\":2026", json);
            Assert.Contains("\"tarifaLicenciaFederativaId\":18", json);

            var responseBody = JsonSerializer.Serialize(new
            {
                id = Guid.Parse("5a7c88d5-64f2-4971-a8d4-e96d7719b601"),
                temporada = 2026,
                estado = "Pendiente",
                fechaCreacionUtc = "2026-06-02T09:10:00Z",
                tarifaLicenciaFederativaId = 18,
                licencia = "B Plus",
                categoria = "Adulto",
                territorio = "Nacional",
                mediaTemporada = false,
                precioClub = 88.50,
                precioIndependiente = 120.00
            });

            return new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent(responseBody, Encoding.UTF8, "application/json")
            };
        });

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://localhost")
        };

        var sut = new FederativeLicenseApiClient(httpClient, sessionService);

        var result = await sut.CreateFederativeLicenseRequestAsync(new CreateFederativeLicenseRequest(2026, 18));

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2026, result.Value.Temporada);
        Assert.False(result.Value.MediaTemporada);
        Assert.Equal("B Plus", result.Value.Licencia);
    }

    [Fact]
    public async Task GetMyFederativeLicensesAsync_ReturnsNotMemberError_ForAdminRole()
    {
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(new AuthSession("token", "Bearer", 3600, false, new[] { "Admin" }));

        using var httpClient = new HttpClient(new StubHttpMessageHandler(_ => throw new InvalidOperationException("Should not call API")))
        {
            BaseAddress = new Uri("https://localhost")
        };

        var sut = new FederativeLicenseApiClient(httpClient, sessionService);

        var result = await sut.GetMyFederativeLicensesAsync();

        Assert.False(result.IsSuccess);
        Assert.Equal("licenses.not_member", result.Error?.Code);
    }
}
