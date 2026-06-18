## ADDED Requirements

### Requirement: Estructura base de layout de login reutilizable
El sistema MUST renderizar una pantalla de login inicial compuesta por una estructura reutilizable en SharedUI, separando claramente cabecera visual, formulario principal y pie de acciones.

#### Scenario: Renderizado de layout completo
- **WHEN** un usuario navega a la ruta de acceso de la aplicación
- **THEN** el sistema MUST mostrar la estructura completa del layout de login con cabecera, formulario y pie

### Requirement: Experiencia responsive para móvil y escritorio
El sistema MUST adaptar el layout de login a pantallas móviles y de escritorio manteniendo legibilidad, jerarquía visual y accesibilidad básica de controles.

#### Scenario: Visualización en móvil
- **WHEN** la aplicación se abre en una pantalla móvil
- **THEN** el sistema MUST presentar el formulario en una columna única con espaciado y tamaños adecuados para interacción táctil

#### Scenario: Visualización en escritorio
- **WHEN** la aplicación se abre en una pantalla de escritorio
- **THEN** el sistema MUST mantener la jerarquía visual del login con contenedor centrado y distribución proporcional del contenido

### Requirement: Estilo inicial alineado con guía visual de login
El sistema MUST aplicar el estilo base del login usando tipografía Outfit, color de acento naranja y componentes visuales equivalentes a la referencia proporcionada en `openspec/design/login`.

#### Scenario: Aplicación de identidad visual
- **WHEN** se renderiza la pantalla de login
- **THEN** el sistema MUST reflejar tipografía, paleta y patrones visuales definidos como base del layout inicial

### Requirement: Gestión de estilos exclusivamente en SCSS global
El sistema MUST definir los estilos del layout de login en archivos SCSS organizados e importados desde el fichero global `style.scss`, sin estilos inline ni estilos específicos en componentes Razor.

#### Scenario: Definición de estilos centralizada
- **WHEN** un desarrollador revisa la implementación de estilos del login
- **THEN** el sistema MUST tener las reglas de estilo en parciales SCSS globales registrados en `style.scss`

### Requirement: Literales localizados para pantalla de login
El sistema MUST obtener todos los textos visibles del layout de login desde recursos localizados en español mediante claves cortas inyectadas con localizer.

#### Scenario: Resolución de textos del formulario
- **WHEN** la pantalla de login se renderiza
- **THEN** el sistema MUST mostrar labels, placeholders y acciones usando claves de recursos en lugar de literales hardcodeados

### Requirement: Servicios frontend desacoplados por interfaz
El sistema MUST definir contratos de servicio en la capa de aplicación y utilizar inyección de dependencias para que la UI no dependa directamente de implementaciones HTTP.

#### Scenario: Consumo desde UI mediante contrato
- **WHEN** un componente de login necesita una operación de servicio
- **THEN** el componente MUST depender de una interfaz de servicio y no de una clase concreta de acceso HTTP

### Requirement: Configuración de cliente HTTP mediante factoría
El sistema MUST configurar consumo de API usando `IHttpClientFactory` y clientes tipados registrados en DI, evitando instanciación manual de `HttpClient`.

#### Scenario: Registro de cliente tipado
- **WHEN** se inicializa la aplicación
- **THEN** el sistema MUST registrar al menos un cliente tipado con base URL y configuración centralizada para servicios del dominio login

### Requirement: Reglas de código limpio en servicios y presentación
El sistema MUST aplicar principios de código limpio en la base del frontend: responsabilidad única por clase, nombres explícitos, separación de lógica de vista y manejo uniforme de errores.

#### Scenario: Revisión de implementación base
- **WHEN** se revisa la implementación de componentes y servicios del login
- **THEN** el sistema MUST evidenciar separación de responsabilidades y convenciones consistentes de manejo de errores
