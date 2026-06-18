# frontend-member-file-cargo-selection Specification

## Purpose
TBD - created by archiving change habilitar-edicion-cargo-ficha-socio. Update Purpose after archive.
## Requirements
### Requirement: Cargo legible en la ficha de socio
La ficha de socio MUST mostrar el cargo con una descripción legible para el usuario y MUST no exponer el identificador numérico `CargoId` como valor visible principal del campo.

#### Scenario: Miembro consulta su ficha con cargo asignado
- **WHEN** un usuario autenticado abre su ficha y el perfil contiene un `CargoId` válido
- **THEN** el sistema MUST mostrar el nombre del cargo asociado
- **THEN** el sistema MUST no mostrar el identificador numérico como contenido principal del campo

#### Scenario: Admin consulta una ficha con cargo asignado
- **WHEN** un usuario autenticado con rol `Admin` abre una ficha de socio que contiene un `CargoId` válido
- **THEN** el sistema MUST resolver y mostrar la descripción del cargo correspondiente en la interfaz

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

### Requirement: Cargo visible pero no editable para miembros
La ficha de socio MUST mantener el campo de cargo visible para usuarios autenticados con rol `Member`, pero MUST presentarlo en modo no editable.

#### Scenario: Member consulta su ficha
- **WHEN** un usuario autenticado con rol `Member` abre su ficha
- **THEN** el sistema MUST mostrar el nombre del cargo si existe
- **THEN** el sistema MUST impedir modificar el valor del campo de cargo

#### Scenario: Member sin cargo asignado
- **WHEN** un usuario autenticado con rol `Member` abre su ficha y no tiene cargo asignado
- **THEN** el sistema MUST mostrar el campo de cargo en modo no editable
- **THEN** el sistema MUST reflejar de forma clara que no hay cargo asignado

