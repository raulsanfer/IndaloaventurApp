namespace IndaloaventurApp.Web.Client.Infrastructure.Inventory;

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using IndaloaventurApp.SharedUI.Abstractions.Inventory;
using IndaloaventurApp.SharedUI.Abstractions.Session;
using IndaloaventurApp.SharedUI.Models.Common;
using IndaloaventurApp.SharedUI.Models.Inventory;

/// <summary>
/// HTTP client for the administrator inventory endpoints.
/// </summary>
public sealed class InventoryApiClient(HttpClient httpClient, ISessionService sessionService) : IInventoryService
{
    private const string CategoriesEndpoint = "/api/inventario-categorias";
    private const string StatusesEndpoint = "/api/inventario-estados";
    private const string MaterialsEndpoint = "/api/inventario-materiales";

    /// <inheritdoc />
    public Task<ServiceResult<IReadOnlyList<InventoryCategory>>> GetCategoriesAsync(CancellationToken cancellationToken = default)
        => SendAsync(
            HttpMethod.Get,
            CategoriesEndpoint,
            null,
            ReadCategoriesAsync,
            cancellationToken);

    /// <inheritdoc />
    public Task<ServiceResult<int>> CreateCategoryAsync(CreateInventoryCategoryRequest request, CancellationToken cancellationToken = default)
        => SendAsync(
            HttpMethod.Post,
            CategoriesEndpoint,
            new CategoryApiRequest(request.Name, request.ParentCategoryId),
            ReadCreatedIdentifierAsync,
            cancellationToken);

    /// <inheritdoc />
    public Task<ServiceResult<bool>> UpdateCategoryAsync(UpdateInventoryCategoryRequest request, CancellationToken cancellationToken = default)
        => SendAsync(
            HttpMethod.Put,
            $"{CategoriesEndpoint}/{request.Id}",
            new CategoryApiRequest(request.Name, request.ParentCategoryId),
            ReadNoContentSuccessAsync,
            cancellationToken);

    /// <inheritdoc />
    public Task<ServiceResult<bool>> DeleteCategoryAsync(int id, CancellationToken cancellationToken = default)
        => SendAsync(
            HttpMethod.Delete,
            $"{CategoriesEndpoint}/{id}",
            null,
            ReadNoContentSuccessAsync,
            cancellationToken);

    /// <inheritdoc />
    public Task<ServiceResult<IReadOnlyList<InventoryStatus>>> GetStatusesAsync(CancellationToken cancellationToken = default)
        => SendAsync(
            HttpMethod.Get,
            StatusesEndpoint,
            null,
            ReadStatusesAsync,
            cancellationToken);

    /// <inheritdoc />
    public Task<ServiceResult<int>> CreateStatusAsync(CreateInventoryStatusRequest request, CancellationToken cancellationToken = default)
        => SendAsync(
            HttpMethod.Post,
            StatusesEndpoint,
            new StatusApiRequest(request.Name),
            ReadCreatedIdentifierAsync,
            cancellationToken);

    /// <inheritdoc />
    public Task<ServiceResult<bool>> UpdateStatusAsync(UpdateInventoryStatusRequest request, CancellationToken cancellationToken = default)
        => SendAsync(
            HttpMethod.Put,
            $"{StatusesEndpoint}/{request.Id}",
            new StatusApiRequest(request.Name),
            ReadNoContentSuccessAsync,
            cancellationToken);

    /// <inheritdoc />
    public Task<ServiceResult<bool>> DeleteStatusAsync(int id, CancellationToken cancellationToken = default)
        => SendAsync(
            HttpMethod.Delete,
            $"{StatusesEndpoint}/{id}",
            null,
            ReadNoContentSuccessAsync,
            cancellationToken);

    /// <inheritdoc />
    public Task<ServiceResult<IReadOnlyList<InventoryMaterial>>> GetMaterialsAsync(CancellationToken cancellationToken = default)
        => SendAsync(
            HttpMethod.Get,
            MaterialsEndpoint,
            null,
            ReadMaterialsAsync,
            cancellationToken);

