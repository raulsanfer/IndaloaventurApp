# frontend-signal-detail-page Specification

## Purpose
TBD - created by archiving change agregar-detalle-signal-con-tabs. Update Purpose after archive.
## Requirements
### Requirement: El sistema MUST disponer de una pagina de detalle de signal accesible para cualquier usuario autenticado
El sistema MUST ofrecer una pagina dedicada de detalle para cada signal, accesible mediante una ruta estable basada en el identificador de la signal y disponible para usuarios autenticados de cualquier rol que ya puedan consultar el listado.

#### Scenario: Acceso directo al detalle por URL
- **WHEN** un usuario autenticado navega a la ruta de detalle de una signal existente
- **THEN** el sistema MUST cargar la pagina de detalle correspondiente a ese identificador
- **AND** el sistema MUST mantener una estructura coherente con el resto de paginas autenticadas

#### Scenario: Apertura del detalle desde el flujo de signals
- **WHEN** un usuario autenticado accede al detalle de una signal desde la experiencia de `Signals`
- **THEN** el sistema MUST mostrar la misma pagina dedicada de detalle
- **AND** el sistema MUST conservar el contexto de navegacion de la funcionalidad

### Requirement: La pagina de detalle MUST recuperar y manejar el detalle completo de una signal por identificador
El sistema MUST obtener los datos del detalle desde una capa frontend desacoplada del componente visual y MUST informar correctamente de los estados de carga, error y recurso no disponible.

#### Scenario: Carga correcta del detalle
- **WHEN** la pagina de detalle se inicializa con un identificador valido
- **THEN** el sistema MUST solicitar la informacion de la signal correspondiente
- **AND** el sistema MUST renderizar el contenido del detalle cuando la carga finalice correctamente

#### Scenario: Error al recuperar el detalle
- **WHEN** el servicio de detalle no puede completarse correctamente
- **THEN** el sistema MUST mostrar un estado de error comprensible
- **AND** el sistema MUST evitar mostrar informacion parcial o inconsistente de otra signal

#### Scenario: Signal no encontrada
- **WHEN** el identificador solicitado no corresponde a ninguna signal disponible
- **THEN** el sistema MUST mostrar un estado no encontrado o vacio comprensible
- **AND** el sistema MUST no dejar la interfaz en un estado roto

### Requirement: La pagina de detalle MUST conservar el breadcrumb actual
El sistema MUST mantener el breadcrumb existente del detalle de signal sin alterar su funcion, su presencia ni su papel como contexto de navegacion, aunque se rediseñe el resto de la pagina.

#### Scenario: Render del detalle con breadcrumb
- **WHEN** el detalle de una signal se muestra correctamente
- **THEN** el sistema MUST mantener visible el breadcrumb actual
- **AND** el sistema MUST no sustituirlo por otra cabecera de navegacion incompatible

#### Scenario: Estados especiales con breadcrumb persistente
- **WHEN** la pagina entra en estado de carga, error o no encontrado
- **THEN** el sistema MUST preservar el breadcrumb actual siempre que la estructura de la pagina siga renderizada
- **AND** el sistema MUST mantener el contexto de navegacion del usuario

### Requirement: La pagina de detalle MUST adoptar el layout del boceto usando DaisyUI preferentemente
El sistema MUST reorganizar la pagina de detalle segun el boceto de `openspec/design/signal/detail`, priorizando componentes DaisyUI y evitando depender de estilos CSS propios salvo ajustes estructurales minimos permitidos por la arquitectura del proyecto.

#### Scenario: Cabecera y resumen alineados con el boceto
- **WHEN** el detalle de la signal se muestra correctamente
- **THEN** el sistema MUST presentar una cabecera con titulo y metadatos principales
- **AND** el sistema MUST mostrar categoria, estado y bloques de resumen en una jerarquia visual equivalente al boceto
- **AND** el texto derivado de `GetHeaderSummary()` MUST renderizarse con un peso visual menor que el titulo principal

#### Scenario: Rejilla de overview densa en movil
- **WHEN** la pagina de detalle se renderiza en un ancho de movil
- **THEN** `signal-detail__overview-grid` MUST mantener como minimo dos cards por fila
- **AND** el sistema MUST ajustar spacing, margenes o dimensiones auxiliares de cada `article` antes de degradar la rejilla a una sola columna

#### Scenario: Rejilla estable en viewport de 412px
- **WHEN** la pagina de detalle se renderiza en un viewport cercano a `412px` de ancho
- **THEN** `signal-detail__overview-grid` MUST seguir mostrando dos cards por fila
- **AND** cada `article` MUST reducir sus margenes o espacios excesivos para no forzar el apilado vertical

