# frontend-admin-signal-categories-management Specification

## Purpose
TBD - created by archiving change agregar-gestion-categorias-signals-admin. Update Purpose after archive.
## Requirements
### Requirement: La opción administrativa Signals MUST estar disponible solo para Admin dentro de Configuración
El sistema MUST mostrar una entrada administrativa `Signals` dentro de `Mi Cuenta -> Configuración` únicamente para usuarios autenticados con rol `Admin` y MUST ocultarla al resto de usuarios.

#### Scenario: Admin abre Configuración
- **WHEN** un usuario con rol `Admin` accede a `Configuración`
- **THEN** el sistema MUST mostrar una opción administrativa `Signals`
- **AND** el sistema MUST mantener visible el resto de herramientas administrativas existentes

#### Scenario: Usuario no Admin abre Configuración
- **WHEN** un usuario autenticado sin rol `Admin` accede a `Configuración`
- **THEN** el sistema MUST no mostrar la opción `Signals`

### Requirement: La zona administrativa Signals MUST actuar como contenedor de Categorías
El sistema MUST ofrecer una superficie administrativa `Signals` bajo `Configuración` y MUST incluir dentro de ella una entrada visible a `Categorías` como primera herramienta de gestión del dominio.

#### Scenario: Admin navega a la zona Signals
- **WHEN** un administrador pulsa la opción `Signals` desde `Configuración`
- **THEN** el sistema MUST navegar a una pantalla administrativa específica de `Signals`
- **AND** el sistema MUST mostrar una entrada accionable `Categorías`
- **AND** el sistema MUST mantener breadcrumb coherente con `Mi Cuenta -> Configuración -> Signals`

### Requirement: La gestión de Categorías MUST listar todas las categorías de Signals
El sistema MUST ofrecer una pantalla administrativa `Categorías` que cargue el listado completo de `SignalTypes` consumiendo `GET /api/signal-types`.

#### Scenario: Carga inicial de categorías
- **WHEN** un administrador accede a `Signals -> Categorías`
- **THEN** el sistema MUST invocar `GET /api/signal-types`
- **AND** el sistema MUST mostrar el listado completo recibido
- **AND** el sistema MUST renderizar para cada categoría al menos su nombre e icono textual

#### Scenario: No hay categorías disponibles
- **WHEN** `GET /api/signal-types` devuelve una colección vacía
- **THEN** el sistema MUST mostrar un estado vacío comprensible
- **AND** el sistema MUST mantener accesible la capacidad de crear una categoría nueva

### Requirement: El administrador MUST poder crear categorías con nombre e icono
El sistema MUST permitir crear una categoría nueva desde la pantalla de `Categorías` usando los campos `Nombre` e `Icono` y MUST persistirla mediante `POST /api/signal-types`.

#### Scenario: Creación correcta de categoría
- **WHEN** un administrador informa un nombre y un icono válidos y guarda una nueva categoría
- **THEN** el sistema MUST invocar `POST /api/signal-types`
- **AND** el sistema MUST refrescar o actualizar el listado con la nueva categoría
- **AND** el sistema MUST mostrar feedback de éxito controlado

#### Scenario: Validación de nombre obligatorio
- **WHEN** un administrador intenta crear una categoría sin nombre
- **THEN** el sistema MUST bloquear el guardado
- **AND** el sistema MUST mostrar una validación comprensible indicando que el nombre es obligatorio

### Requirement: El administrador MUST poder editar categorías existentes
El sistema MUST permitir seleccionar una categoría existente para editar sus campos `Nombre` e `Icono` y MUST persistir los cambios mediante `PUT /api/signal-types/{id}`.

#### Scenario: Edición correcta de categoría
- **WHEN** un administrador edita una categoría existente y confirma el guardado
- **THEN** el sistema MUST invocar `PUT /api/signal-types/{id}`
- **AND** el sistema MUST reflejar los cambios en el listado
- **AND** el sistema MUST mostrar feedback de éxito controlado

### Requirement: El administrador MUST poder eliminar categorías solo cuando la operación sea válida
El sistema MUST permitir solicitar el borrado de una categoría existente mediante `DELETE /api/signal-types/{id}` y MUST impedir que la operación se considere completada cuando la categoría no pueda eliminarse, incluyendo el caso de categorías con señales asignadas.

#### Scenario: Borrado correcto de categoría
- **WHEN** un administrador confirma el borrado de una categoría que el backend permite eliminar
- **THEN** el sistema MUST invocar `DELETE /api/signal-types/{id}`
- **AND** el sistema MUST retirar la categoría del listado visible
- **AND** el sistema MUST mostrar feedback de éxito controlado

#### Scenario: Categoría no eliminable por uso existente
- **WHEN** el backend rechaza el borrado de una categoría porque no puede eliminarse
- **THEN** el sistema MUST mantener visible la categoría en el listado
- **AND** el sistema MUST mostrar un mensaje de error administrativo comprensible
- **AND** el sistema MUST no dejar la pantalla en un estado inconsistente

### Requirement: La gestión administrativa de Categorías MUST manejar estados de carga y error
La pantalla administrativa de `Categorías` MUST informar de forma controlada los estados de carga, error y reintento tanto en la carga inicial como en las operaciones de guardado o borrado.

#### Scenario: Error al cargar categorías
- **WHEN** falla la recuperación de categorías desde `GET /api/signal-types`
- **THEN** el sistema MUST mostrar un estado de error controlado
- **AND** el sistema MUST ofrecer una acción de reintento

#### Scenario: Error al crear, editar o borrar una categoría
- **WHEN** falla una operación de creación, edición o borrado
- **THEN** el sistema MUST mantener visible la pantalla administrativa
- **AND** el sistema MUST mostrar un mensaje de error comprensible sin perder silenciosamente el contexto de edición actual

