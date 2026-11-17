## MODIFIED Requirements

### Requirement: El administrador MUST poder consultar solicitudes de cualquier usuario
El sistema MUST permitir que un usuario con rol `Admin` consulte solicitudes de cualquier usuario, incluidas las suyas, mostrando temporada, nombre de tipo y cobertura informada, con identidad por solicitud.

#### Scenario: Consulta de solicitudes de un tercero
- **WHEN** un Admin abre el contexto de otro usuario con varias solicitudes de una temporada
- **THEN** el sistema MUST mostrar todas las solicitudes con sus identificadores, tipo y cobertura sin colapsarlas por temporada

#### Scenario: Consulta de solicitudes propias desde el modo administrativo
- **WHEN** un Admin abre su propio contexto
- **THEN** el sistema MUST mostrar sus solicitudes con tipo y cobertura dentro del flujo administrativo

#### Scenario: Cobertura nula de registros migrados
- **WHEN** una solicitud administrativa tiene fechas nulas
- **THEN** el sistema MUST conservarla visible sin inventar fechas

### Requirement: El administrador MUST poder editar solicitudes de cualquier usuario
El sistema MUST permitir que Admin actualice únicamente el estado autorizado de solicitudes propias o ajenas por usuario e identificador de solicitud, conservando tipo, tarifa, propietario, creación y cobertura.

#### Scenario: Edición de una solicitud de un tercero
- **WHEN** un Admin cambia el estado de una solicitud de otro usuario entre varias de la misma temporada
- **THEN** el sistema MUST enviar únicamente el estado a `PUT /api/licencias-federativas/admin/users/{userId}/solicitudes/{solicitudId}/estado` y reflejar la respuesta en la solicitud concreta
- **AND** MUST conservar tipo, tarifa, propietario, creación y cobertura, sin alterar otras solicitudes ni ofrecer edición de estos datos

#### Scenario: Edición de una solicitud propia desde el modo administrativo
- **WHEN** un Admin modifica el estado de una solicitud propia
- **THEN** el sistema MUST actualizar solo su estado y mostrar tipo y cobertura conservados en la respuesta