    /// <inheritdoc />
    public Task<ServiceResult<int>> CreateMaterialAsync(SaveInventoryMaterialRequest request, CancellationToken cancellationToken = default)
        => SendAsync(
            HttpMethod.Post,
            MaterialsEndpoint,
            new MaterialApiRequest(request),
            ReadCreatedIdentifierAsync,
            cancellationToken);

    /// <inheritdoc />
    public Task<ServiceResult<bool>> UpdateMaterialAsync(UpdateInventoryMaterialRequest request, CancellationToken cancellationToken = default)
        => SendAsync(
            HttpMethod.Put,
            $"{MaterialsEndpoint}/{request.Id}",
            new MaterialApiRequest(request.Material),
            ReadNoContentSuccessAsync,
            cancellationToken);

    /// <inheritdoc />
    public Task<ServiceResult<bool>> DeleteMaterialAsync(int id, CancellationToken cancellationToken = default)
        => SendAsync(
            HttpMethod.Delete,
            $"{MaterialsEndpoint}/{id}",
            null,
            ReadNoContentSuccessAsync,
            cancellationToken);

    private async Task<ServiceResult<T>> SendAsync<T>(
        HttpMethod method,
        string endpoint,
        object? payload,
        Func<HttpResponseMessage, CancellationToken, Task<ServiceResult<T>>> readSuccessAsync,
        CancellationToken cancellationToken)
    {
        try
        {
            using var request = CreateAuthorizedRequest(method, endpoint);
            if (payload is not null)
            {
                request.Content = JsonContent.Create(payload);
            }

            using var response = await httpClient.SendAsync(request, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                return await readSuccessAsync(response, cancellationToken);
            }

            return ServiceResult<T>.Failure(await ReadErrorAsync(response, cancellationToken));
        }
        catch (HttpRequestException)
        {
            return ServiceResult<T>.Failure(new ServiceError("inventory.unavailable", "No se pudo conectar con el inventario."));
        }
        catch (TaskCanceledException)
        {
            return ServiceResult<T>.Failure(new ServiceError("inventory.timeout", "Tiempo de espera agotado."));
        }
        catch (JsonException)
        {
            return ServiceResult<T>.Failure(new ServiceError("inventory.invalid_payload", "La respuesta de inventario no tiene un formato válido."));
        }
        catch (NotSupportedException)
        {
            return ServiceResult<T>.Failure(new ServiceError("inventory.invalid_payload", "La respuesta de inventario no tiene un formato válido."));
        }
    }

    private HttpRequestMessage CreateAuthorizedRequest(HttpMethod method, string endpoint)
    {
        var request = new HttpRequestMessage(method, endpoint);

        if (sessionService.IsAuthenticated && !string.IsNullOrWhiteSpace(sessionService.CurrentSession?.AccessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue(
                sessionService.CurrentSession!.TokenType,
                sessionService.CurrentSession.AccessToken);
        }

        return request;
    }

    private static async Task<ServiceResult<IReadOnlyList<InventoryCategory>>> ReadCategoriesAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        var payload = await response.Content.ReadFromJsonAsync<InventoryCategoryDto[]>(cancellationToken: cancellationToken);
        if (payload is null)
        {
            return ServiceResult<IReadOnlyList<InventoryCategory>>.Failure(InvalidPayloadError());
        }

        return ServiceResult<IReadOnlyList<InventoryCategory>>.Success(payload.Select(MapCategory).ToArray());
    }

    private static async Task<ServiceResult<IReadOnlyList<InventoryStatus>>> ReadStatusesAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        var payload = await response.Content.ReadFromJsonAsync<InventoryStatusDto[]>(cancellationToken: cancellationToken);
        if (payload is null)
        {
            return ServiceResult<IReadOnlyList<InventoryStatus>>.Failure(InvalidPayloadError());
        }

