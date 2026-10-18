using IndaloAventurApi.Domain.Abstractions;

namespace IndaloAventurApi.Domain.Inventario;

public sealed class InventarioNombre
{
    public const int MaxLength = 120;

    private InventarioNombre(string valor)
    {
        Valor = valor;
        ValorNormalizado = valor.ToUpperInvariant();
    }

    public string Valor { get; }
    public string ValorNormalizado { get; }

    public static InventarioNombre Crear(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new DomainException("El nombre es obligatorio.");
        }

        var valor = nombre.Trim();
        if (valor.Length > MaxLength)
        {
            throw new DomainException($"El nombre no puede superar {MaxLength} caracteres.");
        }

        if (valor.Any(char.IsControl))
        {
            throw new DomainException("El nombre contiene caracteres no permitidos.");
        }

        return new InventarioNombre(valor);
    }
}
