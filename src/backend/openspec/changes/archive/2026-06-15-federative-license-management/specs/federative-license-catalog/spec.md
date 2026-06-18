## ADDED Requirements

### Requirement: El sistema MUST persistir un catalogo de tarifas federativas por temporada
El sistema MUST almacenar en persistencia un catalogo de tarifas federativas por temporada derivado de la fuente oficial de tarifas, y cada tarifa SHALL incluir al menos `Temporada`, `Licencia`, `Categoria`, `PrecioClub`, `PrecioIndependiente` y `Territorio`.

#### Scenario: Carga del catalogo 2026 desde la fuente oficial
- **WHEN** se aplica la inicializacion de datos de licencias federativas para la temporada `2026`
- **THEN** el sistema SHALL persistir las tarifas del fichero `Tarifas_Federacion_2026_Estructurado.xlsx` como registros estructurados del catalogo para `Temporada = 2026`

### Requirement: El catalogo MUST preservar la estructura tarifaria del Excel 2026
El sistema MUST conservar la granularidad de una tarifa por combinacion de licencia y categoria, incluyendo importes diferenciados para `Clubes` e `Independientes` cuando existan en la fuente.

#### Scenario: Tarifa con ambos importes
- **WHEN** una fila del catalogo oficial contiene importes para `Clubes` e `Independientes`
- **THEN** el sistema SHALL persistir ambos valores en la tarifa catalogada

#### Scenario: Tarifa sin importe para independientes
- **WHEN** una fila del catalogo oficial no contiene importe para `Independientes`
- **THEN** el sistema SHALL permitir persistir ese importe como nulo sin invalidar la tarifa

### Requirement: El sistema MUST garantizar unicidad de tarifa por temporada, licencia y categoria
El sistema MUST impedir duplicados de tarifas para la misma combinacion de `Temporada`, `Licencia` y `Categoria`.

#### Scenario: Insercion duplicada de tarifa catalogada
- **WHEN** un proceso intenta persistir una segunda tarifa con la misma `Temporada`, `Licencia` y `Categoria` que otra ya existente
- **THEN** el sistema SHALL rechazar el duplicado o impedirlo mediante restriccion de persistencia
