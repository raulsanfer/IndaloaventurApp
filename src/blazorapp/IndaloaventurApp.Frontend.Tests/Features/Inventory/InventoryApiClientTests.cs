namespace IndaloaventurApp.Frontend.Tests.Features.Inventory;

using System.Net;
using System.Text;
using System.Text.Json;
using IndaloaventurApp.SharedUI.Models.Inventory;
using IndaloaventurApp.Web.Client.Infrastructure.Inventory;

public sealed class InventoryApiClientTests
{
    [Fact]
    public async Task CatalogOperations_UseAuthorizedEndpointsAndExpectedPayloads()
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

            switch (requestKey)
            {
                case "GET /api/inventario-categorias":
                    return JsonResponse("[{\"id\":10,\"nombre\":\"Material técnico\",\"categoriaPadreId\":null,\"subcategorias\":[{\"id\":11,\"nombre\":\"Cascos\",\"categoriaPadreId\":10,\"subcategorias\":[]}]}]");
                case "POST /api/inventario-categorias":
                    await AssertJsonPropertyAsync(request, "nombre", "Cascos");
                    await AssertJsonPropertyAsync(request, "categoriaPadreId", 10);
                    return JsonResponse("12", HttpStatusCode.Created);
                case "PUT /api/inventario-categorias/12":
                    await AssertJsonPropertyAsync(request, "nombre", "Cascos homologados");
                    await AssertJsonPropertyAsync(request, "categoriaPadreId", 10);
                    return new HttpResponseMessage(HttpStatusCode.NoContent);
                case "DELETE /api/inventario-categorias/12":
                    return new HttpResponseMessage(HttpStatusCode.NoContent);
                case "GET /api/inventario-estados":
                    return JsonResponse("[{\"id\":3,\"nombre\":\"Disponible\"}]");
                case "POST /api/inventario-estados":
                    await AssertJsonPropertyAsync(request, "nombre", "En revisión");
                    return JsonResponse("4", HttpStatusCode.Created);
                case "PUT /api/inventario-estados/4":
                    await AssertJsonPropertyAsync(request, "nombre", "Revisado");
                    return new HttpResponseMessage(HttpStatusCode.NoContent);
                case "DELETE /api/inventario-estados/4":
                    return new HttpResponseMessage(HttpStatusCode.NoContent);
                default:
                    throw new InvalidOperationException($"Unexpected request {requestKey}");
            }
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://localhost") };
        var sut = new InventoryApiClient(httpClient, sessionService);

        var categories = await sut.GetCategoriesAsync();
        var createdCategory = await sut.CreateCategoryAsync(new CreateInventoryCategoryRequest("Cascos", 10));
        var updatedCategory = await sut.UpdateCategoryAsync(new UpdateInventoryCategoryRequest(12, "Cascos homologados", 10));
        var deletedCategory = await sut.DeleteCategoryAsync(12);
        var statuses = await sut.GetStatusesAsync();
        var createdStatus = await sut.CreateStatusAsync(new CreateInventoryStatusRequest("En revisión"));
        var updatedStatus = await sut.UpdateStatusAsync(new UpdateInventoryStatusRequest(4, "Revisado"));
        var deletedStatus = await sut.DeleteStatusAsync(4);

