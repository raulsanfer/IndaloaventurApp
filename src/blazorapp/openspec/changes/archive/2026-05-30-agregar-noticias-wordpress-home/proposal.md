## Why

La Home actual muestra bloques estáticos, pero no aprovecha todavía el contenido editorial real disponible desde WordPress. Incorporar un carrusel horizontal de noticias publicadas y una pantalla de detalle permite dar valor inmediato a la portada autenticada y conectar la app con la fuente viva de contenidos del club.

## What Changes

- Se crea un componente de noticias para Home que consume `GET /api/wordpress/posts` y recupera los últimos 10 posts publicados.
- Se presenta ese contenido en un carrusel horizontal desplazable con touch, mostrando en cada item la imagen destacada y el título del post.
- Se añade navegación desde cada noticia hacia una nueva pantalla de detalle.
- Se crea una nueva página de detalle de noticia que consume `GET /api/wordpress/posts/{slug}` y renderiza la información completa del post.
- Se integra el componente de noticias siempre al final de la página `Home`.
- Se actualiza la capa de servicios frontend para consumir los endpoints de WordPress descritos en `openspec/endpoints.json`.

## Capabilities

### New Capabilities
- `frontend-home-wordpress-news-carousel`: Define la carga y presentación horizontal de noticias de WordPress en la parte inferior de Home.
- `frontend-wordpress-post-detail-page`: Define la pantalla de detalle de noticia y su navegación desde el listado de Home.

### Modified Capabilities
- Ninguna.

## Impact

- Afecta a `IndaloaventurApp.SharedUI` en `HomeDashboard` y en los nuevos componentes reutilizables de noticia/listado/detalle.
- Afecta a `IndaloaventurApp.Web.Client` en nuevas rutas/páginas y en la capa de servicios para WordPress.
- Afecta al SCSS/CSS global para soportar scroll horizontal táctil, cards de noticia y layout de detalle.
- Afecta a los recursos localizados ES para títulos, estados de carga y mensajes de error de noticias.
