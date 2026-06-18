## MODIFIED Requirements

### Requirement: SignalHome MUST presentar cada señal como una card de listado alineada con el diseño
El sistema MUST mostrar cada señal dentro de una card visualmente separada, preparada para incluir imagen principal real, categoría, fecha, texto principal y metadato destacado siguiendo el layout base del diseño suministrado.

#### Scenario: Render de una señal en el listado
- **WHEN** una señal se muestra en la lista de `SignalHome`
- **THEN** el sistema MUST agrupar su contenido dentro de una card independiente
- **AND** el sistema MUST mostrar la categoría de la señal en formato píldora
- **AND** el sistema MUST mostrar una referencia temporal visible de la señal
- **AND** el sistema MUST mostrar el contenido textual principal sin romper la composición de la card

#### Scenario: Signal con imagen principal disponible
- **WHEN** una señal del listado dispone de imagen principal utilizable
- **THEN** el sistema MUST renderizar esa imagen real en la zona principal de media de la card
- **AND** el sistema MUST no sustituirla por el placeholder por defecto

#### Scenario: Campo visual opcional ausente
- **WHEN** una señal no disponga de imagen principal u otro campo visual opcional del diseño
- **THEN** el sistema MUST conservar una card estable y legible
- **AND** el sistema MUST usar el placeholder solo cuando la imagen real no esté disponible
- **AND** el sistema MUST evitar huecos rotos o errores de renderizado por datos ausentes
