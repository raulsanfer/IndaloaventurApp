namespace IndaloaventurApp.SharedUI.Models.Signals;

public sealed record UpdateSignalCategoryRequest(
    int Id,
    string Name,
    string? IconName);
