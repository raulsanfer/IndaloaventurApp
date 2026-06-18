namespace IndaloaventurApp.SharedUI.Models.Signals;

public sealed record SignalListQuery(
    string? SearchText = null,
    int? CategoryId = null,
    bool OnlyActive = true);
