## Why

La funcionalidad `Alertas Alimentarias` ya existe como superficie propia, pero desde `HomeDashboard` sigue ocupándose espacio con una tarjeta de `Actividades` que no debe permanecer como acceso principal. Sustituirla ahora mejora la encontrabilidad de alertas alimentarias desde Home y alinea el dashboard con las prioridades funcionales actuales.

## What Changes

- Sustituir en `HomeDashboard.razor` el bloque visual asociado a `home_card_activities_title` por un acceso a `Alertas Alimentarias`.
- Hacer que ese acceso navegue a la página principal de `Alertas Alimentarias`, que actúa como entrada al listado por categorías ya definido.
- Mostrar el acceso con icono `bi bi-basket2` a la izquierda y el título a la derecha.
- Mantener el resto de tarjetas y la composición de Home sin cambios de alcance no solicitados.

## Capabilities

### New Capabilities

### Modified Capabilities
- `frontend-food-alerts-pages`: ampliar la discoverability de `Alertas Alimentarias` para que la funcionalidad sea accesible desde `HomeDashboard` sustituyendo la tarjeta actual de `Actividades`.

## Impact

- `IndaloaventurApp.SharedUI`: ajuste de `HomeDashboard.razor` y posible actualización mínima de su markup o estilos asociados.
- `IndaloaventurApp.SharedUI/Resources`: reutilización o ajuste de literales de Home y de `Alertas Alimentarias`.
- `IndaloaventurApp.Web/wwwroot/scss`: posible ajuste SCSS de la tarjeta/acceso en Home para soportar la composición con icono a la izquierda y título a la derecha.
