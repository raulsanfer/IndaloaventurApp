## ADDED Requirements

### Requirement: Navegación y jerarquía visual de gestión de cargos
La página de gestión de `Cargos` MUST seguir la línea visual de `Configuración` mostrando breadcrumb y títulos con jerarquía tipográfica destacada.

#### Scenario: Admin abre la página de cargos
- **WHEN** un usuario con rol `Admin` navega a la gestión de `Cargos`
- **THEN** el sistema MUST mostrar un breadcrumb de retorno dentro del contexto de `Configuración`
- **THEN** el sistema MUST mostrar un título principal visualmente más prominente que los títulos de sección

### Requirement: Fieldset superior para alta y edición de cargos
La gestión de `Cargos` MUST mostrar un `fieldset` por encima de la lista con un único campo de texto para el nombre del cargo y un botón `Guardar`.

#### Scenario: Vista inicial en modo nuevo cargo
- **WHEN** la página de gestión de `Cargos` termina de cargar correctamente
- **THEN** el sistema MUST mostrar un `fieldset` titulado `Nuevo Cargo`
- **THEN** el sistema MUST mostrar dentro del `fieldset` un campo de texto para el nombre del cargo y un botón `Guardar`

### Requirement: Lista administrativa de cargos
La página de gestión de `Cargos` MUST presentar el catálogo en formato de lista administrativa debajo del `fieldset`.

#### Scenario: Catálogo con elementos
- **WHEN** `GET /api/cargos` devuelve uno o más cargos
- **THEN** el sistema MUST renderizar cada cargo como un elemento de lista debajo del `fieldset`
- **THEN** cada elemento MUST mostrar el nombre del cargo y sus acciones disponibles

#### Scenario: Catálogo vacío
- **WHEN** `GET /api/cargos` devuelve una colección vacía
- **THEN** el sistema MUST mantener visible el `fieldset` de creación
- **THEN** el sistema MUST mostrar un estado vacío en la zona de listado

### Requirement: Alta de cargos desde el fieldset
El formulario superior MUST permitir crear un cargo nuevo y refrescar el listado al completarse la operación.

#### Scenario: Creación satisfactoria desde el modo nuevo cargo
- **WHEN** un usuario `Admin` escribe un nombre válido en el campo y pulsa `Guardar` en modo `Nuevo Cargo`
- **THEN** el sistema MUST invocar `POST /api/cargos`
- **THEN** el sistema MUST refrescar o actualizar el listado visible tras la creación satisfactoria
- **THEN** el sistema MUST dejar el formulario preparado para crear un nuevo cargo adicional

### Requirement: Edición de cargos desde la lista
Cada cargo listado MUST ofrecer una acción `Editar` que active el modo edición en el `fieldset` superior.

#### Scenario: Entrada en modo edición
- **WHEN** un usuario `Admin` pulsa `Editar` sobre un cargo del listado
- **THEN** el sistema MUST cambiar el `fieldset` superior a modo edición
- **THEN** el sistema MUST cargar el nombre del cargo seleccionado en el campo de texto

#### Scenario: Guardado de cambios en modo edición
- **WHEN** un usuario `Admin` modifica el nombre del cargo cargado y pulsa `Guardar`
- **THEN** el sistema MUST invocar `PUT /api/cargos/{id}`
- **THEN** el sistema MUST reflejar el cambio en el listado tras completarse la operación

### Requirement: Acción de borrado diferida
Cada cargo listado MUST mostrar una acción `Borrar`, pero esta iteración MUST dejar la eliminación operativa fuera de alcance.

#### Scenario: Visualización de acción futura de borrado
- **WHEN** un usuario `Admin` revisa un cargo listado
- **THEN** el sistema MUST mostrar una acción `Borrar` junto a `Editar`
- **THEN** el sistema MUST no ejecutar `DELETE /api/cargos/{id}` como parte de este cambio
