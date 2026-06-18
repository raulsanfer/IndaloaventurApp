## Context

El backend actual sigue una arquitectura con capas `Api`, `Application`, `Infrastructure` y dominio, usando DI para servicios y configuración desde `appsettings`. No existe integración con WordPress ni endpoints para consultar posts remotos.

La necesidad inmediata es consultar posts de una web WordPress mediante una clave configurable en `appsettings`, sin persistencia local. Al mismo tiempo, se quiere que el diseño permita crecer en el futuro para incluir operaciones de publicación sin rediseño mayor.

## Goals / Non-Goals

**Goals:**
- Definir un servicio inyectable para operaciones con WordPress, empezando por consulta de posts.
- Consumir WordPress vía HTTP usando configuración tipada en `appsettings`.
- Exponer un endpoint backend para obtener posts de forma consistente para clientes.
- Mantener el diseño extensible para futuras operaciones de escritura/publicación.
- No persistir posts consultados en la base de datos local.

**Non-Goals:**
- Implementar publicación/edición/borrado de posts en esta iteración.
- Implementar caché distribuida o almacenamiento de posts consultados.
- Añadir sincronización en segundo plano de contenido WordPress.

## Decisions

1. Contrato orientado a futuro para WordPress
- Decisión: crear una abstracción `IWordPressService` en Application que agrupe operaciones de contenido WordPress, con método inicial de lectura de posts.
- Rationale: permite añadir métodos de publicación en el mismo contrato sin romper consumidores.
- Alternativas:
  - Servicio específico `IWordPressPostsReader`: más simple hoy, pero limita evolución y obliga a refactor posterior.

2. Configuración tipada con `IOptions`
- Decisión: modelar `WordPressOptions` con claves necesarias (`BaseUrl`, `ApiKey`, endpoints relativos y timeouts).
- Rationale: evita strings dispersos, centraliza validación y facilita gestión por entorno.
- Alternativas:
  - Lectura directa de `IConfiguration` en cada clase: acoplamiento alto y menor mantenibilidad.

3. Cliente HTTP en Infrastructure
- Decisión: usar `HttpClient` (preferiblemente typed/named client) para consumir WordPress REST API.
- Rationale: control de timeout, cabeceras y resiliencia sin introducir dependencias extra.
- Alternativas:
  - Cliente SDK externo: más abstracción, pero dependencia adicional innecesaria en esta fase.

4. Endpoint read-only sin persistencia
- Decisión: añadir controlador read-only (`GET`) que delega en el servicio y devuelve DTOs de posts.
- Rationale: cumple necesidad actual y respeta el alcance sin impactar persistencia.
- Alternativas:
  - Sin endpoint y uso interno solo por servicios: no cubre requerimiento de consulta desde cliente.

## Risks / Trade-offs

- [Cambios en contrato WordPress externo] → Mitigación: aislar mapeo de payload en Infrastructure y manejar errores de deserialización de forma controlada.
- [Credenciales o clave mal configuradas] → Mitigación: validar opciones al arrancar y devolver error controlado al cliente.
- [Latencia/fallos del sitio WordPress] → Mitigación: timeout configurable y manejo explícito de errores de red con mensajes user-facing en español.
- [Diseño demasiado genérico para futuro] → Mitigación: mantener contrato simple en esta iteración, agregando solo lo necesario para lectura.

## Migration Plan

1. Añadir configuración `WordPress` en `appsettings` y binding en composición de servicios.
2. Implementar contrato y DTOs de consulta en Application.
3. Implementar cliente/servicio WordPress en Infrastructure.
4. Exponer endpoint `GET` en Api y conectar con servicio.
5. Añadir pruebas unitarias/integración para escenarios de éxito/error.
6. Desplegar sin migraciones de base de datos (no hay cambios de persistencia).

Rollback:
- Revertir registro DI y controlador WordPress; no requiere rollback de datos ni migraciones.

## Open Questions

- ¿El endpoint backend debe requerir autenticación JWT desde el primer día o puede ser público para consumo controlado?
- ¿Qué campos mínimos de post se devolverán (id, slug, título, extracto, fecha, enlace, contenido)?
- ¿La clave de WordPress será cabecera personalizada, bearer token o parámetro de consulta según el sitio objetivo?
