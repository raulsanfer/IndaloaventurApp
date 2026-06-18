using IndaloAventurApi.Domain.Abstractions;
using IndaloAventurApi.Domain.FichasContacto.ValueObjects;

namespace IndaloAventurApi.Domain.FichasContacto;

public sealed class FichaContacto : Entity, IAggregateRoot
{
    private FichaContacto()
    {
    }

    private FichaContacto(
        Guid id,
        DateTime fechaAlta,
        NombreContacto nombre,
        TelefonoContacto telefono1,
        TelefonoContacto? telefono2,
        EmailContacto? email,
        DireccionContacto? direccion,
        ObservacionesContacto? observaciones)
    {
        Id = id;
        FechaAlta = fechaAlta;
        Nombre = nombre;
        Telefono1 = telefono1;
        Telefono2 = telefono2;
        Email = email;
        Direccion = direccion;
        Observaciones = observaciones;
    }

    public Guid Id { get; private set; }

    public DateTime FechaAlta { get; private set; }

    public NombreContacto Nombre { get; private set; } = null!;

    public TelefonoContacto Telefono1 { get; private set; } = null!;

    public TelefonoContacto? Telefono2 { get; private set; }

    public EmailContacto? Email { get; private set; }

    public DireccionContacto? Direccion { get; private set; }

    public ObservacionesContacto? Observaciones { get; private set; }

    public static FichaContacto Crear(
        string nombre,
        string telefono1,
        string? telefono2,
        string? email,
        string? direccion,
        string? observaciones,
        DateTime? fechaAlta = null)
    {
        return new FichaContacto(
            Guid.NewGuid(),
            (fechaAlta ?? DateTime.UtcNow).ToUniversalTime(),
            NombreContacto.Create(nombre),
            TelefonoContacto.Create(telefono1),
            TelefonoContacto.CreateOptional(telefono2),
            EmailContacto.CreateOptional(email),
            DireccionContacto.CreateOptional(direccion),
            ObservacionesContacto.CreateOptional(observaciones));
    }

    public void Actualizar(string nombre, string telefono1, string? telefono2, string? email, string? direccion, string? observaciones)
    {
        Nombre = NombreContacto.Create(nombre);
        Telefono1 = TelefonoContacto.Create(telefono1);
        Telefono2 = TelefonoContacto.CreateOptional(telefono2);
        Email = EmailContacto.CreateOptional(email);
        Direccion = DireccionContacto.CreateOptional(direccion);
        Observaciones = ObservacionesContacto.CreateOptional(observaciones);
    }
}
