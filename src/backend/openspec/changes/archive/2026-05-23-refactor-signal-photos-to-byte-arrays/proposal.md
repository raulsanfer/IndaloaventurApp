## Why

El caso de uso de `Signal` actualmente maneja `Fotos` como texto y no permite recibir contenido binario real de imagen, lo que limita la calidad de integracion con cliente movil y dificulta la validacion de tamano/estructura de cada foto. Se necesita refactorizar ahora para soportar dos fotos en bytes desde API hasta persistencia.

## What Changes

- **BREAKING** Refactorizar contratos de creacion y edicion de `Signal` para reemplazar `Fotos` (string) por dos parametros binarios (`byte[] Foto1`, `byte[] Foto2`).
- Ajustar validaciones de aplicacion y dominio para requerir exactamente dos fotos no vacias en altas y ediciones.
- Valorar y definir un endpoint dedicado para obtener solo las imagenes de una `Signal`, desacoplando la carga binaria de las consultas generales.
- Adaptar contratos API para que la lectura de detalles/imagenes devuelva ambas fotos en formato apto para transporte JSON (base64).
- **BREAKING** Modificar persistencia (`Signal` + EF Core + migracion) para almacenar ambas fotos en columnas binarias separadas.
- Actualizar pruebas unitarias e integracion del flujo completo (crear, editar, buscar y obtener imagenes) con fotos binarias.

## Capabilities

### New Capabilities
- `signal-binary-photos`: Gestion de dos fotos binarias por senal desde API, aplicacion, dominio y persistencia.

### Modified Capabilities
- Ninguna.

## Impact

- API: `SignalsController` y contratos request/response para `Create`/`Update`/`Search`, mas endpoint especifico de imagenes.
- Application: comandos, validadores, handlers y DTOs de `Signal`/imagenes.
- Domain: entidad `Signal` y reglas de actualizacion de fotos.
- Infrastructure: configuracion EF, migraciones y consultas de `Signal`.
- Tests: suites de `TrailSignals` (unitarias e integracion) por cambio de contrato.