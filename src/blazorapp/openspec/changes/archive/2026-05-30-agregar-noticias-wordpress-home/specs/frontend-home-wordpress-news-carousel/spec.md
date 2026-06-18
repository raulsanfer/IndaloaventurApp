## ADDED Requirements

### Requirement: Home MUST mostrar un carrusel horizontal con los últimos posts de WordPress
El sistema MUST cargar en Home los últimos 10 posts de WordPress desde `GET /api/wordpress/posts` y mostrarlos siempre al final de la página.

#### Scenario: Carga inicial de noticias en Home
- **WHEN** la página Home se renderiza
- **THEN** el sistema MUST solicitar `GET /api/wordpress/posts` con `page = 1`
- **AND** el sistema MUST solicitar `pageSize = 10`
- **AND** el sistema MUST ubicar el bloque de noticias al final de Home

### Requirement: El listado de noticias MUST ser desplazable horizontalmente con touch
El sistema MUST presentar las noticias en un contenedor desplazable horizontalmente que pueda usarse de forma natural en dispositivos táctiles.

#### Scenario: Scroll horizontal de noticias
- **WHEN** el usuario interactúa con el bloque de noticias en un dispositivo táctil o puntero
- **THEN** el sistema MUST permitir desplazamiento horizontal del listado

### Requirement: Cada noticia de Home MUST mostrar imagen destacada y título
El sistema MUST representar cada item del carrusel con la imagen destacada del post y el título visible debajo de dicha imagen.

#### Scenario: Render de una card de noticia
- **WHEN** una noticia se muestra en el carrusel de Home
- **THEN** el sistema MUST mostrar la imagen destacada si existe
- **AND** el sistema MUST mostrar el título debajo de la imagen

### Requirement: El bloque de noticias MUST permitir abrir el detalle del post
El sistema MUST permitir que el usuario entre a una pantalla de detalle al pulsar una noticia del carrusel.

#### Scenario: Navegación desde Home al detalle
- **WHEN** el usuario pulsa una noticia del carrusel
- **THEN** el sistema MUST navegar a la pantalla de detalle correspondiente al post seleccionado
