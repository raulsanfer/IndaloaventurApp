## Purpose

Permitir que un socio real gestione de forma segura sus propias solicitudes de material desde Mi Club, sin alterar desde el cliente la disponibilidad que controla la API.

## ADDED Requirements

### Requirement: Las reservas de material MUST estar disponibles solamente para socios reales
El sistema MUST mostrar `Reservas de material` inmediatamente después de `Teléfonos de interés` en `Mi Club` solamente cuando la sesión tenga el rol `Member` e `IsMember=true`. La ruta de reservas MUST impedir la consulta de datos personales de reservas si la sesión no cumple ambos requisitos, aplicando el patrón de navegación de sesión existente. Toda operación personal MUST usar el token Bearer de la sesión, y la respuesta de autorización de la API MUST ser la decisión final.

#### Scenario: Socio real abre Mi Club
- **WHEN** una persona autenticada con rol `Member` e `IsMember=true` accede a `Mi Club`
- **THEN** el sistema MUST mostrar la opción `Reservas de material` inmediatamente después de `Teléfonos de interés`

#### Scenario: Persona sin condición de socio abre Mi Club
- **WHEN** una persona no autenticada, sin rol `Member` o con `IsMember=false` accede a `Mi Club`
- **THEN** el sistema MUST no mostrar la opción de reservas de material

#### Scenario: Persona sin acceso navega a la ruta de reservas
- **WHEN** una persona no autenticada, sin rol `Member` o con `IsMember=false` intenta abrir una ruta de reservas de material
- **THEN** el sistema MUST redirigirla mediante el flujo de sesión aplicable sin solicitar reservas ni revelar datos de reservas

#### Scenario: La API rechaza el acceso personal
- **WHEN** una solicitud de reservas personales recibe `401 Unauthorized` o `403 Forbidden`
- **THEN** el sistema MUST iniciar el flujo de sesión aplicable para `401` o impedir nuevos reintentos y explicar la falta de condición de socio para `403`

### Requirement: El socio MUST consultar sus reservas en tarjetas compactas ordenadas
El sistema MUST obtener el listado con `GET /api/inventario-reservas/me` y MUST presentarlo en tarjetas individuales dispuestas verticalmente. Las tarjetas MUST ordenarse por fecha de solicitud descendente, de modo que la solicitud más reciente aparezca primero, incluso si la respuesta no llega ordenada. Cada tarjeta MUST mostrar exclusivamente el material solicitado y sus unidades, la fecha de solicitud, la fecha prevista de recogida y el estado explícito devuelto por la API; MUST NOT derivar el estado de fechas ni confirmaciones ni exponer en el listado el resto de fechas de la reserva.

#### Scenario: Listado con varias solicitudes
- **WHEN** el socio abre sus reservas y la API devuelve solicitudes con fechas de solicitud distintas
- **THEN** el sistema MUST mostrarlas como tarjetas verticales desde la solicitud más reciente hasta la más antigua
- **AND** cada tarjeta MUST mostrar material y unidades, fecha de solicitud, recogida prevista y estado

#### Scenario: Reserva con fechas efectivas y confirmaciones
- **WHEN** el listado contiene una reserva que también incluye retirada, devolución o confirmaciones
- **THEN** la tarjeta MUST conservar el estado explícito de la API
- **AND** MUST NOT mostrar esas fechas o confirmaciones adicionales en la tarjeta

#### Scenario: Listado sin reservas
- **WHEN** la consulta personal termina correctamente sin reservas
- **THEN** el sistema MUST mostrar un estado vacío accesible sin presentar datos de reserva anteriores

### Requirement: El socio MUST acceder al detalle de una reserva propia
Al seleccionar una tarjeta, el sistema MUST cargar el recurso seleccionado mediante `GET /api/inventario-reservas/me/{reservaId}` antes de presentar sus datos y acciones. El detalle MUST presentar de forma organizada el material, unidades, estado y los datos de la propia solicitud necesarios para comprender su planificación, incluidas la retirada prevista y la devolución estimada. El sistema MUST usar exclusivamente el identificador de la reserva seleccionada y MUST NOT permitir elegir ni consultar el identificador de otro socio.

