namespace IndaloaventurApp.SharedUI.Abstractions.FoodAlerts;

using IndaloaventurApp.SharedUI.Models.Common;
using IndaloaventurApp.SharedUI.Models.FoodAlerts;

public interface IFoodAlertService
{
    Task<ServiceResult<IReadOnlyList<FoodAlertListItem>>> GetAlertsAsync(string categoryCode, CancellationToken cancellationToken = default);

    Task<ServiceResult<FoodAlertDetailItem>> GetAlertAsync(string alertId, CancellationToken cancellationToken = default);
}
