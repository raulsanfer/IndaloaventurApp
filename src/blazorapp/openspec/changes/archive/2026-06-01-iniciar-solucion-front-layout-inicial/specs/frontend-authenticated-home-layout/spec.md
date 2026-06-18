## ADDED Requirements

### Requirement: Home autenticada basada en app shell reutilizable
El sistema MUST renderizar la pantalla Home tras validación de usuario dentro de un app shell reutilizable compuesto por cabecera, área de contenido y navegación inferior.

#### Scenario: Entrada a home autenticada
- **WHEN** el usuario está validado y accede a la home
- **THEN** el sistema MUST mostrar la home dentro del shell autenticado completo

### Requirement: Cabecera de aplicación en estado autenticado
El sistema MUST mostrar una cabecera de aplicación consistente con la referencia de `openspec/design/home` cuando el usuario esté autenticado.

#### Scenario: Cabecera visible en home
- **WHEN** se renderiza la home autenticada
- **THEN** el sistema MUST mostrar la cabecera con estructura visual y jerarquía coherentes con el diseño de referencia

### Requirement: Menú inferior o botonera persistente
El sistema MUST mostrar un menú inferior/botonera persistente en la experiencia autenticada para facilitar navegación principal en móvil.

#### Scenario: Navegación inferior activa
- **WHEN** el usuario está en el área autenticada
- **THEN** el sistema MUST mantener visible el menú inferior y resaltar el destino activo

### Requirement: Diseño responsive de home autenticada
El sistema MUST adaptar la home autenticada para móvil y escritorio preservando legibilidad, jerarquía y áreas táctiles mínimas.

#### Scenario: Home en móvil
- **WHEN** la home autenticada se visualiza en dispositivo móvil
- **THEN** el sistema MUST garantizar botones táctiles adecuados y layout optimizado de cabecera, contenido y menú inferior

#### Scenario: Home en escritorio
- **WHEN** la home autenticada se visualiza en escritorio
- **THEN** el sistema MUST mantener la misma semántica de navegación con distribución adaptada al ancho disponible

### Requirement: Estilos del shell autenticado centralizados en SCSS
El sistema MUST definir estilos de cabecera y menú inferior en parciales SCSS globales, registrados en `style.scss`, sin estilos inline.

#### Scenario: Revisión de estilos del shell
- **WHEN** se revisa la implementación del shell autenticado
- **THEN** el sistema MUST tener estilos únicamente en ficheros SCSS organizados

### Requirement: Literales localizados de home autenticada
El sistema MUST obtener todos los textos visibles de home, cabecera y menú inferior desde recursos localizados en español mediante claves cortas.

#### Scenario: Resolución de textos de navegación
- **WHEN** se renderiza la home autenticada
- **THEN** el sistema MUST mostrar etiquetas y acciones de navegación mediante claves de recurso
