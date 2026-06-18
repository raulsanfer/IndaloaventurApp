namespace IndaloaventurApp.SharedUI.Models.Member;

using System.ComponentModel.DataAnnotations;

public sealed class MemberSelfProfileFormModel : IValidatableObject
{
    [Range(1, int.MaxValue, ErrorMessage = "El cargo debe ser un identificador positivo.")]
    public int? CargoId { get; set; }

    public string? CargoLabel { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(80, ErrorMessage = "El nombre no puede superar los 80 caracteres.")]
    [NoUnsafeText(ErrorMessage = "El nombre contiene caracteres no permitidos.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "Los apellidos son obligatorios.")]
    [StringLength(120, ErrorMessage = "Los apellidos no pueden superar los 120 caracteres.")]
    [NoUnsafeText(ErrorMessage = "Los apellidos contienen caracteres no permitidos.")]
    public string Apellidos { get; set; } = string.Empty;

    [StringLength(20, ErrorMessage = "El DNI no puede superar los 20 caracteres.")]
    [RegularExpression(@"^[0-9A-Za-z\-]*$", ErrorMessage = "El DNI solo puede contener letras, números o guiones.")]
    public string? Dni { get; set; }

    public DateOnly FechaNacimiento { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    [StringLength(180, ErrorMessage = "La dirección no puede superar los 180 caracteres.")]
    [NoUnsafeText(ErrorMessage = "La dirección contiene caracteres no permitidos.")]
    public string? Direccion { get; set; }

    [RegularExpression(@"^\d{5}$", ErrorMessage = "El código postal debe tener 5 dígitos.")]
    public string? CodigoPostal { get; set; }

    [StringLength(80, ErrorMessage = "La población no puede superar los 80 caracteres.")]
    [NoUnsafeText(ErrorMessage = "La población contiene caracteres no permitidos.")]
    public string? Poblacion { get; set; }

    [StringLength(80, ErrorMessage = "La provincia no puede superar los 80 caracteres.")]
    [NoUnsafeText(ErrorMessage = "La provincia contiene caracteres no permitidos.")]
    public string? Provincia { get; set; }

    [StringLength(20, ErrorMessage = "El teléfono no puede superar los 20 caracteres.")]
    [RegularExpression(@"^[0-9+\s().-]*$", ErrorMessage = "El teléfono contiene caracteres no permitidos.")]
    public string? Tlf { get; set; }

    [Required(ErrorMessage = "El email es obligatorio.")]
    [EmailAddress(ErrorMessage = "El email no tiene un formato válido.")]
    [StringLength(120, ErrorMessage = "El email no puede superar los 120 caracteres.")]
    public string Email { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "El campo de alergias no puede superar los 500 caracteres.")]
    [NoUnsafeText(ErrorMessage = "El campo de alergias contiene caracteres no permitidos.")]
    public string? Alergias { get; set; }

    public bool AceptaPoliticaPrivacidad { get; set; }

    public bool AceptaUsoImagenes { get; set; }

    public bool AceptaCobroCuenta { get; set; }

    public static MemberSelfProfileFormModel FromProfile(MemberSelfProfile profile)
    {
        return new MemberSelfProfileFormModel
        {
            CargoId = profile.CargoId,
            CargoLabel = profile.CargoLabel,
            Nombre = profile.Nombre ?? string.Empty,
            Apellidos = profile.Apellidos ?? string.Empty,
            Dni = profile.Dni,
            FechaNacimiento = profile.FechaNacimiento,
            Direccion = profile.Direccion,
            CodigoPostal = profile.CodigoPostal,
            Poblacion = profile.Poblacion,
            Provincia = profile.Provincia,
            Tlf = profile.Tlf,
            Email = profile.Email ?? string.Empty,
            Alergias = profile.Alergias,
            AceptaPoliticaPrivacidad = profile.AceptaPoliticaPrivacidad,
            AceptaUsoImagenes = profile.AceptaUsoImagenes,
            AceptaCobroCuenta = profile.AceptaCobroCuenta
        };
    }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (FechaNacimiento > DateOnly.FromDateTime(DateTime.Today))
        {
            yield return new ValidationResult(
                "La fecha de nacimiento no puede estar en el futuro.",
                new[] { nameof(FechaNacimiento) });
        }

        if (FechaNacimiento < new DateOnly(1900, 1, 1))
        {
            yield return new ValidationResult(
                "La fecha de nacimiento debe ser posterior al 1 de enero de 1900.",
                new[] { nameof(FechaNacimiento) });
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    private sealed class NoUnsafeTextAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is not string text || string.IsNullOrWhiteSpace(text))
            {
                return true;
            }

            return text.All(character => !char.IsControl(character) && character is not '<' and not '>');
        }
    }
}
