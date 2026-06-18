## Context

El backend ya implementa autenticación JWT/Identity, CQRS y ProblemDetails, pero aún contiene literales user-facing en inglés (por ejemplo mensajes de validación, conflicto de negocio y autenticación). Como la app no tendrá localización por ahora, se necesita un único idioma de salida consistente: español.

## Goals / Non-Goals

**Goals:**
- Asegurar que cualquier texto expuesto al front desde backend esté en español.
- Estandarizar mensajes de error de autenticación, validación y negocio en español.
- Evitar regresiones agregando validaciones de pruebas sobre contenido textual.
- Definir regla de mantenimiento para no introducir literales user-facing en inglés mientras no haya i18n.

**Non-Goals:**
- Implementar infraestructura de localización o múltiples idiomas.
- Cambiar códigos HTTP o contratos estructurales de respuestas.
- Traducir comentarios internos de código no expuestos al cliente.

## Decisions

1. Alcance de traducción: solo texto user-facing
- Decisión: Refactorizar únicamente cadenas potencialmente visibles en clientes (ProblemDetails `title/detail`, mensajes de validación, mensajes de auth y conflictos funcionales).
- Rationale: Maximiza impacto funcional sin tocar aspectos internos irrelevantes para UX.
- Alternativas:
  - Traducir absolutamente todos los literales del código: más ruido y menor valor inmediato.

2. Idioma único temporal: español
- Decisión: Establecer español como idioma único de salida hasta que exista módulo de localización.
- Rationale: Simplifica consistencia funcional y evita mezcla de idiomas.
- Alternativas:
  - Mantener inglés técnico: incoherente con producto y usuarios objetivo.

3. Verificación por pruebas
- Decisión: Actualizar/añadir tests que validen mensajes esperados en español en escenarios clave.
- Rationale: Previene regresiones silenciosas en nuevos cambios.
- Alternativas:
  - Revisión manual: poco confiable y difícil de sostener.

## Risks / Trade-offs

- [Tests frágiles por texto exacto] -> Mitigación: validar fragmentos estables y mensajes canónicos por categoría.
- [Mensajes hardcodeados en español dificultan futura i18n] -> Mitigación: centralizar textos en constantes/clases de mensajes para migración futura.
- [Cambios breaking para clientes que parsean texto] -> Mitigación: comunicar cambio y mantener inalterados códigos/estructura HTTP.

## Migration Plan

1. Identificar cadenas user-facing en controladores, handlers, validadores y exception mapping.
2. Refactorizar mensajes a español en todos los puntos detectados.
3. Centralizar literales de error comunes donde aplique.
4. Ajustar pruebas unitarias/integración para aserciones en español.
5. Ejecutar suite completa y validar contratos de status code sin cambios.

Rollback:
- Revertir commit de refactor textual manteniendo estructura de endpoints.

## Open Questions

- ¿Preferimos registro formal ("No autorizado") o tono más natural ("No tienes permiso") para todos los mensajes?
- ¿Deseamos definir glosario corto de términos canónicos (p.ej. "regla de negocio", "validación") para futuras features?