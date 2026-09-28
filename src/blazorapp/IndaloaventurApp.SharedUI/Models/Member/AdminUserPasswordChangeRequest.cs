namespace IndaloaventurApp.SharedUI.Models.Member;

/// <summary>
/// Contains the new password requested by an administrator for a managed account.
/// </summary>
public sealed record AdminUserPasswordChangeRequest(string NewPassword);
