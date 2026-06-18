# wordpress-posts-integration Specification

## Purpose
TBD - created by archiving change add-wordpress-posts-service-and-controller. Update Purpose after archive.
## Requirements
### Requirement: Servicio inyectable para operaciones con WordPress
El sistema MUST exponer un servicio inyectable para operar con WordPress, y dicho servicio SHALL estar disenado para soportar operaciones de lectura en esta iteracion y ampliacion futura a publicacion de posts.

#### Scenario: Resolucion por inyeccion de dependencias
- **WHEN** la aplicacion inicia y registra servicios de infraestructura
- **THEN** el contenedor de dependencias SHALL poder resolver el servicio de WordPress sin errores de configuracion

### Requirement: Configuracion centralizada en appsettings para WordPress
El sistema MUST leer toda la configuracion de integracion con WordPress desde `appsettings` mediante opciones tipadas, incluyendo al menos URL base del sitio, clave de acceso y parametros operativos necesarios para llamadas HTTP. El sistema SHALL permitir configurar en `appsettings` el numero por defecto de posts a consultar en el endpoint de listado cuando el cliente no informe un `pageSize`.

#### Scenario: Configuracion valida
- **WHEN** existen claves de WordPress validas en `appsettings`
- **THEN** el servicio SHALL construir peticiones al endpoint WordPress usando esa configuracion
- **AND** el listado SHALL usar el numero de posts por defecto configurado cuando no se informe `pageSize`

#### Scenario: Configuracion incompleta o invalida
- **WHEN** falta una clave obligatoria de configuracion WordPress
- **THEN** el sistema SHALL fallar de forma controlada con un error user-facing en espanol

### Requirement: Consulta de posts WordPress sin persistencia local
El sistema MUST permitir consultar posts de WordPress desde un endpoint del backend y SHALL devolver los datos obtenidos sin persistirlos en la base de datos local. Para el listado de posts, el sistema SHALL solicitar y devolver unicamente la informacion de resumen necesaria: titulo, descripcion, fecha de publicacion e imagen destacada.

#### Scenario: Consulta exitosa de posts
- **WHEN** se invoca el endpoint de consulta y WordPress responde correctamente
- **THEN** la API SHALL devolver una coleccion de posts mapeada al contrato resumido del backend
- **AND** cada item del listado SHALL incluir `Titulo`, `Resumen`, `FechaPublicacionUtc` e `ImagenDestacadaUrl`
- **AND** el listado SHALL no incluir `Contenido` ni `Enlace`
- **AND** no SHALL realizar inserciones ni actualizaciones de posts en la base de datos local

#### Scenario: Error remoto de WordPress
- **WHEN** WordPress devuelve error HTTP o la llamada falla por red/timeout
- **THEN** la API SHALL devolver un error controlado al cliente en espanol

#### Scenario: Listado con imagen destacada disponible
- **WHEN** WordPress devuelve un post listado con imagen destacada publicada
- **THEN** el sistema SHALL mapear `ImagenDestacadaUrl` correctamente en la respuesta del listado

### Requirement: Controlador API para exponer posts de WordPress
El sistema MUST habilitar un controlador API dedicado para consulta de posts WordPress y SHALL soportar parametros basicos de consulta definidos por el contrato backend. Cuando el cliente no informe `pageSize`, el controlador SHALL aplicar el valor por defecto configurado para el listado.

#### Scenario: Endpoint de consulta disponible
- **WHEN** un cliente llama al endpoint configurado para posts WordPress
- **THEN** el controlador SHALL delegar la operacion en el servicio WordPress
- **AND** la respuesta SHALL mantener estructura consistente del API

#### Scenario: Cliente omite pageSize
- **WHEN** un cliente llama al endpoint de listado sin informar `pageSize`
- **THEN** el sistema SHALL consultar WordPress usando el numero de posts por defecto configurado

