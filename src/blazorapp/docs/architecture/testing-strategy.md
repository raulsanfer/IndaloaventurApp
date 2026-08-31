# Estrategia de testing

Los tests de frontend usan xUnit y bUnit. Estan orientados a comportamiento observable: markup, eventos, rutas, payloads HTTP, errores y estado de servicios.

## Organizacion

Los tests viven en `IndaloaventurApp.Frontend.Tests/Features`:

- `Auth`
- `Navigation`
- `Licenses`
- `Profile`
- `Signals`
- `FoodAlerts`
- `Settings`
- `Admin`
- `Pwa`

`TestDoubles.cs` contiene dobles compartidos como servicios de grabacion, localizer de test, factorias JWT y `StubHttpMessageHandler`.

## Tests de componentes

Los tests bUnit renderizan componentes de `SharedUI`, registran servicios fake y verifican:

- contenido visible;
- estados de carga y error;
- llamadas a servicios;
- navegacion;
- cambios de formulario;
- acciones de usuario.

## Tests de clientes API

Los tests de clientes HTTP verifican:

- metodo HTTP;
- path y query string;
- payload serializado;
- mapeo de respuestas;
- errores funcionales como forbidden, unauthorized, timeout o payload invalido.

## Convenciones

- Agrupa tests por feature, no por tipo tecnico global.
- Usa nombres de test que describan escenario y resultado.
- Documenta cada `[Fact]` con `/// <summary>` cuando se agreguen nuevos tests.
- Mantiene dobles compartidos en `TestDoubles.cs` si aplican a varias features.

## Validacion

Comando principal:

```powershell
dotnet test IndaloaventurApp.Frontend.Tests/IndaloaventurApp.Frontend.Tests.csproj
```
