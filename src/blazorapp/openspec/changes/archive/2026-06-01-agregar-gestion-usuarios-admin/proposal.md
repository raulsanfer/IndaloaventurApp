## Why

La pantalla `Configuración` ya agrupa herramientas administrativas, pero hoy no ofrece una forma operativa de localizar usuarios y convertir a no socios en fichas de socio editables desde el frontend. Incorporar ahora una utilidad `Usuarios` completa este flujo administrativo y conecta la gestión de cuentas con la creación y edición de fichas del club.

## What Changes

- Añadir una nueva opción `Usuarios` dentro de `Configuración -> Administración`, visible solo para usuarios con rol `Admin`.
- Crear una nueva página administrativa de `Usuarios` con un buscador por email y un listado de resultados.
- Consumir `GET /api/users` para localizar usuarios por email, bajo la asunción de que el endpoint soporta esa búsqueda o un filtro equivalente.
- Mostrar para cada resultado una acción `Editar` cuando `IsMember = true`, redirigiendo a la ficha administrativa de ese socio.
- Mostrar para cada resultado una acción `Crear Ficha` cuando `IsMember = false`, creando automáticamente una nueva ficha con al menos el email del usuario y redirigiendo después a la ficha administrativa para continuar la edición.
- Añadir la capacidad administrativa de cargar, crear y editar fichas de socio de terceros usando `GET/POST/PUT /api/fichas-socio/{userId}`.
- Mantener la coherencia visual del área de administración con breadcrumb, títulos, listado usable y estados de carga/error.

## Capabilities

### New Capabilities
- `frontend-admin-users-management`: Define la búsqueda administrativa de usuarios, la detección de si ya son socios y la creación o edición de su ficha de socio.
- `frontend-admin-member-file-page`: Define la edición administrativa de la ficha de socio de un usuario concreto a partir de su `userId`.

### Modified Capabilities
- `frontend-settings-admin-page`: Amplía el bloque `Administración` para incluir la nueva opción `Usuarios` junto a las herramientas administrativas existentes.

## Impact

- Afecta a `IndaloaventurApp.SharedUI` con nuevos componentes de administración de usuarios y ficha de socio administrativa.
- Afecta a `IndaloaventurApp.Web.Client` con nuevas rutas/páginas admin, clientes API o extensiones de servicios para `users` y `fichas-socio/{userId}`.
- Afecta a recursos localizados ES y SCSS global del área `Configuración` y de formularios administrativos.
- Requiere confirmar o documentar el mecanismo exacto de búsqueda por email en `GET /api/users`, ya que el `endpoints.json` actual no publica parámetros de query.
