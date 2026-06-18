## Context

`Signal` se usa hoy como modelo principal para alta, edicion, busqueda y detalle de incidencias de senderos. El agregado de dominio, los comandos CQRS, `SignalDto`, los contratos HTTP y la configuracion EF Core comparten el supuesto de que solo existe `Descripcion` como texto descriptivo principal.

Anadir `Titulo` es un cambio transversal porque afecta a:
- El agregado `Signal` y sus validaciones de creacion/edicion.
- Los contratos de entrada (`CreateSignal`, `UpdateSignal`) y de salida (`SignalDto`).
- La persistencia SQL y la migracion de datos existentes.
- Las pruebas unitarias e integracion que hoy crean o validan `Signal` sin este campo.

Tambien hay una restriccion funcional relevante: el sistema ya contiene registros persistidos de `Signal`, por lo que la nueva columna no puede introducirse de forma que deje filas antiguas invalidas o haga fallar lecturas previas al rellenado.

## Goals / Non-Goals

**Goals:**
- Incorporar `Titulo` como campo obligatorio y separado de `Descripcion` en `Signal`.
- Exponer `Titulo` en los payloads de lectura reutilizados por listado y detalle.
- Mantener compatibilidad de datos existentes mediante una migracion segura y determinista.
- Ajustar validaciones y tests para que el nuevo contrato quede cubierto end-to-end.

**Non-Goals:**
- Anadir nuevos filtros de busqueda por `Titulo`.
- Redisenar la pantalla o experiencia de detalle de `Signal`.
- Cambiar la semantica actual de `Descripcion`, `Tags`, fotos o auditoria.

## Decisions

1. `Titulo` se modelara como texto obligatorio independiente de `Descripcion`.
- Rationale: el objetivo del cambio es separar el resumen visible del contenido explicativo. Mantener ambos campos evita sobrecargar listados con textos largos y conserva la descripcion completa.
- Alternative considered: reutilizar `Descripcion` como pseudo-titulo en cliente. Se descarta porque no resuelve el problema de modelado ni evita ambiguedad entre resumen y detalle.

2. `Titulo` se anadira al agregado, comandos, requests API y `SignalDto`.
- Rationale: el campo debe existir de forma coherente en todas las capas que crean, editan o exponen `Signal`. `SignalDto` ya se reutiliza en busqueda y detalle, por lo que centralizar ahi el nuevo campo evita contratos divergentes.
- Alternative considered: anadir `Titulo` solo en persistencia y detalle. Se descarta porque dejaria inconsistencia entre create/update y lectura, y no resolveria el caso principal de uso en listados.

3. La migracion poblara `Titulo` de registros existentes copiando el valor actual de `Descripcion` antes de imponer `NOT NULL`.
- Rationale: preserva informacion ya publicada y evita tener filas historicas invalidas o depender de una correccion manual posterior.
- Alternative considered: crear `Titulo` nullable y tolerar signals antiguas sin titulo. Se descarta porque complica el dominio, obliga a ramas condicionales en lectura y retrasa el cierre real del cambio.

4. La busqueda mantendra sus filtros actuales y solo ampliara el payload devuelto con `Titulo`.
- Rationale: el requerimiento pedido es anadir una nueva columna al modelo, no redefinir la experiencia de busqueda. Mantener filtros reduce alcance y riesgo.
- Alternative considered: incorporar filtro adicional por `Titulo`. Se descarta en este cambio para mantenerlo pequeno y centrado.

## Risks / Trade-offs

- [Riesgo] La copia de `Descripcion` a `Titulo` puede producir titulos demasiado largos para uso visual si se define una longitud menor. -> Mitigacion: decidir una longitud persistente compatible con el backfill o truncar de forma controlada y documentada durante la implementacion.
- [Riesgo] El cambio rompe todos los payloads de alta/edicion y varios tests existentes. -> Mitigacion: actualizar contratos y pruebas en la misma entrega, verificando application e integration tests de `TrailSignals`.
- [Trade-off] Reutilizar `SignalDto` simplifica contratos, pero acopla busqueda y detalle. -> Mitigacion: mantenerlo porque ambos flujos ya comparten DTO; si divergen en el futuro, extraer un DTO especifico en otro cambio.

## Migration Plan

1. Anadir la columna `Titulo` a `Signals`.
2. Ejecutar un backfill SQL copiando `Descripcion` en `Titulo` para todas las filas existentes.
3. Aplicar la restriccion definitiva (`NOT NULL` y longitud configurada).
4. Desplegar codigo de dominio/API/aplicacion que ya exige `Titulo` en creacion y edicion.
5. Verificar regresion de create, update, search y detail de `Signal`.

Rollback:
- Si el despliegue falla antes de exponer el nuevo contrato, revertir migracion y codigo conjuntamente.
- Si falla despues de desplegar el contrato, no hacer rollback parcial de API sin revertir tambien la migracion, porque el modelo de datos y los payloads quedarian desalineados.

## Open Questions

- Ninguna por ahora; la propuesta asume que `Titulo` debe ser obligatorio y que el valor historico inicial debe derivarse de `Descripcion`.
