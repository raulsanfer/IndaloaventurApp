## Why

La edición administrativa de la ficha de usuario ya permite asignar cargos, pero no deja claro ni formaliza la posibilidad de quitar un cargo existente y guardarlo como `null`. Resolverlo ahora evita que un usuario mantenga un cargo obsoleto cuando ya no tiene ninguno activo.

## What Changes

- Añadir una opción vacía explícita en el selector administrativo de `Cargo` para representar “sin cargo”.
- Permitir que un administrador guarde la ficha con `CargoId = null` cuando el usuario ya no tenga cargo activo.
- Alinear el comportamiento visual del combo y el payload enviado con la semántica de campo opcional.

## Capabilities

### New Capabilities

### Modified Capabilities
- `frontend-member-file-cargo-selection`: El selector administrativo de cargo debe permitir limpiar la selección y persistir el valor nulo.

## Impact

- `IndaloaventurApp.SharedUI`: `AdminMemberProfileView`, `MemberCargoFieldComponentBase` y posibles literales del selector.
- Tests frontend de edición administrativa de ficha.
- No requiere nuevos endpoints; reutiliza el contrato actual de guardado de ficha con `CargoId` nullable.
