namespace IndaloaventurApp.Web.Infrastructure.FoodAlerts;

using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using IndaloaventurApp.SharedUI.Abstractions.FoodAlerts;
using IndaloaventurApp.SharedUI.Models.Common;
using IndaloaventurApp.SharedUI.Models.FoodAlerts;

public sealed class ExternalFoodAlertService(HttpClient httpClient) : IFoodAlertService
{
    private const string AlertsEndpoint = "/api/Alerts";

    public async Task<ServiceResult<IReadOnlyList<FoodAlertListItem>>> GetAlertsAsync(string categoryCode, CancellationToken cancellationToken = default)
    {
        if (!FoodAlertCatalog.IsSupportedCode(categoryCode))
        {
            return ServiceResult<IReadOnlyList<FoodAlertListItem>>.Failure(new ServiceError("food_alerts.invalid_category", "Categoria no valida."));
        }

        try
        {
            using var response = await httpClient.GetAsync($"{AlertsEndpoint}/all?code={Uri.EscapeDataString(categoryCode)}", cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return ServiceResult<IReadOnlyList<FoodAlertListItem>>.Failure(new ServiceError("food_alerts.unavailable", $"Error HTTP {(int)response.StatusCode}."));
            }

            var payload = await response.Content.ReadFromJsonAsync<FoodAlertEnvelope<FoodAlertListDto[]>>(cancellationToken: cancellationToken);
            var items = payload?.Data ?? Array.Empty<FoodAlertListDto>();

            return ServiceResult<IReadOnlyList<FoodAlertListItem>>.Success(
                items.Select(item => MapListItem(item, categoryCode)).ToArray());
        }
        catch (HttpRequestException)
        {
            return ServiceResult<IReadOnlyList<FoodAlertListItem>>.Failure(new ServiceError("food_alerts.unavailable", "No se pudo conectar con las alertas alimentarias."));
        }
        catch (TaskCanceledException)
        {
            return ServiceResult<IReadOnlyList<FoodAlertListItem>>.Failure(new ServiceError("food_alerts.timeout", "Tiempo de espera agotado."));
        }
        catch (JsonException)
        {
            return ServiceResult<IReadOnlyList<FoodAlertListItem>>.Failure(new ServiceError("food_alerts.invalid_payload", "La respuesta no tiene un formato valido."));
        }
        catch (NotSupportedException)
        {
            return ServiceResult<IReadOnlyList<FoodAlertListItem>>.Failure(new ServiceError("food_alerts.invalid_payload", "La respuesta no tiene un formato valido."));
        }
    }

    public async Task<ServiceResult<FoodAlertDetailItem>> GetAlertAsync(string alertId, CancellationToken cancellationToken = default)
    {
        try
        {
            using var response = await httpClient.GetAsync($"{AlertsEndpoint}/{Uri.EscapeDataString(alertId)}", cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return ServiceResult<FoodAlertDetailItem>.Failure(new ServiceError("food_alerts.not_found", "La alerta no existe."));
            }

            if (!response.IsSuccessStatusCode)
            {
                return ServiceResult<FoodAlertDetailItem>.Failure(new ServiceError("food_alerts.unavailable", $"Error HTTP {(int)response.StatusCode}."));
            }

            var payload = await response.Content.ReadFromJsonAsync<FoodAlertDetailDto>(cancellationToken: cancellationToken);
            if (payload is null)
            {
                return ServiceResult<FoodAlertDetailItem>.Failure(new ServiceError("food_alerts.invalid_payload", "La respuesta no tiene un formato valido."));
            }

            return ServiceResult<FoodAlertDetailItem>.Success(MapDetailItem(payload));
        }
        catch (HttpRequestException)
        {
            return ServiceResult<FoodAlertDetailItem>.Failure(new ServiceError("food_alerts.unavailable", "No se pudo conectar con las alertas alimentarias."));
        }
        catch (TaskCanceledException)
        {
            return ServiceResult<FoodAlertDetailItem>.Failure(new ServiceError("food_alerts.timeout", "Tiempo de espera agotado."));
        }
        catch (JsonException)
        {
            return ServiceResult<FoodAlertDetailItem>.Failure(new ServiceError("food_alerts.invalid_payload", "La respuesta no tiene un formato valido."));
        }
        catch (NotSupportedException)
        {
            return ServiceResult<FoodAlertDetailItem>.Failure(new ServiceError("food_alerts.invalid_payload", "La respuesta no tiene un formato valido."));
        }
    }

    private static FoodAlertListItem MapListItem(FoodAlertListDto dto, string categoryCode)
    {
        var description = FoodAlertTextFormatter.NormalizeDescription(dto.Descripcion);
        return new FoodAlertListItem(
            NormalizeId(dto.Id),
            categoryCode,
            FoodAlertTextFormatter.NormalizeText(dto.Titulo, "Alerta alimentaria"),
            FoodAlertTextFormatter.BuildSummary(description),
            ParseDate(dto.Fecha),
            NormalizeUrl(dto.Url));
    }

    private static FoodAlertDetailItem MapDetailItem(FoodAlertDetailDto dto)
    {
        return new FoodAlertDetailItem(
            NormalizeId(dto.Id),
            null,
            FoodAlertTextFormatter.NormalizeText(dto.Titulo, "Alerta alimentaria"),
            FoodAlertTextFormatter.NormalizeDescription(dto.Descripcion),
            ParseDate(dto.Fecha),
            NormalizeUrl(dto.Url));
    }

    private static string NormalizeId(JsonElement id)
    {
        return id.ValueKind switch
        {
            JsonValueKind.String => string.IsNullOrWhiteSpace(id.GetString()) ? string.Empty : id.GetString()!.Trim(),
            JsonValueKind.Number => id.ToString(),
            _ => string.Empty
        };
    }

    private static string? NormalizeUrl(string? url)
        => string.IsNullOrWhiteSpace(url) ? null : url.Trim();

    private static DateTime? ParseDate(string? dateText)
    {
        if (string.IsNullOrWhiteSpace(dateText))
        {
            return null;
        }

        if (DateTime.TryParse(
                dateText,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                out var parsed))
        {
            return DateTime.SpecifyKind(parsed, DateTimeKind.Utc);
        }

        return null;
    }

    private sealed record FoodAlertEnvelope<T>(
        bool Error,
        string? Mensaje,
        T? Data);

    private sealed record FoodAlertListDto(
        JsonElement Id,
        string? Fecha,
        string? Titulo,
        string? Url,
        string? Descripcion);

    private sealed record FoodAlertDetailDto(
        JsonElement Id,
        string? Fecha,
        string? Titulo,
        string? Url,
        string? Descripcion);
}
