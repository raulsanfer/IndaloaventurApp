## Why

El dominio ya dispone de catalogo y solicitudes de licencias federativas en persistencia, pero el cliente movil todavia no puede operar sobre esa informacion. Necesitamos habilitar una API de autoservicio para que cada usuario pueda solicitar su licencia y consultar el historial y estado de sus propias solicitudes sin depender de procesos manuales.

## What Changes

- Exponer endpoints autenticados para crear solicitudes de licencia federativa del usuario actual usando el modelo ya persistido.
- Restringir la creacion de solicitudes a usuarios cuyo claim `IsMember` sea `true`, devolviendo una respuesta de autorizacion coherente cuando no se cumpla.
- Exponer consultas de "mis solicitudes" para recuperar el historial del usuario autenticado y el detalle completo de una solicitud concreta.
- Incluir en las respuestas de consulta la informacion relevante de la solicitud, la tarifa seleccionada y el estado de negocio.
- Mantener el aislamiento por usuario para impedir que un usuario consulte o cree solicitudes en nombre de otro.

## Capabilities

### New Capabilities
- `member-federative-license-api`: API CQRS de autoservicio para crear y consultar solicitudes de licencias federativas del usuario autenticado.

### Modified Capabilities

## Impact

- API: nuevo controlador o nuevas rutas autenticadas para operaciones `me` de licencias federativas.
- Application: nuevos comandos, queries, DTOs, validaciones y reglas de autorizacion basadas en el claim `IsMember`.
- Infrastructure: consultas Dapper y uso de repositorios existentes para materializar altas y lecturas por usuario autenticado.
- Seguridad: enforcement explicito de acceso solo sobre solicitudes propias y bloqueo del alta cuando `IsMember` sea `false`.