        return ServiceResult<IReadOnlyList<InventoryStatus>>.Success(
            payload.Select(static item => new InventoryStatus(item.Id, NormalizeRequiredText(item.Nombre))).ToArray());
    }

    private static async Task<ServiceResult<IReadOnlyList<InventoryMaterial>>> ReadMaterialsAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        var payload = await response.Content.ReadFromJsonAsync<InventoryMaterialDto[]>(cancellationToken: cancellationToken);
        if (payload is null)
        {
            return ServiceResult<IReadOnlyList<InventoryMaterial>>.Failure(InvalidPayloadError());
        }

        return ServiceResult<IReadOnlyList<InventoryMaterial>>.Success(payload.Select(MapMaterial).ToArray());
    }

    private static async Task<ServiceResult<int>> ReadCreatedIdentifierAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        var identifier = await response.Content.ReadFromJsonAsync<int?>(cancellationToken: cancellationToken);
        return identifier.HasValue && identifier.Value > 0
            ? ServiceResult<int>.Success(identifier.Value)
            : ServiceResult<int>.Failure(InvalidPayloadError());
    }

    private static Task<ServiceResult<bool>> ReadNoContentSuccessAsync(
        HttpResponseMessage _,
        CancellationToken __)
        => Task.FromResult(ServiceResult<bool>.Success(true));

    private static InventoryCategory MapCategory(InventoryCategoryDto category)
    {
        var subcategories = category.Subcategorias?.Select(MapCategory).ToArray()
            ?? Array.Empty<InventoryCategory>();

        return new InventoryCategory(
            category.Id,
            NormalizeRequiredText(category.Nombre),
            category.CategoriaPadreId,
            subcategories);
    }

    private static InventoryMaterial MapMaterial(InventoryMaterialDto material)
        => new(
            material.Id,
            NormalizeRequiredText(material.Descripcion),
            NormalizeOptionalText(material.MarcaModelo),
            material.Cantidad,
            material.CategoriaId,
            NormalizeRequiredText(material.CategoriaNombre),
            material.CategoriaPadreId,
            NormalizeOptionalText(material.CategoriaPadreNombre),
            material.EstadoId,
            NormalizeRequiredText(material.EstadoNombre),
            material.FechaAdquisicion,
            NormalizeRequiredText(material.AdquiridoDonado),
            NormalizeOptionalText(material.Observaciones),
            NormalizeOptionalText(material.NumeroSerie));

    private static async Task<ServiceError> ReadErrorAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            return new ServiceError("auth.session_invalid", "La sesión ya no es válida.");
        }

        if (response.StatusCode == HttpStatusCode.Forbidden)
        {
            return new ServiceError("inventory.forbidden", "No tienes permisos para gestionar el inventario.");
        }

        if (response.StatusCode is not (HttpStatusCode.BadRequest or HttpStatusCode.NotFound or HttpStatusCode.Conflict))
        {
            return new ServiceError("inventory.unavailable", "No se pudo completar la operación de inventario.");
        }

        try
        {
            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
            var root = document.RootElement;
            var detail = GetStringProperty(root, "detail") ?? GetStringProperty(root, "title") ?? GetFallbackMessage(response.StatusCode);
            return new ServiceError(GetErrorCode(response.StatusCode), detail, GetValidationErrors(root));
        }
        catch (JsonException)
        {
            return new ServiceError(GetErrorCode(response.StatusCode), GetFallbackMessage(response.StatusCode));
        }
    }

    private static IReadOnlyList<ServiceValidationError> GetValidationErrors(JsonElement root)
    {
        if (!TryGetProperty(root, "errors", out var errors))
        {
            return Array.Empty<ServiceValidationError>();
        }

        var validationErrors = new List<ServiceValidationError>();
        if (errors.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in errors.EnumerateArray())
            {
                var propertyName = GetStringProperty(item, "propertyName");
                var errorMessage = GetStringProperty(item, "errorMessage");
                if (!string.IsNullOrWhiteSpace(propertyName) && !string.IsNullOrWhiteSpace(errorMessage))
                {
                    validationErrors.Add(new ServiceValidationError(propertyName, errorMessage));
                }
            }

            return validationErrors;
        }

        if (errors.ValueKind != JsonValueKind.Object)
        {
            return validationErrors;
        }

        foreach (var property in errors.EnumerateObject())
        {
            if (property.Value.ValueKind == JsonValueKind.Array)
            {
                foreach (var message in property.Value.EnumerateArray())
                {
                    if (message.ValueKind == JsonValueKind.String && !string.IsNullOrWhiteSpace(message.GetString()))
                    {
                        validationErrors.Add(new ServiceValidationError(property.Name, message.GetString()!));
                    }
                }
            }
            else if (property.Value.ValueKind == JsonValueKind.String && !string.IsNullOrWhiteSpace(property.Value.GetString()))
            {
                validationErrors.Add(new ServiceValidationError(property.Name, property.Value.GetString()!));
            }
        }

        return validationErrors;
    }

    private static string GetErrorCode(HttpStatusCode statusCode)
        => statusCode switch
        {
            HttpStatusCode.BadRequest => "inventory.validation",
            HttpStatusCode.NotFound => "inventory.not_found",
            HttpStatusCode.Conflict => "inventory.conflict",
            _ => "inventory.unavailable"
        };

    private static string GetFallbackMessage(HttpStatusCode statusCode)
        => statusCode switch
        {
            HttpStatusCode.BadRequest => "Revisa los datos indicados.",
            HttpStatusCode.NotFound => "El recurso solicitado ya no está disponible.",
            HttpStatusCode.Conflict => "No se puede completar la operación con los datos actuales.",
            _ => "No se pudo completar la operación de inventario."
        };

    private static string? GetStringProperty(JsonElement element, string propertyName)
        => TryGetProperty(element, propertyName, out var property) && property.ValueKind == JsonValueKind.String
            ? property.GetString()
            : null;

    private static bool TryGetProperty(JsonElement element, string propertyName, out JsonElement property)
    {
        foreach (var candidate in element.EnumerateObject())
        {
            if (string.Equals(candidate.Name, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                property = candidate.Value;
                return true;
            }
        }

        property = default;
        return false;
    }

    private static ServiceError InvalidPayloadError()
        => new("inventory.invalid_payload", "La respuesta de inventario no tiene un formato válido.");

    private static string NormalizeRequiredText(string? value) => value?.Trim() ?? string.Empty;

    private static string? NormalizeOptionalText(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private sealed record InventoryCategoryDto(
        int Id,
        string? Nombre,
        int? CategoriaPadreId,
        InventoryCategoryDto[]? Subcategorias);

    private sealed record InventoryStatusDto(int Id, string? Nombre);

    private sealed record InventoryMaterialDto(
        int Id,
        string? Descripcion,
        string? MarcaModelo,
        int Cantidad,
        int CategoriaId,
        string? CategoriaNombre,
        int? CategoriaPadreId,
        string? CategoriaPadreNombre,
        int EstadoId,
        string? EstadoNombre,
        DateOnly? FechaAdquisicion,
        string? AdquiridoDonado,
        string? Observaciones,
        string? NumeroSerie);

    private sealed record CategoryApiRequest(string Nombre, int? CategoriaPadreId);

    private sealed record StatusApiRequest(string Nombre);

    private sealed record MaterialApiRequest
    {
        public MaterialApiRequest(SaveInventoryMaterialRequest request)
        {
            Descripcion = request.Description;
            MarcaModelo = request.BrandModel;
            Cantidad = request.Quantity;
            CategoriaId = request.CategoryId;
            EstadoId = request.StatusId;
            FechaAdquisicion = request.AcquisitionDate;
            AdquiridoDonado = request.AcquisitionOrDonation;
            Observaciones = request.Notes;
            NumeroSerie = request.SerialNumber;
        }

        public string Descripcion { get; }

        public string? MarcaModelo { get; }

        public int Cantidad { get; }

        public int CategoriaId { get; }

        public int EstadoId { get; }

        public DateOnly? FechaAdquisicion { get; }

        public string AdquiridoDonado { get; }

        public string? Observaciones { get; }

        public string? NumeroSerie { get; }
    }
}
