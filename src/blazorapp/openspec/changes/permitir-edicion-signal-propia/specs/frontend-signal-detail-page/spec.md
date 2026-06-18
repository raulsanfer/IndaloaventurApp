## ADDED Requirements

### Requirement: La pagina de detalle MUST mostrar acceso de edicion solo para la signal propia
La pagina de detalle MUST mostrar una accion visible de `Editar` unicamente cuando la signal cargada pertenezca al usuario autenticado y MUST ocultarla para cualquier signal ajena.

#### Scenario: Propietario con accion de edicion disponible
- **WHEN** un usuario autenticado visualiza el detalle de una signal cuya autoria coincide con su propia sesion
- **THEN** el sistema MUST mostrar una accion visible de `Editar` dentro de la experiencia de detalle
- **AND** el sistema MUST mantener esa accion asociada a la signal concreta que se esta consultando

#### Scenario: Usuario no propietario sin accion de edicion
- **WHEN** un usuario autenticado visualiza el detalle de una signal creada por otra persona
- **THEN** el sistema MUST no mostrar la accion `Editar`
- **AND** el sistema MUST mantener el resto del detalle en modo de solo lectura
