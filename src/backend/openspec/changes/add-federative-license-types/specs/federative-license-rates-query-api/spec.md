## MODIFIED Requirements

### Requirement: El sistema MUST permitir consultar el catalogo de tarifas de licencias federativas a cualquier usuario autenticado
El sistema MUST exponer `GET /api/licencias-federativas/tarifas` a cualquier usuario autenticado, sin exigir un rol específico. Cada tarifa SHALL incluir `Id`, `Temporada`, `TipoLicenciaId`, `TipoLicenciaNombre`, `Licencia`, `Categoria`, `Territorio`, `PrecioClub` y `PrecioIndependiente`, sustituyendo `MediaTemporada`.

#### Scenario: Consulta satisfactoria del catalogo completo
- **WHEN** un usuario autenticado de cualquier rol consulta el catálogo sin filtros
- **THEN** el sistema SHALL devolver `200 OK` con todas las tarifas disponibles y la identificación y nombre de su tipo

#### Scenario: Usuario anonimo intenta consultar el catalogo
- **WHEN** un cliente sin autenticación JWT intenta consultar el catálogo
- **THEN** el sistema SHALL rechazar la petición con una respuesta de autenticación

### Requirement: El sistema MUST permitir filtrar las tarifas federativas por temporada y variante de temporada
El sistema MUST aceptar filtros opcionales `temporada` y `tipoLicenciaId` y aplicar ambos conjuntamente cuando estén presentes. La respuesta SHALL contener solo coincidencias y mantener orden determinista. El filtro anterior `mediaTemporada` SHALL dejar de estar soportado.

#### Scenario: Consulta filtrada por temporada existente
- **WHEN** un usuario autenticado consulta una temporada con tarifas registradas y omite el tipo
- **THEN** el sistema SHALL devolver todas las tarifas de esa temporada, de cualquier tipo

#### Scenario: Consulta filtrada por temporada sin tarifas
- **WHEN** un usuario autenticado consulta una temporada sin tarifas
- **THEN** el sistema SHALL devolver `200 OK` con una colección vacía

#### Scenario: Consulta filtrada por media temporada
- **WHEN** un usuario autenticado consulta usando el identificador de `Media 01/07 - 31/12`
- **THEN** el sistema SHALL devolver solo tarifas de ese tipo, respetando también la temporada si se informó

#### Scenario: Consulta de tarifas diarias
- **WHEN** un usuario consulta una temporada y el identificador de `Día`
- **THEN** el sistema SHALL devolver solo las tarifas diarias de esa temporada o una colección vacía si no existen

#### Scenario: Consulta filtrada solo por tipo
- **WHEN** un usuario consulta con `tipoLicenciaId` sin informar `temporada`
- **THEN** el sistema SHALL devolver tarifas de ese tipo en todas las temporadas disponibles

#### Scenario: Combinación de filtros sin coincidencias
- **WHEN** se informa una temporada y un identificador positivo de tipo sin tarifas coincidentes
- **THEN** el sistema SHALL devolver `200 OK` con una colección vacía

#### Scenario: Filtro de tipo inválido
- **WHEN** se informa un `tipoLicenciaId` no positivo o con formato incorrecto
- **THEN** el sistema SHALL devolver `400 Bad Request` con ProblemDetails en español

#### Scenario: Cliente que todavía envía el filtro antiguo
- **WHEN** un cliente envía `mediaTemporada`, incluso si también informa `tipoLicenciaId`
- **THEN** el sistema SHALL devolver `400 Bad Request` indicando en español que debe usar `tipoLicenciaId`, sin ignorar silenciosamente el filtro antiguo

## ADDED Requirements

### Requirement: El catálogo MUST permitir resolver el precio de la selección completa
Los datos consultados MUST permitir obtener opciones deduplicadas en el orden temporada, tipo, licencia y categoría. Una combinación válida SHALL resolver una única tarifa y mostrar su `PrecioClub`, sin calcular importes a partir de las fechas de cobertura.

#### Scenario: Selección completa con precio de club
- **WHEN** el cliente selecciona una temporada, un tipo, una licencia y una categoría presentes en el catálogo
- **THEN** SHALL resolver un único `Id` de tarifa y su `PrecioClub`, manteniendo `PrecioIndependiente` como dato separado y anulable

#### Scenario: Misma licencia en varios tipos
- **WHEN** existen tarifas de `Completa`, `Media` y `Día` con la misma temporada, licencia y categoría
- **THEN** el tipo seleccionado SHALL determinar inequívocamente la tarifa y su precio

#### Scenario: Cobertura diaria de más de un día
- **WHEN** se selecciona una tarifa `Día` y una cobertura con inicio y fin distintos
- **THEN** el precio mostrado SHALL ser el `PrecioClub` catalogado, sin multiplicación automática por la duración
