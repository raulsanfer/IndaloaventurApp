## Why

Actualmente `IsMember` ya existe en la entidad `Usuario`, pero no forma parte de la identidad autenticada ni del contrato de login. Esto impide reutilizar el dato en autorizacion backend y obliga al frontend a depender de consultas adicionales para conocer si el usuario es socio.

## What Changes

- Emitir un claim estable `IsMember` en cada JWT generado tras autenticacion correcta.
- Incluir `IsMember` en la respuesta de login para mantener compatibilidad con consumidores actuales.
- Alinear los flujos de login clasico y social para que ambos expongan el mismo valor persistido.
- Añadir pruebas de aplicacion e integracion que validen el claim emitido y el contrato HTTP resultante.

## Capabilities

### New Capabilities

### Modified Capabilities
- `jwt-identity-authentication`: el JWT y la respuesta de autenticacion deben exponer el estado de socio persistido del usuario autenticado.

## Impact

- Aplicacion: contratos de `IIdentityService`, `IJwtTokenService` y handlers de autenticacion.
- Infraestructura: generacion de claims JWT basada en `Usuario.IsMember`.
- API: `LoginResponse` y documentacion Swagger derivada del contrato.
- Tests: cobertura de login clasico y social para claims y payloads de autenticacion.
