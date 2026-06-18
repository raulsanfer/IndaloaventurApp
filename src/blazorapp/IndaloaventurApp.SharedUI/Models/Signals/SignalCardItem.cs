namespace IndaloaventurApp.SharedUI.Models.Signals;

public sealed record SignalCardItem(
    Guid Id,
    string Title,
    string Summary,
    int CategoryId,
    string CategoryName,
    string? CategoryIconName,
    DateTime Timestamp,
    string? MetaLabel,
    bool IsActive,
    string? ImageUrl,
    string? Tags,
    float Latitude,
    float Longitude);
