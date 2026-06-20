# frontend-admin-federative-license-management Specification

## Purpose
TBD - created by archiving change alinear-acceso-licencias-federativas. Update Purpose after archive.
## Requirements
### Requirement: El administrador MUST poder acceder al modo administrativo de licencias federativas
El sistema MUST permitir que un usuario autenticado con rol `Admin` acceda a una funcionalidad de gestión de solicitudes de licencias federativas independientemente del valor de `IsMember`.

#### Scenario: Admin con IsMember false accede a la gestión
- **WHEN** un usuario autenticado con rol `Admin` y `IsMember = false` entra en la funcionalidad administrativa de licencias federativas
- **THEN** el sistema MUST permitir el acceso operativo a la gestión

#### Scenario: Admin con IsMember true accede a la gestión
- **WHEN** un usuario autenticado con rol `Admin` y `IsMember = true` entra en la funcionalidad administrativa de licencias federativas
- **THEN** el sistema MUST permitir el acceso operativo a la gestión

### Requirement: El administrador MUST poder consultar solicitudes de cualquier usuario
El sistema MUST permitir que un usuario con rol `Admin` consulte las solicitudes de licencias federativas de cualquier usuario, incluidas las suyas.

#### Scenario: Consulta de solicitudes de un tercero
- **WHEN** un usuario con rol `Admin` selecciona o abre el contexto de otro usuario
- **THEN** el sistema MUST cargar y mostrar las solicitudes asociadas a ese usuario objetivo

#### Scenario: Consulta de solicitudes propias desde el modo administrativo
- **WHEN** un usuario con rol `Admin` abre el contexto de su propio usuario
- **THEN** el sistema MUST cargar y mostrar sus propias solicitudes dentro del mismo flujo administrativo

### Requirement: El administrador MUST poder editar solicitudes de cualquier usuario
El sistema MUST permitir que un usuario con rol `Admin` edite solicitudes de licencias federativas de cualquier usuario dentro de los campos autorizados por el backend, incluidas las suyas.

#### Scenario: Edición de una solicitud de un tercero
- **WHEN** un usuario con rol `Admin` modifica una solicitud perteneciente a otro usuario y confirma los cambios
- **THEN** el sistema MUST enviar la actualización sobre la solicitud del usuario objetivo
- **THEN** el sistema MUST reflejar el resultado actualizado en la vista administrativa

#### Scenario: Edición de una solicitud propia desde el modo administrativo
- **WHEN** un usuario con rol `Admin` modifica una solicitud perteneciente a su propio usuario y confirma los cambios
- **THEN** el sistema MUST persistir la actualización y mostrar el estado actualizado dentro del flujo administrativo

