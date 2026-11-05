namespace IndaloaventurApp.Frontend.Tests.Features.Inventory;

using System.Net;
using System.Text;
using System.Text.Json;
using IndaloaventurApp.SharedUI.Models.Inventory;
using IndaloaventurApp.Web.Client.Infrastructure.Inventory;

public sealed class MemberMaterialReservationApiClientTests
{
    private static readonly Guid ReservationId = Guid.Parse("6181a41c-7e1a-48eb-90dd-cc9f441400d2");
    private static readonly Guid MemberId = Guid.Parse("bb97d3e0-e79f-4ab8-bfcf-cb549efcdbbf");

    [Fact]
    public async Task MemberOperations_UseAuthorizedPersonalEndpointsPayloadsAndActiveMaterials()
    {
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(TestSessions.MemberSession);
        var observedRequests = new List<string>();
        var handler = new StubHttpMessageHandler(async request =>
        {
            Assert.Equal("Bearer", request.Headers.Authorization?.Scheme);
            Assert.Equal("access-token", request.Headers.Authorization?.Parameter);
            var requestKey = $"{request.Method} {request.RequestUri!.AbsolutePath}";
            observedRequests.Add(requestKey);

            if (requestKey == "POST /api/inventario-reservas/me")
            {
                return await AssertCreateRequestAsync(request);
            }

            if (requestKey == $"DELETE /api/inventario-reservas/me/{ReservationId}")
            {
                Assert.Null(request.Content);
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }

            if (requestKey == $"POST /api/inventario-reservas/me/{ReservationId}/confirmacion-devolucion")
            {
                Assert.Null(request.Content);
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }

            return requestKey switch
            {
                "GET /api/inventario-reservas/me" => JsonResponse($"[{ReservationJson(MaterialReservationStatus.Requested)}]"),
                _ when requestKey == $"GET /api/inventario-reservas/me/{ReservationId}" => JsonResponse(ReservationJson(MaterialReservationStatus.PickedUp)),
                "GET /api/inventario-materiales" => JsonResponse("[{\"id\":42,\"descripcion\":\"Piolet técnico\",\"activo\":true,\"cantidadDisponible\":3},{\"id\":43,\"descripcion\":\"Casco\",\"activo\":false,\"cantidadDisponible\":0}]"),
                _ => throw new InvalidOperationException($"Unexpected request {requestKey}")
            };
        });
        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://localhost") };
        var sut = new MemberMaterialReservationApiClient(httpClient, sessionService);

        var reservations = await sut.GetMyReservationsAsync();
        var reservation = await sut.GetMyReservationAsync(ReservationId);
        var materials = await sut.GetReservableMaterialsAsync();
        var created = await sut.CreateReservationAsync(new CreateMemberMaterialReservationRequest(
            42,
            2,
            new DateOnly(2026, 11, 12),
            new DateOnly(2026, 11, 15),
            "Recogeré el material por la tarde."));
        var cancelled = await sut.CancelReservationAsync(ReservationId);
        var confirmed = await sut.ConfirmReturnAsync(ReservationId);

        Assert.True(reservations.IsSuccess);
        Assert.Equal("Piolet técnico", reservations.Value![0].MaterialDescription);
        Assert.True(reservation.IsSuccess);
        Assert.Equal(MaterialReservationStatus.PickedUp, reservation.Value!.Status);
        Assert.True(materials.IsSuccess);
        Assert.Collection(materials.Value!, material =>
        {
            Assert.Equal(42, material.Id);
            Assert.True(material.IsActive);
            Assert.Equal(3, material.AvailableQuantity);
        });
        Assert.True(created.IsSuccess);
        Assert.True(cancelled.IsSuccess);
        Assert.True(confirmed.IsSuccess);
        Assert.Equal(6, observedRequests.Count);
    }

