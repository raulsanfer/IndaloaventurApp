## Why

La pantalla de `Licencias Federativas` ya permite al socio consultar su histórico, pero todavía no le deja completar la acción principal de negocio: solicitar una nueva licencia desde la propia app. Ahora conviene cerrar ese flujo para que el usuario `Member` con claim `IsMember = true` pueda seleccionar la combinación válida de temporada, tipología y categoría, ver el `PrecioClub` antes de confirmar y registrar la solicitud con recarga inmediata del listado.

## What Changes

- Activar el botón `Solicitar Licencia` de la pantalla de `Licencias Federativas` para abrir un popup modal responsive con formulario de alta.
- Precargar en el modal el combo de `Temporada` con dos únicos valores válidos (`año actual` y `año actual + 1`) y, tras la selección, cargar las opciones de `Tipología` (`Licencia`) y `Categoría` desde el catálogo filtrado por esa temporada.
- Encadenar la selección de los combos para que el usuario solo pueda confirmar una combinación válida y para resolver internamente la tarifa concreta asociada.
- Mostrar el `PrecioClub` de la combinación elegida debajo de los campos obligatorios antes de la confirmación.
- Confirmar la solicitud mediante el endpoint de creación correspondiente y recargar inmediatamente el listado de licencias del usuario tras un alta correcta.
- Mantener cancelación, validación, estados de carga/error y comportamiento de acceso coherente con las reglas ya existentes para miembros.

## Capabilities

### New Capabilities
- `frontend-member-federative-license-request-flow`: Define el flujo de apertura de modal, selección de tarifa disponible, confirmación y refresco del listado de licencias federativas del socio.

### Modified Capabilities

## Impact

- Afecta a la página y componentes de `Licencias Federativas` ya existentes en `IndaloaventurApp.Web.Client` y/o `IndaloaventurApp.SharedUI`.
- Requiere ampliar el cliente/frontend de licencias federativas para consumir `GET /api/licencias-federativas/tarifas` como catálogo de opciones y `POST /api/licencias-federativas/me/solicitudes` para confirmar la solicitud.
- Afecta a modelos de UI, localización ES, validaciones de formulario, estilos SCSS globales y tests del flujo de solicitud y recarga.
