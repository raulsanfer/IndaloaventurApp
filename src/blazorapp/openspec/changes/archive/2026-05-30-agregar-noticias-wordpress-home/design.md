## Context

La aplicación ya dispone de una Home autenticada, pero su contenido principal todavía es estático y no refleja las publicaciones reales del club. El `endpoints.json` actualizado expone dos endpoints relevantes para WordPress:

- `GET /api/wordpress/posts` con soporte de `page`, `pageSize` y `search`
- `GET /api/wordpress/posts/{slug}` para obtener el detalle de un post

Además, el contrato `WordPressPostDto` ya incluye los campos necesarios para una experiencia básica de listados y detalle: `slug`, `titulo`, `resumen`, `contenido`, `imagenDestacadaUrl`, `enlace` y `fechaPublicacionUtc`. El cambio afecta a Home, navegación de páginas, capa de servicios frontend y estilos globales para un carrusel horizontal táctil.

## Goals / Non-Goals

**Goals:**
- Mostrar en Home los últimos 10 posts publicados obtenidos desde `GET /api/wordpress/posts`.
- Presentarlos en una tira horizontal desplazable con touch, donde cada card muestre imagen destacada y título.
- Navegar desde cada item a una nueva pantalla de detalle por `slug`.
- Cargar en la pantalla de detalle la información completa del post desde `GET /api/wordpress/posts/{slug}`.
- Insertar el bloque de noticias siempre al final de `HomeDashboard`.

**Non-Goals:**
- Implementar filtros, paginación visible o búsqueda de noticias en esta iteración.
- Introducir edición de posts, favoritos o bookmarking.
- Replicar toda la maquetación pública completa de WordPress dentro de la app.
- Cambiar el contrato backend o añadir nuevos endpoints para noticias.

## Decisions

1. Servicio frontend dedicado para WordPress
- Decision: crear un contrato desacoplado de UI y un cliente HTTP tipado para noticias de WordPress, siguiendo el patrón ya usado por `MemberProfileApiClient`.
- Rationale: mantiene la UI limpia, facilita pruebas y alinea el consumo de API con el resto de la app.
- Alternatives considered:
  - Llamadas HTTP directas desde `HomeDashboard`: rechazado por mezclar transporte y presentación.

2. El listado Home usa `page=1&pageSize=10`
- Decision: solicitar siempre la primera página con tamaño 10 para poblar el carrusel inferior.
- Rationale: coincide con el requisito de “últimos 10 posts” y aprovecha el contrato existente sin crear lógica adicional de orden o paginado visible.
- Alternatives considered:
  - Pedir más posts y truncar en cliente: rechazado por trabajo y tráfico innecesarios.

3. Navegación de detalle por `slug`
- Decision: la nueva pantalla de detalle se identificará por `slug` en la ruta y consumirá `GET /api/wordpress/posts/{slug}`.
- Rationale: el `slug` ya es parte del contrato, es amigable para rutas y evita exponer internamente la dependencia del `id` numérico para navegación.
- Alternatives considered:
  - Navegar por `id`: rechazado porque el endpoint de detalle actualizado está definido por `slug`.

4. Carrusel horizontal implementado con scroll nativo y touch
- Decision: usar contenedor con `overflow-x: auto`, `scroll-snap` y cards de ancho fijo/responsivo.
- Rationale: ofrece una experiencia táctil natural, ligera y fácil de mantener sin depender de librerías de carrusel.
- Alternatives considered:
  - Librería JS de slider: rechazada por complejidad extra y dependencia innecesaria.

5. El detalle renderiza contenido completo con fallback seguro
- Decision: la vista de detalle mostrará imagen, título, fecha y contenido completo del post, con manejo defensivo si faltan imagen o resumen.
- Rationale: el contrato puede traer campos opcionales, así que la UI no debe romper cuando falte media destacada.
- Alternatives considered:
  - Forzar imagen obligatoria: rechazado porque `imagenDestacadaUrl` es nullable.

## Risks / Trade-offs

- [Contenido HTML rico en `contenido` puede requerir render controlado] → Mitigacion: contemplar render seguro del contenido y definir explícitamente cómo se mostrará en implementación.
- [La API devuelve posts pero no expresa explícitamente “status publicado” en la query] → Mitigacion: asumir que el endpoint ya expone posts publicados según el contrato actualizado; si no se verifica en implementación, revisar backend/servicio.
- [Imágenes destacadas ausentes o de tamaño irregular] → Mitigacion: diseñar la card con placeholder/fallback visual y `object-fit`.
- [Home puede crecer visualmente y competir con los bloques actuales] → Mitigacion: colocar noticias al final y mantener jerarquía clara con un título de sección compacto.

## Migration Plan

- Añadir el servicio/frontend models para WordPress.
- Integrar el bloque de noticias al final de Home.
- Crear la nueva ruta/página de detalle por `slug`.
- Añadir estilos globales para carrusel y detalle.
- Rollback simple: retirar el bloque de Home y la nueva página de detalle, manteniendo el resto de la Home intacto.

## Open Questions

- ¿Quieres que en el detalle se renderice también el `resumen` si viene informado, o prefieres ir directamente a imagen + título + contenido completo?
