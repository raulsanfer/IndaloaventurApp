## ADDED Requirements

### Requirement: Navegación desde Mi Cuenta a Configuración
El sistema MUST ofrecer acceso a la página `Configuración` desde la opción `Configuración` ubicada en la pantalla `Mi cuenta`.

#### Scenario: Apertura de Configuración desde Mi Cuenta
- **WHEN** el usuario autenticado pulsa la opción `Configuración` en `Mi cuenta`
- **THEN** el sistema MUST navegar a la ruta de la página `Configuración`

### Requirement: Renderizado base de la página Configuración
El sistema MUST renderizar una página `Configuración` accesible para usuarios autenticados aunque no tengan rol `Admin`.

#### Scenario: Usuario autenticado abre Configuración
- **WHEN** un usuario autenticado navega a `Configuración`
- **THEN** el sistema MUST mostrar la estructura base de la página sin depender del rol `Admin`

### Requirement: Visibilidad condicional del bloque Administración
El sistema MUST mostrar el bloque `Administración` dentro de `Configuración` únicamente cuando el usuario autenticado tenga el rol `Admin`.

#### Scenario: Usuario Admin visualiza Configuración
- **WHEN** el usuario autenticado tiene rol `Admin` y abre `Configuración`
- **THEN** el sistema MUST renderizar un bloque o panel con el título `Administración`

#### Scenario: Usuario no Admin visualiza Configuración
- **WHEN** el usuario autenticado no tiene rol `Admin` y abre `Configuración`
- **THEN** el sistema MUST no renderizar el bloque `Administración`

### Requirement: Opción administrativa Cargos restringida a Admin
El sistema MUST mostrar dentro del bloque `Administración` la opción `Cargos` únicamente a usuarios con rol `Admin`.

#### Scenario: Opción Cargos visible para Admin
- **WHEN** el usuario autenticado tiene rol `Admin` y el bloque `Administración` se renderiza
- **THEN** el sistema MUST mostrar la opción `Cargos` como acceso operativo

#### Scenario: Opción Cargos oculta para no Admin
- **WHEN** el usuario autenticado no tiene rol `Admin`
- **THEN** el sistema MUST no mostrar la opción `Cargos`

### Requirement: Localización y estilos de Configuración
El sistema MUST obtener los textos visibles de `Configuración` desde recursos localizados ES y MUST definir sus estilos en SCSS global modular.

#### Scenario: Revisión de implementación de Configuración
- **WHEN** se revisa la implementación de la página `Configuración`
- **THEN** el sistema MUST usar claves de recursos y parciales SCSS globales sin estilos inline
