## Context

La spec actual de `frontend-signal-home-page` ya cubre el listado, filtros y navegación de `SignalHome`, pero no contempla todavía acciones de alta. El usuario ha definido ahora una segunda fase con dos piezas estrechamente relacionadas:

- un FAB persistente en la esquina inferior derecha de `SignalHome`
- un flujo de creación de varias pantallas o steps con progreso visible en cabecera

El proyecto ya usa DaisyUI en otras vistas recientes y la spec consolidada de Signals fija esa misma base visual para el módulo. Además, el contrato backend ya expone `POST /api/signals` con los campos `latitud`, `longitud`, `descripcion`, `foto1`, `foto2`, `activo`, `tipo` y `tags`, y mantiene `GET /api/signal-types` como catálogo de tipologías.

En web móvil, los dos puntos técnicamente sensibles de esta fase son:

- la captura de ubicación, porque el contrato solo acepta coordenadas y el campo es opcional
- la captura de fotos, porque el disparo de cámara depende del soporte del navegador y del dispositivo

## Goals / Non-Goals

**Goals:**
- Añadir un FAB visualmente consistente con DaisyUI en `SignalHome` para iniciar la creación.
- Permitir a cualquier usuario autenticado abrir un flujo guiado de creación de señales.
- Mostrar un indicador horizontal de cuatro steps con el paso actual claramente marcado.
- Capturar tipología, descripción obligatoria, tags, ubicación opcional y hasta dos fotos antes del guardado.
- Ofrecer un resumen final que permita revisar los datos antes de enviar el alta.
- Enviar el alta al backend mediante `POST /api/signals`, con `activo=true` por defecto.

**Non-Goals:**
- No introducir todavía edición de señales existentes.
- No implementar comentarios ni adjuntos adicionales fuera de `foto1` y `foto2`.
- No resolver en esta fase un mapa rico o selector cartográfico completo si la ubicación puede capturarse de forma más simple.
- No ampliar todavía el flujo con borradores persistentes entre sesiones.

## Decisions

### 1. Añadir un FAB de acción simple en `SignalHome`

`SignalHome` incorporará un FAB fijo en la esquina inferior derecha que navegue a la ruta de creación. Aunque la referencia visual parte del componente FAB de DaisyUI, para este caso se usará una única acción principal, no un speed dial de varias acciones.

Rationale:
- El usuario ha pedido una única acción `+` que abra directamente el flujo.
- Mantiene el patrón visual de DaisyUI sin introducir complejidad innecesaria de menú flotante.

Alternatives considered:
- Abrir varias acciones desde el FAB como speed dial.
  Rechazado porque el alcance actual solo requiere crear una nueva señal.

### 2. Implementar el alta como wizard de cuatro pasos bajo una ruta propia

El flujo de alta vivirá bajo una ruta dedicada, por ejemplo `/signals/nueva`, y mantendrá el estado del formulario dentro de un componente contenedor de wizard. Cada step cambiará el contenido principal sin perder el progreso ya completado.

Rationale:
- Aísla la complejidad del alta respecto al listado.
- Facilita navegación, validación progresiva y reutilización futura de subcomponentes.

Alternatives considered:
- Mostrar el formulario completo en un modal sobre `SignalHome`.
  Rechazado porque el proceso es demasiado largo para un modal y requiere captura de cámara.
- Crear una ruta separada por cada step.
  Rechazado en esta fase para no complicar innecesariamente la persistencia del estado intermedio.

### 3. Usar DaisyUI `steps` horizontales para el progreso del wizard

La cabecera del flujo mostrará un indicador horizontal basado en `steps` y `step`, marcando visualmente el paso actual y los ya completados. En móvil podrá envolverse en contenedor scrollable si el ancho no basta.

Rationale:
- Sigue exactamente el patrón visual pedido y el componente DaisyUI referenciado.
- Refuerza orientación del usuario en un proceso de varias pantallas.

