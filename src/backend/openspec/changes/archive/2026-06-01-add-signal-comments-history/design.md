## Context

La capacidad actual de `Signal` cubre alta, edicion y busqueda de incidencias, pero no permite enriquecer una incidencia existente con observaciones posteriores de otros usuarios. El nuevo requerimiento introduce un historico colaborativo que debe encajar en la arquitectura actual (API REST + CQRS + EF Core + JWT), reutilizando las mismas reglas de autenticacion y evitando aumentar demasiado el peso de las consultas generales de signals.

El cambio afecta a varias capas: modelo de dominio, persistencia relacional, casos de uso CQRS, contratos API y pruebas. Tambien conviene preservar un alcance acotado: no habra hilos de respuestas, ni fotos, ni operaciones de edicion/eliminacion de comentarios en esta fase.

## Goals / Non-Goals

**Goals:**
- Permitir que cualquier usuario autenticado que pueda consultar `Signal` anada comentarios a una incidencia existente.
- Mantener un historico cronologico por `Signal` con autor, fecha/hora y texto del comentario.
- Exponer una lectura dedicada del historial para no sobrecargar necesariamente las consultas generales de busqueda de `Signal`.
- Garantizar integridad referencial entre comentario y `Signal`, y consistencia en auditoria minima del comentario.

**Non-Goals:**
- Soportar comentarios anidados, respuestas o menciones.
- Permitir fotos, archivos adjuntos o reacciones en comentarios.
- Implementar edicion o borrado de comentarios en esta fase.
- Cambiar las reglas existentes de creacion, edicion o busqueda base de `Signal`.

## Decisions

1. Crear una entidad hija `SignalComment` relacionada 1:N con `Signal`.
- Rationale: el historico tiene ciclo de vida propio, cardinalidad variable y no encaja bien como campos embebidos dentro de `Signal`.
- Alternatives considered:
  - Guardar comentarios serializados en un campo JSON de `Signal`: descartado por peor capacidad de consulta, validacion e integridad.

2. Exponer endpoints dedicados para alta y consulta de comentarios por `Signal`.
- Rationale: separa claramente la gestion del historico del payload principal de busqueda de signals y evita engordar respuestas existentes sin necesidad.
- Alternatives considered:
  - Incluir siempre comentarios en `signal-search`: descartado por impacto de rendimiento y sobretransporte para listados.

3. Mantener autorizacion alineada con la capacidad de lectura de signals: cualquier usuario autenticado puede consultar y anadir comentarios.
- Rationale: el usuario describio comentarios del resto de usuarios que visualizan la incidencia; reutilizar la misma barrera de acceso evita reglas inconsistentes.
- Alternatives considered:
  - Restringir comentarios a `Admin` y `Member`: descartado salvo que negocio defina posteriormente una limitacion adicional.

4. Tratar los comentarios como append-only en esta fase.
- Rationale: simplifica trazabilidad e historico, y reduce discusiones sobre moderacion/versionado.
- Alternatives considered:
  - Permitir edicion o borrado: descartado por ampliar demasiado alcance funcional y de seguridad.

## Risks / Trade-offs

- [Riesgo] El volumen de comentarios por `Signal` puede crecer y afectar la lectura del historial. -> Mitigacion: definir consulta dedicada y dejar preparada futura paginacion si hiciera falta.
- [Riesgo] Permitir comentar a cualquier autenticado puede requerir moderacion posterior. -> Mitigacion: dejar el modelo acotado y sin edicion; futuras reglas de negocio podran anadir control adicional.
- [Trade-off] No incluir comentarios en la busqueda general reduce carga de listados, pero obliga a una llamada adicional para ver historial. -> Mitigacion: documentar el endpoint dedicado de comentarios.

## Migration Plan

1. Anadir nueva tabla de comentarios de `Signal` con FK a `Signals` e indices por `SignalId` y fecha.
2. Incorporar repositorios, entidad y casos de uso CQRS para crear y listar comentarios.
3. Exponer endpoints autenticados para `POST` y `GET` de comentarios por `Signal`.
4. Ejecutar pruebas unitarias e integracion de los nuevos flujos junto con regresion basica de `Signal`.
5. Rollback: revertir despliegue de aplicacion y migracion asociada si se detectan fallos antes de poblar comentarios productivos.

## Open Questions

- Confirmar si el historial debe devolverse en orden ascendente (mas antiguo primero) o descendente (mas reciente primero); esta propuesta asume orden cronologico ascendente.
- Confirmar si existe un limite funcional deseado para la longitud maxima del texto del comentario; en ausencia de definicion, se aplicara una validacion razonable de backend durante implementacion.
