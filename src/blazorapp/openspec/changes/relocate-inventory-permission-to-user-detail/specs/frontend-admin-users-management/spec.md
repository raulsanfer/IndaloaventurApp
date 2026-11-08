## ADDED Requirements

### Requirement: Permiso de gestión de inventario en la ficha administrativa del socio
La ficha administrativa de un socio MUST incluir una sección específica, visible solo para administradores, que permita consultar, conceder o retirar el rol `InventoryManager` mediante el control `Gestor/a de inventario`. El listado administrativo de usuarios MUST NOT mostrar ese control, su acción de guardado ni mensajes propios de su actualización.

Al preparar la actualización, el sistema MUST conservar todos los roles distintos de `InventoryManager` y MUST confirmar el estado mostrado recargando los datos del usuario después de una respuesta correcta del servidor.

#### Scenario: Administrador consulta el listado de usuarios
- **WHEN** un administrador abre el listado administrativo de usuarios
- **THEN** el sistema MUST mostrar las acciones de navegación y gestión existentes de cada usuario
- **THEN** el sistema MUST NOT mostrar un selector ni una acción para cambiar `Gestor/a de inventario` en ninguna fila

#### Scenario: Administrador concede el permiso desde la ficha de un socio
- **WHEN** un administrador activa `Gestor/a de inventario` en la ficha administrativa de un socio y guarda el cambio
- **THEN** el sistema MUST actualizar el usuario con su email, condición de socio y colección completa de roles que incluya `InventoryManager`
- **THEN** el sistema MUST conservar los roles distintos de `InventoryManager` y recargar la ficha antes de mostrar el estado confirmado

#### Scenario: Administrador retira el permiso desde la ficha de un socio
- **WHEN** un administrador desactiva `Gestor/a de inventario` en la ficha administrativa de un socio y guarda el cambio
- **THEN** el sistema MUST actualizar la colección completa de roles sin `InventoryManager`
- **THEN** el sistema MUST conservar los roles no administrados por ese control

### Requirement: Manejo seguro de cambios del permiso desde la ficha administrativa
La sección de permiso de inventario MUST mantener la autorización del servidor como decisión definitiva y MUST tratar los resultados de actualización sin cambio optimista. Tras un cambio confirmado, el sistema MUST informar de que la persona afectada debe iniciar sesión de nuevo. Si la persona administradora modifica sus propios roles, el sistema MUST cerrar su sesión local y navegar al inicio de sesión.

Si la actualización devuelve `401 Unauthorized`, el sistema MUST limpiar la sesión local y navegar al inicio de sesión. Si devuelve `403 Forbidden`, el sistema MUST conservar la sesión, MUST mostrar un error de permisos y MUST NOT mostrar el permiso como actualizado.

#### Scenario: Administrador cambia el permiso de otra persona
- **WHEN** la actualización del permiso de un socio distinto devuelve correctamente sin contenido
- **THEN** el sistema MUST recargar la ficha y comunicar que esa persona debe iniciar sesión de nuevo para aplicar sus permisos actuales

#### Scenario: Administrador cambia su propio permiso
- **WHEN** la actualización del permiso de la ficha del administrador devuelve correctamente
- **THEN** el sistema MUST cerrar la sesión local y navegar al inicio de sesión

#### Scenario: Token inválido durante el guardado del permiso
- **WHEN** la actualización del permiso desde la ficha devuelve `401 Unauthorized`
- **THEN** el sistema MUST limpiar la sesión local y navegar al inicio de sesión

#### Scenario: Actualización del permiso prohibida
- **WHEN** la actualización del permiso desde la ficha devuelve `403 Forbidden`
- **THEN** el sistema MUST conservar la sesión local y mostrar un error de permisos
- **THEN** el sistema MUST NOT presentar el permiso como si se hubiera actualizado
