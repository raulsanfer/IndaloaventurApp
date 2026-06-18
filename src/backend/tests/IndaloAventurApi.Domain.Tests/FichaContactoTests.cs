using IndaloAventurApi.Domain.Abstractions;
using IndaloAventurApi.Domain.FichasContacto;

namespace IndaloAventurApi.Domain.Tests;

public sealed class FichaContactoTests
{
    [Fact]
    public void Crear_WithValidValues_ShouldCreateFicha()
    {
        var ficha = FichaContacto.Crear("Refugio Poqueira", "+34 600 123 456", null, "refugio@club.test", "Calle Refugio 1", "Solo llamadas en horario de tarde");

        Assert.NotEqual(Guid.Empty, ficha.Id);
        Assert.Equal("Refugio Poqueira", ficha.Nombre.Value);
        Assert.Equal("+34 600 123 456", ficha.Telefono1.Value);
        Assert.Null(ficha.Telefono2);
        Assert.Equal("refugio@club.test", ficha.Email?.Value);
        Assert.Equal("Calle Refugio 1", ficha.Direccion?.Value);
        Assert.Equal("Solo llamadas en horario de tarde", ficha.Observaciones?.Value);
    }

    [Fact]
    public void Crear_WithInvalidNombre_ShouldThrowDomainException()
    {
        var action = () => FichaContacto.Crear(" ", "+34 600123456", null, null, null, null);

        Assert.Throws<DomainException>(action);
    }

    [Fact]
    public void Crear_WithInvalidTelefono1_ShouldThrowDomainException()
    {
        var action = () => FichaContacto.Crear("Contacto", "abc", null, null, null, null);

        Assert.Throws<DomainException>(action);
    }

    [Fact]
    public void Crear_WithInvalidTelefono2_ShouldThrowDomainException()
    {
        var action = () => FichaContacto.Crear("Contacto", "+34 600123456", "12", null, null, null);

        Assert.Throws<DomainException>(action);
    }

    [Fact]
    public void Crear_WithTooLongObservaciones_ShouldThrowDomainException()
    {
        var tooLongObservaciones = new string('a', 501);

        var action = () => FichaContacto.Crear("Contacto", "+34 600123456", null, null, null, tooLongObservaciones);

        Assert.Throws<DomainException>(action);
    }

    [Fact]
    public void Crear_WithInvalidEmail_ShouldThrowDomainException()
    {
        var action = () => FichaContacto.Crear("Contacto", "+34 600123456", null, "correo-invalido", null, null);

        Assert.Throws<DomainException>(action);
    }

    [Fact]
    public void Actualizar_WithValidValues_ShouldUpdateMutableFields()
    {
        var ficha = FichaContacto.Crear("Contacto", "+34 600123456", null, null, null, null);
        var id = ficha.Id;
        var fechaAlta = ficha.FechaAlta;

        ficha.Actualizar("Contacto actualizado", "+34 699000111", "+34 699000222", "actualizado@club.test", "Avenida Principal 10", "Observaciones actualizadas");

        Assert.Equal(id, ficha.Id);
        Assert.Equal(fechaAlta, ficha.FechaAlta);
        Assert.Equal("Contacto actualizado", ficha.Nombre.Value);
        Assert.Equal("+34 699000111", ficha.Telefono1.Value);
        Assert.Equal("+34 699000222", ficha.Telefono2?.Value);
        Assert.Equal("actualizado@club.test", ficha.Email?.Value);
        Assert.Equal("Avenida Principal 10", ficha.Direccion?.Value);
        Assert.Equal("Observaciones actualizadas", ficha.Observaciones?.Value);
    }
}
