## Why

La consulta actual de listado de posts WordPress no está recuperando la imagen destacada porque el contrato solicitado no coincide con el mapeo esperado, y además fuerza un tamaño fijo de página en el endpoint en lugar de usar una configuración centralizada. También está devolviendo y potencialmente solicitando más información de la necesaria para el listado, lo que añade coste de red y acopla el contrato a campos que no hacen falta en esa vista.

## What Changes

- Añadir configuración en `appsettings` para definir el número de posts a consultar por defecto en el listado de WordPress.
- Corregir la consulta del listado para recuperar la imagen destacada sin pedir contenido innecesario al API de WordPress.
- **BREAKING** Ajustar el contrato del listado de posts WordPress para devolver solo la información requerida por la vista de resumen: título, descripción/resumen, fecha de publicación e imagen destacada, evitando el resto de campos.
- Mantener el detalle por `slug` como flujo separado para seguir consultando información completa cuando sea necesaria.

## Capabilities

### New Capabilities

### Modified Capabilities

- `wordpress-posts-integration`: el listado de posts debe usar tamaño por defecto configurable, recuperar la imagen destacada correctamente y minimizar el payload solicitado y devuelto.

## Impact

- Afecta a `WordPressOptions`, al servicio de infraestructura de WordPress y al endpoint `GET /api/wordpress/posts`.
- Puede requerir separar el contrato DTO de listado respecto al de detalle para evitar cargar y exponer contenido no necesario.
- Impacta `appsettings`, pruebas de integración de WordPress y clientes que consuman el listado actual.
