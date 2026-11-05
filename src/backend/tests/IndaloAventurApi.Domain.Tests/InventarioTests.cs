using IndaloAventurApi.Domain.Abstractions;
using IndaloAventurApi.Domain.Inventario;
using IndaloAventurApi.Domain.Inventario.ReservasMaterial;

namespace IndaloAventurApi.Domain.Tests;

public sealed class InventarioTests
{
    [Fact]
    public void InventarioNombre_Crear_NormalizaYRechazaEntradasInvalidas()
    {
        var nombre = InventarioNombre.Crear("  Material tecnico  ");

        Assert.Equal("Material tecnico", nombre.Valor);
        Assert.Equal("MATERIAL TECNICO", nombre.ValorNormalizado);
        Assert.Throws<DomainException>(() => InventarioNombre.Crear(" "));
        Assert.Throws<DomainException>(() => InventarioNombre.Crear("Material\u0001"));
        Assert.Throws<DomainException>(() => InventarioNombre.Crear(new string('a', InventarioNombre.MaxLength + 1)));
    }

    [Fact]
    public void InventarioCategoriaYEstado_Crear_ConservanNombreNormalizado()
    {
        var categoria = InventarioCategoria.Crear("  Cascos ", null);
        var estado = InventarioEstado.Crear(" Disponible ");

        Assert.Equal(0, categoria.Id);
        Assert.Equal("Cascos", categoria.Nombre);
        Assert.Equal("CASCOS", categoria.NombreNormalizado);
        Assert.Null(categoria.CategoriaPadreId);
        Assert.Equal("Disponible", estado.Nombre);
        Assert.Equal("DISPONIBLE", estado.NombreNormalizado);
    }

    [Fact]
    public void InventarioMaterial_ActivoSeInicializaYSePuedeDesactivar()
    {
        var material = InventarioMaterial.Crear(
            "Casco",
            null,
            2,
            1,
            1,
            null,
            "Adquirido",
            null,
            null);

        Assert.True(material.Activo);

        material.Actualizar("Casco", null, 2, 1, 1, null, "Adquirido", null, null, false);

        Assert.False(material.Activo);
    }

    [Fact]
    public void ReservaMaterial_Crear_ValidaSolicitudYConservaFechasPrevistas()
    {
        var usuarioId = Guid.NewGuid();
        var retirada = new DateOnly(2026, 11, 3);
        var devolucion = new DateOnly(2026, 11, 5);
        var solicitud = new DateTime(2026, 11, 1, 10, 0, 0, DateTimeKind.Utc);

        var reserva = ReservaMaterial.Crear(usuarioId, 4, 2, retirada, devolucion, "  Para salida del club. ", solicitud);

        Assert.NotEqual(Guid.Empty, reserva.Id);
        Assert.Equal(usuarioId, reserva.UserId);
        Assert.Equal(EstadoReservaMaterial.Solicitada, reserva.Estado);
        Assert.Equal(2, reserva.CantidadSolicitada);
        Assert.Equal(solicitud, reserva.FechaSolicitudUtc);
        Assert.Equal(retirada, reserva.FechaRetiradaPrevista);
        Assert.Equal(devolucion, reserva.FechaDevolucionEstimada);
        Assert.Equal("Para salida del club.", reserva.Observaciones);
        Assert.Null(reserva.FechaRetirada);
        Assert.Null(reserva.FechaDevolucionEfectiva);
        Assert.False(reserva.ConfirmacionDevolucionSocio);
        Assert.False(reserva.ConfirmacionDevolucionAdmin);
    }

    [Fact]
    public void ReservaMaterial_Crear_RechazaUsuarioMaterialCantidadYFechasInvalidas()
    {
        var retirada = new DateOnly(2026, 11, 3);

        Assert.Throws<DomainException>(() => ReservaMaterial.Crear(Guid.Empty, 1, 1, retirada, retirada, null));
        Assert.Throws<DomainException>(() => ReservaMaterial.Crear(Guid.NewGuid(), 0, 1, retirada, retirada, null));
        Assert.Throws<DomainException>(() => ReservaMaterial.Crear(Guid.NewGuid(), 1, 0, retirada, retirada, null));
        Assert.Throws<DomainException>(() => ReservaMaterial.Crear(Guid.NewGuid(), 1, 1, retirada, retirada.AddDays(-1), null));
    }

    [Fact]
    public void ReservaMaterial_RetiradaYDobleConfirmacion_CompletanDevolucion()
    {
        var reserva = CrearReserva();
        var retirada = new DateOnly(2026, 11, 3);
        var devolucion = new DateOnly(2026, 11, 6);

        reserva.RegistrarRetirada(retirada);
        reserva.ConfirmarDevolucionSocio();

        Assert.Equal(EstadoReservaMaterial.Retirada, reserva.Estado);
        Assert.True(reserva.ConfirmacionDevolucionSocio);
        Assert.False(reserva.ConfirmacionDevolucionAdmin);

        reserva.ConfirmarDevolucionAdministracion(devolucion);

        Assert.Equal(EstadoReservaMaterial.Entregada, reserva.Estado);
        Assert.True(reserva.ConfirmacionDevolucionAdmin);
        Assert.Equal(devolucion, reserva.FechaDevolucionEfectiva);
    }

    [Fact]
    public void ReservaMaterial_TransicionesInvalidasYCancelacionDeRetirada_SeRechazan()
    {
        var reserva = CrearReserva();

        Assert.Throws<DomainException>(() => reserva.ConfirmarDevolucionSocio());
        Assert.Throws<DomainException>(() => reserva.ActualizarSolicitud(1, 1, new DateOnly(2026, 11, 4), new DateOnly(2026, 11, 3), null));

        reserva.RegistrarRetirada(new DateOnly(2026, 11, 3));

        Assert.Throws<DomainException>(() => reserva.ActualizarSolicitud(1, 1, new DateOnly(2026, 11, 3), new DateOnly(2026, 11, 4), null));
        Assert.Throws<DomainException>(() => reserva.CancelarPorSocio());
        Assert.Throws<DomainException>(() => reserva.CancelarPorAdministracion(" "));
        Assert.Throws<DomainException>(() => reserva.ConfirmarDevolucionAdministracion(new DateOnly(2026, 11, 2)));

        reserva.CancelarPorAdministracion("Material no devuelto en condiciones.");

        Assert.Equal(EstadoReservaMaterial.Cancelada, reserva.Estado);
        Assert.Equal("Material no devuelto en condiciones.", reserva.ObservacionAdministracion);
        Assert.Throws<DomainException>(() => reserva.RegistrarRetirada(new DateOnly(2026, 11, 4)));
    }

    private static ReservaMaterial CrearReserva()
        => ReservaMaterial.Crear(
            Guid.NewGuid(),
            1,
            1,
            new DateOnly(2026, 11, 3),
            new DateOnly(2026, 11, 5),
            null,
            new DateTime(2026, 11, 1, 10, 0, 0, DateTimeKind.Utc));
}
