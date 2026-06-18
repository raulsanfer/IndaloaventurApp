## ADDED Requirements

### Requirement: El sistema MUST disponer de una página SignalHome accesible desde Home y desde el menú footer
El sistema MUST ofrecer una página dedicada `SignalHome` como punto de entrada de la funcionalidad `Señales`, accesible desde la experiencia Home del socio, accesible también desde el menú footer autenticado y preparada para alojar las siguientes fases del dominio.

#### Scenario: Navegación a SignalHome
- **WHEN** el usuario accede a la funcionalidad `Señales` desde Home
- **THEN** el sistema MUST cargar una página específica `SignalHome`
- **AND** el sistema MUST mostrar un título principal coherente con la funcionalidad `Señales`

#### Scenario: Navegación a SignalHome desde el footer
- **WHEN** el usuario accede a la entrada `Signals` del menú footer autenticado
- **THEN** el sistema MUST cargar la misma página `SignalHome`
- **AND** el sistema MUST mantener la navegación inferior coherente con el destino activo

### Requirement: El menú footer autenticado MUST incluir Signals para todos los roles
El sistema MUST mostrar una entrada de navegación `Signals` en el menú footer autenticado para cualquier rol de usuario y MUST representarla con una iconografía visual asociada a advertencia o incidencia.

#### Scenario: Footer visible para cualquier rol autenticado
- **WHEN** un usuario autenticado de cualquier rol visualiza el shell principal
- **THEN** el sistema MUST mostrar la entrada `Signals` en el menú footer
- **AND** el sistema MUST no ocultar dicha entrada por permisos de rol específicos

#### Scenario: Iconografía de advertencia en la navegación
- **WHEN** la entrada `Signals` se renderiza en el menú footer
- **THEN** el sistema MUST mostrar un icono con semántica de advertencia o incidencia
- **AND** el sistema MUST mantener el mismo patrón visual base que el resto de destinos del footer

#### Scenario: Posición estable de Signals en el footer
- **WHEN** el menú footer autenticado se renderiza
- **THEN** el sistema MUST ubicar `Signals` entre `Home` y `Mi Cuenta`
- **AND** el sistema MUST priorizar un orden donde `Signals` aparezca antes de `Mi Club`

### Requirement: SignalHome MUST mostrar breadcrumb contextual
El sistema MUST mostrar en `SignalHome` un breadcrumb visible y accionable que permita regresar a la superficie de origen y entender el contexto de navegación actual.

#### Scenario: Breadcrumb renderizado en SignalHome
- **WHEN** la página `SignalHome` se renderiza
- **THEN** el sistema MUST mostrar un enlace accionable de retorno a Home
- **AND** el sistema MUST mostrar el contexto actual de `Señales` dentro del breadcrumb
- **AND** el sistema MUST mantener el breadcrumb por encima del listado principal

### Requirement: SignalHome MUST recuperar el listado de señales desde el API
El sistema MUST obtener la información del listado consumiendo el API de signals mediante una capa frontend desacoplada de la vista y MUST poder complementar la presentación con categorías de `signal-types`.

#### Scenario: Carga inicial del listado
- **WHEN** la página `SignalHome` se inicializa
- **THEN** el sistema MUST recuperar las señales desde el servicio frontend asociado al API de `signals`
- **AND** el sistema MUST resolver las categorías visibles a partir del API de `signal-types`
- **AND** el sistema MUST mostrar las señales recuperadas en la vista de listado

### Requirement: SignalHome MUST ofrecer búsqueda textual y filtro por categorías
El sistema MUST mostrar un campo de búsqueda y una fila de categorías en formato píldora para refinar el listado de señales durante esta primera fase.

#### Scenario: Filtrado por texto
- **WHEN** el usuario introduce un texto de búsqueda en `SignalHome`
- **THEN** el sistema MUST refinar el listado usando ese criterio textual
- **AND** el sistema MUST mantener visible el valor introducido en el control de búsqueda

#### Scenario: Filtrado por categoría
- **WHEN** el usuario selecciona una categoría de la fila de píldoras
- **THEN** el sistema MUST marcar visualmente esa categoría como activa
- **AND** el sistema MUST refinar el listado para mostrar las señales asociadas a dicha categoría

### Requirement: La fila de categorías MUST soportar scroll horizontal
El sistema MUST renderizar las categorías como chips de una sola línea y MUST permitir desplazamiento horizontal cuando el número de categorías supere el ancho disponible.

#### Scenario: Muchas categorías en pantalla móvil
- **WHEN** la cantidad de categorías no cabe en el ancho visible de `SignalHome`
- **THEN** el sistema MUST permitir scroll horizontal sobre la fila de chips
- **AND** el sistema MUST evitar que las categorías salten a múltiples líneas
- **AND** el sistema MUST mantener accesible la selección de cualquier categoría

### Requirement: SignalHome MUST presentar cada señal como una card de listado alineada con el diseño
El sistema MUST mostrar cada señal dentro de una card visualmente separada, preparada para incluir imagen, categoría, fecha, texto principal y metadato destacado siguiendo el layout base del diseño suministrado.

#### Scenario: Render de una señal en el listado
- **WHEN** una señal se muestra en la lista de `SignalHome`
- **THEN** el sistema MUST agrupar su contenido dentro de una card independiente
- **AND** el sistema MUST mostrar la categoría de la señal en formato píldora
- **AND** el sistema MUST mostrar una referencia temporal visible de la señal
- **AND** el sistema MUST mostrar el contenido textual principal sin romper la composición de la card

#### Scenario: Campo visual opcional ausente
- **WHEN** una señal no disponga de alguno de los campos visuales opcionales del diseño
- **THEN** el sistema MUST conservar una card estable y legible
- **AND** el sistema MUST evitar huecos rotos o errores de renderizado por datos ausentes

### Requirement: SignalHome MUST usar DaisyUI como base de sus nuevos patrones visuales
El sistema MUST construir los nuevos componentes visuales de `SignalHome` sobre la base de DaisyUI y MUST usar SCSS complementario únicamente para adaptar detalles de integración, spacing o acabado al lenguaje visual del proyecto.

#### Scenario: Render de la base visual de SignalHome
- **WHEN** se renderizan cards, chips, campos de búsqueda y superficies interactivas de `SignalHome`
- **THEN** el sistema MUST apoyarse en patrones base de DaisyUI
- **AND** el sistema MUST permitir ajustes SCSS acotados para mantener coherencia con otras vistas del proyecto

### Requirement: SignalHome MUST manejar estados de carga, vacío y error
El sistema MUST informar de forma comprensible cuando el listado de señales esté cargando, no devuelva resultados o falle al recuperarse.

#### Scenario: Carga en progreso
- **WHEN** `SignalHome` está esperando la respuesta del servicio de señales
- **THEN** el sistema MUST mostrar un estado de carga comprensible para el usuario

#### Scenario: Sin resultados
- **WHEN** el servicio devuelve cero señales para el filtro actual
- **THEN** el sistema MUST mostrar un estado vacío comprensible
- **AND** el sistema MUST mantener visibles los controles de búsqueda y categorías

#### Scenario: Error al recuperar señales
- **WHEN** el servicio de señales no puede completarse correctamente
- **THEN** el sistema MUST mostrar un estado de error comprensible
- **AND** el sistema MUST evitar dejar la página en un estado inconsistente
