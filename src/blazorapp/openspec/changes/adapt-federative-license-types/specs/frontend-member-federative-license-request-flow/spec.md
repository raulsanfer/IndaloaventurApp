## MODIFIED Requirements

### Requirement: El socio MUST poder iniciar una solicitud desde su pantalla de Licencias Federativas
El sistema MUST permitir que un usuario autenticado con rol `Member` y claim `IsMember = true` abra un popup modal de solicitud desde la pantalla `Licencias Federativas`.

#### Scenario: Apertura del modal desde la acción principal
- **WHEN** un socio pulsa `Solicitar Licencia`
- **THEN** el sistema MUST abrir un modal responsive con los campos en orden `Temporada`, `Tipo de licencia`, `Nombre de licencia` y `Categoría`, seguidos del precio y, para `Día`, las fechas de cobertura

#### Scenario: Usuario no autorizado en el flujo de solicitud
- **WHEN** un usuario con rol `Member` y `IsMember = false`, o un usuario sin rol `Member`, intenta acceder al flujo
- **THEN** el sistema MUST no ofrecer visualización operativa ni flujo operativo de alta

### Requirement: El modal MUST cargar opciones deduplicadas desde licencias disponibles
El sistema MUST obtener tipos del catálogo autenticado y opciones de licencia y categoría de las tarifas disponibles, sin deducirlas de solicitudes previas. MUST mantener opciones deduplicadas y limpiar dependencias al cambiar selecciones superiores.

#### Scenario: Modalidad inicial precargada al abrir el modal
- **WHEN** el socio abre el modal
- **THEN** el sistema MUST consultar `GET /api/licencias-federativas/tipos`, mostrar en orden `1 = Completa`, `2 = Media 01/07 - 31/12`, `3 = Día` y preseleccionar Completa cuando esté disponible
- **AND** MUST mostrar los nombres recibidos sin ofrecer mantenimiento de tipos

#### Scenario: Temporadas precargadas sin llamada inicial
- **WHEN** el socio abre el modal
- **THEN** el sistema MUST ofrecer únicamente el año actual y el siguiente sin consultar el API para poblar las temporadas

#### Scenario: Carga del catálogo filtrado por temporada y modalidad por defecto
- **WHEN** el usuario selecciona temporada manteniendo Completa
- **THEN** el sistema MUST consultar `GET /api/licencias-federativas/tarifas` con `temporada` y `tipoLicenciaId=1`, sin enviar `mediaTemporada`
- **AND** MUST deduplicar nombre de licencia y categoría

#### Scenario: Carga del catálogo filtrado por media temporada
- **WHEN** el usuario selecciona temporada y tipo Media 01/07 - 31/12
- **THEN** el sistema MUST consultar tarifas con esa temporada y `tipoLicenciaId=2`

#### Scenario: Cambio de modalidad con temporada ya seleccionada
- **WHEN** cambia el tipo o la temporada
- **THEN** el sistema MUST limpiar nombre de licencia, categoría, tarifa y precio y consultar la nueva combinación
- **AND** MUST limpiar las fechas al salir de Día o cambiar temporada e ignorar respuestas de combinaciones anteriores

#### Scenario: Temporada sin tarifas todavía publicadas
- **WHEN** el catálogo filtrado devuelve una colección vacía, incluido el tipo Día sin tarifas publicadas
- **THEN** el sistema MUST informar que no hay tarifas disponibles y bloquear Confirmar sin inventar opciones ni precios

#### Scenario: Filtro dependiente entre selecciones
- **WHEN** cambia el nombre de licencia
- **THEN** el sistema MUST borrar categoría, tarifa y precio y ofrecer únicamente categorías deduplicadas disponibles para la licencia, temporada y tipo elegidos

### Requirement: La selección completa MUST resolver una tarifa válida y mostrar el PrecioClub
El sistema MUST exigir temporada, tipo, nombre de licencia y categoría que resuelvan una única tarifa, mostrar su `PrecioClub` y exigir cobertura válida para Día antes de confirmar.

#### Scenario: Precio visible tras completar la selección
- **WHEN** una combinación resuelve una única tarifa
- **THEN** el sistema MUST mostrar debajo de los selectores su `PrecioClub`, separado de `PrecioIndependiente`, sin multiplicarlo por los días de cobertura

#### Scenario: Confirmación bloqueada con formulario incompleto
- **WHEN** falta una selección, hay carga pendiente, la combinación no resuelve una única tarifa o la cobertura diaria es inválida
- **THEN** el sistema MUST mantener no operativa la acción Confirmar

