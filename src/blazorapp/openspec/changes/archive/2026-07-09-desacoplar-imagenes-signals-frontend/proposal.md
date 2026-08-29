## Why

El backend ha movido la persistencia interna de las fotos de `Signal` a filesystem sin cambiar el contrato HTTP público. En el frontend Blazor, sin embargo, la capa actual sigue mezclando metadatos e imágenes: el listado dispara una llamada adicional por cada signal para obtener binarios y la conversión de fotos sigue embebida en componentes visuales.

## What Changes

- Separar en el cliente frontend las responsabilidades de metadatos de `Signal` e imágenes de `Signal`.
- Evitar que `SignalHome` dependa de llamadas binarias por registro para poder renderizar el listado principal.
- Hacer que la página de detalle cargue imágenes desde `GET /api/signals/{id}/images` como una operación independiente del detalle base.
- Aislar los fallos de carga de imágenes para que no bloqueen el render del detalle ni degraden el resto de la información.
- Extraer la serialización y deserialización de fotos a una capa reutilizable del dominio `signals`, manteniendo intacto el contrato vigente de `POST /api/signals`.

## Capabilities

### New Capabilities

None.

### Modified Capabilities

- `frontend-signal-home-page`: el listado debe seguir siendo usable sin depender de recuperar binarios de imágenes por cada tarjeta.
- `frontend-signal-detail-page`: el detalle debe cargar y presentar imágenes desde un endpoint dedicado con manejo de error parcial independiente del detalle base.

## Impact

- Código afectado en `ISignalService`, `SignalApiClient`, modelos de `signals` y componentes `SignalHomeView`, `SignalDetailView` y `SignalCreateView`.
- Nuevos helpers o servicios internos para codec/transporte de imágenes en el dominio `signals`.
- Actualización de tests frontend para cubrir listado desacoplado, carga diferida de imágenes en detalle y errores parciales de imágenes.
- Sin cambios en rutas ni payloads públicos del API actual de `signals`.
