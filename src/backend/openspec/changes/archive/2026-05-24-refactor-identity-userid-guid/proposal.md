## Why

El sistema de identidad usa `string` como tipo de clave de usuario, lo que introduce conversiones manuales y riesgo de inconsistencias con entidades de dominio que trabajan con `Guid`. Cambiar `AspNetUsers.Id` a `Guid` ahora reduce deuda técnica y mejora seguridad de tipos antes de que siga creciendo el modelo relacional.

## What Changes

- **BREAKING**: Migrar el modelo de identidad para usar `IdentityUser<Guid>` y `IdentityRole<Guid>` en lugar de claves `string`.
- Refactorizar servicios de identidad, autenticación JWT y gestión de usuarios para trabajar con `Guid` de forma nativa.
- Ajustar entidades relacionadas, persistencia, migraciones y pruebas unitarias/integración para el nuevo tipo de clave.
- Mantener contratos API existentes siempre que sea posible, validando compatibilidad de serialización en respuestas.

## Capabilities

### New Capabilities
- Ninguna.

### Modified Capabilities
- `jwt-identity-authentication`: Las operaciones de identidad y claims de autenticación deben usar identificadores de usuario `Guid` como tipo primario.
- `repository-pattern-persistence`: La persistencia Identity debe mapear claves `Guid` en usuarios, roles y relaciones asociadas.

## Impact

- Cambios en `ApplicationDbContext`, bootstrap de Identity y `IdentityService`.
- Nuevas migraciones sobre esquema Identity y tablas relacionadas (`AspNetUsers`, claims, logins, roles, tokens, user-roles).
- Ajustes en handlers, DTOs y controladores de gestión de usuarios/autenticación.
- Actualización de tests unitarios/integración de login, registro y administración de usuarios.