#### Scenario: Restriccion de implementacion visual
- **WHEN** se implemente el rediseño del detalle
- **THEN** el sistema MUST resolver la mayor parte de la interfaz con DaisyUI y utilidades existentes
- **AND** el sistema MUST no basar el resultado en estilos inline o CSS ad hoc que reemplacen componentes que DaisyUI ya cubre

### Requirement: La pagina de detalle MUST organizar la informacion en tabs DaisyUI
El sistema MUST presentar el detalle mediante una interfaz de tabs basada en DaisyUI con dos pestañas visibles: `Datos de la signal` y `Mapa/Ubicacion`, manteniendo la nueva jerarquia visual del rediseño.

#### Scenario: Render inicial de las tabs
- **WHEN** el detalle de la signal se muestra correctamente
- **THEN** el sistema MUST renderizar dos tabs para `Datos de la signal` y `Mapa/Ubicacion`
- **AND** el sistema MUST no mostrar `Comentarios` como una pestaña adicional
- **AND** el sistema MUST aplicar estilos coherentes con DaisyUI y el lenguaje visual existente

#### Scenario: Cambio entre tabs
- **WHEN** el usuario cambia de una pestaña a otra
- **THEN** el sistema MUST mostrar solo el contenido asociado a la pestaña activa
- **AND** el sistema MUST mantener visible la navegacion de tabs sin recargar la pagina

### Requirement: El tab de datos MUST mostrar la informacion principal de la signal
El sistema MUST mostrar en el tab `Datos de la signal` la informacion principal disponible para lectura, incluyendo al menos descripcion, categoria, fecha relevante, estado y metadatos significativos cuando existan, sin mezclar en ese mismo bloque final los comentarios.

#### Scenario: Signal con informacion completa
- **WHEN** una signal dispone de descripcion, categoria, estado y metadatos
- **THEN** el sistema MUST mostrarlos de forma legible dentro del tab de datos
- **AND** el sistema MUST evitar duplicidades visuales innecesarias

#### Scenario: Campos opcionales ausentes
- **WHEN** una signal no dispone de alguno de los metadatos opcionales
- **THEN** el sistema MUST mantener un layout estable y comprensible
- **AND** el sistema MUST no inventar contenido no proporcionado por el servicio

### Requirement: El tab de mapa o ubicacion MUST usar latitud y longitud cuando existan
El sistema MUST utilizar la latitud y longitud de la signal para representar su ubicacion en el tab `Mapa/Ubicacion` cuando ambas coordenadas existan y MUST mostrar un estado vacio comprensible cuando no esten disponibles.

#### Scenario: Signal con coordenadas
- **WHEN** la signal dispone de latitud y longitud validas
- **THEN** el sistema MUST mostrar una representacion de ubicacion basada en esas coordenadas
- **AND** el sistema MUST hacer visible la relacion entre el punto mostrado y la signal consultada

#### Scenario: Signal sin coordenadas
- **WHEN** la signal no dispone de latitud y longitud utilizables
- **THEN** el sistema MUST mostrar un estado vacio comprensible en el tab de ubicacion
- **AND** el sistema MUST mantener disponible la estructura de tabs del detalle

### Requirement: La pagina de detalle MUST mostrar comentarios fuera del sistema de tabs
El sistema MUST renderizar el bloque de `Comentarios` debajo del bloque de tabs del detalle, como una seccion fija del flujo de lectura y no como una pestaña independiente.

#### Scenario: Signal con comentarios
- **WHEN** una signal dispone de comentarios asociados
- **THEN** el sistema MUST mostrar el bloque `Comentarios` debajo del contenido tabulado
- **AND** el sistema MUST listar los comentarios sin exigir cambio de tab para leerlos

#### Scenario: Signal sin comentarios
- **WHEN** una signal no dispone de comentarios
- **THEN** el sistema MUST seguir mostrando la seccion `Comentarios` debajo del bloque tabulado
- **AND** el sistema MUST comunicar un estado vacio comprensible

### Requirement: La pagina de detalle MUST dejar las etiquetas al final del flujo
El sistema MUST renderizar las etiquetas de la signal al final de la pagina de detalle, por debajo del bloque de comentarios.

#### Scenario: Signal con etiquetas
- **WHEN** una signal dispone de etiquetas
- **THEN** el sistema MUST mostrar el bloque de etiquetas despues del bloque de comentarios
- **AND** el sistema MUST mantener una lista legible y estable de badges o elementos equivalentes

