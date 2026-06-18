## Why

La API necesita una base sólida y mantenible para crecer en funcionalidades del club sin degradar calidad, seguridad ni rendimiento. Definir desde OpenSpec una arquitectura limpia con DDD, CQRS, autenticación robusta y patrones consistentes reduce deuda técnica desde el inicio y acelera futuras entregas.

## What Changes

- Se introduce una solución API REST en C# (.NET 9) con arquitectura por capas siguiendo Clean Architecture y DDD.
- Se define el uso de controladores MVC clásicos de ASP.NET Core (sin Minimal APIs).
- Se incorpora patrón CQRS para separar comandos y consultas, con pipeline de validación y manejo uniforme de errores.
- Se establece persistencia híbrida:
  - Comandos y transacciones con Entity Framework Core.
  - Consultas optimizadas con Dapper.
- Se integra autenticación y autorización con JWT y ASP.NET Identity.
- Se estandariza el manejo de errores HTTP mediante ProblemDetails (RFC 7807) para respuestas consistentes.
- Se formaliza el patrón Repository en la capa de dominio/aplicación para abstracción de acceso a datos.

## Capabilities

### New Capabilities
- `clean-architecture-ddd-foundation`: Estructura de solución por capas (Domain, Application, Infrastructure, API) con límites y dependencias de Clean Architecture + DDD.
- `cqrs-command-query-split`: Implementación de CQRS para separación de escrituras (EF Core) y lecturas (Dapper).
- `jwt-identity-authentication`: Autenticación/autorización basada en JWT con ASP.NET Identity.
- `problem-details-error-contract`: Contrato de errores API usando ProblemDetails para validación, dominio e infraestructura.
- `repository-pattern-persistence`: Definición y uso del patrón Repository para agregados y operaciones de persistencia transaccional.

### Modified Capabilities
- Ninguna (no existen capacidades previas en `openspec/specs/` para modificar).

## Impact

- Se crearán nuevos proyectos/carpetas de solución para capas de dominio, aplicación, infraestructura y API.
- Se definirán endpoints REST iniciales con controladores clásicos y contratos DTO.
- Se añadirán dependencias OSS: EF Core (SQL Server), Dapper, ASP.NET Identity, autenticación JWT, y librerías de soporte CQRS/validación según necesidad.
- Se incorporarán políticas de seguridad (emisión/validación de token, expiración, claims/roles) y convenciones de error unificadas para clientes móviles.
- Incrementa complejidad inicial de bootstrap, pero establece base mantenible y extensible para siguientes cambios.