        Assert.True(categories.IsSuccess, categories.Error?.Message);
        Assert.Equal("Material técnico", categories.Value![0].Name);
        Assert.Equal("Cascos", categories.Value[0].Subcategories[0].Name);
        Assert.Equal(12, createdCategory.Value);
        Assert.True(updatedCategory.IsSuccess);
        Assert.True(deletedCategory.IsSuccess);
        Assert.Equal("Disponible", statuses.Value![0].Name);
        Assert.Equal(4, createdStatus.Value);
        Assert.True(updatedStatus.IsSuccess);
        Assert.True(deletedStatus.IsSuccess);
        Assert.Equal(8, observedRequests.Count);
    }

    [Fact]
    public async Task MaterialOperations_SendCompleteCamelCasePayloadAndHandleNoContent()
    {
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(TestSessions.AdminSession);
        var observedRequests = new List<string>();
        var requestModel = new SaveInventoryMaterialRequest(
            "Casco de escalada",
            "Petzl Boreo",
            1,
            11,
            3,
            new DateOnly(2026, 1, 18),
            "Adquirido",
            null,
            "CASCO-001");

        var handler = new StubHttpMessageHandler(async request =>
        {
            Assert.Equal("Bearer", request.Headers.Authorization?.Scheme);
            var requestKey = $"{request.Method} {request.RequestUri!.AbsolutePath}";
            observedRequests.Add(requestKey);

            switch (requestKey)
            {
                case "GET /api/inventario-materiales":
                    return JsonResponse("[{\"id\":42,\"descripcion\":\"Casco de escalada\",\"marcaModelo\":\"Petzl Boreo\",\"cantidad\":1,\"categoriaId\":11,\"categoriaNombre\":\"Cascos\",\"categoriaPadreId\":10,\"categoriaPadreNombre\":\"Material técnico\",\"estadoId\":3,\"estadoNombre\":\"Disponible\",\"fechaAdquisicion\":\"2026-01-18\",\"adquiridoDonado\":\"Adquirido\",\"observaciones\":null,\"numeroSerie\":\"CASCO-001\"}]");
                case "POST /api/inventario-materiales":
                    await AssertMaterialPayloadAsync(request);
                    return JsonResponse("43", HttpStatusCode.Created);
                case "PUT /api/inventario-materiales/43":
                    await AssertMaterialPayloadAsync(request);
                    return new HttpResponseMessage(HttpStatusCode.NoContent);
                case "DELETE /api/inventario-materiales/43":
                    return new HttpResponseMessage(HttpStatusCode.NoContent);
                default:
                    throw new InvalidOperationException($"Unexpected request {requestKey}");
            }
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://localhost") };
        var sut = new InventoryApiClient(httpClient, sessionService);

        var materials = await sut.GetMaterialsAsync();
        var created = await sut.CreateMaterialAsync(requestModel);
        var updated = await sut.UpdateMaterialAsync(new UpdateInventoryMaterialRequest(43, requestModel));
        var deleted = await sut.DeleteMaterialAsync(43);

        Assert.True(materials.IsSuccess);
        Assert.Equal("Material técnico", materials.Value![0].ParentCategoryName);
        Assert.Equal(new DateOnly(2026, 1, 18), materials.Value[0].AcquisitionDate);
        Assert.Equal(43, created.Value);
        Assert.True(updated.IsSuccess);
        Assert.True(deleted.IsSuccess);
        Assert.Equal(4, observedRequests.Count);
    }

    [Fact]
    public async Task ReturnsSafeProblemDetailsAndTransportErrors()
    {
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(TestSessions.AdminSession);
        var responses = new Queue<Func<HttpResponseMessage>>(
        [
            () => JsonResponse("{\"detail\":\"La cantidad debe ser positiva.\",\"errors\":[{\"propertyName\":\"cantidad\",\"errorMessage\":\"La cantidad debe ser mayor que cero.\"}]}", HttpStatusCode.BadRequest),
            () => new HttpResponseMessage(HttpStatusCode.Unauthorized),
            () => new HttpResponseMessage(HttpStatusCode.Forbidden),
            () => JsonResponse("{\"detail\":\"El estado ya no existe.\"}", HttpStatusCode.NotFound),
            () => JsonResponse("{\"detail\":\"Número de serie duplicado.\"}", HttpStatusCode.Conflict),
            () => JsonResponse("{\"detail\":\"secret internals\"}", HttpStatusCode.InternalServerError)
        ]);

        var handler = new StubHttpMessageHandler(_ => Task.FromResult(responses.Dequeue().Invoke()));
        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://localhost") };
        var sut = new InventoryApiClient(httpClient, sessionService);

        var validation = await sut.CreateStatusAsync(new CreateInventoryStatusRequest("Disponible"));
        var unauthorized = await sut.GetStatusesAsync();
        var forbidden = await sut.GetStatusesAsync();
        var notFound = await sut.GetStatusesAsync();
        var conflict = await sut.GetStatusesAsync();
        var unavailable = await sut.GetStatusesAsync();

        Assert.Equal("inventory.validation", validation.Error!.Code);
        Assert.Equal("La cantidad debe ser positiva.", validation.Error.Message);
        Assert.Collection(validation.Error.ValidationErrors!, error =>
        {
            Assert.Equal("cantidad", error.PropertyName);
            Assert.Equal("La cantidad debe ser mayor que cero.", error.ErrorMessage);
        });
        Assert.Equal("auth.session_invalid", unauthorized.Error!.Code);
        Assert.Equal("inventory.forbidden", forbidden.Error!.Code);
        Assert.Equal("inventory.not_found", notFound.Error!.Code);
        Assert.Equal("El estado ya no existe.", notFound.Error.Message);
        Assert.Equal("inventory.conflict", conflict.Error!.Code);
        Assert.Equal("Número de serie duplicado.", conflict.Error.Message);
        Assert.Equal("inventory.unavailable", unavailable.Error!.Code);
        Assert.DoesNotContain("secret", unavailable.Error.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ReturnsInvalidPayloadAndTimeoutErrors()
    {
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(TestSessions.AdminSession);
        var calls = 0;
        var handler = new StubHttpMessageHandler(_ =>
        {
            calls++;
            if (calls == 1)
            {
                return Task.FromResult(JsonResponse("{}"));
            }

            throw new TaskCanceledException();
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://localhost") };
        var sut = new InventoryApiClient(httpClient, sessionService);

        var invalidPayload = await sut.GetCategoriesAsync();
        var timeout = await sut.GetCategoriesAsync();

        Assert.Equal("inventory.invalid_payload", invalidPayload.Error!.Code);
        Assert.Equal("inventory.timeout", timeout.Error!.Code);
    }

    private static HttpResponseMessage JsonResponse(string body, HttpStatusCode statusCode = HttpStatusCode.OK)
        => new(statusCode)
        {
            Content = new StringContent(body, Encoding.UTF8, "application/json")
        };

    private static async Task AssertJsonPropertyAsync(HttpRequestMessage request, string propertyName, object expectedValue)
    {
        using var document = JsonDocument.Parse(await request.Content!.ReadAsStringAsync());
        var property = document.RootElement.GetProperty(propertyName);
        Assert.Equal(expectedValue.ToString(), property.ToString());
    }

    private static async Task AssertMaterialPayloadAsync(HttpRequestMessage request)
    {
        using var document = JsonDocument.Parse(await request.Content!.ReadAsStringAsync());
        var payload = document.RootElement;
        Assert.Equal("Casco de escalada", payload.GetProperty("descripcion").GetString());
        Assert.Equal("Petzl Boreo", payload.GetProperty("marcaModelo").GetString());
        Assert.Equal(1, payload.GetProperty("cantidad").GetInt32());
        Assert.Equal(11, payload.GetProperty("categoriaId").GetInt32());
        Assert.Equal(3, payload.GetProperty("estadoId").GetInt32());
        Assert.Equal("2026-01-18", payload.GetProperty("fechaAdquisicion").GetString());
        Assert.Equal("Adquirido", payload.GetProperty("adquiridoDonado").GetString());
        Assert.Equal(JsonValueKind.Null, payload.GetProperty("observaciones").ValueKind);
        Assert.Equal("CASCO-001", payload.GetProperty("numeroSerie").GetString());
    }
}