#### Scenario: Signal sin etiquetas
- **WHEN** una signal no dispone de etiquetas
- **THEN** el sistema MUST mantener el bloque final de etiquetas en una posicion coherente al final de la pagina
- **AND** el sistema MUST mostrar un estado vacio comprensible

### Requirement: La pagina de detalle MUST recuperar las imagenes de signal como recurso independiente
El sistema MUST obtener las imágenes de una signal mediante el endpoint dedicado de imágenes y MUST tratarlas como un recurso separado del detalle base, de forma que el contenido principal del detalle no dependa del éxito de esa carga secundaria.

#### Scenario: Detalle con imagenes disponibles
- **WHEN** el usuario abre el detalle de una signal existente y el recurso de imágenes responde correctamente
- **THEN** el sistema MUST cargar el detalle base y las imágenes como operaciones diferenciadas
- **AND** el sistema MUST renderizar las fotos disponibles dentro de la página de detalle sin alterar el breadcrumb ni el resto de bloques ya definidos

#### Scenario: Detalle con una sola foto utilizable
- **WHEN** el recurso de imágenes devuelve solo `Foto1` o solo una foto válida utilizable
- **THEN** el sistema MUST mostrar la foto disponible
- **AND** el sistema MUST mantener un layout estable sin inventar una segunda imagen inexistente

### Requirement: La pagina de detalle MUST aislar errores y ausencias de imagenes
El sistema MUST permitir que la página de detalle siga siendo usable cuando la carga de imágenes falle o no devuelva fotos utilizables, mostrando un estado parcial comprensible para el bloque de imágenes.

#### Scenario: Error parcial al cargar imagenes
- **WHEN** el detalle base de la signal se recupera correctamente pero la carga de imágenes falla
- **THEN** el sistema MUST mantener visible la información principal de la signal
- **AND** el sistema MUST mostrar en el bloque de imágenes un placeholder o mensaje breve de error sin convertir toda la página en estado fallido

#### Scenario: Signal sin fotos utilizables
- **WHEN** el recurso de imágenes responde correctamente pero no contiene fotos utilizables
- **THEN** el sistema MUST mostrar un estado vacío comprensible en el bloque de imágenes
- **AND** el sistema MUST mantener disponible el resto del detalle y sus tabs

### Requirement: La pagina de detalle MUST mostrar la accion de comentar dentro del bloque de comentarios solo a perfiles autorizados
La pagina de detalle MUST mostrar una accion visible `Anadir comentario` dentro del bloque `Comentarios` unicamente para usuarios autenticados con rol `Admin` o con rol `Member` y claim `IsMember = true`.

#### Scenario: Accion visible para Admin
- **WHEN** un usuario autenticado con rol `Admin` visualiza el detalle de una signal
- **THEN** el sistema MUST mostrar la accion `Anadir comentario` dentro de la seccion `Comentarios`
- **AND** el sistema MUST mantener el resto del detalle sin cambios de navegacion

#### Scenario: Accion visible para Member con IsMember verdadero
- **WHEN** un usuario autenticado con rol `Member` y claim `IsMember = true` visualiza el detalle de una signal
- **THEN** el sistema MUST mostrar la accion `Anadir comentario` dentro de la seccion `Comentarios`

#### Scenario: Accion oculta para perfiles no autorizados
- **WHEN** un usuario autenticado que no cumple la regla de permiso visualiza el detalle de una signal
- **THEN** el sistema MUST no mostrar la accion `Anadir comentario`
- **AND** el sistema MUST seguir mostrando la lista de comentarios en modo de consulta

### Requirement: La pagina de detalle MUST mostrar acceso de edicion solo para la signal propia
La pagina de detalle MUST mostrar una accion visible de `Editar` unicamente cuando la signal cargada pertenezca al usuario autenticado y MUST ocultarla para cualquier signal ajena.

#### Scenario: Propietario con accion de edicion disponible
- **WHEN** un usuario autenticado visualiza el detalle de una signal cuya autoria coincide con su propia sesion
- **THEN** el sistema MUST mostrar una accion visible de `Editar` dentro de la experiencia de detalle
- **AND** el sistema MUST mantener esa accion asociada a la signal concreta que se esta consultando

#### Scenario: Usuario no propietario sin accion de edicion
- **WHEN** un usuario autenticado visualiza el detalle de una signal creada por otra persona
- **THEN** el sistema MUST no mostrar la accion `Editar`
- **AND** el sistema MUST mantener el resto del detalle en modo de solo lectura

