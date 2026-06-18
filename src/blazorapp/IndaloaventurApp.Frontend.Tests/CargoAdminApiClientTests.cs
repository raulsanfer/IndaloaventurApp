namespace IndaloaventurApp.Frontend.Tests;

using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using IndaloaventurApp.SharedUI.Models.Cargos;
using IndaloaventurApp.Web.Client.Infrastructure.Cargos;

public sealed class CargoAdminApiClientTests
{
    [Fact]
    public async Task GetCargosAsync_UsesEndpointAndMapsPayload()
    {
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(TestSessions.AdminSession);

        var handler = new StubHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Get, request.Method);
            Assert.Equal("/api/cargos", request.RequestUri?.AbsolutePath);
            Assert.Equal("Bearer", request.Headers.Authorization?.Scheme);
            Assert.Equal("admin-token", request.Headers.Authorization?.Parameter);

            var responseBody = JsonSerializer.Serialize(new[]
            {
                new { id = 1, descripcion = "Presidente" },
                new { id = 2, descripcion = "Tesorero" }
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

        var sut = new CargoAdminApiClient(httpClient, sessionService);

        var result = await sut.GetCargosAsync();

        Assert.True(result.IsSuccess);
        Assert.Collection(
            result.Value!,
            cargo =>
            {
                Assert.Equal(1, cargo.Id);
                Assert.Equal("Presidente", cargo.Description);
            },
            cargo =>
            {
                Assert.Equal(2, cargo.Id);
                Assert.Equal("Tesorero", cargo.Description);
            });
    }

    [Fact]
    public async Task CreateCargoAsync_PostsPayloadAndReturnsMappedCargo()
    {
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(TestSessions.AdminSession);

        var handler = new StubHttpMessageHandler(async request =>
        {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.Equal("/api/cargos", request.RequestUri?.AbsolutePath);

            var payload = await request.Content!.ReadFromJsonAsync<CreateCargoApiPayload>();
            Assert.NotNull(payload);
            Assert.Equal("Secretario", payload.Descripcion);

            return new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent("7", Encoding.UTF8, "application/json")
            };
        });

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://localhost")
        };

        var sut = new CargoAdminApiClient(httpClient, sessionService);

        var result = await sut.CreateCargoAsync(new CreateCargoRequest("Secretario"));

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(7, result.Value.Id);
        Assert.Equal("Secretario", result.Value.Description);
    }

    [Fact]
    public async Task UpdateCargoAsync_PutsPayloadToEndpoint()
    {
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(TestSessions.AdminSession);

        var handler = new StubHttpMessageHandler(async request =>
        {
            Assert.Equal(HttpMethod.Put, request.Method);
            Assert.Equal("/api/cargos/3", request.RequestUri?.AbsolutePath);

            var payload = await request.Content!.ReadFromJsonAsync<UpdateCargoApiPayload>();
            Assert.NotNull(payload);
            Assert.Equal("Vicepresidencia", payload.Descripcion);

            return new HttpResponseMessage(HttpStatusCode.NoContent);
        });

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://localhost")
        };

        var sut = new CargoAdminApiClient(httpClient, sessionService);

        var result = await sut.UpdateCargoAsync(new UpdateCargoRequest(3, "Vicepresidencia"));

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(3, result.Value.Id);
        Assert.Equal("Vicepresidencia", result.Value.Description);
    }

    [Fact]
    public async Task DeleteCargoAsync_ReturnsConflictError_OnConflict()
    {
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(TestSessions.AdminSession);

        var handler = new StubHttpMessageHandler(_ => Task.FromResult(new HttpResponseMessage(HttpStatusCode.Conflict)));

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://localhost")
        };

        var sut = new CargoAdminApiClient(httpClient, sessionService);

        var result = await sut.DeleteCargoAsync(9);

        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
        Assert.Equal("cargos.conflict", result.Error.Code);
    }

    private sealed record CreateCargoApiPayload(string Descripcion);

    private sealed record UpdateCargoApiPayload(string Descripcion);
}
