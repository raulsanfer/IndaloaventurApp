namespace IndaloaventurApp.SharedUI.Models.Member;

public static class MemberSelfProfileMapper
{
    public static UpdateMemberSelfProfileRequest ToRequest(MemberSelfProfileFormModel form)
    {
        return new UpdateMemberSelfProfileRequest(
            form.CargoId,
            NormalizeText(form.Nombre),
            NormalizeText(form.Apellidos),
            NormalizeIdentifier(form.Dni),
            form.FechaNacimiento,
            NormalizeText(form.Direccion),
            NormalizePostalCode(form.CodigoPostal),
            NormalizeText(form.Poblacion),
            NormalizeText(form.Provincia),
            NormalizePhone(form.Tlf),
            NormalizeEmail(form.Email),
            NormalizeText(form.Alergias),
            form.AceptaPoliticaPrivacidad,
            form.AceptaUsoImagenes,
            form.AceptaCobroCuenta);
    }

    public static string? NormalizeText(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var sanitized = new string(value.Trim().Where(character => !char.IsControl(character) && character is not '<' and not '>').ToArray());
        return string.IsNullOrWhiteSpace(sanitized) ? null : sanitized;
    }

    public static string? NormalizeIdentifier(string? value)
        => NormalizeText(value)?.ToUpperInvariant();

    public static string? NormalizePostalCode(string? value)
        => NormalizeText(value)?.Replace(" ", string.Empty, StringComparison.Ordinal);

    public static string? NormalizePhone(string? value)
        => NormalizeText(value);

    public static string? NormalizeEmail(string? value)
        => NormalizeText(value)?.ToLowerInvariant();
}
