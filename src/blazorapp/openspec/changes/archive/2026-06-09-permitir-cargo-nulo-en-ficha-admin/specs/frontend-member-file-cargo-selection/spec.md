## MODIFIED Requirements

### Requirement: Edición de cargo solo para administradores
La ficha de socio MUST permitir modificar el cargo únicamente cuando el usuario autenticado tenga rol `Admin`, usando un selector alimentado con el catálogo de cargos disponible, y MUST permitir también limpiar la asignación de cargo dejando el valor en `null`.

#### Scenario: Admin edita el cargo desde su propia ficha o una ficha administrativa
- **WHEN** un usuario autenticado con rol `Admin` visualiza una ficha editable
- **THEN** el sistema MUST cargar el catálogo de cargos desde el API
- **THEN** el sistema MUST renderizar un selector con las opciones disponibles
- **THEN** el sistema MUST permitir enviar el `CargoId` seleccionado al guardar la ficha

#### Scenario: Admin limpia un cargo existente
- **WHEN** un usuario autenticado con rol `Admin` abre una ficha con cargo asignado y selecciona la opción vacía del combo de cargos
- **THEN** el sistema MUST tratar esa selección como ausencia de cargo
- **THEN** al guardar la ficha el sistema MUST enviar `CargoId = null`

#### Scenario: Catálogo de cargos vacío
- **WHEN** un usuario `Admin` abre una ficha editable y el catálogo de cargos no contiene elementos
- **THEN** el sistema MUST mostrar el campo de cargo en un estado controlado sin opciones seleccionables
- **THEN** el sistema MUST informar de que no hay cargos disponibles o impedir una edición inconsistente
