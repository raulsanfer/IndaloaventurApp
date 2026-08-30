namespace IndaloaventurApp.SharedUI.Models.Signals;

public sealed record CreateSignalCommentRequest(
    Guid SignalId,
    string Text);
