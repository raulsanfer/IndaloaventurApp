## Why

El flujo actual de creación de `Signal` penaliza el uso móvil porque orienta la captura de imágenes a la cámara y no cubre de forma explícita la selección desde la galería del dispositivo. Además, el límite actual de 2 MB puede bloquear fotos válidas para el usuario aunque puedan adaptarse automáticamente antes del envío.

## What Changes

- Ampliar el paso de fotos del flujo de creación de `Signal` para permitir, en navegadores móviles compatibles, seleccionar imágenes tanto desde cámara como desde la galería.
- Mantener la selección de fichero en escritorio sin degradar el comportamiento ya existente.
- Incorporar un procesamiento cliente previo al guardado para redimensionar o recomprimir imágenes que superen el límite admitido, intentando ajustarlas a un máximo de 2 MB por foto.
- Informar de forma comprensible cuando una imagen no pueda adaptarse automáticamente al tamaño permitido.

## Capabilities

### New Capabilities

### Modified Capabilities
- `frontend-signal-create-flow`: cambia los requisitos del paso de fotos para admitir galería móvil y normalización automática de imágenes antes de validar el tamaño máximo.

## Impact

- Afecta al wizard de creación de señales en `IndaloaventurApp.SharedUI` y a la lógica de captura/carga de imágenes del frontend.
- Puede requerir JS interop o utilidades de navegador para abrir selectores de archivo con pistas de cámara/galería y para redimensionar/recomprimir imágenes en cliente.
- Mantiene el contrato actual de `POST /api/signals`, pero modifica cómo se prepara `foto1` y `foto2` antes de enviarlas.
