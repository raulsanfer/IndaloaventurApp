using IndaloAventurApi.Domain.Abstractions;
using IndaloAventurApi.Domain.ClubPositions;
using IndaloAventurApi.Domain.TrailSignals;

namespace IndaloAventurApi.Domain.Tests;

public sealed class ClubPositionAndSignalTypeTests
{
    [Fact]
    public void Cargo_Crear_ShouldTrimDescripcion_AndLeaveIdForPersistence()
    {
        var cargo = Cargo.Crear("  Presidente  ");

        Assert.Equal(0, cargo.Id);
        Assert.Equal("Presidente", cargo.Descripcion);
    }

    [Fact]
    public void Cargo_Crear_WithEmptyDescripcion_ShouldThrow()
    {
        Assert.Throws<DomainException>(() => Cargo.Crear(" "));
    }

    [Fact]
    public void SignalType_Crear_ShouldTrimFields_AndLeaveIdForPersistence()
    {
        var signalType = SignalType.Crear("  Aviso  ", "  aviso  ");

        Assert.Equal(0, signalType.Id);
        Assert.Equal("Aviso", signalType.Nombre);
        Assert.Equal("aviso", signalType.Icono);
    }

    [Fact]
    public void SignalType_Crear_WithInvalidData_ShouldThrow()
    {
        Assert.Throws<DomainException>(() => SignalType.Crear("", "icono"));
        Assert.Throws<DomainException>(() => SignalType.Crear("Aviso", " "));
    }
}
