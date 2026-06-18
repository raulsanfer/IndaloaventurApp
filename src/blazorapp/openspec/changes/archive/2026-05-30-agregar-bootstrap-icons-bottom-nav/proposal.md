## Why

La `BottomNav` actual usa abreviaturas de texto (`IN`, `CL`, `MC`) en lugar de iconos, lo que no refleja la referencia visual deseada ni establece una base reutilizable para iconografía en el frontend. Incorporar `Bootstrap Icons` ahora permite estandarizar el uso de iconos en la app y acercar la botonera inferior al diseño objetivo sin depender de `svg` inline dispersos.

## What Changes

- Se agrega `Bootstrap Icons` al frontend como paquete/librería base de iconografía reutilizable.
- Se redefine `BottomNav` para renderizar iconos en lugar de abreviaturas de texto, manteniendo los labels localizados existentes.
- Se intenta mapear los iconos de la referencia visual adjunta para `Home`, `Mi Club` y `Mi Cuenta` usando el catálogo de `Bootstrap Icons`.
- Se formaliza que, si un icono equivalente no puede localizarse con suficiente fidelidad en `Bootstrap Icons`, la implementación MUST dejar constancia del hueco y requerir aportación manual del icono final antes de cerrar el ajuste visual.
- Se ajustan estilos globales de la botonera para soportar iconos y labels en la composición deseada.

## Capabilities

### New Capabilities
- `frontend-bootstrap-icons`: Define el uso de `Bootstrap Icons` como sistema de iconografía reutilizable en el frontend.
- `frontend-bottom-nav-icons`: Define la representación iconográfica de la botonera inferior según la referencia visual y el comportamiento esperado cuando falten equivalencias adecuadas.

### Modified Capabilities
- Ninguna.

## Impact

- Afecta a `IndaloaventurApp.Web` en la carga de la librería de iconos y su disponibilidad global.
- Afecta a `IndaloaventurApp.SharedUI` en `BottomNav.razor` y `BottomNav.razor.cs` para reemplazar abreviaturas por iconos configurables.
- Afecta al SCSS/CSS global de la navegación inferior para alinear composición, tamaños y estados activos con la referencia.
- Puede abrir la puerta a reutilizar `Bootstrap Icons` en otras áreas como `MyAccount` o futuras pantallas, aunque eso queda fuera de esta iteración.
