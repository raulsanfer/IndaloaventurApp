using IndaloAventurApi.Domain.Abstractions;

namespace IndaloAventurApi.Domain.TrailSignals;

public sealed class SignalType : Entity, IAggregateRoot
{
    private SignalType()
    {
    }

    private SignalType(string nombre, string icono)
    {
        Nombre = nombre;
        Icono = icono;
    }

    public int Id { get; private set; }
    public string Nombre { get; private set; } = string.Empty;
    public string Icono { get; private set; } = string.Empty;

    public static SignalType Crear(string nombre, string icono)
    {
        if (string.IsNullOrWhiteSpace(nombre)) throw new DomainException("El nombre del tipo de señal es obligatorio.");
        if (string.IsNullOrWhiteSpace(icono)) throw new DomainException("El icono del tipo de señal es obligatorio.");
        return new SignalType(nombre.Trim(), icono.Trim());
    }

    public void Actualizar(string nombre, string icono)
    {
        if (string.IsNullOrWhiteSpace(nombre)) throw new DomainException("El nombre del tipo de señal es obligatorio.");
        if (string.IsNullOrWhiteSpace(icono)) throw new DomainException("El icono del tipo de señal es obligatorio.");
        Nombre = nombre.Trim();
        Icono = icono.Trim();
    }
}
