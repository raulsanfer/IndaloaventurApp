## Why

Actualmente existe el riesgo de que un usuario desactivado pueda seguir obteniendo acceso mediante autenticacion o reutilizando tokens validos, lo que rompe la expectativa de seguridad del estado `Activo`. Este cambio formaliza el comportamiento para bloquear de forma consistente autenticacion y autorizacion cuando el usuario no esta activo.

## What Changes

- Modificar la especificacion de autenticacion JWT para exigir que solo usuarios activos puedan autenticarse.
- Modificar la especificacion de autorizacion para exigir que usuarios desactivados no puedan acceder a endpoints protegidos, incluso si presentan token.
- Definir respuestas de rechazo coherentes para los casos de usuario desactivado en login y acceso protegido.
- Añadir pruebas unitarias e integracion para cubrir los escenarios de bloqueo por estado inactivo.

## Capabilities

### New Capabilities
- Ninguna.

### Modified Capabilities
- `jwt-identity-authentication`: se amplia la validacion para que autenticacion y autorizacion requieran usuario activo.

## Impact

- Application: handlers de login/social-login y servicios de identidad usados para validacion de estado activo.
- Infrastructure/Security: validaciones de usuario activo durante generacion y validacion de token.
- API: comportamiento de endpoints protegidos ante usuarios desactivados.
- Tests: ampliacion de pruebas unitarias y de integracion en flujos de autenticacion/autorizacion.