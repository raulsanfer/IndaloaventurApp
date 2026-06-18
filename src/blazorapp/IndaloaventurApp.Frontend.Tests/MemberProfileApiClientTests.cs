namespace IndaloaventurApp.Frontend.Tests;

using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using IndaloaventurApp.SharedUI.Models.Member;
using IndaloaventurApp.Web.Client.Infrastructure.Member;

public sealed class MemberProfileApiClientTests
{
    [Fact]
    public async Task GetMyMemberFileAsync_GetsMemberFileFromMeEndpoint()
    {
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(TestSessions.MemberSession);

        var handler = new StubHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Get, request.Method);
            Assert.Equal("/api/fichas-socio/me", request.RequestUri?.AbsolutePath);

            var responseBody = JsonSerializer.Serialize(new
            {
                userId = Guid.Parse("2af7d8c8-4112-4a6e-b48f-65821b4f2914"),
                cargoId = 5,
                nombre = "Ana",
                apellidos = "Montes",
                dni = "12345678A",
                fechaNacimiento = "1990-04-18",
                direccion = "Calle Sierra 5",
                codigoPostal = "04001",
                poblacion = "Almería",
                provincia = "Almería",
                tlf = "600123123",
                email = "ana@club.es",
                alergias = "Polen",
                aceptaPoliticaPrivacidad = true,
                aceptaUsoImagenes = false,
                aceptaCobroCuenta = true
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

        var sut = new MemberProfileApiClient(httpClient, sessionService);

        var result = await sut.GetMyMemberFileAsync();

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Ana", result.Value.Nombre);
        Assert.Equal("Montes", result.Value.Apellidos);
        Assert.Equal(new DateOnly(1990, 4, 18), result.Value.FechaNacimiento);
        Assert.Equal("ana@club.es", result.Value.Email);
    }

    [Fact]
    public async Task UpdateMyMemberFileAsync_PutsPayloadWithoutMembershipFields()
    {
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(TestSessions.MemberSession);

        var handler = new StubHttpMessageHandler(async request =>
        {
            Assert.Equal(HttpMethod.Put, request.Method);
            Assert.Equal("/api/fichas-socio/me", request.RequestUri?.AbsolutePath);

            var rawJson = await request.Content!.ReadAsStringAsync();
            var payload = JsonSerializer.Deserialize<UpdateMemberFileApiPayload>(rawJson, new JsonSerializerOptions(JsonSerializerDefaults.Web));
            Assert.NotNull(payload);
            Assert.Equal(4, payload.CargoId);
            Assert.Equal("Ana", payload.Nombre);
            Assert.Equal("Montes", payload.Apellidos);
            Assert.Equal("12345678A", payload.Dni);
            Assert.Equal(new DateOnly(1990, 4, 18), payload.FechaNacimiento);
            Assert.Equal("04001", payload.CodigoPostal);
            Assert.Equal("ana@club.es", payload.Email);

            Assert.DoesNotContain("isMember", rawJson, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("roles", rawJson, StringComparison.OrdinalIgnoreCase);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(rawJson, Encoding.UTF8, "application/json")
            };
        });

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://localhost")
        };

        var sut = new MemberProfileApiClient(httpClient, sessionService);

        var result = await sut.UpdateMyMemberFileAsync(new UpdateMemberSelfProfileRequest(
            4,
            "Ana",
            "Montes",
            "12345678A",
            new DateOnly(1990, 4, 18),
            "Calle Sierra 5",
            "04001",
            "Almería",
            "Almería",
            "600123123",
            "ana@club.es",
            "Polen",
            true,
            false,
            true));

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Ana", result.Value.Nombre);
        Assert.Equal("ana@club.es", result.Value.Email);
    }

    [Fact]
    public async Task GetMyMemberFileAsync_ReturnsNotMemberError_ForNonMemberSession()
    {
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(new IndaloaventurApp.SharedUI.Models.Auth.AuthSession("token", "Bearer", 3600, false));

        using var httpClient = new HttpClient(new StubHttpMessageHandler(_ => throw new InvalidOperationException("Should not call API")))
        {
            BaseAddress = new Uri("https://localhost")
        };

        var sut = new MemberProfileApiClient(httpClient, sessionService);

        var result = await sut.GetMyMemberFileAsync();

        Assert.False(result.IsSuccess);
        Assert.Equal("profile.not_member", result.Error?.Code);
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
