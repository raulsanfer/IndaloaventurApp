namespace IndaloaventurApp.SharedUI.Models.Auth;

using System.ComponentModel.DataAnnotations;

public sealed class ResetPasswordFormModel
{
    [Required(ErrorMessage = "Indica la nueva contraseña.")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirma la nueva contraseña.")]
    [Compare(nameof(NewPassword), ErrorMessage = "La confirmación de la nueva contraseña no coincide.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
