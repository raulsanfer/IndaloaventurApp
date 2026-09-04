namespace IndaloaventurApp.SharedUI.Models.Common;

/// <summary>
/// Describes a stable service error that UI components can map to localized feedback.
/// </summary>
/// <param name="Code">Stable machine-readable error code.</param>
/// <param name="Message">Fallback human-readable error message.</param>
public sealed record ServiceError(string Code, string Message);
