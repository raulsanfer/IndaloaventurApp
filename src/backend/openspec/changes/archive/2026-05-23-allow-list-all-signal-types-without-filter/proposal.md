## Why

Actualmente la API de `signal-types` solo expone casos de uso de gestion (crear, editar, eliminar) para rol `Admin`, pero no ofrece un caso de uso de consulta para recuperar el catalogo completo de tipos de senal. Esto bloquea escenarios del cliente movil que necesitan cargar y mostrar todos los tipos disponibles sin depender de filtros de busqueda.

## What Changes

- Anadir un caso de uso de consulta para listar todos los `SignalType` sin aplicar filtros de busqueda.
- Exponer un endpoint `GET /api/signal-types` para recuperar el catalogo completo de tipos.
- Permitir acceso a la consulta a usuarios autenticados (independiente del rol), manteniendo las operaciones de alta/edicion/eliminacion restringidas a `Admin`.
- Incorporar pruebas unitarias e integracion para validar la consulta sin filtros y sus reglas de autorizacion.

## Capabilities

### New Capabilities
- `signal-type-query`: Consulta del catalogo de tipos de senal sin filtros para usuarios autenticados.

### Modified Capabilities
- Ninguna.

## Impact

- API: `SignalTypesController` con nuevo endpoint `GET /api/signal-types`.
- Aplicacion: nuevo `Query` y `QueryHandler` CQRS para listar `SignalType`.
- Infraestructura: ampliacion de `ISignalTypeRepository` y su implementacion para obtener todos los registros.
- Seguridad: ajuste de autorizacion para permitir lectura autenticada sin romper restricciones `Admin` en mutaciones.
- Tests: nuevas pruebas unitarias de handler y de integracion de endpoint.