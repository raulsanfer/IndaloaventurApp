namespace IndaloaventurApp.SharedUI.Models.FoodAlerts;

public sealed record FoodAlertListItem(
    string Id,
    string CategoryCode,
    string Title,
    string Summary,
    DateTime? PublishedAt,
    string? SourceUrl);
