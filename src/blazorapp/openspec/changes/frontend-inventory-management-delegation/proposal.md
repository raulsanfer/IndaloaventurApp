## Why

La gestión de inventario y las reservas administrativas siguen presentándose en el frontend como facultades exclusivas de `Admin`, aunque el backend ya admite la delegación segura a socios con el rol `InventoryManager`. La aplicación debe reflejar esa regla sin ampliar otros permisos administrativos ni confiar en la interfaz como frontera de seguridad.

## What Changes

- Incorporar una capacidad de sesión reutilizable que exprese `Admin OR (InventoryManager AND IsMember)` y usarla para navegar, mostrar acciones y evitar peticiones administrativas de inventario no autorizadas.
- Habilitar con esa capacidad las rutas, enlaces y operaciones de escritura de categorías, estados, materiales y reservas administrativas; la administración de usuarios permanecerá exclusiva de `Admin`.
- Extender la gestión administrativa de usuarios para que un administrador pueda conceder o retirar `InventoryManager` a un socio mediante el `PUT /api/users/{userId}` existente, preservando roles ajenos a este permiso y normalizando el caso heredado de rol asignado sin condición de socio.
- Gestionar la revocación de tokens originada por cambios de rol o membresía: comunicar el nuevo inicio de sesión requerido, cerrar la sesión propia si se modifica, y tratar de forma segura los `401` y `403`.
- Añadir cobertura de regresión para roles, condición de socio, rutas directas, conservación de roles, respuesta `204` y errores de autorización.

## Capabilities

### New Capabilities

- `frontend-inventory-management-delegation`: Acceso de interfaz y manejo de sesión para la gestión delegada de inventario y reservas administrativas.

### Modified Capabilities

- `frontend-admin-users-management`: La gestión de usuarios permite asignar y retirar el rol limitado `InventoryManager` con las restricciones de membresía y conservación de roles correspondientes.

## Impact

- `IndaloaventurApp.SharedUI`: `AuthSession`, contratos y componentes de configuración de usuarios, inventario y reservas; nuevos literales y estilos SCSS globales si hacen falta.
- `IndaloaventurApp.Web.Client`: guardas de rutas de Configuración y cliente administrativo de usuarios para `PUT /api/users/{userId}`.
- `IndaloaventurApp.Frontend.Tests`: pruebas de cliente HTTP, componentes, rutas y tratamiento de sesión.
- `docs/features/settings-admin.md` y, si procede, documentación de arquitectura y comentarios XML públicos. No se añaden dependencias ni rutas de API; la API conserva la autorización definitiva.
- La entrega depende de integrar los cambios OpenSpec activos `frontend-inventory-management` y `frontend-admin-material-reservations`, que aportan los componentes y rutas administrativas que esta propuesta habilita para el delegado.
