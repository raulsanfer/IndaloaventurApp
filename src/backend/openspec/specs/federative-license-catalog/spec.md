# federative-license-catalog Specification

## Purpose
TBD - created by archiving change federative-license-management. Update Purpose after archive.

## Requirements
### Requirement: El sistema MUST persistir un catalogo de tarifas federativas por temporada
El sistema MUST almacenar en persistencia un catalogo de tarifas federativas por temporada derivado de la fuente oficial de tarifas, y cada tarifa SHALL incluir al menos `Temporada`, `Licencia`, `Categoria`, `PrecioClub`, `PrecioIndependiente`, `MediaTemporada` y `Territorio`.

#### Scenario: Carga del catalogo 2026 desde la fuente oficial
- **WHEN** se aplica la inicializacion de datos de licencias federativas para la temporada `2026`
- **THEN** el sistema SHALL persistir las tarifas del fichero `Tarifas_Federacion_2026_Estructurado.xlsx` como registros estructurados del catalogo para `Temporada = 2026`

#### Scenario: Tarifa creada sin informar media temporada
- **WHEN** un proceso crea o transforma una tarifa catalogada sin marcar explicitamente `MediaTemporada`
- **THEN** el sistema SHALL persistir `MediaTemporada = false`

### Requirement: El catalogo MUST preservar la estructura tarifaria del Excel 2026
El sistema MUST conservar la granularidad de una tarifa por combinacion de licencia, categoria y variante de temporada, incluyendo importes diferenciados para `Clubes` e `Independientes` cuando existan en la fuente.

#### Scenario: Tarifa con temporada completa y media temporada
- **WHEN** la fuente oficial expresa una misma licencia y categoria con precio de temporada completa y precio de media temporada
- **THEN** el sistema SHALL persistir dos tarifas diferenciadas con el mismo `Temporada`, `Licencia` y `Categoria`, usando `MediaTemporada = false` para temporada completa y `MediaTemporada = true` para media temporada

#### Scenario: Tarifa sin importe para independientes
- **WHEN** una fila del catalogo oficial no contiene importe para `Independientes`
- **THEN** el sistema SHALL permitir persistir ese importe como nulo sin invalidar la tarifa

### Requirement: El sistema MUST garantizar unicidad de tarifa por temporada, licencia, categoria y variante de temporada
El sistema MUST impedir duplicados de tarifas para la misma combinacion de `Temporada`, `Licencia`, `Categoria` y `MediaTemporada`.

#### Scenario: Insercion duplicada de tarifa catalogada con la misma variante
- **WHEN** un proceso intenta persistir una segunda tarifa con la misma `Temporada`, `Licencia`, `Categoria` y `MediaTemporada` que otra ya existente
- **THEN** el sistema SHALL rechazar el duplicado o impedirlo mediante restriccion de persistencia

#### Scenario: Insercion de dos variantes de temporada para la misma licencia
- **WHEN** un proceso persiste dos tarifas con la misma `Temporada`, `Licencia` y `Categoria` pero distinto valor de `MediaTemporada`
- **THEN** el sistema SHALL permitir ambas tarifas como variantes validas del catalogo
