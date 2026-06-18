## Context

El flujo de creación de `Signal` ya tiene un paso de fotos implementado en `SignalCreateView` con dos `InputFile` ocultos, ambos configurados con `accept="image/*"` y `capture="environment"`. La lógica actual rechaza el fichero antes de leerlo si `file.Size` supera `MaxPhotoBytes`, lo que impide aprovechar fotos existentes en galería y también bloquea imágenes que podrían reducirse en cliente antes del envío.

La app es una Blazor Web App con JS ya cargado para helpers de `signals`, por lo que el cambio puede apoyarse en interop sin introducir una arquitectura ajena al proyecto. El contrato backend no cambia: seguimos enviando `foto1` y `foto2` como contenido binario dentro de `CreateSignalRequest`.

## Goals / Non-Goals

**Goals:**
- Permitir que cada foto del wizard pueda obtenerse desde cámara o desde galería/selector de archivos, especialmente en móvil.
- Mantener el límite efectivo de 2 MB por imagen en el dato que se termina guardando o enviando.
- Adaptar automáticamente imágenes grandes en cliente mediante redimensión y/o recompresión antes de rechazarlas.
- Conservar la experiencia actual de preview, validación y revisión final.

**Non-Goals:**
- Cambiar el contrato del API de creación de señales.
- Añadir edición manual avanzada de imágenes, recorte o rotación.
- Garantizar que absolutamente cualquier imagen pueda reducirse con calidad perfecta; cuando no sea viable dentro del límite, se mostrará error comprensible.

## Decisions

### 1. Separar la captura desde cámara de la selección desde galería/archivo
Se reemplazará el único disparador por slot por dos acciones explícitas: una orientada a cámara y otra a galería/archivo. La razón es que `capture="environment"` sesga o fuerza el flujo de cámara en muchos navegadores móviles y no garantiza acceso cómodo a galería.

Alternativas consideradas:
- Mantener un único `InputFile` sin `capture`: simplifica UI, pero deja el comportamiento demasiado en manos del navegador y no asegura una entrada clara a cámara.
- Mantener `capture="environment"` y confiar en que el sistema ofrezca galería: no resuelve el problema descrito por el usuario.

### 2. Introducir un helper de normalización de imágenes en cliente
La preparación de imágenes se moverá a un helper dedicado que reciba el fichero original, genere una variante optimizada y devuelva bytes finales, `contentType` y preview. Este helper podrá vivir detrás de JS interop apoyándose en `canvas` para redimensionar y re-encodear progresivamente a JPEG hasta quedar por debajo del umbral o alcanzar límites mínimos razonables.

Alternativas consideradas:
- Rechazar por tamaño como ahora: incumple el objetivo principal.
- Usar solo `RequestImageFileAsync`: reduce complejidad, pero ofrece menos control para iterar calidad/tamaño final y puede quedarse corto con fotos muy grandes.

### 3. Validar el límite después del procesamiento, no antes de la selección
La validación de tamaño dejará de ejecutarse sobre el fichero original y pasará a ejecutarse sobre el resultado normalizado. Así el usuario puede elegir cualquier foto y el sistema intenta adaptarla antes de decidir si es válida.

Alternativas consideradas:
- Mantener una prevalidación dura sobre tamaño original y luego optimizar: seguiría bloqueando casos recuperables.

### 4. Conservar el modelo de wizard y el contrato de guardado
`SignalPhotoDraft`, la preview y el resumen final seguirán existiendo. Solo cambia la forma de poblar `Draft.Photo1` y `Draft.Photo2`, minimizando impacto sobre steps posteriores y sobre `CreateSignalAsync`.

## Risks / Trade-offs

- [Compresión con pérdida al re-encodear a JPEG] → Mitigar con escalado progresivo y calidad mínima razonable, priorizando que el resultado siga siendo legible.
- [Diferencias entre navegadores móviles al abrir cámara/galería] → Mitigar con dos inputs/acciones diferenciadas y validación manual en Android/iOS si están en alcance del equipo.
- [Procesamiento en cliente de imágenes grandes puede consumir memoria o tardar] → Mitigar limitando dimensiones máximas, procesando una imagen cada vez y mostrando mensajes de estado/error claros.
- [Algunas imágenes podrían no entrar en 2 MB incluso tras optimización] → Mitigar con mensaje explícito indicando que no se pudo adaptar automáticamente y que debe elegirse otra imagen.

## Migration Plan

No requiere migración de datos ni cambios backend. El despliegue consiste en publicar el frontend con el nuevo helper de imágenes y validar el flujo de creación en móvil y escritorio. Si apareciera una incidencia, el rollback puede hacerse revirtiendo únicamente la UI y el helper del wizard de fotos.

## Open Questions

- Si el backend depende del `contentType` original, conviene confirmar si aceptar siempre JPEG re-encodeado es correcto o si debe preservarse PNG cuando ya cumple tamaño.
- Falta decidir el umbral mínimo de calidad/dimensión a partir del cual el sistema deja de intentar optimizar y muestra error.
