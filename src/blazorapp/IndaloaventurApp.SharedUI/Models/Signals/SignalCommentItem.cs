namespace IndaloaventurApp.SharedUI.Models.Signals;

public sealed record SignalCommentItem(
    Guid Id,
    DateTime CommentedAt,
    string Text);