#### Scenario: Socio abre el detalle de una tarjeta
- **WHEN** el socio selecciona una reserva mostrada en su listado
- **THEN** el sistema MUST recuperar el detalle mediante la ruta personal de esa reserva
- **AND** MUST mostrar el material, unidades, estado, retirada prevista y devolución estimada recibidos

#### Scenario: La reserva no está disponible para la identidad actual
- **WHEN** la consulta del detalle recibe `404 Not Found`
- **THEN** el sistema MUST retirar de la vista el detalle seleccionado y volver al listado sin conservar ni revelar datos de una reserva ajena o inexistente

### Requirement: El socio MUST poder iniciar una nueva solicitud desde el listado
El listado MUST incluir un botón flotante, situado a la derecha y distinguible mediante el símbolo `+`, que abra la vista de solicitud de material. El formulario MUST obtener el catálogo autenticado con `GET /api/inventario-materiales`, MUST permitir seleccionar solamente materiales activos y MUST mostrar para el material elegido la cantidad disponible calculada por el servidor como información de la consulta. Esa cantidad MUST indicar que puede cambiar antes de enviar la solicitud y MUST NOT sustituir la validación de la API. El formulario MUST enviar el alta a `POST /api/inventario-reservas/me` con el identificador de material, cantidad, retirada prevista, devolución estimada y observaciones opcionales. Tras un `201 Created`, el sistema MUST recargar las reservas antes de presentar el resultado confirmado.

#### Scenario: Socio abre una nueva solicitud
- **WHEN** el socio pulsa el botón flotante `+` del listado
- **THEN** el sistema MUST abrir un formulario de solicitud y cargar el catálogo autenticado de materiales
- **AND** MUST excluir de la selección los materiales inactivos

#### Scenario: Socio selecciona un material activo
- **WHEN** el socio elige un material activo en el formulario
- **THEN** el sistema MUST mostrar las unidades disponibles recibidas del servidor para ese material
- **AND** MUST explicar que se volverán a validar al enviar la solicitud

#### Scenario: Socio crea una solicitud válida
- **WHEN** el socio envía material activo, cantidad y fechas válidas
- **THEN** el sistema MUST crear la solicitud mediante la ruta personal de alta
- **AND** tras recibir `201 Created` MUST recargar el listado antes de confirmar visualmente la nueva reserva

### Requirement: El formulario de solicitud MUST validar datos sin sustituir a la API
El formulario MUST exigir un material con identificador positivo, una cantidad entera positiva, fecha prevista de recogida y fecha estimada de devolución en formato de fecha, y MUST rechazar localmente una devolución anterior a la recogida prevista. Las observaciones MUST ser opcionales, admitir saltos de línea, limitarse a 2.000 caracteres y MUST NOT incorporar caracteres de control no permitidos. El sistema MAY presentar exclusivamente la cantidad disponible recibida del servidor, etiquetada como una fotografía informativa; MUST NOT derivarla a partir de la cantidad total ni prometerla como disponibilidad garantizada, pues la comprobación definitiva corresponde a la API.

#### Scenario: Devolución estimada anterior a la recogida
- **WHEN** el socio informa una fecha de devolución anterior a la fecha de recogida prevista
- **THEN** el sistema MUST bloquear el envío y asociar un mensaje de validación al campo de devolución

#### Scenario: Cantidad inválida
- **WHEN** el socio introduce una cantidad no entera, cero o negativa
- **THEN** el sistema MUST bloquear el envío y mostrar una validación comprensible de cantidad positiva

#### Scenario: El servidor detecta que ya no hay disponibilidad
- **WHEN** el alta recibe `409 Conflict` por disponibilidad, material inactivo o cambio concurrente
- **THEN** el sistema MUST conservar los datos introducidos, mostrar el detalle seguro del conflicto y recargar el catálogo o listado pertinente antes de permitir reintentar

### Requirement: Las solicitudes creadas MUST NOT poder editarse desde el cliente
El sistema MUST NOT ofrecer una acción de edición ni enviar `PUT /api/inventario-reservas/me/{reservaId}` para ninguna reserva. Para cambiar material, cantidad, fechas u observaciones de una solicitud pendiente, el socio MUST cancelarla y crear una nueva solicitud, de modo que el servidor vuelva a evaluar la disponibilidad.

