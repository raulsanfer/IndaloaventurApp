namespace IndaloaventurApp.SharedUI.Models.FoodAlerts;

public sealed record FoodAlertDetailItem(
    string Id,
    string? CategoryCode,
    string Title,
    string Description,
    DateTime? PublishedAt,
    string? SourceUrl);
