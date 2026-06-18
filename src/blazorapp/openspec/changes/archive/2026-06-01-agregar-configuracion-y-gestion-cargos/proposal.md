## Why

La pantalla `Mi cuenta` ya muestra la opción `Configuración`, pero hoy no existe una página destino ni una vía segura para exponer herramientas administrativas solo a usuarios con rol `Admin`. Incorporar ahora esta entrada permite completar la navegación prevista y abrir un punto controlado para futuras utilidades de administración empezando por la gestión del catálogo de `Cargos`.

## What Changes

- Crear una nueva página `Configuración` accesible desde la opción del mismo nombre ubicada en `Mi cuenta`.
- Añadir en `Configuración` un bloque `Administración` visible únicamente para usuarios autenticados con rol `Admin`.
- Incorporar dentro del bloque `Administración` una opción `Cargos` visible únicamente para usuarios con rol `Admin`.
- Definir la gestión de `Cargos` como una funcionalidad administrativa con operaciones de listado, alta, edición y borrado usando `/api/cargos` y `/api/cargos/{id}`.
- Añadir navegación, recursos localizados y estilos SCSS globales necesarios para las nuevas pantallas y acciones.
- Mantener fuera de alcance en esta iteración la asignación de cargos a fichas de socio; esta propuesta cubre solo la administración del catálogo de cargos.

## Capabilities

### New Capabilities
- `frontend-settings-admin-page`: Define la navegación a `Configuración` desde `Mi cuenta` y la visibilidad condicional del bloque `Administración` según rol `Admin`.
- `frontend-admin-cargos-management`: Define la gestión administrativa del catálogo `Cargos` consumiendo los endpoints `/api/cargos` y `/api/cargos/{id}`.

### Modified Capabilities

## Impact

- Afecta a `IndaloaventurApp.SharedUI` con nuevos componentes reutilizables para `Configuración`, panel de administración y vista de `Cargos`.
- Afecta a `IndaloaventurApp.Web.Client` con nuevas rutas/páginas, integración de navegación y clientes API para `Cargos`.
- Afecta a `openspec/endpoints.json` como contrato de referencia para `GET/POST /api/cargos` y `PUT/DELETE /api/cargos/{id}`.
- Afecta al sistema de recursos localizados ES y al SCSS global modular.
- Requiere control explícito de autorización en UI para no exponer acciones administrativas a usuarios sin rol `Admin`.