#### Scenario: Socio abre una solicitud pendiente
- **WHEN** el detalle de una reserva tiene estado `Solicitada`
- **THEN** el sistema MUST no mostrar controles de edición
- **AND** MUST indicar la cancelación y una nueva solicitud como alternativa para modificar sus datos

### Requirement: El detalle MUST ofrecer solo las decisiones de socio permitidas por estado
Para una reserva `Solicitada`, el detalle MUST ofrecer exclusivamente la acción de cancelar; para una reserva `Retirada`, MUST ofrecer exclusivamente la confirmación de devolución del socio; y para una reserva `Entregada` o `Cancelada`, MUST ser de solo lectura. La cancelación MUST usar `DELETE /api/inventario-reservas/me/{reservaId}` y la confirmación MUST usar `POST /api/inventario-reservas/me/{reservaId}/confirmacion-devolucion` sin cuerpo. Ambas decisiones MUST requerir confirmación explícita y, tras un `204 No Content`, MUST recargar el detalle y el listado antes de mostrar el resultado.

#### Scenario: Socio cancela una solicitud pendiente
- **WHEN** el socio confirma la cancelación de una reserva `Solicitada`
- **THEN** el sistema MUST enviar la cancelación a la ruta personal correspondiente
- **AND** tras `204 No Content` MUST recargar el detalle y el listado antes de presentar el estado `Cancelada` confirmado por la API

#### Scenario: Socio confirma la devolución de material retirado
- **WHEN** el socio confirma que ha devuelto una reserva `Retirada`
- **THEN** el sistema MUST enviar la confirmación sin cuerpo a la ruta personal correspondiente
- **AND** tras `204 No Content` MUST recargar el detalle y conservar el estado recibido, incluso si todavía falta la confirmación de Administración

#### Scenario: Socio consulta una reserva final
- **WHEN** el detalle recibido tiene estado `Entregada` o `Cancelada`
- **THEN** el sistema MUST mostrarlo en solo lectura y MUST NOT ofrecer cancelación ni confirmación de devolución

### Requirement: La vista MUST tratar fallos y concurrencia de forma segura y recuperable
El sistema MUST deshabilitar temporalmente la acción en curso y MUST NOT actualizar de forma optimista el estado ni la disponibilidad. Ante `400 Bad Request`, MUST conservar el formulario o la decisión y mostrar los errores de validación junto a sus campos. Ante `409 Conflict`, MUST conservar la entrada y recargar el recurso afectado antes de permitir reintentar. Ante errores de red, tiempo de espera o `5xx`, MUST mostrar un error recuperable sin exponer detalles internos ni afirmar que la operación se completó. El cliente MUST NOT reintentar automáticamente operaciones que cambian reservas.

#### Scenario: Error de validación de la API al crear
- **WHEN** el alta recibe `400 Bad Request` con errores de validación
- **THEN** el sistema MUST mantener los valores del formulario y asociar los errores a los campos aplicables

#### Scenario: Fallo temporal al confirmar una devolución
- **WHEN** la confirmación recibe un error de red, tiempo de espera o `5xx`
- **THEN** el sistema MUST mostrar un mensaje recuperable sin confirmar visualmente la devolución
- **AND** MUST no reintentar la operación de forma automática

### Requirement: Las vistas de reservas MUST seguir el lenguaje visual responsive de la aplicación
Las vistas MUST usar componentes y clases DaisyUI, recursos localizados y estilos SCSS globales de la aplicación. En pantallas estrechas, las tarjetas, el detalle, el formulario y el botón flotante MUST poder consultarse y operarse sin desbordamiento horizontal ni ocultar la información esencial; en pantallas amplias, MUST conservar jerarquía visual y un ancho de lectura cómodo. Los estados de carga, vacío y error MUST ser accesibles y distinguibles de datos confirmados.

#### Scenario: Socio consulta o crea una reserva desde un móvil
- **WHEN** un socio abre el listado, detalle o formulario en una pantalla estrecha
- **THEN** el sistema MUST reorganizar su contenido y mantener la acción flotante accesible sin desbordamiento horizontal

#### Scenario: Carga o fallo de las reservas
- **WHEN** la consulta de reservas o del catálogo está en curso, falla o termina sin resultados
- **THEN** el sistema MUST mostrar un estado de carga, error o vacío accesible
- **AND** MUST no mostrar datos anteriores como si estuvieran confirmados