    [Fact]
    public async Task MemberOperations_MapSafeHttpAndInvalidPayloadErrorsWithoutRetryingMutations()
    {
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(TestSessions.MemberSession);
        var calls = 0;
        var responses = new Queue<Func<HttpResponseMessage>>(
        [
            () => JsonResponse("{\"detail\":\"La fecha de devolución es obligatoria.\",\"errors\":{\"fechaDevolucionEstimada\":[\"Indica una fecha.\"]}}", HttpStatusCode.BadRequest),
            () => new HttpResponseMessage(HttpStatusCode.Unauthorized),
            () => new HttpResponseMessage(HttpStatusCode.Forbidden),
            () => JsonResponse("{\"detail\":\"La reserva ya no existe.\"}", HttpStatusCode.NotFound),
            () => JsonResponse("{\"detail\":\"La reserva cambió de estado.\"}", HttpStatusCode.Conflict),
            () => JsonResponse("{\"detail\":\"secret internals\"}", HttpStatusCode.InternalServerError),
            () => JsonResponse("{}")
        ]);
        var handler = new StubHttpMessageHandler(_ =>
        {
            calls++;
            return Task.FromResult(responses.Dequeue().Invoke());
        });
        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://localhost") };
        var sut = new MemberMaterialReservationApiClient(httpClient, sessionService);

        var validation = await sut.CreateReservationAsync(new CreateMemberMaterialReservationRequest(42, 2, new DateOnly(2026, 11, 12), new DateOnly(2026, 11, 15), null));
        var unauthorized = await sut.GetMyReservationsAsync();
        var forbidden = await sut.GetMyReservationsAsync();
        var notFound = await sut.GetMyReservationAsync(ReservationId);
        var conflict = await sut.CancelReservationAsync(ReservationId);
        var unavailable = await sut.GetMyReservationsAsync();
        var invalidPayload = await sut.GetMyReservationsAsync();

        Assert.Equal("reservations.validation", validation.Error!.Code);
        Assert.Collection(validation.Error.ValidationErrors!, error =>
        {
            Assert.Equal("fechaDevolucionEstimada", error.PropertyName);
            Assert.Equal("Indica una fecha.", error.ErrorMessage);
        });
        Assert.Equal("auth.session_invalid", unauthorized.Error!.Code);
        Assert.Equal("reservations.forbidden", forbidden.Error!.Code);
        Assert.Equal("reservations.not_found", notFound.Error!.Code);
        Assert.Equal("reservations.conflict", conflict.Error!.Code);
        Assert.Equal("reservations.unavailable", unavailable.Error!.Code);
        Assert.DoesNotContain("secret", unavailable.Error.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Equal("reservations.invalid_payload", invalidPayload.Error!.Code);
        Assert.Equal(7, calls);
    }

    [Fact]
    public async Task MemberOperations_ReturnSafeNetworkAndTimeoutFailuresWithoutAutomaticRetries()
    {
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(TestSessions.MemberSession);
        var calls = 0;
        var handler = new StubHttpMessageHandler(_ =>
        {
            calls++;
            if (calls == 1)
            {
                throw new HttpRequestException("network unavailable");
            }

            throw new TaskCanceledException();
        });
        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://localhost") };
        var sut = new MemberMaterialReservationApiClient(httpClient, sessionService);

        var unavailable = await sut.CancelReservationAsync(ReservationId);
        var timeout = await sut.GetMyReservationsAsync();

        Assert.Equal("reservations.unavailable", unavailable.Error!.Code);
        Assert.Equal("reservations.timeout", timeout.Error!.Code);
        Assert.Equal(2, calls);
    }

    private static HttpResponseMessage JsonResponse(string body, HttpStatusCode statusCode = HttpStatusCode.OK)
        => new(statusCode)
        {
            Content = new StringContent(body, Encoding.UTF8, "application/json")
        };

    private static async Task<HttpResponseMessage> AssertCreateRequestAsync(HttpRequestMessage request)
    {
        using var document = JsonDocument.Parse(await request.Content!.ReadAsStringAsync());
        var root = document.RootElement;
        Assert.Equal(42, root.GetProperty("inventarioMaterialId").GetInt32());
        Assert.Equal(2, root.GetProperty("cantidadSolicitada").GetInt32());
        Assert.Equal("2026-11-12", root.GetProperty("fechaRetiradaPrevista").GetString());
        Assert.Equal("2026-11-15", root.GetProperty("fechaDevolucionEstimada").GetString());
        Assert.Equal("Recogeré el material por la tarde.", root.GetProperty("observaciones").GetString());
        Assert.DoesNotContain("userId", root.EnumerateObject().Select(static property => property.Name), StringComparer.OrdinalIgnoreCase);
        return JsonResponse(ReservationJson(MaterialReservationStatus.Requested), HttpStatusCode.Created);
    }

    private static string ReservationJson(MaterialReservationStatus status)
    {
        var pickupDate = status == MaterialReservationStatus.PickedUp ? "\"2026-11-12\"" : "null";
        return $$"""
            {
              "id":"{{ReservationId}}",
              "userId":"{{MemberId}}",
              "inventarioMaterialId":42,
              "materialDescripcion":"Piolet técnico",
              "cantidadSolicitada":2,
              "fechaSolicitudUtc":"2026-10-01T09:30:00Z",
              "fechaRetiradaPrevista":"2026-11-12",
              "fechaRetirada":{{pickupDate}},
              "fechaDevolucionEstimada":"2026-11-15",
              "fechaDevolucionEfectiva":null,
              "observaciones":"Usaré el material en una salida formativa.",
              "observacionAdministracion":null,
              "estado":{{(int)status}},
              "confirmacionDevolucionSocio":false,
              "confirmacionDevolucionAdmin":false,
              "disponibilidad":{"inventarioMaterialId":42,"cantidadTotal":5,"cantidadComprometida":2,"cantidadDisponible":3,"activo":true}
            }
            """;
    }
}
