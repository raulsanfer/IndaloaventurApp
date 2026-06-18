## Why

Actualmente el backend no distingue de forma explícita si un usuario pertenece al club, lo que impide soportar reglas de negocio y visualización basadas en membresía. Necesitamos incorporar este dato ahora para habilitar su gestión administrativa y su consumo consistente en los flujos de usuario.

## What Changes

- Añadir la propiedad booleana `IsMember` en la entidad de usuario.
- Persistir `IsMember` en base de datos con valor por defecto seguro para usuarios existentes.
- Exponer `IsMember` en los flujos de lectura de usuario relevantes (consultas y DTOs de salida).
- Permitir la actualización de `IsMember` en flujos administrativos de modificación de usuario.
- Mantener fuera de alcance en este cambio cualquier ajuste de cliente/front.

## Capabilities

### New Capabilities
Ninguna.

### Modified Capabilities
- `identity-user-lifecycle-management`: añadir gestión del estado de membresía de club (`IsMember`) por parte de administradores y su visibilidad en consultas de usuario.

## Impact

- Dominio: entidad `Usuario` y sus invariantes.
- Aplicación: comandos/queries de gestión de usuario, validaciones y mapeos de DTO.
- Infraestructura: migración de persistencia para la nueva columna booleana.
- API: contratos de endpoints de usuario que devuelven o actualizan información de membresía.
