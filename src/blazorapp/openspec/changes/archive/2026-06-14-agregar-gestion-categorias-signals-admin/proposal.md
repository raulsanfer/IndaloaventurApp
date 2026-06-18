## Why

La aplicación ya consume `signal-types` para clasificar señales, pero todavía no ofrece una superficie privada para que un administrador gestione esas categorías desde la propia app. Necesitamos cerrar ese hueco ahora para preparar la futura gestión administrativa de `Signals` y evitar que la taxonomía dependa de cambios fuera del frontend privado.

## What Changes

- Añadir una nueva entrada administrativa de `Signals` dentro de `Mi Cuenta -> Configuración` visible solo para usuarios `Admin`.
- Ubicar dentro de esa nueva zona una pantalla de `Categorías` para listar, crear, editar y eliminar `SignalTypes`.
- Permitir crear y editar cada categoría con dos campos: `Nombre` e `Icono` textual.
- Soportar borrado únicamente cuando el backend permita eliminar la categoría, mostrando un error controlado cuando la categoría tenga señales asignadas o no pueda borrarse.
- Mantener la navegación administrativa coherente con el patrón actual de `Configuración`, `Usuarios` y `Cargos`.

## Capabilities

### New Capabilities
- `frontend-admin-signal-categories-management`: define la navegación administrativa de `Signals` en Configuración y la gestión CRUD de categorías `SignalTypes` para administradores.

### Modified Capabilities

## Impact

- Afecta a `IndaloaventurApp.SharedUI` con nuevas vistas Razor, estado de listado/editor, recursos localizados y estilos SCSS para la zona administrativa de `Signals`.
- Afecta a `IndaloaventurApp.Web.Client` con nuevas rutas privadas y ampliación del cliente de `signals` para `POST`, `PUT` y `DELETE` sobre `signal-types`.
- Depende de los endpoints ya descritos en `openspec/endpoints.json`: `GET /api/signal-types`, `POST /api/signal-types`, `PUT /api/signal-types/{id}` y `DELETE /api/signal-types/{id}`.
