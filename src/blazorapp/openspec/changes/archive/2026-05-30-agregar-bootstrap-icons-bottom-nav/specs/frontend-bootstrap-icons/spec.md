## ADDED Requirements

### Requirement: El frontend MUST disponer de Bootstrap Icons como librería reutilizable
El sistema MUST cargar `Bootstrap Icons` de forma global para que los componentes del frontend puedan usar iconos consistentes sin depender de `svg` inline locales.

#### Scenario: Librería de iconos disponible en la app
- **WHEN** la aplicación renderiza componentes que dependen de iconos de interfaz
- **THEN** `Bootstrap Icons` MUST estar disponible globalmente para su uso por clases o identificadores del set

### Requirement: Los componentes que adopten Bootstrap Icons MUST poder definir icono y texto por separado
El sistema MUST permitir que los componentes que usen `Bootstrap Icons` modelen la etiqueta textual y la referencia iconográfica como elementos distintos.

#### Scenario: Componente con label e icono desacoplados
- **WHEN** un componente de navegación define sus items visuales
- **THEN** el sistema MUST poder representar el icono sin reutilizar abreviaturas textuales como sustituto visual
- **AND** el sistema MUST conservar el label localizado por separado
