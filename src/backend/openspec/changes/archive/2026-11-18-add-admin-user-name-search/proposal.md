## Why

La administración dispone de búsqueda parcial por email en `GET /api/users`, pero no puede localizar una cuenta mediante el nombre o los apellidos. Es necesario poder encontrar usuarios cuando solo se conoce un fragmento de sus datos personales.

## What Changes

- Añadir el parámetro opcional `nombre` a `GET /api/users` para buscar texto contenido en cualquier posición de `FichaSocio.Nombre` o `FichaSocio.Apellidos`, sin exigir coincidencia exacta ni limitarse al inicio del campo.
- Ignorar mayúsculas/minúsculas y recortar espacios al principio y al final del filtro. Un valor ausente, vacío o compuesto solo por espacios equivale a no aplicar ese filtro.
- Conservar el filtro `email` existente. Si se proporcionan ambos filtros, el usuario debe cumplir el de email y el de nombre/apellidos.
- Utilizar la ficha vinculada por `UserId` como fuente de nombre y apellidos. Una cuenta sin ficha no coincide con un filtro `nombre` activo, pero sigue disponible en los listados sin ese filtro y en las búsquedas por email.
- Mantener la autorización administrativa, el contrato de respuesta, el orden por email ascendente y `200 OK` con colección vacía cuando no haya coincidencias.
- Incorporar pruebas de normalización, coincidencias parciales, combinación de filtros, ausencia de ficha, compatibilidad y autorización.

## Capabilities

### New Capabilities

Ninguna.

### Modified Capabilities

- `admin-user-management-api`: ampliar el listado administrativo con búsqueda opcional por texto contenido en nombre o apellidos.

## Impact

- API: ampliar y documentar el query string del endpoint existente.
- Application: extender `ListManagedUsersQuery`, su handler y `IIdentityService.ListUsersAsync` para transportar y normalizar ambos filtros.
- Infrastructure: consultar `ApplicationDbContext.FichasSocio` desde `IdentityService`, relacionando `FichaSocio.UserId` con `Usuario.Id` y filtrando antes de materializar resultados.
- Tests: ampliar `UserManagementHandlersTests` y las pruebas de integración del listado; actualizar todos los dobles de `IIdentityService` afectados.
- Persistencia: reutilizar los campos y la relación existentes, sin migraciones de base de datos.

## Scope and Non-Goals

El alcance es el contrato y la implementación futura del filtro administrativo en el backend. No incluye cambios del frontend, nuevos campos en Identity o en `ManagedUserDto`, paginación, ordenación configurable, búsqueda aproximada, búsqueda por tokens ni concatenación de nombre y apellidos. Tampoco introduce una garantía nueva de equivalencia entre caracteres con y sin tilde.

## Risks

- Los usuarios sin `FichaSocio` no tienen nombre ni apellidos almacenados en el modelo actual y no pueden aparecer por ese criterio. Se mantiene su acceso por email.
- Una búsqueda por contenido puede requerir recorrer los campos de texto; se aplicará en base de datos, sin cargar todas las fichas en memoria ni realizar una consulta de ficha por usuario.
- Las diferencias entre SQL Server y SQLite pueden afectar a las comparaciones de texto. Las pruebas deberán comprobar la insensibilidad a mayúsculas con una normalización traducible por ambos proveedores y verificar la consulta en SQL Server.
