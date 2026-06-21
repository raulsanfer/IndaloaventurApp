## Why

El catalogo actual de tarifas federativas solo distingue el precio de club de temporada completa y el precio para independientes. Necesitamos soportar una campana de media temporada con importe propio para club y permitir que las consultas de tarifas resuelvan el precio aplicable segun ese contexto sin obligar al cliente a recalcularlo.

## What Changes

- Ampliar el modelo `TarifaLicenciaFederativa` para persistir un nuevo campo `PrecioClubMediaTemporada` sin alterar la semantica existente de `PrecioIndependiente`.
- Reflejar el nuevo importe de media temporada en los flujos de consulta de licencias federativas que devuelven informacion funcional de una tarifa.
- Extender la consulta autenticada del catalogo de tarifas para aceptar un parametro booleano `MediaTemporada` que seleccione el precio de club aplicable.
- Devolver en la consulta de tarifas el precio resuelto para club en funcion de `MediaTemporada = false` (temporada completa) o `MediaTemporada = true` (media temporada), manteniendo disponible la informacion tarifaria necesaria para el cliente.

## Capabilities

### New Capabilities
Ninguna.

### Modified Capabilities
- `federative-license-catalog`: la tarifa persistida pasa a incluir un importe especifico de club para media temporada.
- `federative-license-rates-query-api`: la consulta del catalogo debe aceptar `MediaTemporada` y devolver el precio de club aplicable segun ese criterio.
- `member-federative-license-api`: las lecturas funcionales de solicitudes deben reflejar tambien el nuevo importe de media temporada de la tarifa asociada.
- `admin-federative-license-status-management`: las consultas y respuestas administrativas que incluyen datos de tarifa deben reflejar tambien el nuevo importe de media temporada.

## Impact

- Domain: cambios en `TarifaLicenciaFederativa` y sus invariantes para almacenar el nuevo importe.
- Persistence: migracion EF Core, configuracion de entidad y datos semilla del catalogo de tarifas.
- Application: ampliacion de DTOs, queries y mapeos de licencias federativas para incluir el nuevo importe y resolver el precio aplicable.
- API: extension del contrato de `GET /api/licencias-federativas/tarifas` con el filtro `MediaTemporada` y ajuste de respuestas relacionadas con tarifas.
