namespace IndaloaventurApp.SharedUI.Models.Signals;

public sealed record SignalImagesItem(
    Guid SignalId,
    string? Photo1Url,
    string? Photo2Url);
