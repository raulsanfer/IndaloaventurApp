## Why

La gestión administrativa de usuarios ya existe, pero hoy obliga a empezar siempre por una búsqueda exacta por email y no da visibilidad del conjunto de usuarios disponibles. Además, una vez dentro de la ficha administrativa, el frontend no permite operar el ciclo de vida de la cuenta aunque el backend ya soporta desactivación y reactivación.

## What Changes

- Cargar por defecto el listado completo de usuarios al abrir `Configuración -> Usuarios`, manteniendo la búsqueda por email como filtro opcional.
- Permitir iniciar la edición de un usuario directamente desde su fila del listado, conservando el flujo actual de `Editar` o `Crear ficha` según `IsMember`.
- Mostrar el estado operativo de cada usuario en la experiencia administrativa usando un dato explícito del modelo de respuesta.
- Añadir en la ficha administrativa del usuario una acción para desactivar o reactivar la cuenta según su estado actual.
- Alinear los servicios frontend y el contrato consumido desde `GET /api/users` con esta necesidad de estado administrativo visible.

## Capabilities

### New Capabilities
- `frontend-admin-users-management`: Gestión administrativa de usuarios con listado inicial completo, filtrado por email, edición desde registro y control de activación de cuentas.

### Modified Capabilities

## Impact

- `IndaloaventurApp.SharedUI`: vistas `AdminUsersManagementView` y `AdminMemberProfileView`, nuevos estados visuales y acciones administrativas.
- `IndaloaventurApp.Web.Client`: `IAdminUserManagementService` y `AdminUserManagementApiClient` para soportar listado sin filtro, estado activo e invocaciones a desactivar/reactivar.
- Contrato API consumido por frontend: `ManagedUserDto` deberá exponer el estado operativo del usuario para evitar heurísticas en cliente.
- Tests frontend de administración de usuarios y ficha administrativa.
