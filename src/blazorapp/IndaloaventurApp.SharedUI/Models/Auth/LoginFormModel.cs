namespace IndaloaventurApp.SharedUI.Models.Auth;

using System.ComponentModel.DataAnnotations;

public sealed class LoginFormModel
{
    [Required]
    public string EmailOrUserName { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; }
}
