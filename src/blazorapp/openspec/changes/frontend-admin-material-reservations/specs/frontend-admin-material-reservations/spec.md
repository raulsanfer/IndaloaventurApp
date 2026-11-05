## Purpose

Permitir que el personal administrador gestione de forma segura las reservas de material de montaña desde Configuración, respetando las transiciones y la autoridad final de la API.

## ADDED Requirements

### Requirement: Las reservas de material MUST ser una herramienta exclusiva de Administración
El sistema MUST mostrar `Reservas de material` dentro de Configuración únicamente a una sesión autenticada con rol `Admin`. La ruta `/configuracion/reservas-material` MUST impedir la carga de datos de reservas y de información personal de socios para una sesión no autenticada o sin dicho rol, aplicando el patrón de redirección de las herramientas administrativas existentes. Toda petición al recurso administrativo MUST incluir el token Bearer de la sesión, y el cliente MUST tratar la autorización de la API como la decisión final.

#### Scenario: Admin abre Configuración
- **WHEN** una persona autenticada con rol `Admin` accede a Configuración
- **THEN** el sistema MUST mostrar la entrada `Reservas de material` junto a las demás herramientas administrativas

#### Scenario: Persona sin rol Admin intenta acceder a la ruta
- **WHEN** una persona no autenticada o autenticada sin rol `Admin` navega a `/configuracion/reservas-material`
- **THEN** el sistema MUST redirigirla mediante el patrón de sesión aplicable sin solicitar reservas ni revelar datos de socios

#### Scenario: La API rechaza una operación administrativa
- **WHEN** la API responde `401 Unauthorized` o `403 Forbidden` a una petición de reservas
- **THEN** el sistema MUST iniciar el flujo de reautenticación aplicable para `401` o impedir nuevos reintentos y explicar la falta de permisos para `403`

### Requirement: El administrador MUST consultar el listado y detalle autorizados de reservas
La vista administrativa MUST cargar las reservas con `GET /api/inventario-reservas/admin` y MUST recuperar el detalle seleccionado con `GET /api/inventario-reservas/admin/{reservaId}` antes de presentar sus operaciones. Para cada reserva, el sistema MUST presentar la identidad del socio —nombre completo y correo electrónico—, material, unidades solicitadas, estado devuelto por la API, fecha de solicitud, retirada y devolución previstas, fechas efectivas disponibles, ambas confirmaciones de devolución y observaciones del socio y de Administración cuando existan. La presentación MUST priorizar visualmente Solicitud y Retirada, y MUST usar el estado explícito devuelto por la API, sin inferirlo a partir de fechas o confirmaciones.

#### Scenario: Admin consulta una reserva solicitada
- **WHEN** un administrador selecciona una reserva cuyo estado de la API es `Solicitada`
- **THEN** el sistema MUST mostrar sus datos de socio, material, unidades, fechas previstas y observaciones disponibles
- **AND** MUST identificarla como `Solicitada` sin deducir otro estado por la ausencia de fechas efectivas

#### Scenario: Admin consulta una reserva con devoluciones confirmadas
- **WHEN** un administrador abre el detalle de una reserva que incluye fechas efectivas y confirmaciones de devolución
- **THEN** el sistema MUST mostrar dichos valores junto al estado que devuelve la API

### Requirement: La interfaz MUST ofrecer solamente las decisiones permitidas por el estado de la reserva
Para una reserva `Solicitada`, el sistema MUST ofrecer registrar retirada y cancelar; para una reserva `Retirada`, MUST ofrecer confirmar devolución administrativa y cancelar; y para una reserva `Entregada` o `Cancelada`, MUST mostrar el detalle como solo lectura sin acciones de transición. La observación administrativa de cancelación MUST ser opcional en `Solicitada` y MUST ser obligatoria en `Retirada`. La interfaz MUST solicitar una confirmación explícita antes de enviar cualquier decisión administrativa irreversible.

#### Scenario: Admin abre una reserva solicitada
- **WHEN** el detalle recibido tiene estado `Solicitada`
- **THEN** el sistema MUST permitir abrir las acciones de retirada y cancelación
- **AND** MUST no ofrecer confirmación administrativa de devolución

#### Scenario: Admin abre una reserva retirada
- **WHEN** el detalle recibido tiene estado `Retirada`
- **THEN** el sistema MUST permitir abrir las acciones de confirmación administrativa de devolución y cancelación
- **AND** MUST exigir una observación de cancelación no vacía antes de solicitar esa cancelación

