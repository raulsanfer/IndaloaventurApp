## ADDED Requirements

### Requirement: El sistema MUST disponer de una pagina Alertas Alimentarias con tres categorias fijas
El sistema MUST ofrecer una pagina dedicada `Alertas Alimentarias` como punto de entrada de esta funcionalidad y MUST mostrar exactamente tres categorias en formato lista, cada una dentro de una card con fondo blanco y esquinas redondeadas.

#### Scenario: Render de la pagina de categorias
- **WHEN** el usuario accede a la pagina `Alertas Alimentarias`
- **THEN** el sistema MUST mostrar el titulo principal `Alertas Alimentarias`
- **AND** el sistema MUST mostrar tres cards diferenciadas para `Generales`, `Complementos Alimenticios` y `Alergias`
- **AND** cada card MUST mantener fondo blanco y esquinas redondeadas

### Requirement: Cada categoria MUST mostrar su copy descriptivo definido
El sistema MUST mostrar para cada categoria un titulo en negrita y un subtitulo en fuente mas pequena con el texto funcional definido para esa categoria.

#### Scenario: Copy de las categorias visible
- **WHEN** la pagina `Alertas Alimentarias` renderiza las cards de categoria
- **THEN** la card `Generales` MUST mostrar el subtitulo `Alertas alimentarias de interes para toda la poblacion`
- **AND** la card `Complementos Alimenticios` MUST mostrar el subtitulo `Alertas alimentarias para personas que consumen complementos alimenticios`
- **AND** la card `Alergias` MUST mostrar el subtitulo `Alertas alimentarias para personas con alergias, intolerancias u otros efectos adversos a determinadas sustancias`

### Requirement: Cada categoria MUST navegar a una vista de listado reutilizable por codigo
El sistema MUST reutilizar una misma vista de listado para las tres categorias y MUST resolver la consulta remota usando el codigo asociado a la categoria seleccionada.

#### Scenario: Navegacion a listado por categoria
- **WHEN** el usuario pulsa una card de categoria en `Alertas Alimentarias`
- **THEN** el sistema MUST navegar a una pagina de listado de alertas para esa categoria
- **AND** la categoria `Generales` MUST consultar con `code=general`
- **AND** la categoria `Complementos Alimenticios` MUST consultar con `code=complementos`
- **AND** la categoria `Alergias` MUST consultar con `code=alergenos`

### Requirement: El listado de alertas MUST mostrar titulo y extracto de descripcion
El sistema MUST recuperar las alertas de la categoria seleccionada mediante una capa frontend desacoplada de la vista y MUST mostrar cada resultado con su titulo y un extracto de la descripcion de aproximadamente 100 caracteres.

#### Scenario: Render de una alerta en listado
- **WHEN** la vista de listado recibe alertas para una categoria
- **THEN** el sistema MUST mostrar cada alerta como un item accionable de listado
- **AND** cada item MUST mostrar el titulo de la alerta
- **AND** cada item MUST mostrar solo una parte de la descripcion, limitada aproximadamente a 100 caracteres

### Requirement: El sistema MUST usar el identificador nativo de la alerta para soportar navegacion a detalle
El sistema MUST usar el `Id` nativo expuesto por la integracion de alertas para permitir navegar desde el listado hasta un detalle recargable por URL.

#### Scenario: Navegacion al detalle desde el listado
- **WHEN** el usuario pulsa una alerta del listado
- **THEN** el sistema MUST navegar a una vista de detalle de esa alerta usando el `Id` nativo de la alerta
- **AND** el sistema MUST poder resolver la misma alerta si el usuario recarga la URL de detalle directamente

### Requirement: La vista de detalle MUST mostrar la descripcion completa de la alerta
El sistema MUST obtener toda la informacion disponible de la alerta seleccionada y MUST mostrar su descripcion completa en la vista de detalle.

#### Scenario: Detalle de alerta cargado correctamente
- **WHEN** el usuario accede al detalle de una alerta valida
- **THEN** el sistema MUST mostrar el titulo completo de la alerta
- **AND** el sistema MUST mostrar la descripcion completa asociada a esa alerta
- **AND** el sistema MUST mantener la categoria consultada como contexto funcional del detalle

#### Scenario: Recuperacion del detalle por endpoint dedicado
- **WHEN** la vista de detalle necesita obtener una alerta concreta
- **THEN** el sistema MUST consultar la integracion dedicada de detalle para esa alerta usando la forma `/api/Alerts/{id}`

### Requirement: Las vistas de listado y detalle MUST manejar estados de carga, vacio y error
El sistema MUST informar de forma comprensible cuando una categoria este cargando, no devuelva alertas o falle al recuperarse, y MUST aplicar el mismo criterio al detalle.

#### Scenario: Categoria sin alertas
- **WHEN** el endpoint de una categoria devuelve cero alertas
- **THEN** el sistema MUST mostrar un estado vacio comprensible en la vista de listado

#### Scenario: Error al recuperar listado o detalle
- **WHEN** la integracion de alertas no puede completar la recuperacion del listado o del detalle
- **THEN** el sistema MUST mostrar un estado de error comprensible para el usuario
- **AND** el sistema MUST evitar dejar la vista en un estado inconsistente
