## MODIFIED Requirements

### Requirement: El sistema MUST persistir un catalogo de tarifas federativas por temporada
El sistema MUST almacenar un catálogo de tarifas federativas por temporada derivado de la fuente oficial. Cada tarifa SHALL incluir `Temporada`, `TipoLicenciaId`, `Licencia`, `Categoria`, `PrecioClub`, `PrecioIndependiente` y `Territorio`. `TipoLicenciaId` SHALL referenciar un tipo existente y sustituir a `MediaTemporada`.

#### Scenario: Carga del catalogo 2026 desde la fuente oficial
- **WHEN** se aplica la inicialización de datos de licencias federativas para la temporada `2026`
- **THEN** el sistema SHALL persistir las tarifas de `Tarifas_Federacion_2026_Estructurado.xlsx` como registros estructurados con su tipo de licencia correspondiente

#### Scenario: Tarifa creada sin informar media temporada
- **WHEN** un proceso crea una tarifa sin el booleano retirado `MediaTemporada` e informa un `TipoLicenciaId` válido
- **THEN** el sistema SHALL persistir el tipo explícito, sin asumir temporada completa por omitir el booleano antiguo

#### Scenario: Tarifa sin tipo de licencia
- **WHEN** un proceso intenta persistir una tarifa sin `TipoLicenciaId` o con un tipo inexistente
- **THEN** el sistema SHALL rechazar la tarifa sin asignar silenciosamente el tipo `Completa`

### Requirement: El catalogo MUST preservar la estructura tarifaria del Excel 2026
El sistema MUST conservar una tarifa por combinación de temporada, tipo de licencia, licencia y categoría, incluyendo importes diferenciados para `Clubes` e `Independientes` cuando existan en la fuente.

#### Scenario: Tarifa con temporada completa y media temporada
- **WHEN** la fuente oficial expresa una misma licencia y categoría con precio de temporada completa y precio de media temporada
- **THEN** el sistema SHALL persistir dos tarifas con la misma temporada, licencia y categoría, asociadas respectivamente a `Completa` y `Media 01/07 - 31/12`

#### Scenario: Tarifa sin importe para independientes
- **WHEN** una fila del catálogo oficial no contiene importe para `Independientes`
- **THEN** el sistema SHALL permitir persistir ese importe como nulo sin invalidar la tarifa

### Requirement: El sistema MUST garantizar unicidad de tarifa por temporada, licencia, categoria y variante de temporada
El sistema MUST impedir duplicados para la misma combinación de `Temporada`, `TipoLicenciaId`, `Licencia` y `Categoria`. Dos tarifas que difieran en el tipo SHALL poder coexistir aunque compartan temporada, licencia y categoría.

#### Scenario: Insercion duplicada de tarifa catalogada con la misma variante
- **WHEN** se intenta persistir una segunda tarifa con la misma temporada, tipo, licencia y categoría que una existente
- **THEN** el sistema SHALL impedir el duplicado mediante una restricción de persistencia

#### Scenario: Insercion de dos variantes de temporada para la misma licencia
- **WHEN** se persisten tarifas con la misma temporada, licencia y categoría pero distinto `TipoLicenciaId`
- **THEN** el sistema SHALL permitir las tarifas como variantes válidas del catálogo

## ADDED Requirements

### Requirement: La migración MUST conservar las tarifas y solicitudes existentes
La migración MUST asignar `Completa` a todas las tarifas con `MediaTemporada = 0` y `Media 01/07 - 31/12` a todas las de valor `1`. SHALL conservar identificadores, temporada, licencia, categoría, importes, territorio y solicitudes asociadas, y retirar el booleano solo después de completar la asignación.

#### Scenario: Conversión de tarifas completas y medias con solicitudes
- **WHEN** se migra una base con ambas variantes y solicitudes que las referencian
- **THEN** cada tarifa SHALL conservar su identificador, datos funcionales y referencias, con el tipo correspondiente al valor original del booleano
- **AND** las solicitudes SHALL conservar identificador, usuario, temporada, estado y fecha de creación, con `FechaInicio` y `FechaFin` nulas

#### Scenario: Conversión de tarifas no incluidas en los datos iniciales
- **WHEN** existen tarifas añadidas manualmente o de temporadas distintas de `2026`
- **THEN** la migración SHALL convertirlas usando su valor de `MediaTemporada`, sin limitarse a los identificadores precargados

#### Scenario: Catálogo diario sin importes definidos
- **WHEN** se aplica la migración sin una fuente aprobada de tarifas de `Día`
- **THEN** el sistema SHALL crear el tipo `Día` sin generar tarifas ni importes ficticios

### Requirement: La reversión MUST impedir pérdidas de datos incompatibles con el modelo anterior
La reversión MUST preservar los registros y referencias existentes. Si hay tarifas `Día`, coberturas informadas o varias solicitudes de un usuario en una temporada, SHALL rechazar la reversión antes de modificar datos, en vez de eliminarlos o convertirlos a un booleano sin equivalencia.

#### Scenario: Reversión compatible
- **WHEN** solo existen tipos `Completa` y `Media` en tarifas, todas las fechas de cobertura son nulas y cada usuario tiene como máximo una solicitud por temporada
- **THEN** la reversión SHALL reconstruir `MediaTemporada` con valores `0` y `1` y restaurar la unicidad anterior sin alterar tarifas ni solicitudes

#### Scenario: Reversión con datos diarios
- **WHEN** se solicita revertir y existe una tarifa `Día`, una cobertura informada o varias solicitudes de un usuario en una temporada
- **THEN** el sistema SHALL abortar la reversión antes de efectuar cambios y SHALL conservar íntegramente los datos
