## Why

Actualmente el backend no dispone de una forma estandar de consultar contenido de WordPress para exponerlo a la app cliente. Necesitamos habilitar esta integracion ahora para consumir posts remotos con configuracion por `appsettings` y dejar una base reutilizable para futuras operaciones de publicacion.

## What Changes

- Se añade una nueva integracion de WordPress basada en un servicio inyectable orientado a operaciones de contenido.
- Se implementa inicialmente la consulta de posts desde una web WordPress configurada por clave en `appsettings`.
- Se expone un controlador API para obtener posts desde WordPress sin persistirlos en la base de datos local.
- Se define configuracion centralizada para URL/base endpoint, autenticacion por clave y parametros operativos necesarios para futuras extensiones (como publicacion de posts).
- Se agregan pruebas unitarias/integracion para validar mapeo de respuestas, errores de configuracion y comportamiento del endpoint.

## Capabilities

### New Capabilities
- `wordpress-posts-integration`: Consulta de posts de WordPress mediante un servicio de infraestructura configurable y expuesto por endpoint REST del backend.

### Modified Capabilities
- Ninguna.

## Impact

- Afecta a `src/IndaloAventurApi.Infrastructure` con cliente HTTP y servicio WordPress.
- Afecta a `src/IndaloAventurApi.Application` con contrato/DTOs de consulta.
- Afecta a `src/IndaloAventurApi.Api` con nuevo controlador/endpoints y registro DI.
- Afecta a configuracion en `appsettings*.json` para opciones de WordPress.
- Afecta a tests en `tests/IndaloAventurApi.Application.Tests` y/o `tests/IndaloAventurApi.IntegrationTests`.
