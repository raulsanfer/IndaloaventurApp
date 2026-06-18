namespace IndaloaventurApp.Web.Infrastructure.FoodAlerts;

using IndaloaventurApp.SharedUI.Abstractions.FoodAlerts;
using Microsoft.AspNetCore.Http.HttpResults;

public static class FoodAlertEndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapFoodAlertEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/food-alerts");

        group.MapGet("/", GetAlertsAsync);
        group.MapGet("/{alertId}", GetAlertAsync);

        return endpoints;
    }

    private static async Task<Results<Ok<IReadOnlyList<IndaloaventurApp.SharedUI.Models.FoodAlerts.FoodAlertListItem>>, BadRequest<string>, StatusCodeHttpResult>> GetAlertsAsync(
        string code,
        IFoodAlertService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetAlertsAsync(code, cancellationToken);
        if (result.IsSuccess)
        {
            return TypedResults.Ok(result.Value ?? Array.Empty<IndaloaventurApp.SharedUI.Models.FoodAlerts.FoodAlertListItem>());
        }

        return result.Error?.Code switch
        {
            "food_alerts.invalid_category" => TypedResults.BadRequest("Categoria no valida."),
            _ => TypedResults.StatusCode(StatusCodes.Status503ServiceUnavailable)
        };
    }

    private static async Task<Results<Ok<IndaloaventurApp.SharedUI.Models.FoodAlerts.FoodAlertDetailItem>, NotFound, StatusCodeHttpResult>> GetAlertAsync(
        string alertId,
        IFoodAlertService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetAlertAsync(alertId, cancellationToken);
        if (result.IsSuccess && result.Value is not null)
        {
            return TypedResults.Ok(result.Value);
        }

        return result.Error?.Code switch
        {
            "food_alerts.not_found" => TypedResults.NotFound(),
            _ => TypedResults.StatusCode(StatusCodes.Status503ServiceUnavailable)
        };
    }
}
