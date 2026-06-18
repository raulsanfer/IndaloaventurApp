## ADDED Requirements

### Requirement: Definición de contratos de servicio en capa de aplicación
El sistema MUST definir interfaces de servicio para operaciones de dominio frontend de forma independiente al transporte HTTP.

#### Scenario: Diseño por contrato
- **WHEN** se implementa una funcionalidad que requiere acceso a datos remotos
- **THEN** el sistema MUST exponer una interfaz de servicio que represente la operación antes de crear su implementación concreta

### Requirement: Uso obligatorio de IHttpClientFactory
El sistema MUST crear clientes HTTP para servicios remotos mediante `IHttpClientFactory` con registro en DI y configuración centralizada.

#### Scenario: Creación de cliente HTTP
- **WHEN** un servicio necesita consumir el API
- **THEN** el sistema MUST resolver un cliente tipado o nombrado desde la factoría en lugar de instanciar `HttpClient` manualmente

### Requirement: Implementaciones de servicio desacopladas de UI
El sistema MUST mantener las implementaciones HTTP fuera de componentes de presentación y permitir sustitución por mocks/fakes en pruebas.

#### Scenario: Prueba de componente sin red
- **WHEN** se ejecuta una prueba de componente que depende de un servicio
- **THEN** el sistema MUST poder inyectar una implementación fake del contrato sin realizar llamadas HTTP reales

### Requirement: Convenciones de código limpio para servicios
El sistema MUST aplicar responsabilidad única, nombres semánticos y manejo uniforme de errores en las implementaciones de servicios.

#### Scenario: Revisión de implementación de servicio
- **WHEN** se revisa una implementación de servicio frontend
- **THEN** el sistema MUST mostrar una única responsabilidad principal, nomenclatura explícita y estrategia consistente de tratamiento de errores
