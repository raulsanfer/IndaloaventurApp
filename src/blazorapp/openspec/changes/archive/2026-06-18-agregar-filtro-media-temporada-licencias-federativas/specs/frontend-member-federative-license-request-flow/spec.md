## MODIFIED Requirements

### Requirement: El socio MUST poder iniciar una solicitud desde su pantalla de Licencias Federativas
El sistema MUST permitir que un usuario autenticado con rol `Member` y claim `IsMember = true` abra un popup modal de solicitud desde la pantalla `Licencias Federativas`.

#### Scenario: Apertura del modal desde la acción principal
- **WHEN** un socio visualiza su pantalla de `Licencias Federativas` y pulsa el botón `Solicitar Licencia`
- **THEN** el sistema MUST abrir un popup modal responsive dentro del mismo contexto de pantalla
- **THEN** el sistema MUST mostrar en el modal los campos obligatorios `Modalidad`, `Temporada`, `Tipología` y `Categoría`

#### Scenario: Usuario no autorizado en el flujo de solicitud
- **WHEN** un usuario sin rol `Member` o con `IsMember = false` accede a la vista o intenta activar la acción de solicitud
- **THEN** el sistema MUST no ofrecer un flujo operativo de alta de licencia federativa

### Requirement: El modal MUST cargar opciones deduplicadas desde licencias disponibles
El sistema MUST obtener las opciones del formulario a partir de una consulta de licencias disponibles, no desde las solicitudes ya registradas del usuario, y MUST presentar valores no duplicados en los combos visibles.

#### Scenario: Modalidad inicial precargada al abrir el modal
- **WHEN** el socio abre el modal de solicitud
- **THEN** el sistema MUST mostrar el combo `Modalidad` con las opciones `Temporada Completa` y `Media Temporada`
- **THEN** el sistema MUST preseleccionar `Temporada Completa`, equivalente a `mediaTemporada = false`

#### Scenario: Temporadas precargadas sin llamada inicial
- **WHEN** el socio abre el modal de solicitud
- **THEN** el sistema MUST mostrar en `Temporada` únicamente el `año actual` y el `año actual + 1`
- **THEN** el sistema MUST no requerir una llamada inicial al API para poblar ese combo

#### Scenario: Carga del catálogo filtrado por temporada y modalidad por defecto
- **WHEN** el usuario selecciona una `Temporada` del combo precargado manteniendo `Temporada Completa`
- **THEN** el sistema MUST consultar `GET /api/licencias-federativas/tarifas` filtrando por esa temporada y con `mediaTemporada = false`
- **THEN** el sistema MUST construir las opciones visibles sin duplicar valores repetidos de `Licencia` o `Categoría`

#### Scenario: Carga del catálogo filtrado por media temporada
- **WHEN** el usuario selecciona una `Temporada` del combo precargado y ha elegido `Media Temporada`
- **THEN** el sistema MUST consultar `GET /api/licencias-federativas/tarifas` filtrando por esa temporada y con `mediaTemporada = true`
- **THEN** el sistema MUST construir las opciones visibles usando únicamente tarifas de media temporada

#### Scenario: Cambio de modalidad con temporada ya seleccionada
- **WHEN** el usuario cambia el combo `Modalidad` después de haber seleccionado una `Temporada`
- **THEN** el sistema MUST limpiar las selecciones dependientes de `Tipología` y `Categoría`
- **THEN** el sistema MUST volver a consultar el catálogo usando la misma temporada y el nuevo valor de `mediaTemporada`

#### Scenario: Temporada sin tarifas todavía publicadas
- **WHEN** el usuario selecciona una combinación permitida de `Temporada` y `Modalidad` y el catálogo filtrado devuelve una colección vacía
- **THEN** el sistema MUST mostrar un mensaje indicando que todavía no hay tarifas disponibles para esa combinación
- **THEN** el sistema MUST mantener no operativa la acción `Confirmar` mientras no existan opciones válidas

#### Scenario: Filtro dependiente entre selecciones
- **WHEN** el usuario selecciona una `Tipología`
- **THEN** el sistema MUST recalcular las opciones válidas de `Categoría` usando solo combinaciones disponibles del catálogo de la temporada y modalidad elegidas

### Requirement: La selección completa MUST resolver una tarifa válida y mostrar el PrecioClub
El sistema MUST exigir la selección de `Modalidad`, `Temporada`, `Tipología` y `Categoría` antes de permitir la confirmación, y MUST mostrar el `PrecioClub` asociado a la combinación válida elegida.

#### Scenario: Precio visible tras completar la selección
- **WHEN** el usuario ha seleccionado una combinación válida de `Modalidad`, `Temporada`, `Tipología` y `Categoría`
- **THEN** el sistema MUST resolver la tarifa concreta correspondiente
- **THEN** el sistema MUST mostrar debajo de los selectores el `PrecioClub` de esa tarifa

#### Scenario: Confirmación bloqueada con formulario incompleto
- **WHEN** falta alguno de los campos obligatorios o la combinación no resuelve una tarifa válida
- **THEN** el sistema MUST mantener deshabilitada o no operativa la acción `Confirmar`
