using IndaloAventurApi.Domain.Abstractions;
using IndaloAventurApi.Domain.LicenciasFederativas;

namespace IndaloAventurApi.Domain.Tests;

public sealed class FederativeLicenseTests
{
    [Theory]
    [InlineData(0, "Completa")]
    [InlineData(1, " ")]
    public void TipoLicencia_RechazaDatosInvalidos(int id, string nombre)
        => Assert.Throws<DomainException>(() => TipoLicencia.Crear(id, nombre));

    [Theory]
    [InlineData(1, null, null, true)]
    [InlineData(2, null, null, true)]
    [InlineData(3, null, null, false)]
    [InlineData(3, "2026-05-01", null, false)]
    [InlineData(3, null, "2026-05-01", false)]
    [InlineData(3, "2026-05-01", "2026-05-01", true)]
    [InlineData(3, "2026-05-01", "2026-05-03", true)]
    [InlineData(3, "2026-05-03", "2026-05-01", false)]
    [InlineData(3, "2025-12-31", "2026-01-01", false)]
    [InlineData(3, "2026-12-31", "2027-01-01", false)]
    [InlineData(1, "2026-01-01", "2026-12-31", true)]
    [InlineData(2, "2026-07-01", null, false)]
    [InlineData(2, "2026-12-31", "2026-07-01", false)]
    [InlineData(1, "2027-01-01", "2027-12-31", false)]
    public void Cobertura_ValidaParejaOrdenYTemporadaSegunTipo(int tipo, string? inicio, string? fin, bool valido)
    {
        DateOnly? desde = inicio is null ? null : DateOnly.Parse(inicio);
        DateOnly? hasta = fin is null ? null : DateOnly.Parse(fin);
        var tarifa = TarifaLicenciaFederativa.Crear(2026, "A", "Mayores", 45m, null, "Andalucía", tipo);
        if (!valido)
        {
            Assert.Throws<DomainException>(() => SolicitudLicenciaFederativa.Crear(Guid.NewGuid(), 2026, tarifa, desde, hasta));
            return;
        }
        var solicitud = SolicitudLicenciaFederativa.Crear(Guid.NewGuid(), 2026, tarifa, desde, hasta);
        solicitud.Confirmar();
        Assert.Equal(desde, solicitud.FechaInicio);
        Assert.Equal(hasta, solicitud.FechaFin);
    }

    [Fact]
    public void Tarifa_ExigeTipoYValidaPrecioIndependiente()
    {
        Assert.Throws<DomainException>(() => TarifaLicenciaFederativa.Crear(2026, "A", "Mayores", 45m, null, "Andalucía", 0));
        Assert.Throws<DomainException>(() => TarifaLicenciaFederativa.Crear(2026, "A", "Mayores", 45m, -1, "Andalucía", 1));
        Assert.Throws<DomainException>(() => TarifaLicenciaFederativa.Crear(0, "A", "Mayores", 45m, null, "Andalucía", 1));
    }

    [Fact]
    public void TarifaLicenciaFederativa_Crear_WithValidValues_ShouldCreateTarifa()
    {
        var tarifa = TarifaLicenciaFederativa.Crear(
            2026,
            "A NAC",
            "Mayores",
            52m,
            72m,
            "Espana, Andorra, Pirineo Frances, Portugal y Marruecos", TipoLicencia.CompletaId);

        Assert.Equal(0, tarifa.Id);
        Assert.Equal(2026, tarifa.Temporada);
        Assert.Equal("A NAC", tarifa.Licencia);
        Assert.Equal("Mayores", tarifa.Categoria);
        Assert.Equal(52m, tarifa.PrecioClub);
        Assert.Equal(72m, tarifa.PrecioIndependiente);
        Assert.Equal(TipoLicencia.CompletaId, tarifa.TipoLicenciaId);
    }

    [Fact]
    public void TarifaLicenciaFederativa_Crear_WithInvalidPrecioClub_ShouldThrow()
    {
        Assert.Throws<DomainException>(() => TarifaLicenciaFederativa.Crear(2026, "A", "Mayores", 0m, 10m, "Andalucia", TipoLicencia.CompletaId));
    }

    [Fact]
    public void TarifaLicenciaFederativa_Crear_WithExplicitType_ShouldPreserveType()
    {
        var tarifa = TarifaLicenciaFederativa.Crear(2026, "A", "Mayores", 10m, 20m, "Andalucia", TipoLicencia.CompletaId);

        Assert.Equal(TipoLicencia.CompletaId, tarifa.TipoLicenciaId);
    }

    [Fact]
    public void SolicitudLicenciaFederativa_Crear_ShouldStartPending()
    {
        var userId = Guid.NewGuid();
        var tarifa = TarifaLicenciaFederativa.Crear(2026, "A", "Mayores", 45m, 65m, "Andalucia", TipoLicencia.CompletaId);

        var solicitud = SolicitudLicenciaFederativa.Crear(userId, 2026, tarifa);

        Assert.NotEqual(Guid.Empty, solicitud.Id);
        Assert.Equal(userId, solicitud.UserId);
        Assert.Equal(2026, solicitud.Temporada);
        Assert.Equal(EstadoSolicitudLicenciaFederativa.Pendiente, solicitud.Estado);
        Assert.True(solicitud.FechaCreacionUtc <= DateTime.UtcNow);
    }

    [Fact]
    public void SolicitudLicenciaFederativa_Crear_WithDifferentSeasonThanTariff_ShouldThrow()
    {
        var tarifa = TarifaLicenciaFederativa.Crear(2026, "A", "Mayores", 45m, 65m, "Andalucia", TipoLicencia.CompletaId);

        Assert.Throws<DomainException>(() => SolicitudLicenciaFederativa.Crear(Guid.NewGuid(), 2027, tarifa));
    }

    [Fact]
    public void SolicitudLicenciaFederativa_ShouldAllowControlledStateChanges()
    {
        var solicitud = SolicitudLicenciaFederativa.Crear(
            Guid.NewGuid(),
            2026,
            TarifaLicenciaFederativa.Crear(2026, "A", "Mayores", 45m, 65m, "Andalucia", TipoLicencia.CompletaId));

        solicitud.Confirmar();
        Assert.Equal(EstadoSolicitudLicenciaFederativa.Confirmada, solicitud.Estado);

        solicitud.MarcarPendiente();
        Assert.Equal(EstadoSolicitudLicenciaFederativa.Pendiente, solicitud.Estado);

        solicitud.Cancelar();
        Assert.Equal(EstadoSolicitudLicenciaFederativa.Cancelada, solicitud.Estado);
    }
}
