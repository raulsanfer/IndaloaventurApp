# frontend-admin-users-management Specification

## Purpose
TBD - created by archiving change mejorar-gestion-usuarios-admin-listado-y-estado. Update Purpose after archive.
## Requirements
### Requirement: Opción administrativa Usuarios visible solo para Admin
El sistema MUST mostrar la opción `Usuarios` dentro de `Configuración -> Administración` únicamente para usuarios autenticados con rol `Admin`.

#### Scenario: Admin abre Configuración
- **WHEN** un usuario con rol `Admin` abre `Configuración`
- **THEN** el sistema MUST mostrar la opción `Usuarios` junto al resto de herramientas administrativas

#### Scenario: Usuario no Admin abre Configuración
- **WHEN** un usuario autenticado sin rol `Admin` abre `Configuración`
- **THEN** el sistema MUST no mostrar la opción `Usuarios`

### Requirement: Listado completo inicial de usuarios administrativos
La página administrativa de `Usuarios` MUST cargar el listado completo al abrirse y MUST usar `GET /api/users` sin filtro cuando todavía no se haya indicado un email.

#### Scenario: Admin entra en la página de usuarios
- **WHEN** un administrador navega a `Configuración -> Usuarios`
- **THEN** el sistema MUST invocar `GET /api/users` sin query de email
- **THEN** el sistema MUST mostrar el listado completo recibido como estado inicial de la página

#### Scenario: No hay usuarios que mostrar
- **WHEN** `GET /api/users` sin filtro devuelve una colección vacía
- **THEN** el sistema MUST mostrar un estado vacío controlado sin ocultar la capacidad de filtrado

### Requirement: Filtrado administrativo de usuarios por email
La página administrativa de `Usuarios` MUST permitir refinar el listado por email y MUST consultar `GET /api/users` con el email informado como filtro opcional.

#### Scenario: Admin aplica un filtro por email
- **WHEN** un administrador informa un email y ejecuta la búsqueda
- **THEN** el sistema MUST invocar `GET /api/users` con el email como query string de filtro
- **THEN** el sistema MUST reemplazar el listado visible por los resultados filtrados

#### Scenario: El filtro no encuentra coincidencias
- **WHEN** la búsqueda por email no devuelve usuarios
- **THEN** el sistema MUST mostrar un estado vacío específico para el filtro aplicado

### Requirement: Acciones desde el registro del usuario
Cada registro listado MUST permitir iniciar la operación administrativa correspondiente desde la propia fila del usuario.

#### Scenario: Usuario listado ya es socio
- **WHEN** el usuario listado tiene `IsMember = true`
- **THEN** el sistema MUST mostrar una acción `Editar`
- **THEN** al pulsarla el sistema MUST navegar a la ficha administrativa de ese usuario

#### Scenario: Usuario listado todavía no es socio
- **WHEN** el usuario listado tiene `IsMember = false`
- **THEN** el sistema MUST mostrar una acción `Crear ficha`
- **THEN** al pulsarla el sistema MUST crear la ficha de socio y navegar después a su edición administrativa

### Requirement: Estado operativo visible de la cuenta administrada
La experiencia administrativa de usuarios MUST disponer del estado activo de cada cuenta en el modelo de respuesta consumido por frontend y MUST usarlo para comunicar el estado operativo actual.

#### Scenario: Listado con usuarios activos y desactivados
- **WHEN** `GET /api/users` devuelve registros con estado activo explícito
- **THEN** el sistema MUST mostrar en el listado un indicador coherente con el estado operativo de cada usuario

#### Scenario: Admin abre la ficha de un usuario desactivado
- **WHEN** la ficha administrativa carga un usuario cuyo estado activo indica que está desactivado
- **THEN** el sistema MUST mostrar esa condición antes de presentar acciones de ciclo de vida

### Requirement: Desactivación y reactivación desde la ficha administrativa
La ficha administrativa del usuario MUST permitir desactivar una cuenta activa y reactivar una cuenta desactivada usando los endpoints administrativos existentes.

#### Scenario: Admin desactiva un usuario activo
- **WHEN** un administrador abre la ficha de un usuario activo y pulsa `Desactivar`
- **THEN** el sistema MUST invocar `POST /api/users/{userId}/deactivate`
- **THEN** el sistema MUST refrescar el estado de la ficha y mostrar feedback de éxito controlado

#### Scenario: Admin reactiva un usuario desactivado
- **WHEN** un administrador abre la ficha de un usuario desactivado y pulsa `Reactivar`
- **THEN** el sistema MUST invocar `POST /api/users/{userId}/reactivate`
- **THEN** el sistema MUST refrescar el estado de la ficha y mostrar feedback de éxito controlado

### Requirement: Estados operativos controlados en la gestión de usuarios
La página administrativa de `Usuarios` y la ficha administrativa MUST mostrar estados de carga, éxito y error sin romper la composición principal.

#### Scenario: Error al cargar el listado inicial o filtrado
- **WHEN** falla la carga de usuarios desde `GET /api/users`
- **THEN** el sistema MUST mostrar un estado de error controlado y mantener usable el área de filtros

#### Scenario: Error al desactivar o reactivar un usuario
- **WHEN** falla una operación de desactivación o reactivación
- **THEN** el sistema MUST mantener visible la ficha administrativa y mostrar un mensaje de error controlado

