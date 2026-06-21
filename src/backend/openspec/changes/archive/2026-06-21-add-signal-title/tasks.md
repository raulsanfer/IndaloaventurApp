## 1. Modelo y persistencia

- [x] 1.1 Extender el agregado `Signal` para incluir `Titulo` en creacion, edicion y validaciones sin alterar el resto de reglas existentes.
- [x] 1.2 Actualizar configuracion EF Core y generar una migracion que anada la columna `Titulo` a `Signals`, incluyendo backfill desde `Descripcion` para los registros ya existentes.

## 2. Contratos de aplicacion y API

- [x] 2.1 Actualizar comandos, validadores y handlers de create/update de `Signal` para recibir y propagar `Titulo`.
- [x] 2.2 Actualizar `SignalDto`, query handlers y contratos HTTP de `SignalsController` para devolver `Titulo` en busqueda y detalle.
- [x] 2.3 Revisar repositorios y consultas para asegurar que las lecturas de `Signal` materializan correctamente el nuevo campo.

## 3. Verificacion

- [x] 3.1 Adaptar y ampliar los tests unitarios de `TrailSignals` para cubrir `Titulo` en creacion, edicion y lectura.
- [x] 3.2 Adaptar los tests de integracion de `Signals` para validar el nuevo payload y el comportamiento de compatibilidad tras la migracion.
- [x] 3.3 Ejecutar al menos `dotnet test tests\\IndaloAventurApi.Application.Tests\\IndaloAventurApi.Application.Tests.csproj` y `dotnet test tests\\IndaloAventurApi.IntegrationTests\\IndaloAventurApi.IntegrationTests.csproj` antes de marcar las tareas como completadas.
