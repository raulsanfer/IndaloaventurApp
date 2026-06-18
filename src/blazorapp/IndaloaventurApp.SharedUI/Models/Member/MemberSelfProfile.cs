namespace IndaloaventurApp.SharedUI.Models.Member;

public sealed record MemberSelfProfile(
    Guid UserId,
    int? CargoId,
    string? CargoLabel,
    string? Nombre,
    string? Apellidos,
    string? Dni,
    DateOnly FechaNacimiento,
    string? Direccion,
    string? CodigoPostal,
    string? Poblacion,
    string? Provincia,
    string? Tlf,
    string? Email,
    string? Alergias,
    bool AceptaPoliticaPrivacidad,
    bool AceptaUsoImagenes,
    bool AceptaCobroCuenta);
