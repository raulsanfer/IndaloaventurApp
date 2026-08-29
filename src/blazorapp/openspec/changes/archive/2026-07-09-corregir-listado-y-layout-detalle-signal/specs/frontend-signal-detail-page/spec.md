## MODIFIED Requirements

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

## ADDED Requirements

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
