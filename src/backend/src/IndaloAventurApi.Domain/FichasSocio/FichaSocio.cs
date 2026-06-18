using IndaloAventurApi.Domain.Abstractions;

namespace IndaloAventurApi.Domain.FichasSocio;

public sealed class FichaSocio : Entity, IAggregateRoot
{
    private FichaSocio()
    {
    }

    private FichaSocio(
        Guid id,
        Guid userId,
        int? cargoId,
        string nombre,
        string apellidos,
        string dni,
        DateOnly fechaNacimiento,
        string direccion,
        string codigoPostal,
        string poblacion,
        string provincia,
        string tlf,
        string email,
        string? alergias,
        bool aceptaPoliticaPrivacidad,
        bool aceptaUsoImagenes,
        bool aceptaCobroCuenta)
    {
        Id = id;
        UserId = userId;
        CargoId = cargoId;
        Nombre = nombre;
        Apellidos = apellidos;
        Dni = dni;
        FechaNacimiento = fechaNacimiento;
        Direccion = direccion;
        CodigoPostal = codigoPostal;
        Poblacion = poblacion;
        Provincia = provincia;
        Tlf = tlf;
        Email = email;
        Alergias = alergias;
        AceptaPoliticaPrivacidad = aceptaPoliticaPrivacidad;
        AceptaUsoImagenes = aceptaUsoImagenes;
        AceptaCobroCuenta = aceptaCobroCuenta;
    }

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public int? CargoId { get; private set; }
    public string Nombre { get; private set; } = string.Empty;
    public string Apellidos { get; private set; } = string.Empty;
    public string Dni { get; private set; } = string.Empty;
    public DateOnly FechaNacimiento { get; private set; }
    public string Direccion { get; private set; } = string.Empty;
    public string CodigoPostal { get; private set; } = string.Empty;
    public string Poblacion { get; private set; } = string.Empty;
    public string Provincia { get; private set; } = string.Empty;
    public string Tlf { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string? Alergias { get; private set; }
    public bool AceptaPoliticaPrivacidad { get; private set; }
    public bool AceptaUsoImagenes { get; private set; }
    public bool AceptaCobroCuenta { get; private set; }

    public static FichaSocio Crear(
        Guid userId,
        int? cargoId,
        string nombre,
        string apellidos,
        string dni,
        DateOnly fechaNacimiento,
        string direccion,
        string codigoPostal,
        string poblacion,
        string provincia,
        string tlf,
        string email,
        string? alergias,
        bool aceptaPoliticaPrivacidad,
        bool aceptaUsoImagenes,
        bool aceptaCobroCuenta)
    {
        if (userId == Guid.Empty)
        {
            throw new DomainException("El usuario asociado es obligatorio.");
        }

        return new FichaSocio(
            Guid.NewGuid(),
            userId,
            cargoId,
            nombre.Trim(),
            apellidos.Trim(),
            dni.Trim().ToUpperInvariant(),
            fechaNacimiento,
            direccion.Trim(),
            codigoPostal.Trim(),
            poblacion.Trim(),
            provincia.Trim(),
            tlf.Trim(),
            email.Trim(),
            string.IsNullOrWhiteSpace(alergias) ? null : alergias.Trim(),
            aceptaPoliticaPrivacidad,
            aceptaUsoImagenes,
            aceptaCobroCuenta);
    }

    public void Actualizar(
        int? cargoId,
        string nombre,
        string apellidos,
        string dni,
        DateOnly fechaNacimiento,
        string direccion,
        string codigoPostal,
        string poblacion,
        string provincia,
        string tlf,
        string email,
        string? alergias,
        bool aceptaPoliticaPrivacidad,
        bool aceptaUsoImagenes,
        bool aceptaCobroCuenta)
    {
        CargoId = cargoId;
        Nombre = nombre.Trim();
        Apellidos = apellidos.Trim();
        Dni = dni.Trim().ToUpperInvariant();
        FechaNacimiento = fechaNacimiento;
        Direccion = direccion.Trim();
        CodigoPostal = codigoPostal.Trim();
        Poblacion = poblacion.Trim();
        Provincia = provincia.Trim();
        Tlf = tlf.Trim();
        Email = email.Trim();
        Alergias = string.IsNullOrWhiteSpace(alergias) ? null : alergias.Trim();
        AceptaPoliticaPrivacidad = aceptaPoliticaPrivacidad;
        AceptaUsoImagenes = aceptaUsoImagenes;
        AceptaCobroCuenta = aceptaCobroCuenta;
    }
}
