namespace IndaloaventurApp.Web.Client.Infrastructure.FoodAlerts;

using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using IndaloaventurApp.SharedUI.Abstractions.FoodAlerts;
using IndaloaventurApp.SharedUI.Models.Common;
using IndaloaventurApp.SharedUI.Models.FoodAlerts;

public sealed class FoodAlertAppApiClient(HttpClient httpClient) : IFoodAlertService
{
    private const string Endpoint = "/api/food-alerts";

    public async Task<ServiceResult<IReadOnlyList<FoodAlertListItem>>> GetAlertsAsync(string categoryCode, CancellationToken cancellationToken = default)
    {
        try
        {
            using var response = await httpClient.GetAsync($"{Endpoint}?code={Uri.EscapeDataString(categoryCode)}", cancellationToken);

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return ServiceResult<IReadOnlyList<FoodAlertListItem>>.Failure(new ServiceError("food_alerts.invalid_category", "Categoria no valida."));
            }

            if (!response.IsSuccessStatusCode)
            {
                return ServiceResult<IReadOnlyList<FoodAlertListItem>>.Failure(new ServiceError("food_alerts.unavailable", $"Error HTTP {(int)response.StatusCode}."));
            }

            var payload = await response.Content.ReadFromJsonAsync<FoodAlertListItem[]>(cancellationToken: cancellationToken);
            return ServiceResult<IReadOnlyList<FoodAlertListItem>>.Success(payload ?? Array.Empty<FoodAlertListItem>());
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
            using var response = await httpClient.GetAsync($"{Endpoint}/{Uri.EscapeDataString(alertId)}", cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return ServiceResult<FoodAlertDetailItem>.Failure(new ServiceError("food_alerts.not_found", "La alerta no existe."));
            }

            if (!response.IsSuccessStatusCode)
            {
                return ServiceResult<FoodAlertDetailItem>.Failure(new ServiceError("food_alerts.unavailable", $"Error HTTP {(int)response.StatusCode}."));
            }

            var payload = await response.Content.ReadFromJsonAsync<FoodAlertDetailItem>(cancellationToken: cancellationToken);
            if (payload is null)
            {
                return ServiceResult<FoodAlertDetailItem>.Failure(new ServiceError("food_alerts.invalid_payload", "La respuesta no tiene un formato valido."));
            }

            return ServiceResult<FoodAlertDetailItem>.Success(payload);
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
}
