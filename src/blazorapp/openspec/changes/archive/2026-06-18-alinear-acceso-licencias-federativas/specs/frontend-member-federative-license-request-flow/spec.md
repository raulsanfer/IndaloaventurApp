## MODIFIED Requirements

### Requirement: El socio MUST poder iniciar una solicitud desde su pantalla de Licencias Federativas
El sistema MUST permitir que un usuario autenticado con rol `Member` y claim `IsMember = true` abra un popup modal de solicitud desde la pantalla `Licencias Federativas`.

#### Scenario: Apertura del modal desde la acción principal
- **WHEN** un socio visualiza su pantalla de `Licencias Federativas` y pulsa el botón `Solicitar Licencia`
- **THEN** el sistema MUST abrir un popup modal responsive dentro del mismo contexto de pantalla
- **THEN** el sistema MUST mostrar en el modal los campos obligatorios `Temporada`, `Tipología` y `Categoría`

#### Scenario: Usuario no autorizado en el flujo de solicitud
- **WHEN** un usuario con rol `Member` y `IsMember = false`, o un usuario sin rol `Member`, accede a la vista o intenta activar la acción de solicitud
- **THEN** el sistema MUST no ofrecer visualización operativa ni flujo operativo de alta de licencia federativa

### Requirement: La confirmación MUST crear la solicitud y refrescar el listado inmediatamente
El sistema MUST crear la solicitud del socio usando el endpoint correspondiente de alta y MUST recargar el listado de licencias federativas del propio usuario tras una creación satisfactoria.

#### Scenario: Creación correcta de la solicitud
- **WHEN** el usuario confirma una selección válida en el modal
- **THEN** el sistema MUST invocar el endpoint de creación de solicitud con la `Temporada` y el `TarifaLicenciaFederativaId` resuelto
- **THEN** el sistema MUST cerrar el modal al completarse correctamente la operación
- **THEN** el sistema MUST recargar inmediatamente el listado de `Licencias Federativas` del propio usuario

#### Scenario: Nueva solicitud visible tras el refresco
- **WHEN** la creación se completa correctamente y el listado se recarga
- **THEN** el sistema MUST mostrar la nueva solicitud dentro de la temporada correspondiente con la información actualizada del histórico propio

## ADDED Requirements

### Requirement: El flujo de socio MUST quedar limitado a solicitudes propias
El sistema MUST restringir el modo socio de `Licencias Federativas` a la consulta y creación sobre el usuario autenticado, sin exponer lectura ni edición de solicitudes de terceros.

#### Scenario: El listado muestra únicamente solicitudes del propio usuario
- **WHEN** un usuario con rol `Member` y `IsMember = true` entra en `Licencias Federativas`
- **THEN** el sistema MUST cargar únicamente las solicitudes asociadas al usuario autenticado

#### Scenario: El modo socio no expone contexto de otro usuario
- **WHEN** un usuario con rol `Member` y `IsMember = true` utiliza la pantalla de `Licencias Federativas`
- **THEN** el sistema MUST no mostrar selectores, acciones ni navegación para elegir otro usuario objetivo

#### Scenario: El modo socio no permite editar solicitudes existentes
- **WHEN** un usuario con rol `Member` y `IsMember = true` visualiza sus solicitudes ya registradas
- **THEN** el sistema MUST no ofrecer acciones de edición sobre solicitudes existentes
