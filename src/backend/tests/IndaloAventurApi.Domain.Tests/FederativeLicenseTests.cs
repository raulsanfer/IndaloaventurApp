using IndaloAventurApi.Domain.Abstractions;
using IndaloAventurApi.Domain.LicenciasFederativas;

namespace IndaloAventurApi.Domain.Tests;

public sealed class FederativeLicenseTests
{
    [Fact]
    public void TarifaLicenciaFederativa_Crear_WithValidValues_ShouldCreateTarifa()
    {
        var tarifa = TarifaLicenciaFederativa.Crear(
            2026,
            "A NAC",
            "Mayores",
            52m,
            72m,
            "Espana, Andorra, Pirineo Frances, Portugal y Marruecos");

        Assert.Equal(0, tarifa.Id);
        Assert.Equal(2026, tarifa.Temporada);
        Assert.Equal("A NAC", tarifa.Licencia);
        Assert.Equal("Mayores", tarifa.Categoria);
        Assert.Equal(52m, tarifa.PrecioClub);
        Assert.Equal(72m, tarifa.PrecioIndependiente);
        Assert.False(tarifa.MediaTemporada);
    }

    [Fact]
    public void TarifaLicenciaFederativa_Crear_WithInvalidPrecioClub_ShouldThrow()
    {
        Assert.Throws<DomainException>(() => TarifaLicenciaFederativa.Crear(2026, "A", "Mayores", 0m, 10m, "Andalucia"));
    }

    [Fact]
    public void TarifaLicenciaFederativa_Crear_WithoutMediaTemporada_ShouldDefaultToFalse()
    {
        var tarifa = TarifaLicenciaFederativa.Crear(2026, "A", "Mayores", 10m, 20m, "Andalucia");

        Assert.False(tarifa.MediaTemporada);
    }

    [Fact]
    public void SolicitudLicenciaFederativa_Crear_ShouldStartPending()
    {
        var userId = Guid.NewGuid();
        var tarifa = TarifaLicenciaFederativa.Crear(2026, "A", "Mayores", 45m, 65m, "Andalucia");

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
        var tarifa = TarifaLicenciaFederativa.Crear(2026, "A", "Mayores", 45m, 65m, "Andalucia");

        Assert.Throws<DomainException>(() => SolicitudLicenciaFederativa.Crear(Guid.NewGuid(), 2027, tarifa));
    }

    [Fact]
    public void SolicitudLicenciaFederativa_ShouldAllowControlledStateChanges()
    {
        var solicitud = SolicitudLicenciaFederativa.Crear(
            Guid.NewGuid(),
            2026,
            TarifaLicenciaFederativa.Crear(2026, "A", "Mayores", 45m, 65m, "Andalucia"));

        solicitud.Confirmar();
        Assert.Equal(EstadoSolicitudLicenciaFederativa.Confirmada, solicitud.Estado);

        solicitud.MarcarPendiente();
        Assert.Equal(EstadoSolicitudLicenciaFederativa.Pendiente, solicitud.Estado);

        solicitud.Cancelar();
        Assert.Equal(EstadoSolicitudLicenciaFederativa.Cancelada, solicitud.Estado);
    }
}
