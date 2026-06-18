## Why

La primera fase de `Signals` ya permite consultar el listado, pero todavía no da al usuario una forma guiada de registrar una nueva incidencia desde la propia app. Esta segunda fase completa el caso de uso principal del dominio añadiendo una entrada flotante de alta y un flujo progresivo que reduzca fricción en móvil para capturar tipología, datos, fotos y confirmación final.

## What Changes

- Ampliar `SignalHome` con un botón flotante de acción (FAB) fijado en la esquina inferior derecha, con fondo primario y símbolo `+` en blanco.
- Hacer que el FAB navegue a un flujo de creación de `Signal` accesible para cualquier usuario autenticado con acceso a `Signals`.
- Implementar un proceso multi-step para crear señales, mostrando en la parte superior un indicador horizontal de pasos basado en DaisyUI.
- Definir cuatro pasos de creación: selección de tipología, captura de datos principales, captura de fotos y resumen final con guardado.
- Cargar la lista completa de tipologías desde `signal-types` para el primer paso y enviar el alta al endpoint `POST /api/signals`.
- Soportar captura de foto 1 y foto 2 desde botones dedicados que disparen la cámara del dispositivo cuando el navegador lo permita.

## Capabilities

### New Capabilities
- `frontend-signal-create-flow`: Define el flujo guiado de creación de señales, incluyendo steps, validación de campos, captura de fotos y confirmación final.

### Modified Capabilities
- `frontend-signal-home-page`: Añade el FAB de creación en la pantalla de listado y la navegación desde `SignalHome` al flujo de alta.

## Impact

- Afecta a `IndaloaventurApp.SharedUI` con nuevos componentes Razor, estado de formulario, validaciones, recursos localizados y estilos SCSS.
- Afecta a `IndaloaventurApp.Web.Client` con nuevas páginas o rutas del flujo de creación y la ampliación del cliente de `signals` para altas.
- Depende de `GET /api/signal-types` para la selección de tipología y de `POST /api/signals` para persistir la nueva señal.
- Requiere tratar en frontend la codificación de fotos para `foto1` y `foto2`, además de manejar la variabilidad de cámara/geolocalización en navegador móvil.
