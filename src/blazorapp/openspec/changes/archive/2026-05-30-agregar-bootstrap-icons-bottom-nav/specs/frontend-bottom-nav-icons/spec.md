## ADDED Requirements

### Requirement: BottomNav MUST renderizar iconos en lugar de abreviaturas textuales
El sistema MUST reemplazar las abreviaturas visuales actuales de `BottomNav` por iconos de navegación compatibles con la referencia de diseño, manteniendo los textos localizados existentes.

#### Scenario: Render de item con icono y label
- **WHEN** `BottomNav` renderiza cualquiera de sus items
- **THEN** el sistema MUST mostrar un icono visual para el item
- **AND** el sistema MUST mostrar el texto localizado correspondiente bajo o junto al icono según el diseño aplicado

### Requirement: BottomNav MUST intentar mapear la referencia visual con Bootstrap Icons
El sistema MUST seleccionar iconos de `Bootstrap Icons` que representen razonablemente los items `Home`, `Mi Club` y `Mi Cuenta` tomando como guía la referencia visual aportada.

#### Scenario: Mapeo de iconos disponibles
- **WHEN** existe un icono suficientemente equivalente en `Bootstrap Icons` para uno de los items de `BottomNav`
- **THEN** el sistema MUST usar ese icono en la botonera

### Requirement: El sistema MUST avisar cuando un icono de referencia no tenga equivalencia suficiente
El sistema MUST dejar constancia de los casos en que `Bootstrap Icons` no ofrezca una equivalencia suficientemente fiel para un icono requerido por la referencia.

#### Scenario: Icono no localizado con fidelidad aceptable
- **WHEN** no existe en `Bootstrap Icons` un icono suficientemente equivalente para un item de la `BottomNav`
- **THEN** la implementación MUST avisar explícitamente de ese faltante
- **AND** el sistema MUST permitir que el usuario aporte el icono definitivo antes de cerrar el ajuste visual final

### Requirement: BottomNav MUST mantener comportamiento visual coherente en estado activo
El sistema MUST conservar el estado activo de la `BottomNav` al incorporar iconos, sin perder legibilidad ni jerarquía visual del item seleccionado.

#### Scenario: Item activo con icono
- **WHEN** un item de `BottomNav` coincide con `ActiveHref`
- **THEN** el sistema MUST reflejar visualmente el estado activo tanto en el icono como en el label del item
