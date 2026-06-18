## Why

El front necesita cargar la informacion principal de una `Signal` concreta cuando el usuario accede a su pantalla de detalle. Ahora mismo la API permite buscar signals, consultar sus imagenes y consultar sus comentarios, pero no ofrece una consulta directa por `Id` para recuperar el bloque principal de datos.

## What Changes

- Anadir una consulta autenticada para recuperar una `Signal` concreta por su `Id`.
- Devolver en esa consulta los campos principales ya expuestos en el listado de busqueda para que el front pueda construir la vista de detalle.
- Responder con `404 Not Found` cuando la `Signal` solicitada no exista.
- Mantener separadas la consulta principal de detalle y las consultas ya existentes de imagenes y comentarios.

## Capabilities

### New Capabilities
- `signal-detail-query`: Consulta de una signal individual por identificador para obtener su informacion principal.

### Modified Capabilities

## Impact

- API afectada: `GET /api/signals/{id}`.
- Codigo afectado: `SignalsController`, capa Application para la nueva query/handler y `ISignalRepository` reutilizando `GetByIdAsync`.
- Pruebas afectadas: tests de integracion de `signals` para cubrir caso correcto y `404` cuando no exista la signal.
