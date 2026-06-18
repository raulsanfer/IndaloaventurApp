## Context

El agregado `Signal` y los validadores de aplicación exigen actualmente que `Foto1` y `Foto2` tengan contenido. Esa regla se aplica tanto en `Crear` como en `Actualizar`, y el contrato API de `CreateSignalRequest` también refleja que ambas fotos son obligatorias.

La necesidad planteada solo afecta al alta: un usuario debe poder crear una señal aportando una sola foto. Eso obliga a desacoplar la validación de creación de la validación de edición para no relajar reglas no solicitadas fuera del caso de uso de alta.

## Goals / Non-Goals

**Goals:**
- Permitir crear un `Signal` cuando `Foto1` exista y `Foto2` no se informe o llegue vacía.
- Mantener compatibilidad con el almacenamiento actual evitando cambios de alcance innecesarios en persistencia si no son imprescindibles.
- Cubrir con pruebas el alta con una sola foto y la preservación del comportamiento esperado para el resto de validaciones.

**Non-Goals:**
- Rediseñar el modelo de imágenes de `Signal` para soportar un número arbitrario de fotos.
- Cambiar la semántica del endpoint de consulta de imágenes más allá de soportar la ausencia efectiva de la segunda foto.
- Relajar sin necesidad las reglas de actualización si el negocio no lo ha pedido.

## Decisions

### 1. La opcionalidad de `Foto2` se limitará explícitamente a la creación

La creación y la edición no comparten exactamente la misma regla de negocio. La propuesta debe permitir que `CreateSignal` omita `Foto2`, mientras que la edición solo se relajará si el análisis de implementación demuestra que el mismo modelo técnico obliga a ello y sigue siendo coherente con negocio.

Alternativas consideradas:
- Quitar la obligatoriedad de `Foto2` en toda operación. Rechazada porque amplía el cambio más allá del requisito pedido.
- Mantener una validación única para crear y editar. Rechazada porque impide expresar la diferencia de comportamiento necesaria.

### 2. Se conservará el almacenamiento binario actual con una representación válida para ausencia de segunda foto

Dado que `Foto2` hoy se persiste como `byte[]` requerido, la implementación puede resolver la ausencia de segunda foto representándola como vacío si eso evita una migración de base de datos. Solo se propondrá cambio de esquema si la implementación demuestra que el modelo actual no permite expresar la ausencia de manera segura.

Alternativas consideradas:
- Migrar `Foto2` a nullable en base de datos y dominio. Rechazada en la propuesta inicial por aumentar el alcance técnico.
- Seguir forzando un contenido no vacío. Rechazada porque mantiene el bloqueo funcional actual.

## Risks / Trade-offs

- [La misma validación se usa en creación y edición] -> Mitigación: separar reglas o introducir validación específica por caso de uso.
- [Las consultas de imágenes pueden devolver `Foto2` vacía] -> Mitigación: documentar el comportamiento esperado y cubrirlo con pruebas.
- [Persistir ausencia como array vacío puede requerir disciplina en consumidores] -> Mitigación: mantener contrato estable y probar explícitamente ese caso.
