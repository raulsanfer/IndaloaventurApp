## ADDED Requirements

### Requirement: Uso consistente de DaisyUI en la gestión de cargos
La página de gestión de `Cargos` MUST usar de forma consistente patrones DaisyUI para breadcrumb, superficies, fieldset, input, botones y listado.

#### Scenario: Admin abre la página de cargos
- **WHEN** un usuario con rol `Admin` navega a la gestión de `Cargos`
- **THEN** el sistema MUST renderizar el breadcrumb con el mismo patrón DaisyUI usado en `Configuración`
- **THEN** el sistema MUST renderizar el editor y el listado como superficies visuales legibles y consistentes

### Requirement: Layout usable con jerarquía visual clara
La página de gestión de `Cargos` MUST presentar una jerarquía visual clara entre contexto, título principal, editor y listado.

#### Scenario: Vista cargada correctamente
- **WHEN** la gestión de `Cargos` termina de cargar
- **THEN** el sistema MUST mostrar un título principal destacado respecto a los títulos de sección
- **THEN** el sistema MUST separar visualmente el editor superior del listado inferior

### Requirement: Formulario usable y responsive
El editor de cargos MUST presentar un campo de texto y acciones con una disposición usable tanto en pantallas estrechas como amplias.

#### Scenario: Pantalla estrecha
- **WHEN** la página se muestra en una pantalla estrecha
- **THEN** el sistema MUST permitir que el input y las acciones se reorganicen sin desbordes horizontales

#### Scenario: Pantalla amplia
- **WHEN** la página se muestra en una pantalla amplia
- **THEN** el sistema MUST aprovechar el ancho disponible para mejorar legibilidad y ritmo visual sin estirar en exceso el contenido

### Requirement: Listado de cargos legible y accionable
La lista de cargos MUST mostrar cada registro con separación suficiente, información legible y acciones claramente distinguibles.

#### Scenario: Catálogo con cargos
- **WHEN** `GET /api/cargos` devuelve uno o más cargos
- **THEN** el sistema MUST renderizar cada cargo con su nombre y acciones alineadas de forma legible
- **THEN** el sistema MUST mantener usable el listado cuando las acciones pasan a varias líneas en móvil

### Requirement: Estados operativos visibles sin degradar la maquetación
La gestión de `Cargos` MUST mostrar estados de carga, éxito, error o vacío sin romper la composición principal.

#### Scenario: Estado vacío
- **WHEN** `GET /api/cargos` devuelve una colección vacía
- **THEN** el sistema MUST mostrar un estado vacío integrado visualmente con el resto del layout

#### Scenario: Estado de error
- **WHEN** ocurre un error al cargar o guardar cargos
- **THEN** el sistema MUST mostrar un mensaje visible y legible sin colapsar el editor ni el listado
