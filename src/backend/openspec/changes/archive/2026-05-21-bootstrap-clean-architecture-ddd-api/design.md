## Context

El proyecto parte de una API privada para aplicación móvil con SQL Server y .NET 9, sin capacidades funcionales formalizadas aún en OpenSpec. La solución necesita una base arquitectónica estable para evolucionar con seguridad (JWT), consistencia funcional (DDD + CQRS) y buen rendimiento en lecturas (Dapper), manteniendo transaccionalidad robusta en escrituras (EF Core). El equipo requiere controladores ASP.NET Core clásicos para alinearse con su forma de trabajo y evitar Minimal APIs.

## Goals / Non-Goals

**Goals:**
- Definir una arquitectura de referencia basada en Vertical Slice con fronteras limpias entre dominio, aplicación, infraestructura y API.
- Aplicar DDD táctico para agregados, entidades, value objects y reglas de dominio.
- Implementar CQRS con separación explícita de comandos y consultas.
- Usar EF Core para comandos/transacciones y Dapper para consultas.
- Incorporar JWT + ASP.NET Identity para autenticación y autorización.
- Estandarizar respuestas de error con ProblemDetails y trazabilidad básica.
- Usar inyección de dependencias para desacoplar infraestructura, handlers y servicios transversales.

**Non-Goals:**
- Cubrir todos los casos de negocio de la app móvil en este cambio inicial.
- Implementar eventos distribuidos, saga orchestration o microservicios.
- Resolver reporting avanzado o BI.
- Incluir UI/Admin panel.

## Decisions

1. Arquitectura base: Vertical Slice con límites limpios
- Decisión: La API se organizará por slices funcionales (feature folders) y cada slice contendrá comandos, queries, validaciones, contratos y controlador asociado. Se mantendrán límites claros entre dominio, aplicación e infraestructura para preservar principios de Clean Architecture.
- Rationale: Reduce fricción de mantenimiento por caso de uso, facilita evolución incremental y mantiene bajo acoplamiento.
- Alternativas:
  - N-Layer clásico: más simple al inicio, pero tiende a concentrar lógica y aumentar acoplamiento.
  - Clean Architecture estricta por capas sin slices: buena separación, pero más fricción para entregar verticalmente casos de uso.

2. Inyección de dependencias como mecanismo de composición
- Decisión: Se usará el contenedor de DI de ASP.NET Core para registrar handlers CQRS (MediatR), repositorios, servicios de token, acceso a datos y cross-cutting concerns.
- Rationale: Permite inversión de dependencias, testabilidad por sustitución de implementaciones y composición centralizada.
- Alternativas:
  - Instanciación manual en controladores/handlers: mayor acoplamiento y menor testabilidad.

3. DDD táctico para modelado
- Decisión: Modelar agregados y repositorios por agregado raíz; usar value objects para invariantes.
- Rationale: Reduce lógica anémica y protege reglas de negocio.
- Alternativas:
  - CRUD anémico con servicios: más rápido al inicio, peor evolución de dominio.

4. CQRS con pipeline de aplicación
- Decisión: Comandos y queries separados con handlers MediatR; validación previa a ejecución y comportamiento de errores homogéneo.
- Rationale: Separa preocupaciones y permite optimizar lectura y escritura de forma independiente.
- Alternativas:
  - Servicios únicos por caso de uso: menos piezas, pero mezcla responsabilidades y dificulta optimización.

5. Persistencia híbrida: EF Core para comandos, Dapper para queries
- Decisión: `DbContext` y transacciones para comandos; conexiones de solo lectura y SQL explícito para consultas.
- Rationale: Balance entre consistencia transaccional y performance de lectura.
- Alternativas:
  - Solo EF Core para todo: menor complejidad, posible costo de rendimiento y control SQL.
  - Solo Dapper para todo: alto control, mayor costo en mantenimiento de mapeos/consistencia.

6. Seguridad con ASP.NET Identity + JWT
- Decisión: Identity como gestión de usuarios/roles/password policy; JWT Bearer para sesiones stateless del cliente móvil.
- Rationale: Solución madura OSS, bien integrada con ASP.NET Core.
- Alternativas:
  - Autenticación custom: más control, mayor riesgo de seguridad.
  - Proveedor externo completo: reduce código propio, añade dependencia operativa externa.

7. Manejo de errores con ProblemDetails
- Decisión: Middleware/filtro centralizado que traduzca errores de validación, dominio y técnicos a ProblemDetails.
- Rationale: Contrato uniforme para clientes y mejor observabilidad.
- Alternativas:
  - Manejo ad-hoc por controlador: inconsistente y difícil de mantener.

## Risks / Trade-offs

- [Mayor complejidad de bootstrap] -> Mitigación: plantillas de proyecto, convenciones documentadas y pruebas de arquitectura.
- [Doble stack de persistencia (EF + Dapper)] -> Mitigación: contratos claros (ICommandDbContext/IQueryConnectionFactory) y tests de integración de ambos caminos.
- [Errores de configuración JWT/Identity] -> Mitigación: validaciones de startup, variables obligatorias y pruebas de autenticación/autorización.
- [Desalineación entre modelo de dominio y esquema SQL] -> Mitigación: migraciones versionadas y pruebas de repositorio por agregado crítico.
- [Sobreingeniería temprana] -> Mitigación: alcance inicial acotado a capacidades base y vertical slice de referencia.

## Migration Plan

1. Crear estructura inicial por slices funcionales y configurar proyectos/límites técnicos compartidos.
2. Configurar DI (registro de MediatR, repositorios, servicios de seguridad y acceso a datos).
3. Configurar dependencias base (EF Core SQL Server, Dapper, Identity, JWT).
4. Implementar módulo de identidad y emisión de tokens.
5. Implementar vertical slice de referencia (comando + query + controlador) con CQRS completo.
6. Integrar ProblemDetails y validación en pipeline.
7. Ejecutar migraciones iniciales y pruebas automáticas.
8. Despliegue progresivo en entorno no productivo y validación de contrato API.

Rollback:
- Mantener scripts de migración reversible.
- Feature flags para endpoints nuevos si aplica.
- Revertir despliegue al artefacto anterior y ejecutar rollback de migración en caso de fallo crítico.

## Open Questions (Resolved)

- CQRS/Mediación: Se usará MediatR como librería de mediación.
- JWT/Refresh Token: `access token` de 60 minutos, `refresh token` de 7 días, sin rotación.
- Versionado API: No se aplicará versionado en esta fase inicial.
- Multitenancy: No se implementará ahora; se pospone a fase 2.