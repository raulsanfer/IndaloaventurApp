## Why

Actualmente el backend solo expone una consulta paginada de posts de WordPress, pero el frontend necesita cargar el detalle completo de un post concreto a partir de su `slug`. Sin este endpoint, el cliente debe resolver esa lectura por otra vía o sobrecargar listados que no están diseñados para navegación directa a detalle.

## What Changes

- Añadir una consulta de detalle de post de WordPress por `slug`.
- Exponer un endpoint `GET` específico para recuperar un único post con todo su contenido.
- Reutilizar la integración WordPress existente para centralizar mapeo, errores y configuración.
- Definir el comportamiento cuando el `slug` no exista y cubrirlo con pruebas.

## Capabilities

### New Capabilities
- `wordpress-post-detail-query`: Consulta del detalle de un post de WordPress a partir de su `slug` mediante el backend autenticado.

### Modified Capabilities

## Impact

- Afecta a `src/IndaloAventurApi.Application` con nuevo query/handler y extensión del contrato `IWordPressService`.
- Afecta a `src/IndaloAventurApi.Infrastructure` para implementar consulta remota por `slug` y manejo de no encontrado.
- Afecta a `src/IndaloAventurApi.Api` con un nuevo endpoint `GET` en el controlador de WordPress.
- Afecta a pruebas de integración y del servicio WordPress para cubrir éxito, `slug` inexistente y errores remotos.
