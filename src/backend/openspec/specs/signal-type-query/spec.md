# signal-type-query Specification

## Purpose
TBD - created by archiving change allow-list-all-signal-types-without-filter. Update Purpose after archive.
## Requirements
### Requirement: Consultar catalogo completo de tipos de signal
El sistema MUST permitir que cualquier usuario autenticado consulte el catalogo completo de `SignalType` sin aplicar filtros de busqueda.

#### Scenario: Consulta sin filtros devuelve todos los tipos
- **WHEN** un usuario autenticado invoca la consulta de `signal-types` sin parametros
- **THEN** el sistema devuelve la coleccion completa de `SignalType` persistidos con `Id`, `Nombre` e `Icono`

#### Scenario: Consulta sin datos devuelve coleccion vacia
- **WHEN** un usuario autenticado invoca la consulta y no existen registros de `SignalType`
- **THEN** el sistema devuelve una coleccion vacia y respuesta exitosa

### Requirement: Autorizar lectura de tipos para cualquier usuario autenticado
El sistema MUST permitir el acceso al endpoint de lectura de `SignalType` a cualquier usuario autenticado, manteniendo restringidas a rol `Admin` las operaciones de alta, edicion y eliminacion.

#### Scenario: Usuario autenticado sin rol Admin puede leer tipos
- **WHEN** un usuario autenticado con rol distinto de `Admin` solicita `GET /api/signal-types`
- **THEN** el sistema autoriza la peticion y responde con el catalogo de tipos

#### Scenario: Usuario no autenticado no puede leer tipos
- **WHEN** un usuario no autenticado solicita `GET /api/signal-types`
- **THEN** el sistema rechaza la peticion por falta de autenticacion

