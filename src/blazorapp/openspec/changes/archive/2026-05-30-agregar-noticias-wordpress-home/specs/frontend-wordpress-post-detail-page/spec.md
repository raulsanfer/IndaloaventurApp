## ADDED Requirements

### Requirement: El sistema MUST disponer de una pantalla de detalle para posts de WordPress
El sistema MUST ofrecer una nueva pantalla de detalle para noticias de WordPress accesible desde el carrusel de Home.

#### Scenario: Ruta de detalle disponible
- **WHEN** el usuario navega desde una noticia de Home
- **THEN** el sistema MUST cargar una pantalla de detalle específica para el post seleccionado

### Requirement: El detalle MUST cargarse por slug desde el endpoint dedicado
El sistema MUST obtener la información completa del post mediante `GET /api/wordpress/posts/{slug}`.

#### Scenario: Carga del detalle por slug
- **WHEN** la pantalla de detalle se abre para un post concreto
- **THEN** el sistema MUST invocar `GET /api/wordpress/posts/{slug}` usando el slug del post seleccionado

### Requirement: El detalle MUST mostrar la información completa del post
El sistema MUST renderizar la información completa disponible del post para lectura dentro de la app.

#### Scenario: Render del contenido del post
- **WHEN** el detalle del post se carga correctamente
- **THEN** el sistema MUST mostrar el título del post
- **AND** el sistema MUST mostrar el contenido completo del post
- **AND** el sistema MUST mostrar la imagen destacada cuando exista

### Requirement: El sistema MUST manejar estados de carga y error en noticias
El sistema MUST mostrar estados comprensibles cuando el listado o el detalle de noticias no puedan cargarse correctamente.

#### Scenario: Error al cargar el detalle
- **WHEN** el endpoint de detalle devuelve error o no puede completarse
- **THEN** el sistema MUST mostrar un estado de error comprensible para el usuario
- **AND** el sistema MUST evitar dejar la pantalla en un estado inconsistente
