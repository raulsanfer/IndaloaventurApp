namespace IndaloaventurApp.SharedUI.Models.Signals;

public sealed record SignalDetailItem(
    Guid Id,
    string Title,
    string Description,
    int CategoryId,
    string CategoryName,
    string? CategoryIconName,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    string? Tags,
    float Latitude,
    float Longitude,
    Guid OwnerUserId);
