## Why

Actualmente `IsMember` nace a `false` y solo cambia si un administrador lo actualiza manualmente desde la gestion de usuarios. Eso deja una inconsistencia con el flujo de alta de socio, porque la creacion de una `FichaSocio` activa no marca al usuario como socio aunque esa ficha sea precisamente la evidencia operativa del alta.

## What Changes

- Modificar el flujo de creacion administrativa de `FichaSocio` para que marque `IsMember = true` en el usuario vinculado cuando la ficha se cree correctamente.
- Alinear las reglas de identidad para que `IsMember` refleje la existencia de una ficha de socio activa en lugar de depender de una accion manual independiente.
- Añadir cobertura automatizada para verificar la sincronizacion entre la creacion de ficha, las consultas de usuario y los datos consumidos por autenticacion/autorizacion.

## Capabilities

### New Capabilities

### Modified Capabilities

- `ficha-socio-management`: la creacion de una ficha de socio debe activar la condicion de socio del usuario vinculado.
- `identity-user-lifecycle-management`: `IsMember` debe mantenerse sincronizado con el alta efectiva como socio al crear la ficha.

## Impact

- Afecta al flujo CQRS de `CreateFichaSocio`.
- Afecta al servicio de identidad y a la persistencia del campo `Usuario.IsMember`.
- Puede impactar respuestas de gestion de usuarios, claims JWT y validaciones funcionales que dependen de `IsMember`.
