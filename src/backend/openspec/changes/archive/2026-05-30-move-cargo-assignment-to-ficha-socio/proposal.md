## Why

La asignacion de cargos en `User` mezcla identidad/autenticacion con datos funcionales del socio y complica la evolucion del dominio. Necesitamos mover esta relacion a `FichaSocio` para centralizar el perfil del socio en una sola entidad de negocio.

## What Changes

- **BREAKING** Se elimina la relacion de cargo desde el usuario de identidad (`CargoId` en `User`) y sus contratos asociados.
- Se añade la relacion `FichaSocio -> Cargo` (una ficha puede tener un cargo y solo uno).
- Se ajustan casos de uso y endpoints para crear/editar `FichaSocio` incluyendo `CargoId`.
- Se ajusta gestion de usuarios administrados para dejar de recibir o persistir `CargoId`.
- Se agrega migracion de datos/esquema para trasladar o limpiar la referencia de cargo al nuevo modelo.
- Se actualizan pruebas unitarias e integracion para reflejar la nueva fuente de verdad del cargo.

## Capabilities

### New Capabilities
- `ficha-socio-cargo-assignment`: Relacion de cargo gestionada desde `FichaSocio` con validacion de existencia de cargo.

### Modified Capabilities
- `ficha-socio-management`: Se amplian operaciones de ficha para incluir `CargoId` y su validacion.
- `admin-user-management-api`: Se elimina la responsabilidad de asignar cargos desde la gestion de usuarios.

## Impact

- Afecta a `src/IndaloAventurApi.Domain` para modelar la referencia de `Cargo` en `FichaSocio`.
- Afecta a `src/IndaloAventurApi.Application` en comandos/queries/DTOs y validadores de ficha y usuarios.
- Afecta a `src/IndaloAventurApi.Infrastructure` en configuracion EF, repositorios y migraciones.
- Afecta a `src/IndaloAventurApi.Api` en contratos HTTP de ficha socio y gestion de usuarios.
- Afecta a tests de aplicacion e integracion por cambio de contrato y reglas de negocio.