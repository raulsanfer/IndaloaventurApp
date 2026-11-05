using IndaloAventurApi.Domain.Abstractions;

namespace IndaloAventurApi.Domain.Inventario;

public sealed class InventarioMaterial : Entity, IAggregateRoot
{
    private InventarioMaterial() { }
    private InventarioMaterial(string descripcion, string? marcaModelo, int cantidad, int categoriaId, int estadoId, DateOnly? fechaAdquisicion, string adquiridoDonado, string? observaciones, string? numeroSerie, bool activo)
        => Apply(descripcion, marcaModelo, cantidad, categoriaId, estadoId, fechaAdquisicion, adquiridoDonado, observaciones, numeroSerie, activo);

    public int Id { get; private set; }
    public string Descripcion { get; private set; } = string.Empty;
    public string? MarcaModelo { get; private set; }
    public int Cantidad { get; private set; }
    public int CategoriaId { get; private set; }
    public int EstadoId { get; private set; }
    public DateOnly? FechaAdquisicion { get; private set; }
    public string AdquiridoDonado { get; private set; } = string.Empty;
    public string? Observaciones { get; private set; }
    public string? NumeroSerie { get; private set; }
    public string? NumeroSerieNormalizado { get; private set; }
    public bool Activo { get; private set; }

    public static InventarioMaterial Crear(string descripcion, string? marcaModelo, int cantidad, int categoriaId, int estadoId, DateOnly? fechaAdquisicion, string adquiridoDonado, string? observaciones, string? numeroSerie, bool activo = true)
        => new(descripcion, marcaModelo, cantidad, categoriaId, estadoId, fechaAdquisicion, adquiridoDonado, observaciones, numeroSerie, activo);

    public void Actualizar(string descripcion, string? marcaModelo, int cantidad, int categoriaId, int estadoId, DateOnly? fechaAdquisicion, string adquiridoDonado, string? observaciones, string? numeroSerie, bool activo)
        => Apply(descripcion, marcaModelo, cantidad, categoriaId, estadoId, fechaAdquisicion, adquiridoDonado, observaciones, numeroSerie, activo);

    private void Apply(string descripcion, string? marcaModelo, int cantidad, int categoriaId, int estadoId, DateOnly? fechaAdquisicion, string adquiridoDonado, string? observaciones, string? numeroSerie, bool activo)
    {
        if (cantidad <= 0) throw new DomainException("La cantidad debe ser mayor que cero.");
        if (categoriaId <= 0) throw new DomainException("La categoria es obligatoria.");
        if (estadoId <= 0) throw new DomainException("El estado es obligatorio.");
        if (fechaAdquisicion > DateOnly.FromDateTime(DateTime.UtcNow)) throw new DomainException("La fecha de adquisicion no puede ser futura.");
        Descripcion = InventarioMaterialTexto.Required(descripcion, "descripcion", InventarioMaterialTexto.DescripcionMaxLength);
        MarcaModelo = InventarioMaterialTexto.Optional(marcaModelo, "marca/modelo", InventarioMaterialTexto.MarcaModeloMaxLength);
        AdquiridoDonado = InventarioMaterialTexto.Required(adquiridoDonado, "adquirido/donado", InventarioMaterialTexto.AdquiridoDonadoMaxLength);
        Observaciones = InventarioMaterialTexto.Optional(observaciones, "observaciones", InventarioMaterialTexto.ObservacionesMaxLength, true);
        NumeroSerie = InventarioMaterialTexto.Optional(numeroSerie, "numero de serie", InventarioMaterialTexto.NumeroSerieMaxLength);
        if (NumeroSerie is not null && cantidad != 1) throw new DomainException("Un material con numero de serie debe tener cantidad uno.");
        NumeroSerieNormalizado = NumeroSerie is null ? null : InventarioMaterialTexto.NormalizeSerial(NumeroSerie);
        Cantidad = cantidad; CategoriaId = categoriaId; EstadoId = estadoId; FechaAdquisicion = fechaAdquisicion; Activo = activo;
    }
}
