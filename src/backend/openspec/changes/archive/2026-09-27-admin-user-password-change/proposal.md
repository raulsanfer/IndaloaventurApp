## Why

Administracion necesita poder restablecer la contraseña de cualquier cuenta desde el panel interno sin exponer secretos ni reutilizar el flujo público de recuperación. Esta operación de alto impacto debe quedar protegida por autorización, validación estricta, limitación de frecuencia y auditoría segura.

## What Changes

- Añade `PUT /api/users/{userId}/password`, exclusivo para administradores, que cambia la contraseña indicada mediante ASP.NET Core Identity y responde `204 No Content`.
- Valida que el cuerpo contenga únicamente una contraseña alfanumérica ASCII de entre 1 y 20 caracteres, sin registrar ni devolver secretos.
- Invalida los JWT existentes del usuario objetivo mediante el sello de seguridad de Identity y limita los intentos por administrador y usuario objetivo.
- Registra intentos exitosos, rechazados y limitados con metadatos de auditoría no sensibles.

## Capabilities

### New Capabilities

- `admin-user-password-change`: Cambio administrativo de contraseñas con validación, autorización, invalidación de sesiones, limitación y auditoría segura.

### Modified Capabilities

- `jwt-identity-authentication`: Los JWT deberán estar vinculados al sello de seguridad vigente para poder invalidarse tras un cambio de contraseña.

## Impact

- Afecta al controlador y flujos de usuarios, la abstracción e implementación de Identity, la emisión y validación de JWT, el registro de auditoría y la configuración de limitación de frecuencia.
- Añade pruebas unitarias e integración para autorización, validación, persistencia y revocación de tokens.
