## ADDED Requirements

### Requirement: Opción administrativa Usuarios
El sistema MUST mostrar una opción `Usuarios` dentro de `Configuración -> Administración` únicamente para usuarios con rol `Admin`.

#### Scenario: Admin abre Configuración
- **WHEN** un usuario con rol `Admin` abre `Configuración`
- **THEN** el sistema MUST mostrar la opción `Usuarios` junto al resto de herramientas administrativas disponibles

#### Scenario: Usuario no Admin abre Configuración
- **WHEN** un usuario autenticado sin rol `Admin` abre `Configuración`
- **THEN** el sistema MUST no mostrar la opción `Usuarios`

### Requirement: Búsqueda administrativa de usuarios por email
La página administrativa de `Usuarios` MUST ofrecer un buscador por email y MUST consultar `GET /api/users` para localizar usuarios.

#### Scenario: Admin realiza una búsqueda
- **WHEN** un administrador introduce un email y ejecuta la búsqueda
- **THEN** el sistema MUST consultar el endpoint `GET /api/users` usando el email como criterio de búsqueda o filtro equivalente
- **THEN** el sistema MUST mostrar en el listado el usuario encontrado cuando exista

#### Scenario: No hay resultados
- **WHEN** la búsqueda no encuentra usuarios coincidentes
- **THEN** el sistema MUST mostrar un estado vacío o mensaje equivalente en el listado

### Requirement: Acciones según estado de socio
Cada usuario encontrado MUST mostrar una acción distinta según su propiedad `IsMember`.

#### Scenario: Usuario encontrado ya es socio
- **WHEN** el usuario encontrado tiene `IsMember = true`
- **THEN** el sistema MUST mostrar una acción `Editar`
- **THEN** al pulsarla el sistema MUST navegar a la ficha administrativa de ese socio

#### Scenario: Usuario encontrado todavía no es socio
- **WHEN** el usuario encontrado tiene `IsMember = false`
- **THEN** el sistema MUST mostrar una acción `Crear Ficha`

### Requirement: Creación automática de ficha de socio desde Usuarios
La acción `Crear Ficha` MUST crear automáticamente una nueva ficha de socio para el usuario seleccionado y MUST redirigir después a su edición administrativa.

#### Scenario: Admin crea ficha para usuario no socio
- **WHEN** un administrador pulsa `Crear Ficha` sobre un usuario con `IsMember = false`
- **THEN** el sistema MUST invocar `POST /api/fichas-socio/{userId}`
- **THEN** el sistema MUST enviar al menos el email del usuario como parte del payload inicial
- **THEN** el sistema MUST navegar a la ficha administrativa del socio recién creado

### Requirement: Estados operativos del listado de usuarios
La página administrativa de `Usuarios` MUST mostrar estados de carga, éxito y error sin romper la composición principal.

#### Scenario: Error al buscar usuarios
- **WHEN** falla la búsqueda de usuarios
- **THEN** el sistema MUST mostrar un estado de error controlado y mantener usable el buscador
