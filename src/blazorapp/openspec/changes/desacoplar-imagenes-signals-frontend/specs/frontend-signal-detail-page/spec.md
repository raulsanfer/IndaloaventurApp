## ADDED Requirements

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
