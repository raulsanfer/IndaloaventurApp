namespace IndaloaventurApp.SharedUI.Abstractions.FoodAlerts;

using IndaloaventurApp.SharedUI.Models.Common;
using IndaloaventurApp.SharedUI.Models.FoodAlerts;

/// <summary>
/// Provides food alert lists and details to the shared UI components.
/// </summary>
public interface IFoodAlertService
{
    /// <summary>
    /// Gets food alerts for a catalog category.
    /// </summary>
    Task<ServiceResult<IReadOnlyList<FoodAlertListItem>>> GetAlertsAsync(string categoryCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the detail for a food alert.
    /// </summary>
    Task<ServiceResult<FoodAlertDetailItem>> GetAlertAsync(string alertId, CancellationToken cancellationToken = default);
}
