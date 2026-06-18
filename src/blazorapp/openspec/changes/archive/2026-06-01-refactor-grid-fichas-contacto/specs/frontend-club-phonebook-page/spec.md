## MODIFIED Requirements

### Requirement: La agenda telefónica MUST mostrar toda la información disponible del contacto
El sistema MUST mostrar por cada contacto toda la información expuesta por el contrato de `agenda-telefonica`, incluyendo `nombre`, teléfonos y cualquier otro campo disponible para lectura, dentro de una card visualmente separada por registro y adaptada a un layout de grid.

#### Scenario: Render de un contacto en la agenda
- **WHEN** un contacto se muestra en la lista de `Teléfonos de interés`
- **THEN** el sistema MUST mostrar el nombre del contacto
- **AND** el sistema MUST mostrar los teléfonos informados en el contrato
- **AND** el sistema MUST mostrar cualquier dato adicional expuesto por el endpoint sin requerir rediseño de la pantalla
- **AND** el sistema MUST agrupar la información de ese contacto dentro de una card diferenciada del resto de registros
- **AND** el sistema MUST permitir que esa card participe en un grid de fichas homogéneas

### Requirement: La agenda telefónica MUST priorizar el escaneo rápido de contactos
El sistema MUST organizar el contenido de cada contacto para permitir identificar rápidamente teléfonos y metadatos principales en pantallas móviles y de escritorio.

#### Scenario: Lectura rápida de una ficha de contacto
- **WHEN** el usuario revisa una card de contacto en la pantalla `Teléfonos de interés`
- **THEN** el sistema MUST separar visualmente la identidad del contacto de sus teléfonos y datos complementarios
- **AND** el sistema MUST mantener un espaciado suficiente entre cards consecutivas para evitar confusión entre registros
- **AND** el sistema MUST destacar como información básica el nombre, los teléfonos y el email cuando esté disponible

## ADDED Requirements

### Requirement: La agenda telefónica MUST usar un layout grid de fichas de contacto sin icono
El sistema MUST presentar el listado de contactos con un patrón de grid inspirado en `Grid Lists` de Tailwind, usando cards textuales consistentes y sin mostrar avatar o icono de contacto.

#### Scenario: Visualización de la lista en grid
- **WHEN** el usuario visualiza la lista de `Teléfonos de interés`
- **THEN** el sistema MUST renderizar las fichas dentro de una rejilla de tarjetas consistente
- **AND** el sistema MUST omitir cualquier avatar o icono de contacto en la card
- **AND** el sistema MUST mantener una disposición responsive que funcione en móvil y escritorio
