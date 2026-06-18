## Context

IndaloAventurApp arranca la capa frontend sobre Blazor Web App + InteractiveWebAssembly + PWA, con componentes compartidos en `IndaloaventurApp.SharedUI` y consumo posterior de endpoints del API ya disponible. Actualmente no existe una base formal del flujo inicial completo (acceso y estado autenticado), y se han proporcionado referencias visuales en `openspec/design/login` y `openspec/design/home` para acelerar el arranque sin romper los principios del proyecto: SCSS global/modular, localización por claves y separación Razor/partial class. Además, para evitar deuda técnica temprana, se necesita fijar desde el inicio un patrón estándar para servicios frontend y configuración de `HttpClientFactory`.

## Goals / Non-Goals

**Goals:**
- Definir una base técnica de layout de login reusable entre web y futuros clientes híbridos/móvil mediante SharedUI.
- Definir la Home autenticada con cabecera de app y navegación inferior/botonera persistente, alineada con la referencia de diseño.
- Mantener consistencia visual inicial (Outfit, acentos de marca, estructura y espaciado) adaptada a móvil y escritorio.
- Garantizar que todos los literales visibles queden externalizados en recursos ES mediante claves cortas.
- Establecer organización SCSS por parciales desde `style.scss`, evitando estilos inline o encapsulados en componentes.
- Definir un patrón común para creación de servicios frontend con interfaces, implementación desacoplada y consumo HTTP tipado.

**Non-Goals:**
- Implementar lógica completa de negocio de home (carga final de módulos, recomendaciones o analítica).
- Implementar integración total de autenticación real contra API en esta fase.
- Definir todo el sistema de diseño global de la app más allá del baseline de login/home.

## Decisions

1. Composición en SharedUI con orquestación desde Web
- Decisión: crear componentes Razor reutilizables en `IndaloaventurApp.SharedUI` para piezas de login y home autenticada (cabecera, contenido principal, navegación inferior), y ensamblar páginas en `IndaloaventurApp.Web`.
- Rationale: permite reutilización futura en app híbrida, cumpliendo el objetivo de desacoplo.
- Alternativa considerada: implementar todo directamente en Web. Rechazada por acoplamiento y menor portabilidad.

2. Estilo exclusivo en SCSS global y modular
- Decisión: crear parciales separados por dominio (por ejemplo `_login-*.scss`, `_home-*.scss`, `_app-shell.scss`) e importarlos desde `style.scss`.
- Rationale: cumple restricción de proyecto y mejora mantenibilidad del sistema visual.
- Alternativa considerada: estilos en `.razor.css` por componente. Rechazada por incumplir directriz explícita.

3. Literales en recursos localizados (ES)
- Decisión: definir claves cortas para textos de login y home (incluyendo cabecera y botón/menu inferior) y consumirlas con localizer via DI.
- Rationale: evita hardcode, prepara internacionalización y mantiene consistencia de nomenclatura.
- Alternativa considerada: texto embebido temporal. Rechazada por deuda técnica temprana.

4. Contrato visual inicial basado en referencias de diseño
- Decisión: adoptar `openspec/design/login` para acceso y `openspec/design/home` para estado autenticado como baseline visual.
- Rationale: reduce ambigüedad y acelera la entrega del primer flujo completo.
- Alternativa considerada: rediseñar desde cero. Rechazada para mantener foco en velocidad de arranque.

5. Shell autenticado con cabecera + navegación inferior persistente
- Decisión: crear un app shell autenticado que mantenga cabecera y menú inferior de forma consistente mientras cambia el contenido principal.
- Rationale: patrón común y efectivo en PWA/mobile-first, mejora navegación y percepción de estructura.
- Alternativa considerada: cabecera/botonera duplicadas por página. Rechazada por repetición y riesgo de inconsistencias.

6. Patrón de servicios frontend basado en puertos e implementación HTTP
- Decisión: definir interfaces de aplicación (por ejemplo `IAuthService`) en una capa de contratos y una implementación HTTP separada (por ejemplo `AuthHttpService`) para consumir el API.
- Rationale: mejora testabilidad, permite mock sencillo y desacopla UI del transporte.
- Alternativa considerada: llamadas HTTP directas desde componentes Razor. Rechazada por mezclar responsabilidades.

7. Uso de `IHttpClientFactory` con cliente tipado y configuración centralizada
- Decisión: registrar clientes HTTP tipados con base URL, timeouts y handlers comunes desde DI, evitando `new HttpClient()` manual.
- Rationale: patrón recomendado en .NET para resiliencia, mantenimiento y configuración consistente.
- Alternativa considerada: cliente HTTP único estático. Rechazada por rigidez y menor control por servicio.

8. Convenciones de código limpio para UI y servicios
- Decisión: separar lógica de presentación en clases partial C#, aplicar SRP, nombres explícitos, validaciones previas y manejo uniforme de errores con resultados controlados.
- Rationale: reduce complejidad ciclomática y facilita mantenimiento incremental.
- Alternativa considerada: enfoque ad-hoc por componente. Rechazada por inconsistencias futuras.

## Risks / Trade-offs

- [Riesgo de sobreajuste al mock estático] -> Mitigación: definir clases semánticas y componentes parametrizables para evolucionar estructura sin rehacer base.
- [Inconsistencias entre Web y SharedUI] -> Mitigación: fijar fronteras de responsabilidad; SharedUI renderiza piezas, Web compone y conecta servicios.
- [Crecimiento desordenado de SCSS] -> Mitigación: convención de parciales por dominio (`login/*`, `home/*`, `shell/*`) y revisión de imports en `style.scss`.
- [Claves de recursos ambiguas] -> Mitigación: prefijos funcionales (`login_*`, `home_*`, `shell_*`) y checklist de cobertura de literales.
- [Sobrediseño temprano de servicios] -> Mitigación: crear solo contratos y primeras implementaciones mínimas, documentando reglas para crecimiento.

## Migration Plan

- No requiere migración de datos ni despliegue escalonado.
- Se entrega como introducción de nuevos componentes/estilos/recursos y base de servicios sin romper APIs existentes.
- Rollback: revertir carpeta de cambio y referencias en composición de login/home y registros DI asociados.

## Open Questions

- ¿La home autenticada será landing única tras login o habrá redirección según rol?
- ¿Qué acciones exactas debe contener la botonera inferior en la v1?
- ¿Quieres estándar de cliente HTTP tipado por dominio (`AuthClient`, `ActivitiesClient`) o uno genérico por contexto API?
