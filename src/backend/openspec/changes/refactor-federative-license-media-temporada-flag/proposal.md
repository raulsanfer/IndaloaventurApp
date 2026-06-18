## Why

El enfoque actual modela la media temporada como una segunda columna de precio dentro de la misma tarifa, lo que mezcla dos variantes comerciales distintas en un unico registro y complica la persistencia, la consulta y la evolucion del catalogo. Necesitamos que cada tarifa identifique de forma explicita si corresponde a temporada completa o media temporada, manteniendo `false` como valor por defecto para no alterar el comportamiento historico cuando no se informe el nuevo dato.

## What Changes

- **BREAKING** `TarifaLicenciaFederativa` dejara de representar la media temporada mediante `PrecioClubMediaTemporada` y pasara a incluir un flag booleano `MediaTemporada`.
- El catalogo persistira tarifas de temporada completa y de media temporada como registros diferenciados, reutilizando `PrecioClub` y `PrecioIndependiente` y distinguiendolos por `MediaTemporada`.
- La consulta autenticada del catalogo de tarifas expondra `MediaTemporada` para que el cliente pueda identificar cada variante sin depender de un precio club derivado.
- Las consultas de solicitudes propias y administrativas que muestran datos de tarifa tambien expondran `MediaTemporada`, porque ahora puede haber dos tarifas con la misma licencia y categoria.
- La persistencia, los datos semilla y las restricciones de unicidad del catalogo se ajustaran para admitir variantes de la misma licencia y categoria cuando difieran en `MediaTemporada`.

## Capabilities

### New Capabilities
Ninguna.

### Modified Capabilities
- `federative-license-catalog`: la tarifa catalogada pasa a incluir `MediaTemporada` como parte de su identidad funcional y de sus restricciones de unicidad.
- `federative-license-rates-query-api`: la consulta del catalogo debe devolver `MediaTemporada` y tratar temporada completa y media temporada como filas diferenciadas.
- `member-federative-license-api`: las respuestas de autoservicio que incluyen datos de tarifa deben identificar tambien la variante de `MediaTemporada`.
- `admin-federative-license-status-management`: las consultas administrativas que incluyen datos de tarifa deben identificar tambien la variante de `MediaTemporada`.

## Impact

- Domain: refactor del agregado `TarifaLicenciaFederativa` y de sus invariantes.
- Persistence: nueva migracion EF Core para introducir `MediaTemporada`, transformar los datos existentes y redefinir el indice unico del catalogo.
- Application/API: ajuste de DTOs, handlers y contratos de consulta para exponer `MediaTemporada` y dejar de depender de `PrecioClubMediaTemporada`.
