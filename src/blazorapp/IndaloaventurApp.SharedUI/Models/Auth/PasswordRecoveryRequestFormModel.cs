namespace IndaloaventurApp.SharedUI.Models.Auth;

using System.ComponentModel.DataAnnotations;

public sealed class PasswordRecoveryRequestFormModel
{
    [Required(ErrorMessage = "Indica tu email.")]
    [EmailAddress(ErrorMessage = "Introduce un email válido.")]
    public string Email { get; set; } = string.Empty;
}
