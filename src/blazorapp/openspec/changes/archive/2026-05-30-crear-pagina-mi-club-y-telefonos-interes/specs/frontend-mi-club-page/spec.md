## ADDED Requirements

### Requirement: Navegación a página Mi Club desde la botonera inferior
El sistema MUST ofrecer acceso a una página `Mi Club` desde el botón inferior "Mi Club" del shell autenticado.

#### Scenario: Apertura de Mi Club desde una ruta autenticada
- **WHEN** el usuario autenticado pulsa el botón inferior "Mi Club"
- **THEN** el sistema MUST navegar a la ruta de la página `Mi Club`

#### Scenario: Estado activo del botón Mi Club
- **WHEN** el usuario se encuentra en la página `Mi Club`
- **THEN** el sistema MUST mostrar el botón inferior "Mi Club" en estado visual activo

### Requirement: La página Mi Club MUST comportarse como índice de opciones
El sistema MUST renderizar `Mi Club` como una página índice para opciones y utilidades consultables por el socio.

#### Scenario: Visualización inicial de Mi Club
- **WHEN** la página `Mi Club` se renderiza
- **THEN** el sistema MUST mostrar una estructura de índice con al menos una opción navegable

### Requirement: Mi Club MUST mostrar la opción Teléfonos de interés
El sistema MUST incluir `Teléfonos de interés` como primera opción visible dentro de la página `Mi Club`.

#### Scenario: Opción disponible en el índice
- **WHEN** el usuario visualiza la página `Mi Club`
- **THEN** el sistema MUST mostrar la opción `Teléfonos de interés` como elemento interactivo

### Requirement: Mi Club MUST permitir navegar a Teléfonos de interés
El sistema MUST permitir abrir la pantalla de `Teléfonos de interés` desde la opción correspondiente del índice.

#### Scenario: Navegación desde el índice al detalle
- **WHEN** el usuario pulsa la opción `Teléfonos de interés`
- **THEN** el sistema MUST navegar a la pantalla de agenda telefónica asociada

### Requirement: Mi Club MUST usar recursos localizados y estilos globales
El sistema MUST obtener los textos visibles de `Mi Club` desde recursos localizados ES y MUST definir sus estilos en SCSS global modular.

#### Scenario: Revisión de textos y estilos de Mi Club
- **WHEN** se revisa la implementación de la página `Mi Club`
- **THEN** el sistema MUST usar claves de recursos para sus literales visibles
- **AND** el sistema MUST definir sus estilos en parciales SCSS globales
