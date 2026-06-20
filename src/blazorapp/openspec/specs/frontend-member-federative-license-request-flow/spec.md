# frontend-member-federative-license-request-flow Specification

## Purpose
TBD - created by archiving change solicitar-licencia-federativa-member. Update Purpose after archive.
## Requirements
### Requirement: El socio MUST poder iniciar una solicitud desde su pantalla de Licencias Federativas
El sistema MUST permitir que un usuario autenticado con rol `Member` y claim `IsMember = true` abra un popup modal de solicitud desde la pantalla `Licencias Federativas`.

#### Scenario: Apertura del modal desde la acción principal
- **WHEN** un socio visualiza su pantalla de `Licencias Federativas` y pulsa el botón `Solicitar Licencia`
- **THEN** el sistema MUST abrir un popup modal responsive dentro del mismo contexto de pantalla
- **THEN** el sistema MUST mostrar en el modal los campos obligatorios `Temporada`, `Tipología` y `Categoría`

#### Scenario: Usuario no autorizado en el flujo de solicitud
- **WHEN** un usuario con rol `Member` y `IsMember = false`, o un usuario sin rol `Member`, accede a la vista o intenta activar la acción de solicitud
- **THEN** el sistema MUST no ofrecer visualización operativa ni flujo operativo de alta de licencia federativa

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

### Requirement: La confirmación MUST crear la solicitud y refrescar el listado inmediatamente
El sistema MUST crear la solicitud del socio usando el endpoint correspondiente de alta y MUST recargar el listado de licencias federativas del propio usuario tras una creación satisfactoria.

#### Scenario: Creación correcta de la solicitud
- **WHEN** el usuario confirma una selección válida en el modal
- **THEN** el sistema MUST invocar el endpoint de creación de solicitud con la `Temporada` y el `TarifaLicenciaFederativaId` resuelto
- **THEN** el sistema MUST cerrar el modal al completarse correctamente la operación
- **THEN** el sistema MUST recargar inmediatamente el listado de `Licencias Federativas` del propio usuario

#### Scenario: Nueva solicitud visible tras el refresco
- **WHEN** la creación se completa correctamente y el listado se recarga
- **THEN** el sistema MUST mostrar la nueva solicitud dentro de la temporada correspondiente con la información actualizada del histórico propio

### Requirement: El modal MUST ofrecer cancelación y estados operativos controlados
El sistema MUST permitir cancelar el flujo sin persistir cambios y MUST comunicar de forma controlada los estados de carga o error del catálogo y de la creación.

#### Scenario: Cancelación por el usuario
- **WHEN** el usuario pulsa `Cancelar` o cierra el popup sin confirmar
- **THEN** el sistema MUST cerrar el modal sin crear ninguna solicitud

#### Scenario: Error durante la carga o la confirmación
- **WHEN** falla la consulta de licencias disponibles o falla la creación de la solicitud
- **THEN** el sistema MUST mantener la vista principal operativa
- **THEN** el sistema MUST informar del error dentro del flujo de forma clara y coherente con la UI existente

### Requirement: El histórico del socio MUST identificar la modalidad de cada licencia federativa solicitada
El sistema MUST mostrar en el detalle de cada solicitud de `Licencias Federativas` del socio si la licencia solicitada corresponde a `Temporada Completa` o a `Media Temporada`, además de la temporada asociada.

#### Scenario: Solicitud de temporada completa en el histórico
- **WHEN** el socio visualiza una solicitud existente cuyo dato `MediaTemporada = false`
- **THEN** el sistema MUST mostrar en el detalle de esa solicitud la temporada acompañada de la modalidad `Temporada Completa`

#### Scenario: Solicitud de media temporada en el histórico
- **WHEN** el socio visualiza una solicitud existente cuyo dato `MediaTemporada = true`
- **THEN** el sistema MUST mostrar en el detalle de esa solicitud la temporada acompañada de la modalidad `Media Temporada`

#### Scenario: Nueva solicitud visible con modalidad tras el refresco
- **WHEN** el socio crea correctamente una nueva solicitud y el listado se recarga
- **THEN** el sistema MUST mostrar la nueva solicitud en el histórico indicando tanto la temporada como la modalidad realmente solicitada

### Requirement: El flujo de socio MUST quedar limitado a solicitudes propias
El sistema MUST restringir el modo socio de `Licencias Federativas` a la consulta y creación sobre el usuario autenticado, sin exponer lectura ni edición de solicitudes de terceros.

#### Scenario: El listado muestra únicamente solicitudes del propio usuario
- **WHEN** un usuario con rol `Member` y `IsMember = true` entra en `Licencias Federativas`
- **THEN** el sistema MUST cargar únicamente las solicitudes asociadas al usuario autenticado

#### Scenario: El modo socio no expone contexto de otro usuario
- **WHEN** un usuario con rol `Member` y `IsMember = true` utiliza la pantalla de `Licencias Federativas`
- **THEN** el sistema MUST no mostrar selectores, acciones ni navegación para elegir otro usuario objetivo

#### Scenario: El modo socio no permite editar solicitudes existentes
- **WHEN** un usuario con rol `Member` y `IsMember = true` visualiza sus solicitudes ya registradas
- **THEN** el sistema MUST no ofrecer acciones de edición sobre solicitudes existentes

