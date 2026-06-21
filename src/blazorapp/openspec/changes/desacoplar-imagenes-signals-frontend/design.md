## Context

El backend mantiene el mismo contrato HTTP público de `signals`, pero la persistencia interna de fotos ha pasado a filesystem. En el frontend Blazor actual, `SignalApiClient` mezcla metadatos e imágenes: `GetSignalHomeDataAsync` resuelve categorías, obtiene el listado y después realiza una llamada adicional a `/api/signals/{id}/images` por cada signal para generar `ImageUrl`. Además, la normalización de fotos del alta y la conversión de bytes a previews están repartidas entre `SignalCreateView` y el propio cliente API.

El cambio afecta a varias capas del dominio `signals`: contrato interno del servicio frontend, cliente HTTP, modelos compartidos, componentes de listado/detalle y tests de servicio y bUnit. También hay una restricción funcional importante: `PUT /api/signals/{id}` no soporta reemplazo de fotos, así que la propuesta no debe introducir affordances que sugieran lo contrario.

## Goals / Non-Goals

**Goals:**

- Separar la carga de metadatos de `Signal` y la carga de imágenes dentro del frontend.
- Eliminar la dependencia del listado respecto a llamadas binarias por tarjeta.
- Incorporar en el detalle una carga dedicada de imágenes con estado independiente del detalle base.
- Centralizar la lógica `byte[]` <-> preview/data URL para reutilizarla entre create y detail.
- Mantener sin cambios las rutas y payloads públicos actuales del API.

**Non-Goals:**

- Cambiar el contrato HTTP de `POST /api/signals`, `GET /api/signals`, `GET /api/signals/{id}` o `GET /api/signals/{id}/images`.
- Añadir reemplazo de fotos en edición de signals.
- Introducir almacenamiento persistente de caché entre pantallas o sesiones.
- Migrar el transporte de imágenes a `multipart/form-data`, streaming o URLs firmadas en este cambio.

## Decisions

### 1. Introducir una operación explícita de imágenes en la capa de servicio frontend

Se añadirá una operación dedicada en `ISignalService` y su implementación para recuperar las imágenes de una signal como recurso independiente del detalle base. El detalle seguirá usando `GetSignalAsync` para metadatos y una llamada separada para fotos.

Alternativas consideradas:

- Ampliar `GetSignalAsync` para devolver también imágenes.
  Rechazada porque el backend ya separa ambos recursos y mezclarlo en el frontend volvería a unir dos dominios con fallos y tiempos de carga distintos.
- Mantener la carga de imágenes encapsulada solo en `SignalApiClient` sin exponerla al resto del dominio.
  Rechazada porque deja al componente de detalle sin una frontera explícita de estado y hace más difícil testear el comportamiento parcial.

### 2. El listado dejará de enriquecer cada card con llamadas binarias bajo demanda implícita

`SignalHome` se construirá exclusivamente con el resultado de `GET /api/signals` más las categorías. La imagen de la card se tratará como opcional y el fallback visual seguirá siendo válido cuando no exista un thumbnail ya disponible en el modelo.

Alternativas consideradas:

- Mantener el patrón actual de una llamada a `/images` por card.
  Rechazada por coste N+1, mayor latencia en móvil y acoplamiento con una responsabilidad que el listado no necesita para ser usable.
- Añadir una precarga en background invisible tras pintar el listado.
  Rechazada en esta fase para mantener el cambio pequeño y evitar introducir caché o ciclos de vida extra sin necesidad funcional demostrada.

### 3. Crear un helper compartido de imágenes para `signals`

La lógica de conversión de bytes a preview y de normalización de payloads de fotos se moverá a un helper o servicio interno del dominio `signals`. Ese helper concentrará:

- detección o preservación de MIME para previews,
- construcción de `data:` URLs para render,
- normalización de `Foto2` ausente,
- reutilización de la misma codificación en create y detail.

Alternativas consideradas:

- Mantener la lógica en `SignalCreateView` y `SignalApiClient`.
  Rechazada por duplicación y por dejar la semántica binaria repartida entre vista e infraestructura.
- Mover toda la lógica a JavaScript.
  Rechazada porque la serialización del contrato HTTP y la mayoría de tests actuales viven en C#.

### 4. La página de detalle manejará estados separados para metadatos, comentarios e imágenes

`SignalDetailView` mantendrá el detalle base como recurso principal. Las imágenes se cargarán después de resolver el detalle, con un estado independiente que permita:

- mostrar galería si hay una o dos fotos,
- mostrar estado vacío si no hay fotos utilizables,
- mostrar error parcial de imágenes sin ocultar el resto del detalle.

Alternativas consideradas:

- Bloquear toda la pantalla hasta tener detalle, comentarios e imágenes.
  Rechazada porque degrada la resiliencia y hace que un fallo secundario invalide información ya disponible.
- Ignorar por completo los errores de imágenes.
  Rechazada porque ocultaría inconsistencias reales del almacenamiento y dificultaría soporte o diagnóstico.

### 5. La edición seguirá limitada a metadatos

No se añadirán selectores ni persistencia de fotos en el flujo de edición actual. La UI de detalle podrá mostrar imágenes, pero la acción `Editar` seguirá limitada a título, descripción y estado.

Alternativas consideradas:

- Aprovechar el refactor para permitir reemplazo de fotos.
  Rechazada porque el backend actual no lo soporta en `PUT` y mezclar ambos cambios haría la propuesta más arriesgada y menos verificable.

## Risks / Trade-offs

- [Riesgo] Eliminar imágenes reales del listado puede cambiar la percepción visual actual de algunas cards. -> Mitigación: mantener placeholder estable y dejar explícito en la propuesta que la prioridad es desacoplar el render principal.
- [Riesgo] Añadir una nueva operación al servicio frontend obliga a tocar dobles de test y varios componentes. -> Mitigación: introducir un modelo pequeño y cobertura de tests de API client y bUnit desde el primer paso.
- [Riesgo] La conversión a `data:` URLs puede seguir siendo costosa para imágenes grandes en detalle. -> Mitigación: limitar el alcance a una o dos fotos por signal y reutilizar la normalización ya existente en create.
- [Riesgo] El detalle puede mostrar más estados simultáneos y aumentar complejidad visual. -> Mitigación: mantener las imágenes como bloque autónomo con mensajes breves y consistentes con DaisyUI.

## Migration Plan

1. Añadir el modelo y operación dedicada de imágenes en `ISignalService`, `SignalApiClient` y dobles de test.
2. Extraer el helper compartido de conversión de imágenes y reutilizarlo en create y detail.
3. Simplificar `GetSignalHomeDataAsync` para que deje de resolver imágenes por card.
4. Actualizar `SignalDetailView` para cargar imágenes con estado separado y feedback parcial.
5. Actualizar tests de `SignalApiClient` y de vistas para reflejar el nuevo reparto de responsabilidades.

Rollback:

- Revertir el cambio en la interfaz de servicio y volver a la resolución de imágenes en listado si aparece una regresión no asumible.
- No se requiere migración de datos ni cambios de despliegue coordinados con backend mientras el contrato HTTP vigente se mantenga.

## Open Questions

- Si el equipo quiere mantener imagen real en `SignalHome`, debería decidirse en un cambio aparte si se necesita un endpoint de thumbnail o un campo de preview dentro del listado, porque el endpoint actual de imágenes completas no es una base adecuada para el listado.
- Queda abierto si el detalle necesita un botón explícito de reintento para imágenes o si basta con un estado de error parcial sin interacción adicional en esta iteración.
