## Why

La gestión de inventario está reservada hoy al rol técnico `Admin`, aunque hay socios que pueden encargarse del material, sus catálogos y las reservas sin necesitar el resto de facultades administrativas. El club necesita delegar ese trabajo de forma acotada y revocable, manteniendo la garantía de que el delegado sea un socio real.

## What Changes

- Incorporar el rol técnico adicional `InventoryManager` para delegar la gestión administrativa de inventario a usuarios que no sean `Admin`.
- Permitir asignar o retirar ese rol desde la gestión administrativa de usuarios ya existente; no se añade una nueva casilla persistida al perfil ni un claim de permiso independiente.
- Autorizar a `Admin` y a `InventoryManager` para las operaciones administrativas de categorías, estados, materiales y reservas de inventario, incluidas las confirmaciones administrativas de devolución.
- Exigir además `IsMember = true` a quien acceda mediante `InventoryManager`; el rol por sí solo no será suficiente. Los administradores conservarán el acceso que ya tenían, sin requerirles ser socios.
- Mantener el acceso de autoservicio de reservas y sus confirmaciones propias limitado a su titular socio, sin conceder a `InventoryManager` acceso a reservas por esa ruta.
- Actualizar la matriz de autorización, la emisión/revocación de sesión tras cambios de rol y la cobertura de autorización para los nuevos casos permitidos y denegados.

La propuesta interpreta «firma» como las confirmaciones administrativas existentes del flujo de devolución: el sistema no dispone aún de una funcionalidad independiente de firma digital y no se incorpora una en este cambio.

## Capabilities

### New Capabilities

- `inventory-management-delegation`: Delegación segura de la gestión administrativa de inventario a socios con el rol `InventoryManager`.

### Modified Capabilities

Ninguna. Las especificaciones de categorías, estados, material y reservas aún son cambios OpenSpec activos y no forman parte de `openspec/specs/`; esta capacidad transversal define la política que deberán consumir cuando se integren.

## Impact

- Seguridad e identidad: constante y aprovisionamiento del rol, política compuesta de autorización y revocación de los JWT emitidos al modificar roles.
- API: controladores de inventario, matriz de autorización y contratos de gestión de usuarios existentes; no se añaden rutas públicas nuevas ni columnas de base de datos. La implementación se coordinará con los cambios activos de categorías, estados, material y reservas para sustituir su restricción exclusiva a `Admin`.
- Aplicación y pruebas: autorización de los flujos administrativos de reservas y pruebas de integración para `Admin`, delegado socio, delegado no socio y usuarios sin privilegio.
