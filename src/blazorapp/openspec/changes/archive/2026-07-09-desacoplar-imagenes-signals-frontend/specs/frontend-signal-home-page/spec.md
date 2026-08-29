## ADDED Requirements

### Requirement: SignalHome MUST renderizar el listado sin depender de binarios por cada signal
El sistema MUST construir y mostrar el listado principal de `SignalHome` a partir de los metadatos de `signals` y las categorías de `signal-types`, y MUST tratar la preview de imagen de cada card como un elemento opcional que no bloquea el render principal.

#### Scenario: Listado principal disponible con solo metadatos
- **WHEN** `SignalHome` recibe correctamente el listado base de signals y las categorías visibles
- **THEN** el sistema MUST renderizar las cards del listado sin esperar una carga binaria independiente por cada signal
- **AND** el sistema MUST mantener operativos el acceso al detalle, la búsqueda y los filtros

#### Scenario: Preview ausente o diferida
- **WHEN** una signal no dispone de preview utilizable o la vista decide no resolver imágenes en el listado
- **THEN** el sistema MUST mantener una card estable y navegable
- **AND** el sistema MUST usar el fallback visual permitido sin marcar el listado completo como fallido
