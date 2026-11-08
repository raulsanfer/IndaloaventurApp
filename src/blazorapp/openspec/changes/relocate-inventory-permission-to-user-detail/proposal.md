## Why

La asignación de `InventoryManager` aparece hoy en cada fila del listado administrativo, aunque es una acción sensible que se entiende mejor con el contexto de la ficha individual del usuario. Centralizarla en el detalle reduce ruido en el listado y deja la gestión del permiso junto al resto de operaciones administrativas de esa persona.

## What Changes

- Retirar del listado administrativo de usuarios el selector, el guardado y los mensajes asociados al permiso `Gestor/a de inventario`.
- Añadir en la ficha administrativa de detalle del usuario una sección específica para consultar, conceder o retirar `InventoryManager` a un socio, reutilizando la actualización administrativa existente y preservando los demás roles.
- Mantener los controles de seguridad y experiencia existentes: disponibilidad exclusiva para administradores, confirmación desde el servidor, nuevo inicio de sesión tras un cambio de roles, cierre de sesión si la persona administradora se modifica a sí misma y tratamiento seguro de `401` y `403`.
- Dejar fuera de esta entrega el filtro del listado por estado del permiso de gestión de inventario; se abordará como cambio posterior.

## Capabilities

### New Capabilities

Ninguna.

### Modified Capabilities

- `frontend-admin-users-management`: La asignación del rol delegado de inventario pasa del listado de usuarios a la ficha administrativa individual de un socio.

## Impact

- `IndaloaventurApp.SharedUI`: `AdminUsersManagementView` y `AdminMemberProfileView`, recursos localizados y estilos SCSS globales si son necesarios.
- `IndaloaventurApp.Frontend.Tests`: pruebas de listado, ficha de socio administrada y la regresión del cliente de actualización de usuarios.
- `docs/features/settings-admin.md` y `docs/features/members-profile.md`: documentación de la ubicación del control y de sus límites de acceso.
- No se añaden dependencias ni endpoints. Se conserva `PUT /api/users/{userId}` como autoridad de actualización y la autorización del servidor como frontera definitiva.
