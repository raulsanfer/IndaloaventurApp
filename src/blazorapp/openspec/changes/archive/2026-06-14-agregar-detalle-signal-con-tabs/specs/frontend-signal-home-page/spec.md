## ADDED Requirements

### Requirement: SignalHome MUST permitir abrir el detalle de cada signal desde el listado
El sistema MUST hacer accionable cada registro mostrado en `SignalHome` para que el usuario pueda navegar al detalle de una signal concreta desde su card, manteniendo una affordance visual clara y consistente con el resto de la interfaz.

#### Scenario: Navegacion al detalle desde una card
- **WHEN** el usuario interactua con una signal del listado
- **THEN** el sistema MUST navegar al detalle de la signal asociada
- **AND** el sistema MUST conservar el identificador correcto del registro seleccionado

#### Scenario: Card navegable con datos parciales
- **WHEN** una signal del listado no disponga de todos los campos visuales opcionales
- **THEN** el sistema MUST seguir permitiendo la navegacion al detalle desde esa card
- **AND** el sistema MUST mantener una superficie de interaccion comprensible
