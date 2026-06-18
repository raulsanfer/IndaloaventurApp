## Why

El modelo actual de `Signal` obliga a informar siempre `Foto2`, lo que bloquea altas válidas cuando el usuario solo dispone de una imagen. Ese requisito es más restrictivo de lo necesario para el caso de uso de creación y añade fricción innecesaria en la captura de señales.

## What Changes

- Modificar el flujo de creación de `Signal` para que solo `Foto1` sea obligatoria y `Foto2` pueda omitirse.
- Ajustar el modelo de dominio, los contratos de aplicación/API y la validación asociada para aceptar ausencia de segunda foto al crear una señal.
- Añadir cobertura automatizada para verificar la creación de señales con una sola foto y preservar el comportamiento esperado del resto del flujo.

## Capabilities

### New Capabilities

### Modified Capabilities

- `signal-management`: la creación de señales debe permitir omitir `Foto2` manteniendo `Foto1` como imagen obligatoria.

## Impact

- Afecta al agregado `Signal` y a sus reglas de validación.
- Afecta al comando/controlador de creación y posiblemente al contrato de actualización si comparte la misma semántica de validación.
- Impacta pruebas de dominio, aplicación e integración del flujo de signals.
