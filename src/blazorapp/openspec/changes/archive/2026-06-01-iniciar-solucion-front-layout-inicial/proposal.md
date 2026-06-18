## Why

La aplicación necesita una base de frontend coherente y mantenible para arrancar el desarrollo funcional sobre una experiencia de acceso clara en web y PWA. Empezar por el layout de login y la home autenticada permite establecer desde el inicio patrones reutilizables en SharedUI, sistema visual y estructura de estilos SCSS alineados con la arquitectura objetivo.

## What Changes

- Se define una primera capacidad de frontend para la pantalla de acceso con layout responsive (móvil y escritorio) basada en la referencia de `openspec/design/login`.
- Se incorpora una capacidad para la pantalla Home tras autenticación basada en `openspec/design/home`, incluyendo cabecera de app y menú inferior/botonera persistente.
- Se establece el contrato de diseño inicial: tipografía Outfit, paleta de color principal y jerarquía visual consistente entre login y home.
- Se especifica la estructura de componentes reutilizables para login y home (contenedores, cabeceras, contenido principal y navegación inferior).
- Se fija que los estilos se implementen exclusivamente con SCSS global y modular (sin estilos inline ni estilos en componente).
- Se incorpora la base para textos localizables en español mediante claves de recursos para todos los literales visibles.
- Se define una base de arquitectura de servicios frontend con interfaces de aplicación, implementación desacoplada de transporte y cliente HTTP tipado mediante factoría `HttpClient`.
- Se establecen reglas de código limpio para capa UI y servicios (SRP, nombres semánticos, separación de responsabilidades, manejo uniforme de errores y testabilidad).

## Capabilities

### New Capabilities
- `frontend-login-layout-foundation`: Define el layout inicial de login, su estructura visual, responsividad y reglas de reutilización en SharedUI para iniciar la solución frontend.
- `frontend-authenticated-home-layout`: Define el layout de Home tras validación de usuario, incluyendo cabecera de aplicación y navegación inferior persistente.
- `frontend-service-foundation`: Define patrones base para creación de servicios frontend, factoría `HttpClient` y convenciones de código limpio para el consumo futuro del API.

### Modified Capabilities
- Ninguna.

## Impact

- Afecta a `IndaloaventurApp.Web` (composición de pantallas login/home y registro DI).
- Afecta a `IndaloaventurApp.SharedUI` (componentes Razor reutilizables de acceso, cabecera autenticada y menú inferior).
- Afecta al sistema de estilos global SCSS (nuevos parciales y registro en `style.scss`).
- Afecta al sistema de localización (nuevas claves de recursos en español para login y home).
- Afecta a la configuración de clientes HTTP y servicios de aplicación para consumo de API en fases siguientes.
- No introduce cambios en contratos de API; prepara la capa visual y de servicios que consumirá endpoints ya definidos en `openspec/endpoints.json` en fases posteriores.
