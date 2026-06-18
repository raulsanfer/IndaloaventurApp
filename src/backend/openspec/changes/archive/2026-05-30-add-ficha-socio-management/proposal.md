## Why

Actualmente no existe un flujo para que cada persona usuaria gestione su informacion personal de socio de forma centralizada, con control de acceso por identidad. Necesitamos incorporarlo ahora para habilitar experiencias de perfil en la app y reforzar el cumplimiento de consentimiento y datos de contacto.

## What Changes

- Se añade un nuevo modelo de dominio `FichaSocio` vinculado 1:1 con `User` mediante `UserId`.
- Se define el contrato funcional para consultar y editar la ficha con reglas de autorizacion: el propio usuario autenticado o un usuario con rol Administrador.
- Se incorporan validaciones de datos para campos identificativos, contacto, direccion, fecha de nacimiento y banderas de consentimiento.
- Se añaden casos de uso CQRS para obtener y actualizar la ficha de socio.
- Se exponen endpoints REST para gestionar la ficha bajo JWT, manteniendo mensajes y contratos en espanol.
- Se contemplan escenarios de acceso denegado cuando un usuario intenta operar sobre fichas de terceros sin permisos de administrador.

## Capabilities

### New Capabilities
- `ficha-socio-management`: Gestion de ficha de socio (consulta y edicion) vinculada al usuario autenticado con excepcion por rol Administrador.

### Modified Capabilities
- `identity-user-lifecycle-management`: Se amplian reglas funcionales para relacionar cada usuario con su `FichaSocio` y permitir uso administrativo sobre fichas de terceros.

## Impact

- Afecta a `src/IndaloAventurApi.Domain` con nueva entidad `FichaSocio` y reglas basicas de consistencia.
- Afecta a `src/IndaloAventurApi.Application` con comandos/queries, validadores y DTOs del flujo de ficha.
- Afecta a `src/IndaloAventurApi.Infrastructure` con mapeo EF Core, migracion y repositorio/consultas.
- Afecta a `src/IndaloAventurApi.Api` con endpoints protegidos y politicas de autorizacion.
- Afecta a pruebas de aplicacion/integracion para validar permisos, validaciones de campos y comportamiento HTTP.