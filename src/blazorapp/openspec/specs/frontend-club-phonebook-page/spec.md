## Purpose
Definir el comportamiento funcional y de presentación de la pantalla `Teléfonos de interés` accesible desde `Mi Club`.
## Requirements
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

### Requirement: Teléfonos de interés MUST cargarse desde agenda-telefonica
El sistema MUST obtener el listado de contactos consumiendo `GET /api/agenda-telefonica` mediante un servicio frontend desacoplado de la UI.

#### Scenario: Carga de agenda telefónica
- **WHEN** la pantalla de `Teléfonos de interés` se renderiza
- **THEN** el sistema MUST invocar `GET /api/agenda-telefonica`
- **AND** el sistema MUST mostrar los contactos recuperados

### Requirement: La agenda telefónica MUST mostrar toda la información disponible del contacto
El sistema MUST mostrar por cada contacto toda la información expuesta por el contrato de `agenda-telefonica`, incluyendo `nombre`, teléfonos y cualquier otro campo disponible para lectura, dentro de una card visualmente separada por registro.

#### Scenario: Render de un contacto en la agenda
- **WHEN** un contacto se muestra en la lista de `Teléfonos de interés`
- **THEN** el sistema MUST mostrar el nombre del contacto
- **AND** el sistema MUST mostrar los teléfonos informados en el contrato
- **AND** el sistema MUST mostrar cualquier dato adicional expuesto por el endpoint sin requerir rediseño de la pantalla
- **AND** el sistema MUST agrupar la información de ese contacto dentro de una card diferenciada del resto de registros

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

### Requirement: El sistema MUST permitir un piloto DaisyUI acotado en las fichas de contacto
El sistema MUST permitir aplicar DaisyUI únicamente al patrón visual de las fichas de `ClubPhonebookView`, sin extender ese cambio al resto de la pantalla ni a otras vistas del frontend.

#### Scenario: Alcance acotado del piloto
- **WHEN** se implementa el piloto DaisyUI en `ClubPhonebookView`
- **THEN** el sistema MUST limitar el uso de DaisyUI a los items de la lista de contactos
- **AND** el sistema MUST mantener intactas la lógica de carga, la navegación y los estados de la pantalla

### Requirement: El piloto MUST mostrar cada contacto como una card independiente y claramente delimitada
El sistema MUST renderizar cada elemento del listado de contactos como una ficha independiente con borde sencillo y separación visual suficiente respecto a los demás registros.

#### Scenario: Card individual visible en la lista
- **WHEN** una ficha de contacto se renderiza con el piloto DaisyUI
- **THEN** el sistema MUST mostrar un contenedor de card claramente distinguible
- **AND** el sistema MUST mostrar un borde sencillo alrededor de cada registro
- **AND** el sistema MUST mantener separación visual suficiente entre una ficha y la siguiente

### Requirement: El piloto MUST conservar el nombre actual y mejorar la presentación de teléfono y email
El sistema MUST mantener el nombre del contacto como cabecera principal de la ficha y MUST mostrar debajo los datos de teléfono y email con una composición más elaborada que la actual.

#### Scenario: Nombre estable con datos de contacto reforzados
- **WHEN** un contacto se muestra en la lista con el piloto DaisyUI
- **THEN** el sistema MUST destacar como información principal el nombre, teléfonos y email cuando esté disponible
- **AND** el sistema MUST conservar el nombre del contacto como elemento superior de la ficha
- **AND** el sistema MUST mostrar debajo los teléfonos y el email con bloques visuales más trabajados que el texto corrido actual
- **AND** el sistema MUST seguir mostrando dirección y observaciones sin romper la card ni ocultar datos del contrato cuando esos datos existan

### Requirement: La agenda telefónica MUST priorizar el escaneo rápido de contactos
El sistema MUST organizar el contenido de cada contacto para permitir identificar rápidamente teléfonos y metadatos principales en pantallas móviles.

#### Scenario: Lectura rápida de una ficha de contacto
- **WHEN** el usuario revisa una card de contacto en la pantalla `Teléfonos de interés`
- **THEN** el sistema MUST separar visualmente la identidad del contacto de sus teléfonos y datos complementarios
- **AND** el sistema MUST mantener un espaciado suficiente entre cards consecutivas para evitar confusión entre registros