Alternatives considered:
- Usar tabs o breadcrumb para el progreso.
  Rechazado porque expresan peor un flujo secuencial.

### 4. Paso 1: tipología obligatoria desde catálogo remoto

El primer paso cargará todas las tipologías desde `signal-types` y las mostrará como lista selectable con icono a la izquierda y nombre a continuación. La selección de una tipología será obligatoria para poder avanzar.

Rationale:
- El backend ya define `tipo` como entero obligatorio en la creación.
- La lista con icono y nombre responde al diseño funcional pedido.

Alternatives considered:
- Embutir las tipologías en cliente o en enum local.
  Rechazado porque rompería la alineación con backend.

### 5. Paso 2: datos principales con ubicación opcional y validación mínima

El segundo paso capturará:

- ubicación opcional
- descripción obligatoria
- tags como texto libre separado por comas

Para la ubicación, el frontend priorizará una acción del tipo `Usar ubicación actual` basada en geolocalización del navegador, permitiendo al usuario omitirla si no concede permiso o no la desea incluir.

Rationale:
- El contrato espera coordenadas, no una dirección textual.
- Mantiene el campo opcional sin forzar inputs técnicos de latitud/longitud salvo que más adelante hagan falta.

Alternatives considered:
- Pedir latitud y longitud manuales.
  Rechazado por mala UX para un flujo móvil generalista.

### 6. Paso 3: dos capturas de foto con botones dedicados

El paso de fotos mostrará dos acciones independientes, `Foto 1` y `Foto 2`, cada una conectada a un input de archivo con `accept="image/*"` y preferencia de cámara mediante `capture`. El frontend almacenará el resultado en memoria para mostrar preview en el resumen y codificarlo para el payload final.

Rationale:
- Se ajusta al contrato `foto1` / `foto2`.
- Es la forma web más directa de acercarse al comportamiento de “abrir la cámara”.

Alternatives considered:
- Integración nativa de cámara fuera del flujo web estándar.
  Rechazado por exceder el alcance de una Blazor Web App en esta fase.

### 7. Paso 4: resumen final y envío

El último step mostrará un resumen de tipología, descripción, tags, ubicación si existe y previews de fotos si se han capturado. Desde ahí el usuario confirmará con un botón `Guardar`, que enviará `POST /api/signals` con `activo=true`.

Rationale:
- Añade una última validación humana antes de publicar la señal.
- Reduce errores en un flujo con varios pasos y posible captura en exteriores.

Alternatives considered:
- Guardado inmediato al terminar el paso de fotos.
  Rechazado porque elimina la revisión final pedida por el usuario.

## Risks / Trade-offs

- [La apertura directa de cámara no se comporta igual en todos los navegadores] → Mitigación: usar input de archivo con `capture` como preferencia y documentar el fallback del navegador.
- [La geolocalización puede ser denegada o tardar demasiado] → Mitigación: mantener la ubicación como opcional y permitir continuar sin coordenadas.
- [Un wizard largo puede perder estado si el usuario recarga o navega atrás] → Mitigación: centralizar el estado del formulario en un contenedor único durante la sesión actual.
- [El uso de fotos en base64 puede aumentar el peso del payload] → Mitigación: limitar el flujo a dos fotos y evaluar compresión o resize en implementación si el tamaño real lo exige.

## Migration Plan

1. Extender `SignalHome` con el FAB de creación.
2. Añadir la ruta y el contenedor del wizard de alta.
3. Implementar los cuatro steps con navegación progresiva y validaciones.
4. Integrar lectura de tipologías y envío `POST /api/signals`.
5. Añadir recursos localizados, estados de error y estilos del nuevo flujo.
6. Validar manualmente navegación, pasos, cámara, geolocalización opcional y guardado final.

## Open Questions

- Confirmar si en desktop debe mantenerse el mismo flujo de cámara con selector de archivo como fallback natural.
- Confirmar si el resumen final debe permitir edición directa de cada bloque o solo volver al paso correspondiente.
