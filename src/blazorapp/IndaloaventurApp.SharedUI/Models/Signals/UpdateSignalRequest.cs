namespace IndaloaventurApp.SharedUI.Models.Signals;

public sealed record UpdateSignalRequest(
    Guid SignalId,
    string Title,
    string Description,
    bool IsActive);
