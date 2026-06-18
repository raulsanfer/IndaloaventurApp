## Context

El backend ya expone flujos de gestión de usuarios mediante `UsersController` y casos de uso en `Application/Features/Users`. En paralelo, se detecta deuda técnica asociada a carpetas o artefactos de `AdventureMembers` que no aportan comportamiento vigente.

La solución necesita una base mínima de pruebas automatizadas que cubra:
- Casos de uso de gestión de usuarios (comandos/queries).
- Endpoints HTTP de gestión de usuarios (estado HTTP y contratos básicos).

La limpieza debe ejecutarse sin romper compilación, inyección de dependencias ni rutas expuestas.

## Goals / Non-Goals

**Goals:**
- Eliminar artefactos `AdventureMembers` no utilizados y dejar la solución compilando sin referencias huérfanas.
- Definir y añadir una batería mínima de pruebas para los casos de uso de usuarios.
- Definir y añadir pruebas básicas para endpoints de gestión de usuarios, incluyendo autorización mínima.
- Dejar criterios verificables para ejecutar en CI local o pipeline.

**Non-Goals:**
- Rediseñar la arquitectura de identidad/autorización.
- Incrementar cobertura más allá de un baseline mínimo.
- Cambiar contratos funcionales de negocio de gestión de usuarios.

## Decisions

1. Limpieza guiada por referencias reales de compilación
- Decisión: eliminar solo carpetas/archivos `AdventureMembers` sin referencias activas en `.csproj`, `using`, DI o rutas.
- Rationale: minimiza riesgo de regresión en una limpieza estructural.
- Alternativa considerada: borrado masivo por nombre de carpeta. Rechazada por mayor riesgo de romper dependencias indirectas.

2. Baseline de pruebas por capa
- Decisión: separar pruebas de casos de uso (unidad) y pruebas de endpoints (integración/API).
- Rationale: permite detectar regresiones de lógica y de contrato HTTP con menor acoplamiento.
- Alternativa considerada: solo integración end-to-end. Rechazada por mayor coste y diagnóstico más lento.

3. Criterios mínimos de escenarios por endpoint/caso de uso
- Decisión: cubrir al menos camino feliz y un caso de fallo/validación o autorización para cada endpoint/caso de uso principal.
- Rationale: establece un umbral razonable de calidad sin bloquear la entrega por cobertura exhaustiva.
- Alternativa considerada: exigir matriz completa de errores. Rechazada por alcance excesivo para este cambio.

## Risks / Trade-offs

- [Eliminar archivos aparentemente no usados pero con uso implícito] -> Mitigación: búsqueda por referencias (`rg`) y validación con build + tests.
- [Pruebas frágiles por dependencia de infraestructura] -> Mitigación: usar dobles/mocks en aplicación y host de pruebas controlado en API.
- [Falso sentido de seguridad por baseline mínimo] -> Mitigación: documentar explícitamente que es base inicial y extender en cambios posteriores.
