## ADDED Requirements

### Requirement: Ficha administrativa de socio por userId
El sistema MUST ofrecer una página administrativa para ver y editar la ficha de socio de un usuario concreto usando su `userId`.

#### Scenario: Admin abre la ficha de un socio existente
- **WHEN** un administrador navega a la ficha administrativa de un usuario con ficha existente
- **THEN** el sistema MUST cargar los datos mediante `GET /api/fichas-socio/{userId}`
- **THEN** el sistema MUST mostrar un formulario editable coherente con el área administrativa

### Requirement: Edición administrativa de la ficha
La ficha administrativa MUST permitir guardar cambios del socio seleccionado mediante `PUT /api/fichas-socio/{userId}`.

#### Scenario: Admin guarda cambios en ficha existente
- **WHEN** un administrador modifica una ficha administrativa válida y pulsa guardar
- **THEN** el sistema MUST invocar `PUT /api/fichas-socio/{userId}`
- **THEN** el sistema MUST mantener la pantalla en un estado consistente tras el guardado

### Requirement: Navegación administrativa y coherencia visual
La ficha administrativa del socio MUST seguir la línea visual del resto de páginas de administración con breadcrumb, títulos y layout responsive.

#### Scenario: Renderizado de la ficha administrativa
- **WHEN** la ficha administrativa se muestra en pantalla
- **THEN** el sistema MUST mostrar breadcrumb dentro del contexto de `Configuración -> Administración -> Usuarios`
- **THEN** el sistema MUST mantener un formulario usable en móvil y escritorio
