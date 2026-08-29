## Why

La experiencia actual de `Signals` tiene regresiones visibles en dos puntos clave: el listado no muestra la imagen principal real de cada signal y el detalle pierde legibilidad y densidad útil en móvil por el tamaño del resumen, la disposición del grid y una navegación por tabs que ya no cabe correctamente. En anchos de alrededor de `412px` como Samsung Galaxy, los bloques de `signal-detail__overview-grid` siguen apilándose con demasiado margen útil alrededor de cada `article`, lo que rompe la expectativa de ver dos tarjetas por fila. Corregirlo ahora evita consolidar un flujo visual roto en una funcionalidad que ya está expuesta a usuarios autenticados.

## What Changes

- Corregir el listado de `SignalHome` para que cada card use la imagen principal real de la signal cuando exista y solo recurra al placeholder cuando no haya imagen disponible.
- Ajustar el layout del detalle para reducir el peso visual del texto `GetHeaderSummary()` en la cabecera.
- Reorganizar `signal-detail__overview-grid` en móvil para que mantenga al menos dos cards por fila, incluyendo viewports de `412px`, reduciendo márgenes y spacing excesivos de cada `article` antes de degradar a una sola columna.
- Simplificar la navegación por tabs del detalle dejando solo `Datos de la signal` y `Mapa`.
- Mover el bloque de `Comentarios` fuera de tabs y renderizarlo debajo del bloque tabulado.
- Reubicar las `Etiquetas` al final de la página, por debajo de comentarios.

## Capabilities

### New Capabilities

### Modified Capabilities
- `frontend-signal-home-page`: la card de listado debe mostrar la imagen principal de la signal cuando exista como parte del layout esperado.
- `frontend-signal-detail-page`: el detalle cambia su jerarquía visual, la organización de tabs y la disposición responsive de los bloques informativos y secundarios.

## Impact

- `IndaloaventurApp.SharedUI`: componentes `SignalHomeView` y `SignalDetailView`, además de la lógica asociada al detalle y a la estructura de tabs/comentarios/etiquetas.
- `IndaloaventurApp.Web.Client`: `SignalApiClient` y el mapeo de `SignalCardItem` si hoy no se está resolviendo correctamente la imagen principal desde el payload.
- `IndaloaventurApp.Web`: SCSS de `signals` para ajustar responsive, densidad visual y nueva secuencia de bloques.
- Tests frontend de listado y detalle de signals para validar imagen principal, layout del detalle y nueva organización de contenido.
