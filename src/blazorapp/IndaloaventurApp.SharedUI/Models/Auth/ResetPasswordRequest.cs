namespace IndaloaventurApp.SharedUI.Models.Auth;

public sealed record ResetPasswordRequest(string Email, string Token, string NewPassword, string ConfirmPassword);
