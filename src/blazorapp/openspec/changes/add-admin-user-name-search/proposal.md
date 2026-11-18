## Why

La pantalla administrativa de usuarios solo permite filtrar por email. Los administradores necesitan localizar personas por un fragmento de su nombre o apellidos sin conocer su correo. El backend ya define el parámetro opcional `nombre` de `GET /api/users`; falta integrarlo en el frontend y ofrecer una forma explícita de restablecer el listado completo.

## What Changes

- Añadir un campo opcional `Nombre o apellidos` junto al filtro de email en `/configuracion/usuarios`.
- Enviar `nombre` y `email`, individualmente o juntos, a `GET /api/users`, omitiendo valores vacíos tras recortar espacios y codificando cada valor de query.
- Usar la búsqueda parcial del backend: `nombre` coincide con nombre O apellidos de la ficha vinculada, sin distinguir mayúsculas; cuando hay email, ambos filtros se combinan mediante AND.
- Añadir `Limpiar filtros` para vaciar ambos campos y volver a consultar el listado completo, incluso después de una búsqueda sin resultados o fallida.
- Mantener carga, errores seguros, estados vacíos, acciones del listado y acceso reservado a Admin; incorporar recursos localizados, pruebas y documentación del contrato público.

## Capabilities

### New Capabilities

Ninguna.

### Modified Capabilities

- `frontend-admin-users-management`: ampliar el filtrado administrativo con nombre o apellidos, combinación con email y limpieza de filtros; ajustar la carga inicial para contemplar ambos criterios opcionales.

## Impact

- `IndaloaventurApp.SharedUI`: `IAdminUserManagementService`, `AdminUsersManagementView.razor` y su clase parcial, recursos `SharedTexts` y SCSS global si el formulario lo requiere.
- `IndaloaventurApp.Web.Client`: construcción de la consulta en `AdminUserManagementApiClient`, conservando autenticación y respuesta actuales.
- `IndaloaventurApp.Frontend.Tests`: pruebas del componente, del cliente HTTP y actualización de los dobles y llamadores del servicio.
- `docs/features/settings-admin.md`, documentación de arquitectura si procede, comentarios XML del contrato y sincronización de `openspec/endpoints.json` para reflejar `nombre`.
- Dependencia de despliegue: el API utilizado debe exponer la versión con soporte de `nombre`, ya descrita en `../backend/openspec/specs/admin-user-management-api/spec.md` y en el controlador actual. Este cambio no modifica el backend.

## Non-Goals

- No añadir endpoints, consultas de fichas por cada resultado, cambios de roles ni campos nuevos al DTO de usuarios.
- No introducir búsqueda local, autocompletado, búsqueda por nombre completo concatenado, paginación ni nuevas dependencias.
- No prometer equivalencia de acentos distinta de la que ofrezca el backend, ni persistir filtros en URL o almacenamiento local.

## Risks

- Un API desplegado sin soporte de `nombre` podría ignorarlo: verificar la versión del contrato antes de habilitar la funcionalidad.
- Los usuarios sin ficha vinculada no coinciden con `nombre`, aunque su email contenga el texto: documentar esta semántica y conservar la búsqueda solo por email.
- OWASP A01, A04/A09, A05 y A10: mantener autorización administrativa en servidor, no registrar datos personales de búsqueda, codificar valores y probar fallos de permisos y de servicio sin revelar detalles internos.
