## Context

El sistema ya dispone de un servicio WordPress configurable y un endpoint protegido para listar posts. Ese servicio consulta `wp-json/wp/v2/posts` y mapea la respuesta a `WordPressPostDto`, que ya contiene `slug`, `titulo`, `resumen`, `contenido`, enlace y fecha de publicación. La necesidad nueva no es un contrato de datos distinto, sino una forma directa y eficiente de pedir un único post por `slug`.

## Goals / Non-Goals

**Goals:**
- Permitir recuperar un único post de WordPress por `slug`.
- Mantener el mismo DTO de salida ya usado en el listado para evitar contratos duplicados.
- Devolver una semántica HTTP clara cuando el post no exista.
- Reutilizar la configuración y el manejo de errores ya existentes en la integración WordPress.

**Non-Goals:**
- Añadir cache de posts de WordPress.
- Persistir posts de WordPress en base de datos local.
- Modificar el endpoint paginado actual más allá de la reutilización interna que pueda convenir.

## Decisions

- Añadir un nuevo método a `IWordPressService` para buscar un post por `slug` y devolver un único `WordPressPostDto`.
- Consultar el endpoint WordPress existente filtrando por `slug`, ya que eso mantiene una sola fuente de datos y evita crear URLs o payloads alternativos si no hacen falta.
- Si WordPress devuelve colección vacía para ese `slug`, traducirlo a un `404 Not Found` del backend con mensaje user-facing en español.
- Reutilizar `WordPressPostDto` para el detalle, porque ya contiene el contenido completo que el frontend necesita mostrar.

## Risks / Trade-offs

- [WordPress puede devolver más de un elemento para un filtro inesperado] → Mitigar tomando el primer resultado exacto por `slug` y documentando que el `slug` debe identificar el post consumido.
- [El DTO actual incluye contenido completo también en listados] → Mitigar dejando este cambio centrado en la navegación por `slug`; optimizar payloads del listado sería un cambio aparte.
- [Semántica de no encontrado depende del servicio remoto] → Mitigar convirtiendo explícitamente la ausencia de resultados en un comportamiento estable del backend.
