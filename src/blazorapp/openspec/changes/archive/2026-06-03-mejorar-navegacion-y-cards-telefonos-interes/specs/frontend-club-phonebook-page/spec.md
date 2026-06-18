## MODIFIED Requirements

### Requirement: El sistema MUST disponer de una pantalla Teléfonos de interés
El sistema MUST ofrecer una pantalla dedicada de `Teléfonos de interés` accesible desde la página `Mi Club` y MUST mostrar una cabecera compacta con navegación contextual.

#### Scenario: Ruta de agenda telefónica disponible
- **WHEN** el usuario navega desde `Mi Club`
- **THEN** el sistema MUST cargar una pantalla específica de `Teléfonos de interés`

#### Scenario: Cabecera compacta con breadcrumb
- **WHEN** la pantalla `Teléfonos de interés` se renderiza
- **THEN** el sistema MUST mostrar un breadcrumb accionable con el enlace `Volver a Mi Club`
- **AND** el sistema MUST mostrar `Área del socio` como elemento de contexto actual a continuación del enlace
- **AND** el sistema MUST mostrar el título `Teléfonos de interés` como encabezado principal de la pantalla
- **AND** el sistema MUST omitir la descripción informativa bajo el título para priorizar el espacio útil

### Requirement: La agenda telefónica MUST mostrar toda la información disponible del contacto
El sistema MUST mostrar por cada contacto toda la información expuesta por el contrato de `agenda-telefonica`, incluyendo `nombre`, teléfonos y cualquier otro campo disponible para lectura, dentro de una card visualmente separada por registro.

#### Scenario: Render de un contacto en la agenda
- **WHEN** un contacto se muestra en la lista de `Teléfonos de interés`
- **THEN** el sistema MUST mostrar el nombre del contacto
- **AND** el sistema MUST mostrar los teléfonos informados en el contrato
- **AND** el sistema MUST mostrar cualquier dato adicional expuesto por el endpoint sin requerir rediseño de la pantalla
- **AND** el sistema MUST agrupar la información de ese contacto dentro de una card diferenciada del resto de registros

## ADDED Requirements

### Requirement: La agenda telefónica MUST priorizar el escaneo rápido de contactos
El sistema MUST organizar el contenido de cada contacto para permitir identificar rápidamente teléfonos y metadatos principales en pantallas móviles.

#### Scenario: Lectura rápida de una ficha de contacto
- **WHEN** el usuario revisa una card de contacto en la pantalla `Teléfonos de interés`
- **THEN** el sistema MUST separar visualmente la identidad del contacto de sus teléfonos y datos complementarios
- **AND** el sistema MUST mantener un espaciado suficiente entre cards consecutivas para evitar confusión entre registros
