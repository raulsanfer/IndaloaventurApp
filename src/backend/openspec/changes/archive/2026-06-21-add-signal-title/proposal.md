## Why

Las `Signal` solo disponen hoy de `Descripcion`, lo que obliga a usar un texto largo tanto como resumen visible como detalle explicativo. Anadir un `Titulo` permite representar mejor cada incidencia en listados y vistas de detalle sin perder la descripcion completa ya existente.

## What Changes

- Anadir el campo `Titulo` al modelo funcional de `Signal`, manteniendo `Descripcion` como contenido complementario.
- Actualizar los flujos de alta y edicion de `Signal` para recibir y validar `Titulo` como parte del payload.
- Exponer `Titulo` en los contratos de lectura que reutilizan `SignalDto`, incluyendo listados y consultas por identificador.
- Persistir la nueva columna en la tabla de `Signals` con una migracion compatible con los registros existentes.

## Capabilities

### New Capabilities

Ninguna.

### Modified Capabilities

- `signal-management`: la creacion y edicion de `Signal` pasan a requerir y almacenar tambien un `Titulo`.
- `signal-search`: las lecturas de `Signal` deben incluir `Titulo` junto a `Descripcion` en el bloque principal de datos devuelto al cliente.

## Impact

- Dominio: `src/IndaloAventurApi.Domain/TrailSignals/Signal.cs`.
- Aplicacion: comandos, validadores y `SignalDto` en `src/IndaloAventurApi.Application/Features/TrailSignals/Signals/`.
- API: contratos HTTP en `src/IndaloAventurApi.Api/Features/TrailSignals/SignalsController.cs`.
- Infraestructura: configuracion EF Core, repositorio y nueva migracion sobre `Signals`.
- Tests: suites unitarias e integracion relacionadas con creacion, edicion, busqueda y detalle de `Signal`.
