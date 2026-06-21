namespace IndaloaventurApp.Frontend.Tests;

using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using IndaloaventurApp.SharedUI.Models.Signals;
using IndaloaventurApp.Web.Client.Infrastructure.Signals;

public sealed class SignalApiClientTests
{
    [Fact]
    public async Task GetSignalHomeDataAsync_DoesNotCallImagesEndpointAndLeavesImageUrlNull()
    {
        var signalId = Guid.Parse("d1f11ecb-0d25-4432-89f0-ae39e34f5461");
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(TestSessions.MemberSession);
        var imagesEndpointCalls = 0;

        var handler = new StubHttpMessageHandler(request =>
        {
            if (request.RequestUri?.AbsolutePath == "/api/signal-types")
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonSerializer.Serialize(new[]
                    {
                        new
                        {
                            id = 8,
                            nombre = "Senderismo",
                            icono = "sign-turn-right-fill"
                        }
                    }), Encoding.UTF8, "application/json")
                });
            }

            if (request.RequestUri?.AbsolutePath == "/api/signals")
            {
                Assert.Contains("activo=true", request.RequestUri?.Query);

                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonSerializer.Serialize(new[]
                    {
                        new
                        {
                            id = signalId,
                            latitud = 36.834652,
                            longitud = -2.463714,
                            titulo = "Piedra desprendida",
                            descripcion = "Hay una piedra de gran tamano invadiendo parte del sendero.",
                            activo = true,
                            userIdAlta = Guid.NewGuid(),
                            fechaAlta = new DateTime(2026, 6, 3, 20, 6, 0, DateTimeKind.Utc),
                            fechaModificacion = new DateTime(2026, 6, 3, 22, 6, 0, DateTimeKind.Utc),
                            userIdModificacion = Guid.NewGuid(),
                            tipo = 8,
                            tags = "senderos, piedras"
                        }
                    }), Encoding.UTF8, "application/json")
                });
            }

            if (request.RequestUri?.AbsolutePath == $"/api/signals/{signalId}/images")
            {
                imagesEndpointCalls++;
            }

            throw new InvalidOperationException($"Unexpected endpoint {request.RequestUri}");
        });

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://localhost")
        };

        var sut = new SignalApiClient(httpClient, sessionService);

        var result = await sut.GetSignalHomeDataAsync(new SignalListQuery(string.Empty, null, OnlyActive: true));

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        var signal = Assert.Single(result.Value.Signals);
        Assert.Equal("Piedra desprendida", signal.Title);
        Assert.Null(signal.ImageUrl);
        Assert.Equal(0, imagesEndpointCalls);
    }

    [Fact]
    public async Task GetSignalImagesAsync_MapsPhotosToDataUrls()
    {
        var signalId = Guid.Parse("d1f11ecb-0d25-4432-89f0-ae39e34f5461");
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(TestSessions.MemberSession);

        var handler = new StubHttpMessageHandler(request =>
        {
            if (request.RequestUri?.AbsolutePath == $"/api/signals/{signalId}/images")
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonSerializer.Serialize(new
                    {
                        signalId,
                        foto1 = new byte[] { 0xFF, 0xD8, 0xFF, 0xEE },
                        foto2 = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0, 0, 0, 0 }
                    }), Encoding.UTF8, "application/json")
                });
            }

            throw new InvalidOperationException($"Unexpected endpoint {request.RequestUri}");
        });

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://localhost")
        };

        var sut = new SignalApiClient(httpClient, sessionService);

        var result = await sut.GetSignalImagesAsync(signalId);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.StartsWith("data:image/jpeg;base64,", result.Value.Photo1Url);
        Assert.StartsWith("data:image/png;base64,", result.Value.Photo2Url);
    }

    [Fact]
    public async Task GetSignalImagesAsync_ReturnsEmptyImageUrls_WhenPayloadHasNoUsablePhoto()
    {
        var signalId = Guid.Parse("d1f11ecb-0d25-4432-89f0-ae39e34f5461");
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(TestSessions.MemberSession);

        var handler = new StubHttpMessageHandler(request =>
        {
            if (request.RequestUri?.AbsolutePath == $"/api/signals/{signalId}/images")
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonSerializer.Serialize(new
                    {
                        signalId,
                        foto1 = Array.Empty<byte>(),
                        foto2 = Array.Empty<byte>()
                    }), Encoding.UTF8, "application/json")
                });
            }

            throw new InvalidOperationException($"Unexpected endpoint {request.RequestUri}");
        });

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://localhost")
        };

        var sut = new SignalApiClient(httpClient, sessionService);

        var result = await sut.GetSignalImagesAsync(signalId);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Null(result.Value.Photo1Url);
        Assert.Null(result.Value.Photo2Url);
    }

    [Fact]
    public async Task GetSignalAsync_UsesSignalDetailEndpointAndMapsResolvedCategory()
    {
        var signalId = Guid.Parse("8f57dc91-7a2f-49d7-b85b-5ce5848d8052");
        var ownerUserId = Guid.Parse("7d57dc91-7a2f-49d7-b85b-5ce5848d8052");
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(TestSessions.MemberSession);

        var handler = new StubHttpMessageHandler(request =>
        {
            if (request.RequestUri?.AbsolutePath == $"/api/signals/{signalId}")
            {
                Assert.Equal(HttpMethod.Get, request.Method);

                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonSerializer.Serialize(new
                    {
                        id = signalId,
                        latitud = 36.834652,
                        longitud = -2.463714,
                        descripcion = "Rama caida sobre el sendero principal.",
                        activo = true,
                        userIdAlta = ownerUserId,
                        fechaAlta = new DateTime(2026, 6, 7, 8, 15, 0, DateTimeKind.Utc),
                        fechaModificacion = new DateTime(2026, 6, 8, 10, 30, 0, DateTimeKind.Utc),
                        userIdModificacion = Guid.NewGuid(),
                        tipo = 2,
                        tags = "sendero, rama"
                    }), Encoding.UTF8, "application/json")
                });
            }

            if (request.RequestUri?.AbsolutePath == "/api/signal-types")
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonSerializer.Serialize(new[]
                    {
                        new
                        {
                            id = 2,
                            nombre = "Obstaculo",
                            icono = "sign-turn-right-fill"
                        }
                    }), Encoding.UTF8, "application/json")
                });
            }

            throw new InvalidOperationException($"Unexpected endpoint {request.RequestUri}");
        });

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://localhost")
        };

        var sut = new SignalApiClient(httpClient, sessionService);

        var result = await sut.GetSignalAsync(signalId);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(signalId, result.Value.Id);
        Assert.Equal("Obstaculo", result.Value.CategoryName);
        Assert.Equal("bi-sign-turn-right-fill", result.Value.CategoryIconName);
        Assert.Equal("Rama caida sobre el sendero principal", result.Value.Title);
        Assert.Equal(ownerUserId, result.Value.OwnerUserId);
    }

    [Fact]
    public async Task GetSignalAsync_ReturnsNotFound_WhenApiReturns404()
    {
        var signalId = Guid.NewGuid();
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(TestSessions.MemberSession);

        using var httpClient = new HttpClient(new StubHttpMessageHandler(_ => Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound))))
        {
            BaseAddress = new Uri("https://localhost")
        };

        var sut = new SignalApiClient(httpClient, sessionService);

        var result = await sut.GetSignalAsync(signalId);

        Assert.False(result.IsSuccess);
        Assert.Equal("signals.not_found", result.Error?.Code);
    }

    [Fact]
    public async Task GetSignalCommentsAsync_UsesCommentsEndpointAndMapsComments()
    {
        var signalId = Guid.Parse("8f57dc91-7a2f-49d7-b85b-5ce5848d8052");
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(TestSessions.MemberSession);

        var handler = new StubHttpMessageHandler(request =>
        {
            if (request.RequestUri?.AbsolutePath == $"/api/signals/{signalId}/comments")
            {
                Assert.Equal(HttpMethod.Get, request.Method);

                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonSerializer.Serialize(new[]
                    {
                        new
                        {
                            id = Guid.NewGuid(),
                            signalId,
                            userId = Guid.NewGuid(),
                            fechaComentario = new DateTime(2026, 6, 9, 12, 0, 0, DateTimeKind.Utc),
                            texto = "La incidencia sigue activa."
                        }
                    }), Encoding.UTF8, "application/json")
                });
            }

            throw new InvalidOperationException($"Unexpected endpoint {request.RequestUri}");
        });

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://localhost")
        };

        var sut = new SignalApiClient(httpClient, sessionService);

        var result = await sut.GetSignalCommentsAsync(signalId);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Single(result.Value);
        Assert.Equal("La incidencia sigue activa.", result.Value[0].Text);
    }

    [Fact]
    public async Task CreateSignalCategoryAsync_PostsToSignalTypesEndpoint()
    {
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(TestSessions.AdminSession);

        var handler = new StubHttpMessageHandler(async request =>
        {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.Equal("/api/signal-types", request.RequestUri?.AbsolutePath);

            var payload = await request.Content!.ReadAsStringAsync();
            using var document = JsonDocument.Parse(payload);
            Assert.Equal("Obstaculo", document.RootElement.GetProperty("nombre").GetString());
            Assert.Equal("bi-sign-turn-right-fill", document.RootElement.GetProperty("icono").GetString());

            return new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent("12", Encoding.UTF8, "application/json")
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://localhost") };
        var sut = new SignalApiClient(httpClient, sessionService);

        var result = await sut.CreateSignalCategoryAsync(new CreateSignalCategoryRequest("Obstaculo", "bi-sign-turn-right-fill"));

        Assert.True(result.IsSuccess);
        Assert.Equal(12, result.Value);
    }

    [Fact]
    public async Task UpdateSignalCategoryAsync_PutsToSignalTypeDetailEndpoint()
    {
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(TestSessions.AdminSession);

        var handler = new StubHttpMessageHandler(async request =>
        {
            Assert.Equal(HttpMethod.Put, request.Method);
            Assert.Equal("/api/signal-types/7", request.RequestUri?.AbsolutePath);

            var payload = await request.Content!.ReadAsStringAsync();
            Assert.Contains("Seguridad", payload);
            return new HttpResponseMessage(HttpStatusCode.NoContent);
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://localhost") };
        var sut = new SignalApiClient(httpClient, sessionService);

        var result = await sut.UpdateSignalCategoryAsync(new UpdateSignalCategoryRequest(7, "Seguridad", "bi-shield-fill"));

        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
    }

    [Fact]
    public async Task DeleteSignalCategoryAsync_ReturnsBlockedError_OnConflict()
    {
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(TestSessions.AdminSession);

        using var httpClient = new HttpClient(new StubHttpMessageHandler(_ => Task.FromResult(new HttpResponseMessage(HttpStatusCode.Conflict))))
        {
            BaseAddress = new Uri("https://localhost")
        };

        var sut = new SignalApiClient(httpClient, sessionService);

        var result = await sut.DeleteSignalCategoryAsync(7);

        Assert.False(result.IsSuccess);
        Assert.Equal("signals.categories.delete_blocked", result.Error?.Code);
    }

    [Fact]
    public async Task CreateSignalAsync_PostsTitleAndPhotosToSignalsEndpoint()
    {
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(TestSessions.MemberSession);

        var handler = new StubHttpMessageHandler(async request =>
        {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.Equal("/api/signals", request.RequestUri?.AbsolutePath);

            var payload = await request.Content!.ReadAsStringAsync();
            using var document = JsonDocument.Parse(payload);

            Assert.Equal("Rama caida en sendero", document.RootElement.GetProperty("titulo").GetString());
            Assert.Equal("Rama caida en sendero principal.", document.RootElement.GetProperty("descripcion").GetString());
            Assert.Equal("sendero,seguridad", document.RootElement.GetProperty("tags").GetString());
            Assert.Equal(3, document.RootElement.GetProperty("tipo").GetInt32());
            Assert.True(document.RootElement.GetProperty("activo").GetBoolean());
            Assert.Equal(Convert.ToBase64String(new byte[] { 1, 2, 3 }), document.RootElement.GetProperty("foto1").GetString());
            Assert.Equal(string.Empty, document.RootElement.GetProperty("foto2").GetString());

            return new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent(JsonSerializer.Serialize(Guid.Parse("74af2536-c38f-4feb-8b59-2af57e9ad66b")), Encoding.UTF8, "application/json")
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://localhost") };
        var sut = new SignalApiClient(httpClient, sessionService);

        var result = await sut.CreateSignalAsync(new SignalCreateRequest(
            36.834652f,
            -2.463714f,
            "Rama caida en sendero",
            "Rama caida en sendero principal.",
            new byte[] { 1, 2, 3 },
            Array.Empty<byte>(),
            true,
            3,
            "sendero,seguridad"));

        Assert.True(result.IsSuccess);
        Assert.Equal(Guid.Parse("74af2536-c38f-4feb-8b59-2af57e9ad66b"), result.Value);
    }

    [Fact]
    public async Task UpdateSignalAsync_PutsOnlyEditableFieldsToSignalDetailEndpoint()
    {
        var signalId = Guid.Parse("74af2536-c38f-4feb-8b59-2af57e9ad66b");
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(TestSessions.MemberSession);

        var handler = new StubHttpMessageHandler(async request =>
        {
            Assert.Equal(HttpMethod.Put, request.Method);
            Assert.Equal($"/api/signals/{signalId}", request.RequestUri?.AbsolutePath);

            var payload = await request.Content!.ReadAsStringAsync();
            using var document = JsonDocument.Parse(payload);

            Assert.Equal("Titulo actualizado", document.RootElement.GetProperty("titulo").GetString());
            Assert.Equal("Descripcion actualizada", document.RootElement.GetProperty("descripcion").GetString());
            Assert.False(document.RootElement.GetProperty("activo").GetBoolean());
            Assert.False(document.RootElement.TryGetProperty("foto1", out _));
            Assert.False(document.RootElement.TryGetProperty("tipo", out _));

            return new HttpResponseMessage(HttpStatusCode.NoContent);
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://localhost") };
        var sut = new SignalApiClient(httpClient, sessionService);

        var result = await sut.UpdateSignalAsync(new UpdateSignalRequest(
            signalId,
            "Titulo actualizado",
            "Descripcion actualizada",
            false));

        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
    }

    [Fact]
    public async Task UpdateSignalAsync_ReturnsForbidden_WhenApiReturns403()
    {
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(TestSessions.MemberSession);

        using var httpClient = new HttpClient(new StubHttpMessageHandler(_ => Task.FromResult(new HttpResponseMessage(HttpStatusCode.Forbidden))))
        {
            BaseAddress = new Uri("https://localhost")
        };

        var sut = new SignalApiClient(httpClient, sessionService);

        var result = await sut.UpdateSignalAsync(new UpdateSignalRequest(
            Guid.NewGuid(),
            "Titulo",
            "Descripcion",
            true));

        Assert.False(result.IsSuccess);
        Assert.Equal("signals.update_forbidden", result.Error?.Code);
    }
}
