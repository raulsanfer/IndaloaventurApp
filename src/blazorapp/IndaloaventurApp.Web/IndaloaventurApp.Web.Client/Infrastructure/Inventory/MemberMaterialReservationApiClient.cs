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
/// HTTP client for the authenticated member material reservation endpoints.
/// </summary>
public sealed class MemberMaterialReservationApiClient(HttpClient httpClient, ISessionService sessionService)
    : IMemberMaterialReservationService
{
    private const string MyReservationsEndpoint = "/api/inventario-reservas/me";
    private const string MaterialsEndpoint = "/api/inventario-materiales";

    /// <inheritdoc />
    public Task<ServiceResult<IReadOnlyList<MaterialReservation>>> GetMyReservationsAsync(CancellationToken cancellationToken = default)
        => SendAsync(HttpMethod.Get, MyReservationsEndpoint, null, ReadReservationsAsync, cancellationToken);

    /// <inheritdoc />
    public Task<ServiceResult<MaterialReservation>> GetMyReservationAsync(Guid reservationId, CancellationToken cancellationToken = default)
        => SendAsync(HttpMethod.Get, ReservationEndpoint(reservationId), null, ReadReservationAsync, cancellationToken);

    /// <inheritdoc />
    public Task<ServiceResult<IReadOnlyList<ReservableInventoryMaterial>>> GetReservableMaterialsAsync(CancellationToken cancellationToken = default)
        => SendAsync(HttpMethod.Get, MaterialsEndpoint, null, ReadReservableMaterialsAsync, cancellationToken);

    /// <inheritdoc />
    public Task<ServiceResult<MaterialReservation>> CreateReservationAsync(
        CreateMemberMaterialReservationRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        return SendAsync(
            HttpMethod.Post,
            MyReservationsEndpoint,
            new ReservationApiRequest(
                request.InventoryMaterialId,
                request.RequestedQuantity,
                request.ScheduledPickupDate,
                request.EstimatedReturnDate,
                request.MemberNotes),
            ReadReservationAsync,
            cancellationToken);
    }

    /// <inheritdoc />
    public Task<ServiceResult<bool>> CancelReservationAsync(Guid reservationId, CancellationToken cancellationToken = default)
        => SendAsync(HttpMethod.Delete, ReservationEndpoint(reservationId), null, ReadNoContentSuccessAsync, cancellationToken);

    /// <inheritdoc />
    public Task<ServiceResult<bool>> ConfirmReturnAsync(Guid reservationId, CancellationToken cancellationToken = default)
        => SendAsync(
            HttpMethod.Post,
            $"{ReservationEndpoint(reservationId)}/confirmacion-devolucion",
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
            return ServiceResult<T>.Failure(new ServiceError("reservations.unavailable", "No se pudo conectar con las reservas de material."));
        }
        catch (TaskCanceledException)
        {
            return ServiceResult<T>.Failure(new ServiceError("reservations.timeout", "Tiempo de espera agotado."));
        }
        catch (JsonException)
        {
            return ServiceResult<T>.Failure(InvalidPayloadError());
        }
        catch (NotSupportedException)
        {
            return ServiceResult<T>.Failure(InvalidPayloadError());
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

    private static async Task<ServiceResult<IReadOnlyList<MaterialReservation>>> ReadReservationsAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        var payload = await response.Content.ReadFromJsonAsync<ReservationApiDto[]>(cancellationToken: cancellationToken);
        return payload is null
            ? ServiceResult<IReadOnlyList<MaterialReservation>>.Failure(InvalidPayloadError())
            : ServiceResult<IReadOnlyList<MaterialReservation>>.Success(payload.Select(MapReservation).ToArray());
    }

    private static async Task<ServiceResult<MaterialReservation>> ReadReservationAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        var payload = await response.Content.ReadFromJsonAsync<ReservationApiDto>(cancellationToken: cancellationToken);
        return payload is null
            ? ServiceResult<MaterialReservation>.Failure(InvalidPayloadError())
            : ServiceResult<MaterialReservation>.Success(MapReservation(payload));
    }

    private static async Task<ServiceResult<IReadOnlyList<ReservableInventoryMaterial>>> ReadReservableMaterialsAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        var payload = await response.Content.ReadFromJsonAsync<ReservableMaterialApiDto[]>(cancellationToken: cancellationToken);
        if (payload is null || payload.Any(static item => item.Id <= 0 || item.CantidadDisponible < 0 || string.IsNullOrWhiteSpace(item.Descripcion)))
        {
            return ServiceResult<IReadOnlyList<ReservableInventoryMaterial>>.Failure(InvalidPayloadError());
        }

        var materials = payload
            .Where(static item => item.Activo)
            .Select(static item => new ReservableInventoryMaterial(item.Id, item.Descripcion!.Trim(), item.Activo, item.CantidadDisponible))
            .ToArray();

        return ServiceResult<IReadOnlyList<ReservableInventoryMaterial>>.Success(materials);
    }

    private static Task<ServiceResult<bool>> ReadNoContentSuccessAsync(
        HttpResponseMessage response,
        CancellationToken _)
        => Task.FromResult(
            response.StatusCode == HttpStatusCode.NoContent
                ? ServiceResult<bool>.Success(true)
                : ServiceResult<bool>.Failure(InvalidPayloadError()));

    private static MaterialReservation MapReservation(ReservationApiDto reservation)
        => new(
            reservation.Id,
            reservation.UserId,
            reservation.InventarioMaterialId,
            NormalizeRequiredText(reservation.MaterialDescripcion),
            reservation.CantidadSolicitada,
            reservation.FechaSolicitudUtc,
            reservation.FechaRetiradaPrevista,
            reservation.FechaRetirada,
            reservation.FechaDevolucionEstimada,
            reservation.FechaDevolucionEfectiva,
            NormalizeOptionalText(reservation.Observaciones),
            NormalizeOptionalText(reservation.ObservacionAdministracion),
            reservation.Estado,
            reservation.ConfirmacionDevolucionSocio,
            reservation.ConfirmacionDevolucionAdmin,
            null,
            reservation.Disponibilidad is null
                ? null
                : new MaterialReservationAvailability(
                    reservation.Disponibilidad.InventarioMaterialId,
                    reservation.Disponibilidad.CantidadTotal,
                    reservation.Disponibilidad.CantidadComprometida,
                    reservation.Disponibilidad.CantidadDisponible,
                    reservation.Disponibilidad.Activo));

    private static async Task<ServiceError> ReadErrorAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            return new ServiceError("auth.session_invalid", "La sesión ya no es válida.");
        }

        if (response.StatusCode == HttpStatusCode.Forbidden)
        {
            return new ServiceError("reservations.forbidden", "No tienes permisos de socio para gestionar reservas de material.");
        }

        if (response.StatusCode is not (HttpStatusCode.BadRequest or HttpStatusCode.NotFound or HttpStatusCode.Conflict))
        {
            return new ServiceError("reservations.unavailable", "No se pudo completar la operación de reserva.");
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
            HttpStatusCode.BadRequest => "reservations.validation",
            HttpStatusCode.NotFound => "reservations.not_found",
            HttpStatusCode.Conflict => "reservations.conflict",
            _ => "reservations.unavailable"
        };

    private static string GetFallbackMessage(HttpStatusCode statusCode)
        => statusCode switch
        {
            HttpStatusCode.BadRequest => "Revisa los datos indicados.",
            HttpStatusCode.NotFound => "La reserva solicitada ya no está disponible.",
            HttpStatusCode.Conflict => "No se puede completar la operación con el estado actual de la reserva.",
            _ => "No se pudo completar la operación de reserva."
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
        => new("reservations.invalid_payload", "La respuesta de reservas no tiene un formato válido.");

    private static string ReservationEndpoint(Guid reservationId) => $"{MyReservationsEndpoint}/{reservationId}";

    private static string NormalizeRequiredText(string? value) => value?.Trim() ?? string.Empty;

    private static string? NormalizeOptionalText(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private sealed record ReservationApiDto(
        Guid Id,
        Guid UserId,
        int InventarioMaterialId,
        string? MaterialDescripcion,
        int CantidadSolicitada,
        DateTime FechaSolicitudUtc,
        DateOnly FechaRetiradaPrevista,
        DateOnly? FechaRetirada,
        DateOnly FechaDevolucionEstimada,
        DateOnly? FechaDevolucionEfectiva,
        string? Observaciones,
        string? ObservacionAdministracion,
        MaterialReservationStatus Estado,
        bool ConfirmacionDevolucionSocio,
        bool ConfirmacionDevolucionAdmin,
        ReservationAvailabilityApiDto? Disponibilidad);

    private sealed record ReservationAvailabilityApiDto(
        int InventarioMaterialId,
        int CantidadTotal,
        int CantidadComprometida,
        int CantidadDisponible,
        bool Activo);

    private sealed record ReservableMaterialApiDto(int Id, string? Descripcion, bool Activo, int CantidadDisponible);

    private sealed record ReservationApiRequest(
        int InventarioMaterialId,
        int CantidadSolicitada,
        DateOnly FechaRetiradaPrevista,
        DateOnly FechaDevolucionEstimada,
        string? Observaciones);
}
