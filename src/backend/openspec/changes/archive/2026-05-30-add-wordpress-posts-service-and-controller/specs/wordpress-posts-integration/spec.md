## ADDED Requirements

### Requirement: Servicio inyectable para operaciones con WordPress
El sistema MUST exponer un servicio inyectable para operar con WordPress, y dicho servicio SHALL estar diseñado para soportar operaciones de lectura en esta iteración y ampliación futura a publicación de posts.

#### Scenario: Resolución por inyección de dependencias
- **WHEN** la aplicación inicia y registra servicios de infraestructura
- **THEN** el contenedor de dependencias SHALL poder resolver el servicio de WordPress sin errores de configuración

### Requirement: Configuración centralizada en appsettings para WordPress
El sistema MUST leer toda la configuración de integración con WordPress desde `appsettings` mediante opciones tipadas, incluyendo al menos URL base del sitio, clave de acceso y parámetros operativos necesarios para llamadas HTTP.

#### Scenario: Configuración válida
- **WHEN** existen claves de WordPress válidas en `appsettings`
- **THEN** el servicio SHALL construir peticiones al endpoint WordPress usando esa configuración

#### Scenario: Configuración incompleta o inválida
- **WHEN** falta una clave obligatoria de configuración WordPress
- **THEN** el sistema SHALL fallar de forma controlada con un error user-facing en español

### Requirement: Consulta de posts WordPress sin persistencia local
El sistema MUST permitir consultar posts de WordPress desde un endpoint del backend y SHALL devolver los datos obtenidos sin persistirlos en la base de datos local.

#### Scenario: Consulta exitosa de posts
- **WHEN** se invoca el endpoint de consulta y WordPress responde correctamente
- **THEN** la API SHALL devolver una colección de posts mapeada al contrato del backend
- **AND** no SHALL realizar inserciones ni actualizaciones de posts en la base de datos local

#### Scenario: Error remoto de WordPress
- **WHEN** WordPress devuelve error HTTP o la llamada falla por red/timeout
- **THEN** la API SHALL devolver un error controlado al cliente en español

### Requirement: Controlador API para exponer posts de WordPress
El sistema MUST habilitar un controlador API dedicado para consulta de posts WordPress y SHALL soportar parámetros básicos de consulta definidos por el contrato backend.

#### Scenario: Endpoint de consulta disponible
- **WHEN** un cliente llama al endpoint configurado para posts WordPress
- **THEN** el controlador SHALL delegar la operación en el servicio WordPress
- **AND** la respuesta SHALL mantener estructura consistente del API
