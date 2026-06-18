## ADDED Requirements

### Requirement: Acceso administrativo a la gestión de Cargos
El sistema MUST ofrecer una vista de gestión de `Cargos` accesible únicamente desde el panel `Administración` para usuarios con rol `Admin`.

#### Scenario: Admin abre la gestión de Cargos
- **WHEN** un usuario con rol `Admin` pulsa la opción `Cargos` en `Configuración`
- **THEN** el sistema MUST navegar a la vista de gestión de `Cargos`

#### Scenario: Usuario no Admin intenta acceder a Cargos desde la UI
- **WHEN** un usuario autenticado sin rol `Admin` usa la aplicación
- **THEN** el sistema MUST no exponer desde la UI accesos operativos a la gestión de `Cargos`

### Requirement: Listado de cargos desde el API
La vista de gestión de `Cargos` MUST obtener y mostrar el catálogo consumiendo `GET /api/cargos` mediante un servicio desacoplado de la UI.

#### Scenario: Carga correcta del catálogo
- **WHEN** un usuario `Admin` entra en la gestión de `Cargos`
- **THEN** el sistema MUST solicitar `GET /api/cargos` y mostrar la colección recibida

### Requirement: Alta de cargos
La gestión de `Cargos` MUST permitir crear un nuevo cargo consumiendo `POST /api/cargos`.

#### Scenario: Creación satisfactoria de cargo
- **WHEN** un usuario `Admin` envía un formulario válido de nuevo cargo
- **THEN** el sistema MUST invocar `POST /api/cargos` y reflejar el nuevo cargo en el listado tras completarse la operación

### Requirement: Edición de cargos
La gestión de `Cargos` MUST permitir actualizar la descripción de un cargo existente consumiendo `PUT /api/cargos/{id}`.

#### Scenario: Actualización satisfactoria de cargo
- **WHEN** un usuario `Admin` confirma cambios válidos sobre un cargo existente
- **THEN** el sistema MUST invocar `PUT /api/cargos/{id}` y actualizar el listado visible tras completarse la operación

### Requirement: Eliminación de cargos
La gestión de `Cargos` MUST permitir eliminar cargos consumiendo `DELETE /api/cargos/{id}` con manejo explícito de errores controlados.

#### Scenario: Eliminación satisfactoria de cargo
- **WHEN** un usuario `Admin` confirma la eliminación de un cargo existente
- **THEN** el sistema MUST invocar `DELETE /api/cargos/{id}` y retirar el cargo del listado visible tras completarse la operación

#### Scenario: Conflicto al eliminar un cargo en uso
- **WHEN** `DELETE /api/cargos/{id}` devuelve un conflicto controlado
- **THEN** el sistema MUST informar del error al usuario y MUST mantener el listado en un estado consistente

### Requirement: Feedback de carga y error en Cargos
La gestión de `Cargos` MUST ofrecer estados de carga, éxito y error controlado para las operaciones del catálogo.

#### Scenario: Error al cargar cargos
- **WHEN** falla la solicitud inicial a `GET /api/cargos`
- **THEN** el sistema MUST mostrar un estado de error controlado con opción de reintento o recuperación equivalente

### Requirement: Alcance limitado al catálogo de cargos
La gestión de `Cargos` MUST limitarse al catálogo administrativo y MUST no incluir en esta iteración asignación de cargos a fichas de socio.

#### Scenario: Revisión funcional del alcance
- **WHEN** se revisa la vista administrativa de `Cargos`
- **THEN** el sistema MUST limitar las acciones disponibles a listar, crear, editar y eliminar cargos del catálogo
