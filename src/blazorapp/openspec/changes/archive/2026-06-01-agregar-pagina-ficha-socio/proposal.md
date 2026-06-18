## Why

La aplicación ya reconoce si la sesión pertenece a un socio mediante `IsMember`, pero todavía no ofrece una pantalla operativa para que ese usuario consulte y actualice su propia ficha. Añadir ahora esa página completa el flujo básico de autoservicio del socio y aprovecha el contrato ya disponible en `/api/fichas-socio/me`.

## What Changes

- Crear una nueva página de `Ficha de Socio` accesible únicamente para usuarios autenticados cuyo `IsMember` sea `true`.
- Añadir un componente compartido basado en `fieldset` de DaisyUI que muestre y permita editar los campos permitidos de la ficha del socio.
- Mantener coherencia visual con `Mi Cuenta` y `Configuración`, incluyendo breadcrumb, títulos y composición responsive.
- Consumir `GET /api/fichas-socio/me` para cargar la ficha y `PUT /api/fichas-socio/me` para guardar cambios del propio usuario.
- Excluir de la UI y de la edición los campos `IsMember` y cualquier información de roles del usuario.
- Incorporar validaciones de valor y seguridad por campo antes de enviar cambios al API.

## Capabilities

### New Capabilities
- `frontend-member-self-profile-page`: Define la página y el componente de autoservicio para ver y editar la propia ficha de socio con acceso exclusivo para usuarios `IsMember`.

### Modified Capabilities

## Impact

- Afecta a `IndaloaventurApp.SharedUI` con nuevos componentes reutilizables de ficha de socio y modelos/formularios asociados.
- Afecta a `IndaloaventurApp.Web.Client` con una nueva ruta/página, cliente API o extensión del servicio de perfil y navegación desde `Mi Cuenta`.
- Afecta a recursos localizados ES y a SCSS global para sostener la nueva pantalla manteniendo la línea visual de la app.
- Requiere mapear completamente `FichaSocioDto` y `UpdateFichaSocioRequest` desde `openspec/endpoints.json`, filtrando los campos no editables por el propio usuario.
