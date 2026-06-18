## ADDED Requirements

### Requirement: La ficha de socio MUST ser la unica fuente de asignacion de cargo
El sistema MUST vincular el cargo del socio a `FichaSocio` y SHALL permitir como maximo un `CargoId` por ficha.

#### Scenario: Ficha con un unico cargo
- **WHEN** se crea o actualiza una `FichaSocio` con `CargoId`
- **THEN** el sistema SHALL persistir una sola referencia de cargo para esa ficha

### Requirement: El sistema MUST validar la existencia del cargo al asignarlo en ficha
El sistema MUST verificar que el `CargoId` indicado en `FichaSocio` exista en el catalogo de cargos antes de persistir.

#### Scenario: Asignacion de cargo existente
- **WHEN** se envia un `CargoId` valido en la operacion de ficha
- **THEN** el sistema SHALL aceptar y persistir la referencia

#### Scenario: Asignacion de cargo inexistente
- **WHEN** se envia un `CargoId` no existente en la operacion de ficha
- **THEN** el sistema SHALL rechazar la operacion con error de validacion o conflicto