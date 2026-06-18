## ADDED Requirements

### Requirement: El sistema MUST disponer de una pantalla Teléfonos de interés
El sistema MUST ofrecer una pantalla dedicada de `Teléfonos de interés` accesible desde la página `Mi Club`.

#### Scenario: Ruta de agenda telefónica disponible
- **WHEN** el usuario navega desde `Mi Club`
- **THEN** el sistema MUST cargar una pantalla específica de `Teléfonos de interés`

### Requirement: Teléfonos de interés MUST cargarse desde agenda-telefonica
El sistema MUST obtener el listado de contactos consumiendo `GET /api/agenda-telefonica` mediante un servicio frontend desacoplado de la UI.

#### Scenario: Carga de agenda telefónica
- **WHEN** la pantalla `Teléfonos de interés` se renderiza
- **THEN** el sistema MUST invocar `GET /api/agenda-telefonica`
- **AND** el sistema MUST mostrar los contactos recuperados

### Requirement: La agenda telefónica MUST mostrar toda la información disponible del contacto
El sistema MUST mostrar por cada contacto toda la información expuesta por el contrato de `agenda-telefonica`, incluyendo `nombre`, teléfonos y cualquier otro campo disponible para lectura.

#### Scenario: Render de un contacto en la agenda
- **WHEN** un contacto se muestra en la lista de `Teléfonos de interés`
- **THEN** el sistema MUST mostrar el nombre del contacto
- **AND** el sistema MUST mostrar los teléfonos informados en el contrato
- **AND** el sistema MUST mostrar cualquier dato adicional expuesto por el endpoint sin requerir rediseño de la pantalla

### Requirement: El email MUST mostrarse cuando el contrato backend lo proporcione
El sistema MUST mostrar el email del contacto en la agenda telefónica cuando ese dato esté disponible en la respuesta del endpoint.

#### Scenario: Contacto con email disponible
- **WHEN** el endpoint `agenda-telefonica` devuelve un email para un contacto
- **THEN** el sistema MUST mostrar dicho email en la ficha visual del contacto

#### Scenario: Contacto sin email disponible
- **WHEN** el endpoint `agenda-telefonica` no devuelve email para un contacto
- **THEN** el sistema MUST mantener la pantalla consistente sin bloquear el render del resto de datos

### Requirement: La pantalla MUST permitir scroll vertical para agendas largas
El sistema MUST permitir desplazamiento vertical natural cuando el número de contactos supere el alto visible de la pantalla.

#### Scenario: Lista larga de contactos
- **WHEN** la agenda contiene más contactos de los que caben en pantalla
- **THEN** el sistema MUST permitir scroll vertical para consultar el listado completo

### Requirement: La pantalla MUST manejar estados de carga y error comprensibles
El sistema MUST mostrar estados comprensibles cuando la agenda telefónica esté cargando o no pueda recuperarse correctamente.

#### Scenario: Carga en progreso
- **WHEN** la pantalla `Teléfonos de interés` espera la respuesta del endpoint
- **THEN** el sistema MUST mostrar un estado de carga comprensible para el usuario

#### Scenario: Error al cargar agenda
- **WHEN** el endpoint `agenda-telefonica` devuelve error o no puede completarse
- **THEN** el sistema MUST mostrar un estado de error comprensible para el usuario
- **AND** el sistema MUST evitar dejar la pantalla en un estado inconsistente
