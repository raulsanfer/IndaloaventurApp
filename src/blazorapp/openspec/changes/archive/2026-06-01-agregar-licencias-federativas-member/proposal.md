## Why

Las `Licencias Federativas` son una utilidad específica del área de socio y encajan mejor dentro de `Mi Club` que dentro de `Mi Cuenta`. El `endpoints.json` actualizado ya describe los contratos de `LicenciasFederativas`, así que es el momento de añadir esta opción al índice del club y convertirla en una pantalla real de consulta y gestión visual para miembros.

## What Changes

- Crear una nueva página de `Licencias Federativas` accesible desde `Mi Club` solo para usuarios autenticados con rol `Member` y `IsMember = true`.
- Agregar en `Mi Club` una nueva opción visualmente consistente con `Teléfonos de interés`, ubicada debajo de ella y enlazada a la nueva página de licencias.
- Consumir `GET /api/licencias-federativas/me/solicitudes` para cargar el listado de solicitudes del usuario autenticado.
- Representar el listado con una tabla inspirada en DaisyUI y cabeceras/pinned rows por `Temporada`, mostrando dentro de cada grupo una lista con `Licencia`, `Categoria`, `Ambito/Territorio` y `Estado`.
- Añadir en la parte superior derecha un botón `Solicitar` integrado en el diseño visual de la app y explícitamente preparado para una siguiente iteración, sin completar todavía el flujo de creación.
- Definir estados de carga, vacío y error coherentes con la identidad visual actual y con el comportamiento de pantallas de autoservicio ya existentes.
- Mantener oculta esta experiencia para usuarios que no cumplan la condición de socio (`IsMember = false`).

## Capabilities

### New Capabilities
- `frontend-member-federative-licenses-page`: Define la página de consulta de licencias federativas del propio socio con agrupación por temporada y CTA visual de solicitud.

### Modified Capabilities

## Impact

- Afecta a `IndaloaventurApp.Web` con una nueva ruta/página enlazada desde `Mi Club`.
- Afecta a `IndaloaventurApp.SharedUI` con ajustes en el índice de `Mi Club` y uno o más componentes reutilizables para el listado, cabecera de página y estados de la vista.
- Afecta a la capa `Member` o a un cliente API equivalente para mapear `SolicitudLicenciaFederativaDto` desde `openspec/endpoints.json`.
- Afecta a recursos localizados ES y estilos SCSS globales para mantener la coherencia visual de `Mi Club` y el patrón de tabla/listado solicitado.
