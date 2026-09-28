namespace IndaloaventurApp.SharedUI.Models.Member;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Validates the password value accepted by the administrative change-password form.
/// </summary>
public sealed class AdminUserPasswordChangeFormModel
{
    [Required(ErrorMessage = "Indica la nueva contraseña.")]
    [StringLength(20, MinimumLength = 1, ErrorMessage = "La contraseña debe tener entre 1 y 20 caracteres.")]
    [RegularExpression("^[A-Za-z0-9]+$", ErrorMessage = "La contraseña solo puede contener caracteres alfanuméricos.")]
    public string NewPassword { get; set; } = string.Empty;
}
