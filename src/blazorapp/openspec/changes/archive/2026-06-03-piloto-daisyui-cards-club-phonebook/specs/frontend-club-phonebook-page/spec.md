## MODIFIED Requirements

### Requirement: La agenda telefónica MUST mostrar toda la información disponible del contacto
El sistema MUST mostrar por cada contacto toda la información expuesta por el contrato de `agenda-telefonica`, incluyendo `nombre`, teléfonos y cualquier otro campo disponible para lectura, dentro de una card visualmente separada por registro y preparada para admitir un piloto de composición DaisyUI.

#### Scenario: Render de un contacto en la agenda
- **WHEN** un contacto se muestra en la lista de `Teléfonos de interés`
- **THEN** el sistema MUST mostrar el nombre del contacto
- **AND** el sistema MUST mostrar los teléfonos informados en el contrato
- **AND** el sistema MUST mostrar cualquier dato adicional expuesto por el endpoint sin requerir rediseño de la pantalla
- **AND** el sistema MUST agrupar la información de ese contacto dentro de una card diferenciada del resto de registros
- **AND** el sistema MUST permitir implementar esa ficha mediante un piloto visual basado en el patrón `card` de DaisyUI

## ADDED Requirements

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
