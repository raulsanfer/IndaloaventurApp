## Purpose

Permite delegar de forma acotada y revocable la gestión administrativa del inventario a socios reales, sin concederles privilegios generales de administración.

## ADDED Requirements

### Requirement: Rol delegable de gestión de inventario

El sistema MUST reconocer el rol técnico `InventoryManager` y permitir que un administrador lo asigne o retire mediante la gestión administrativa de usuarios existente. El rol SHALL aparecer en las lecturas de usuarios y SHALL emitirse en el token de un usuario que inicie sesión después de recibirlo, igual que los demás roles técnicos.

#### Scenario: Administrador asigna el rol a un socio real

- **WHEN** un administrador actualiza un usuario con `IsMember = true` e incluye `InventoryManager` en sus roles
- **THEN** el sistema SHALL persistir el rol y lo SHALL devolver en la representación administrada del usuario

#### Scenario: Rol no convertido en permiso general

- **WHEN** un usuario recibe el rol `InventoryManager`
- **THEN** el sistema SHALL no concederle acceso a la gestión de usuarios ni a otros endpoints exclusivos de `Admin`

### Requirement: Acceso administrativo de inventario condicionado por rol y condición de socio

El sistema MUST autorizar la gestión administrativa de inventario a un usuario `Admin`, o a un usuario que tenga simultáneamente el rol `InventoryManager` y el claim `IsMember` con valor `true`. Un usuario con solo `InventoryManager`, con `IsMember = false`, o sin ambos privilegios SHALL recibir una denegación de autorización. El acceso preexistente de `Admin` SHALL mantenerse aunque su `IsMember` sea `false`.

#### Scenario: Delegado socio autorizado

- **WHEN** un usuario autenticado presenta los roles `Member` e `InventoryManager` y el claim `IsMember = true`
- **THEN** el sistema SHALL autorizarle las operaciones administrativas de inventario

#### Scenario: Delegado que deja de ser socio denegado

- **WHEN** un usuario autenticado presenta el rol `InventoryManager` pero el claim `IsMember = false`
- **THEN** el sistema SHALL denegarle las operaciones administrativas de inventario

#### Scenario: Administrador no socio conserva acceso

- **WHEN** un usuario autenticado presenta el rol `Admin` y el claim `IsMember = false`
- **THEN** el sistema SHALL autorizarle las operaciones administrativas de inventario

### Requirement: Cobertura de todas las operaciones administrativas de inventario

El sistema MUST aplicar la regla de gestión delegada a las operaciones de alta, edición y borrado de categorías, estados y materiales de inventario. También MUST aplicarla al listado, detalle, retirada, cancelación y confirmación administrativa de devolución de reservas bajo `/api/inventario-reservas/admin`. Las rutas de autoservicio bajo `/api/inventario-reservas/me` SHALL conservar su autorización de socio propietario y no SHALL usar el rol `InventoryManager` como sustituto de la titularidad.

#### Scenario: Delegado socio gestiona un catálogo

- **WHEN** un delegado socio llama a una operación de escritura de categorías, estados o materiales de inventario con una solicitud válida
- **THEN** el sistema SHALL procesar la operación con las mismas reglas funcionales aplicables a un administrador

#### Scenario: Delegado socio gestiona una reserva administrativa

- **WHEN** un delegado socio llama a una ruta administrativa de listado, retirada, cancelación o confirmación de devolución de una reserva
- **THEN** el sistema SHALL procesar la operación con las mismas reglas funcionales aplicables a un administrador

#### Scenario: El rol no evita el aislamiento de autoservicio

- **WHEN** un usuario con `InventoryManager` intenta consultar o modificar una reserva ajena mediante una ruta `/me`
- **THEN** el sistema SHALL conservar el aislamiento por identidad del titular y SHALL no exponer ni modificar la reserva ajena

### Requirement: Cambios de delegación y socio invalidan sesiones emitidas

El sistema MUST invalidar los tokens emitidos de un usuario cuando un administrador cambie sus roles o su valor `IsMember`. Tras un cambio de delegación o de condición de socio, el usuario SHALL volver a autenticarse para que el token contenga los roles y el claim de socio vigentes.

#### Scenario: Retirada de rol invalida un token previo

- **WHEN** un administrador retira `InventoryManager` a un socio que tiene un token válido emitido antes del cambio
- **THEN** el sistema SHALL rechazar ese token en las rutas protegidas y un token obtenido después de autenticarse de nuevo SHALL no permitir la gestión administrativa de inventario

#### Scenario: Cambio de condición de socio invalida un token previo

- **WHEN** un administrador cambia `IsMember` a `false` para un usuario con `InventoryManager` que tiene un token válido emitido antes del cambio
- **THEN** el sistema SHALL rechazar ese token en las rutas protegidas y un token obtenido después de autenticarse de nuevo SHALL no permitir la gestión administrativa de inventario
