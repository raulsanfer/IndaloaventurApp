namespace IndaloaventurApp.Frontend.Tests.Features.Inventory;

using System.Net;
using System.Text;
using System.Text.Json;
using IndaloaventurApp.SharedUI.Models.Inventory;
using IndaloaventurApp.Web.Client.Infrastructure.Inventory;

public sealed class InventoryReservationAdminApiClientTests
{
    private static readonly Guid ReservationId = Guid.Parse("716eed42-1a44-4830-8af1-c74dc588e913");
    private static readonly Guid MemberId = Guid.Parse("1e090655-c3ce-442b-b7e6-32d4848d20a7");

    /// <summary>Covers the authenticated administrative reservation HTTP contract.</summary>
    [Fact]
    public async Task AdministratorOperations_UseAuthorizedContractEndpointsAndPayloads()
    {
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(TestSessions.AdminSession);
        var observedRequests = new List<string>();

        var handler = new StubHttpMessageHandler(async request =>
        {
            Assert.Equal("Bearer", request.Headers.Authorization?.Scheme);
            Assert.Equal("admin-token", request.Headers.Authorization?.Parameter);
            var requestKey = $"{request.Method} {request.RequestUri!.AbsolutePath}";
            observedRequests.Add(requestKey);

            if (requestKey == $"POST /api/inventario-reservas/admin/{ReservationId}/retirada")
            {
                return await AssertPickupAsync(request);
            }

            if (requestKey == $"POST /api/inventario-reservas/admin/{ReservationId}/cancelacion")
            {
                return await AssertCancellationAsync(request);
            }

            if (requestKey == $"POST /api/inventario-reservas/admin/{ReservationId}/confirmacion-devolucion")
            {
                return await AssertReturnConfirmationAsync(request);
            }

            return requestKey switch
            {
                "GET /api/inventario-reservas/admin" => JsonResponse($"[{ReservationJson(MaterialReservationStatus.Requested)}]"),
                _ when requestKey == $"GET /api/inventario-reservas/admin/{ReservationId}" => JsonResponse(ReservationJson(MaterialReservationStatus.PickedUp)),
                _ => throw new InvalidOperationException($"Unexpected request {requestKey}")
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://localhost") };
        var sut = new InventoryReservationAdminApiClient(httpClient, sessionService);

        var reservations = await sut.GetReservationsAsync();
        var reservation = await sut.GetReservationAsync(ReservationId);
        var pickup = await sut.RegisterPickupAsync(ReservationId, new RegisterAdminMaterialReservationPickupRequest(new DateOnly(2026, 11, 2)));
        var cancellation = await sut.CancelReservationAsync(ReservationId, new CancelAdminMaterialReservationRequest("Material dañado"));
        var returnConfirmation = await sut.ConfirmReturnAsync(ReservationId, new ConfirmAdminMaterialReservationReturnRequest(new DateOnly(2026, 11, 8)));

        Assert.True(reservations.IsSuccess);
        Assert.Equal(MaterialReservationStatus.Requested, reservations.Value![0].Status);
        Assert.Equal("Piolet técnico", reservations.Value[0].MaterialDescription);
        Assert.Equal("Ana Socia", reservations.Value[0].Member!.FullName);
        Assert.Equal(3, reservations.Value[0].Availability!.AvailableQuantity);
        Assert.True(reservation.IsSuccess);
        Assert.Equal(MaterialReservationStatus.PickedUp, reservation.Value!.Status);
        Assert.Equal(new DateOnly(2026, 11, 2), reservation.Value.PickupDate);
        Assert.True(pickup.IsSuccess);
        Assert.True(cancellation.IsSuccess);
        Assert.True(returnConfirmation.IsSuccess);
        Assert.Equal(5, observedRequests.Count);
    }

    /// <summary>Covers safe API, transport and invalid-payload failures without write retries.</summary>
    [Fact]
    public async Task ReturnsSafeProblemDetailsTransportAndInvalidPayloadErrorsWithoutRetryingTransitions()
    {
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(TestSessions.AdminSession);
        var calls = 0;
        var responses = new Queue<Func<HttpResponseMessage>>(
        [
            () => JsonResponse("{\"detail\":\"La fecha de retirada es obligatoria.\",\"errors\":{\"fechaRetirada\":[\"Indica una fecha.\"]}}", HttpStatusCode.BadRequest),
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
        var sut = new InventoryReservationAdminApiClient(httpClient, sessionService);

        var validation = await sut.RegisterPickupAsync(ReservationId, new RegisterAdminMaterialReservationPickupRequest(new DateOnly(2026, 11, 2)));
        var unauthorized = await sut.GetReservationsAsync();
        var forbidden = await sut.GetReservationsAsync();
        var notFound = await sut.GetReservationAsync(ReservationId);
        var conflict = await sut.CancelReservationAsync(ReservationId, new CancelAdminMaterialReservationRequest("No disponible"));
        var unavailable = await sut.GetReservationsAsync();
        var invalidPayload = await sut.GetReservationsAsync();

        Assert.Equal("reservations.validation", validation.Error!.Code);
        Assert.Collection(validation.Error.ValidationErrors!, error =>
        {
            Assert.Equal("fechaRetirada", error.PropertyName);
            Assert.Equal("Indica una fecha.", error.ErrorMessage);
        });
        Assert.Equal("auth.session_invalid", unauthorized.Error!.Code);
        Assert.Equal("reservations.forbidden", forbidden.Error!.Code);
        Assert.Equal("reservations.not_found", notFound.Error!.Code);
        Assert.Equal("La reserva ya no existe.", notFound.Error.Message);
        Assert.Equal("reservations.conflict", conflict.Error!.Code);
        Assert.Equal("La reserva cambió de estado.", conflict.Error.Message);
        Assert.Equal("reservations.unavailable", unavailable.Error!.Code);
        Assert.DoesNotContain("secret", unavailable.Error.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Equal("reservations.invalid_payload", invalidPayload.Error!.Code);
        Assert.Equal(7, calls);
    }

    /// <summary>Covers recoverable network and timeout failures without automatic retries.</summary>
    [Fact]
    public async Task ReturnsSafeNetworkAndTimeoutErrorsWithoutAutomaticRetry()
    {
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(TestSessions.AdminSession);
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
        var sut = new InventoryReservationAdminApiClient(httpClient, sessionService);

        var unavailable = await sut.CancelReservationAsync(ReservationId, new CancelAdminMaterialReservationRequest(null));
        Assert.Equal("reservations.unavailable", unavailable.Error!.Code);
        Assert.Equal(1, calls);

        var timeout = await sut.GetReservationsAsync();
        Assert.Equal("reservations.timeout", timeout.Error!.Code);
        Assert.Equal(2, calls);
    }

    private static HttpResponseMessage JsonResponse(string body, HttpStatusCode statusCode = HttpStatusCode.OK)
        => new(statusCode)
        {
            Content = new StringContent(body, Encoding.UTF8, "application/json")
        };

    private static async Task<HttpResponseMessage> AssertPickupAsync(HttpRequestMessage request)
    {
        using var document = JsonDocument.Parse(await request.Content!.ReadAsStringAsync());
        Assert.Equal("2026-11-02", document.RootElement.GetProperty("fechaRetirada").GetString());
        return new HttpResponseMessage(HttpStatusCode.NoContent);
    }

    private static async Task<HttpResponseMessage> AssertCancellationAsync(HttpRequestMessage request)
    {
        using var document = JsonDocument.Parse(await request.Content!.ReadAsStringAsync());
        Assert.Equal("Material dañado", document.RootElement.GetProperty("observacionAdministracion").GetString());
        return new HttpResponseMessage(HttpStatusCode.NoContent);
    }

    private static async Task<HttpResponseMessage> AssertReturnConfirmationAsync(HttpRequestMessage request)
    {
        using var document = JsonDocument.Parse(await request.Content!.ReadAsStringAsync());
        Assert.Equal("2026-11-08", document.RootElement.GetProperty("fechaDevolucionEfectiva").GetString());
        return new HttpResponseMessage(HttpStatusCode.NoContent);
    }

    private static string ReservationJson(MaterialReservationStatus status)
    {
        var pickupDate = status == MaterialReservationStatus.PickedUp ? "\"2026-11-02\"" : "null";
        return $$"""
            {
              "id":"{{ReservationId}}",
              "userId":"{{MemberId}}",
              "inventarioMaterialId":42,
              "materialDescripcion":"Piolet técnico",
              "cantidadSolicitada":2,
              "fechaSolicitudUtc":"2026-10-01T09:30:00Z",
              "fechaRetiradaPrevista":"2026-11-01",
              "fechaRetirada":{{pickupDate}},
              "fechaDevolucionEstimada":"2026-11-08",
              "fechaDevolucionEfectiva":null,
              "observaciones":"Usaré el material en una salida formativa.",
              "observacionAdministracion":null,
              "estado":{{(int)status}},
              "confirmacionDevolucionSocio":false,
              "confirmacionDevolucionAdmin":false,
              "socio":{"userId":"{{MemberId}}","nombreCompleto":"Ana Socia","email":"ana@example.test"},
              "disponibilidad":{"inventarioMaterialId":42,"cantidadTotal":5,"cantidadComprometida":2,"cantidadDisponible":3,"activo":true}
            }
            """;
    }
}
