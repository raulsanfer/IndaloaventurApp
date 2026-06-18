namespace IndaloaventurApp.SharedUI.Models.Signals;

public sealed record SignalPhotoDraft(
    byte[] Content,
    string ContentType,
    string FileName,
    string PreviewUrl);
