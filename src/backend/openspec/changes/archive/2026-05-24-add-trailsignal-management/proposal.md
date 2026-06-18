## Why

El sistema necesita una forma estructurada de compartir avisos e incidencias en rutas para mejorar la seguridad y la toma de decisiones de los usuarios. Ahora es el momento de incorporar esta funcionalidad porque habilita colaboración entre miembros y administración con control por roles.

## What Changes

- Se añade la gestión administrativa de `Signal_Type` para crear, editar y eliminar tipos de señal informativa.
- Se añade la gestión de `Signal` para usuarios con rol `Admin` o `Member`, permitiendo crear y editar señales (sin borrado).
- Se añade búsqueda de señales para cualquier rol mediante filtros por `Tags`, `Activo`, `Descripcion` y `Tipo`.
- Se define el contrato funcional de campos obligatorios y reglas de relación entre `Signal` y `Signal_Type`.

## Capabilities

### New Capabilities
- `signal-type-management`: Gestión CRUD administrativa de tipos de señal (`Signal_Type`) con propiedades `Id`, `Nombre` e `Icono`.
- `signal-management`: Alta y edición de señales (`Signal`) por roles `Admin` y `Member`, incluyendo metadatos de auditoría y relación obligatoria con `Signal_Type`.
- `signal-search`: Consulta de señales para cualquier rol con filtros por `Tags`, `Activo`, `Descripcion` y `Tipo`.

### Modified Capabilities
- Ninguna.

## Impact

- Nuevos endpoints API para `Signal_Type` y `Signal` (comandos y consultas).
- Nuevos modelos de dominio, persistencia y validaciones para tipos y señales.
- Cambios en autorización por rol para operaciones de administración, edición y búsqueda.
- Nuevas pruebas unitarias e integración para casos de uso, validaciones y permisos.