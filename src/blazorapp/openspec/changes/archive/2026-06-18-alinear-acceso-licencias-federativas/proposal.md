## Why

El comportamiento actual de `Licencias Federativas` no deja explícitas ni alineadas las reglas de acceso y alcance por perfil. Hace falta fijar en spec qué puede ver y operar un `Member` no socio, un `Member` socio y un `Admin`, para evitar regresiones entre navegación, carga de datos y edición.

## What Changes

- Aclarar que un usuario con rol `Member` y `IsMember = false` no puede ver ni operar el contenido de `Licencias Federativas`.
- Mantener que un usuario con rol `Member` y `IsMember = true` solo puede consultar sus propias solicitudes y crear solicitudes para sí mismo.
- Añadir un flujo administrativo para que un usuario con rol `Admin` pueda consultar y editar solicitudes de licencias federativas de cualquier usuario, incluidas las suyas.
- Separar en la especificación el alcance de visibilidad, lectura y edición según rol y contexto de usuario objetivo.

## Capabilities

### New Capabilities
- `frontend-admin-federative-license-management`: Gestión administrativa de solicitudes de licencias federativas sobre cualquier usuario.

### Modified Capabilities
- `frontend-member-federative-license-request-flow`: Ajustar las reglas de acceso y alcance del flujo de socio para que un `Member` no socio no tenga acceso y un `Member` socio opere solo sobre sus propias solicitudes.

## Impact

- Componentes y páginas de `Mi Club` y de `Licencias Federativas`.
- Cliente frontend de licencias federativas y contratos de autorización asociados a sesión/roles.
- Posibles nuevas vistas, filtros o acciones para administración sobre solicitudes de terceros.
- Pruebas frontend de visibilidad, acceso operativo y alcance de edición por rol.