### Requirement: La confirmación MUST crear la solicitud y refrescar el listado inmediatamente
El sistema MUST crear la solicitud propia mediante el endpoint de alta y recargar el histórico tras el éxito, conservando cada identificador aunque coincidan temporadas.

#### Scenario: Creación correcta de la solicitud
- **WHEN** el usuario confirma una selección válida
- **THEN** el sistema MUST enviar a `POST /api/licencias-federativas/me/solicitudes` temporada, tarifaLicenciaFederativaId y fechas de cobertura en formato `yyyy-MM-dd` cuando correspondan, sin usuario ni precio
- **AND** MUST cerrar el modal y recargar el listado tras el éxito

#### Scenario: Nueva solicitud visible tras el refresco
- **WHEN** se recarga el listado tras crear una solicitud diaria en una temporada con otra diaria y una no diaria
- **THEN** el sistema MUST mostrar las tres solicitudes con sus identificadores distintos dentro de la temporada

### Requirement: El modal MUST ofrecer cancelación y estados operativos controlados
El sistema MUST permitir cancelar sin persistir y comunicar errores y estados de carga de tipos, tarifas y alta mediante la UI existente, sin exponer detalles internos.

#### Scenario: Cancelación por el usuario
- **WHEN** el usuario cancela o cierra el modal sin confirmar
- **THEN** el sistema MUST cerrar sin crear solicitudes ni aplicar resultados tardíos a una nueva apertura

#### Scenario: Error durante la carga o la confirmación
- **WHEN** falla una consulta o el alta, incluida una respuesta 400 de cobertura inválida
- **THEN** el sistema MUST comunicar el error de forma segura, mantener la vista operativa y bloquear confirmación cuando no haya datos válidos

#### Scenario: Conflicto de solicitud no diaria
- **WHEN** el alta devuelve 409 por existir una solicitud Completa o Media, incluso cancelada, en la temporada
- **THEN** el sistema MUST explicar el límite conjunto de una solicitud no diaria sin afirmar que se prohíben las solicitudes diarias de esa temporada

### Requirement: El histórico del socio MUST identificar la modalidad de cada licencia federativa solicitada
El sistema MUST mostrar temporada y nombre de tipo devuelto por la API en cada solicitud, con su cobertura cuando esté informada, conservando las solicitudes migradas con fechas nulas.

#### Scenario: Solicitud de temporada completa en el histórico
- **WHEN** el socio visualiza una solicitud de tipo Completa
- **THEN** el sistema MUST mostrar temporada y `tipoLicenciaNombre` sin reconstruir la modalidad desde un booleano

#### Scenario: Solicitud de media temporada en el histórico
- **WHEN** el socio visualiza una solicitud de tipo Media 01/07 - 31/12
- **THEN** el sistema MUST mostrar temporada y el nombre recibido del tipo

#### Scenario: Nueva solicitud visible con modalidad tras el refresco
- **WHEN** se recarga el listado tras un alta diaria
- **THEN** el sistema MUST mostrar temporada, nombre de tipo e intervalo de cobertura, admitiendo inicio y fin iguales

#### Scenario: Solicitudes migradas sin cobertura
- **WHEN** una solicitud tiene ambas fechas nulas
- **THEN** el sistema MUST conservarla visible sin fabricar fechas

## ADDED Requirements

### Requirement: La cobertura diaria MUST validarse antes del alta
El sistema MUST exigir fecha de inicio y fin para Día, con inicio menor o igual a fin y ambas fechas dentro del año de temporada. Para Completa y Media MUST omitir ambas fechas o enviarlas nulas desde este formulario.

#### Scenario: Cobertura de un único día
- **WHEN** el socio elige Día y ambas fechas coinciden dentro de la temporada y hay tarifa válida
- **THEN** el sistema MUST permitir confirmar y conservar el precio de la tarifa

#### Scenario: Cobertura inválida
- **WHEN** falta alguna fecha diaria, el inicio supera al fin o alguna pertenece a otro año
- **THEN** el sistema MUST explicar la validación y bloquear el envío

#### Scenario: Solicitudes diarias coincidentes
- **WHEN** existe una solicitud diaria para las mismas fechas y el socio prepara otra con tarifa válida
- **THEN** el sistema MUST no bloquearla por solapamiento ni por existir otras solicitudes de la temporada