#### Scenario: Admin abre una reserva final
- **WHEN** el detalle recibido tiene estado `Entregada` o `Cancelada`
- **THEN** el sistema MUST no ofrecer acciones de transición ni controles editables de decisión

### Requirement: Las acciones administrativas MUST usar las operaciones y validaciones contractuales
El registro de retirada MUST enviar una fecha `fechaRetirada` en formato de fecha a `POST /api/inventario-reservas/admin/{reservaId}/retirada`. La cancelación MUST enviar `observacionAdministracion` a `POST /api/inventario-reservas/admin/{reservaId}/cancelacion`. La confirmación administrativa de devolución MUST enviar `fechaDevolucionEfectiva` a `POST /api/inventario-reservas/admin/{reservaId}/confirmacion-devolucion`, exigir localmente una fecha y rechazar una fecha anterior a la retirada efectiva conocida. El cliente MUST no declarar éxito ni modificar optimistamente el estado ante una solicitud en curso; tras una respuesta `204 No Content`, MUST recargar el listado y el detalle afectado desde la API antes de presentar el estado confirmado.

#### Scenario: Admin registra la retirada
- **WHEN** un administrador confirma una fecha de retirada válida para una reserva `Solicitada`
- **THEN** el sistema MUST enviar la fecha a la operación de retirada correspondiente
- **AND** tras `204 No Content` MUST volver a consultar la reserva y el listado antes de mostrar la transición resultante

#### Scenario: Admin confirma una devolución posterior a la retirada
- **WHEN** un administrador intenta confirmar una devolución con fecha anterior a la fecha de retirada efectiva conocida
- **THEN** el sistema MUST bloquear la solicitud y MUST asociar el error al campo de fecha

#### Scenario: Admin confirma primero la devolución
- **WHEN** la confirmación administrativa de devolución responde `204 No Content` mientras todavía falta la confirmación del socio
- **THEN** el sistema MUST recargar el detalle y MUST conservar el estado que devuelva la API, sin afirmar que la reserva está entregada por anticipado

### Requirement: La vista MUST tratar errores, concurrencia y datos personales de forma segura y recuperable
Ante `400 Bad Request`, el sistema MUST conservar la decisión introducida y mostrar los errores de validación en sus campos. Ante `404 Not Found` del detalle o de una acción, MUST retirar de la vista los datos de la reserva seleccionada y volver al listado sin conservar ni revelar datos del socio. Ante `409 Conflict`, MUST mostrar el `detail` seguro proporcionado por la API, conservar la decisión introducida y recargar el listado y, si sigue disponible, el detalle antes de permitir reintentar. Ante errores de red, tiempo de espera o `5xx`, MUST mostrar un fallo recuperable sin exponer detalles internos ni indicar que la acción se completó. El cliente MUST no reintentar automáticamente peticiones que cambian el estado de una reserva.

#### Scenario: La reserva cambia mientras Admin la gestiona
- **WHEN** una acción administrativa recibe `409 Conflict`
- **THEN** el sistema MUST conservar los valores introducidos y mostrar el detalle seguro del conflicto
- **AND** MUST refrescar el estado remoto antes de permitir un nuevo intento

#### Scenario: La reserva ya no existe
- **WHEN** la consulta de detalle o una acción recibe `404 Not Found`
- **THEN** el sistema MUST eliminar el detalle y los datos de socio de la selección actual
- **AND** MUST volver a mostrar el listado actualizado o su estado de recarga

### Requirement: La gestión MUST seguir el lenguaje visual y responsive de la aplicación
La vista MUST utilizar los componentes y clases DaisyUI, los recursos localizados y los estilos SCSS globales de la aplicación. En pantallas estrechas, la lista, el detalle y los formularios de decisión MUST poder consultarse y operarse sin desbordamiento horizontal ni pérdida de la información esencial; en pantallas amplias, MUST mantener clara la relación entre reserva seleccionada, detalle y acciones. Los estados de carga, vacío y error MUST ser accesibles y distinguibles del contenido confirmado.

#### Scenario: Admin consulta reservas desde un móvil
- **WHEN** un administrador abre la herramienta en una pantalla estrecha
- **THEN** el sistema MUST reorganizar el contenido para mostrar la lista, el detalle y las acciones sin desbordamiento horizontal

#### Scenario: La lista se está cargando o no contiene reservas
- **WHEN** la consulta administrativa está en curso o termina sin reservas
- **THEN** el sistema MUST presentar un estado de carga o vacío accesible
- **AND** MUST no presentar datos de una reserva como confirmados hasta recibirlos de la API
