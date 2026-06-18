## Context

El backend ya soporta autenticacion social con Google mediante `POST /api/auth/social-login`, esperando un payload con `provider = "google"` y `token = <id_token>`, y devolviendo el mismo contrato de sesion que el login por credenciales. En el frontend ya existe `LoginView`, un flujo de sesion local basado en `IAuthService` y dos botones sociales visuales, pero ninguno obtiene tokens de proveedor ni consume el endpoint social.

La implementacion debe respetar la arquitectura del proyecto: componentes Razor reutilizables en `SharedUI`, logica C# en partial class, estilos globales SCSS/CSS y acceso a API mediante servicios desacoplados. Tambien introduce una dependencia externa de cliente con Google Identity Services, por lo que conviene dejar las decisiones tecnicas fijadas antes de implementar.

## Goals / Non-Goals

**Goals:**
- Activar el boton de Google en `LoginView` con un flujo funcional de autenticacion social.
- Obtener un `id_token` de Google desde el cliente y enviarlo a `/api/auth/social-login`.
- Extender el contrato frontend de autenticacion para soportar login social sin romper el login actual por usuario y password.
- Reutilizar el pipeline actual de `AuthSession`, `ISessionService` y `OnLoginSucceeded`.
- Definir el comportamiento de carga, error y fallback visible para el usuario durante el login social.

**Non-Goals:**
- Implementar autenticacion social con Facebook en esta iteracion.
- Cambiar el contrato backend de `social-login` o la validacion de Google en API.
- Rediseñar el layout de `LoginView` mas alla de conectar el boton de Google.
- Implementar alta de cuenta, linking de varios proveedores o desvinculacion de identidades.

## Decisions

1. Integracion cliente con Google Identity Services mediante JS interop
- Decision: usar Google Identity Services en el frontend para solicitar un `id_token` y devolverlo a Blazor mediante JS interop.
- Rationale: el backend actual valida un `id_token` de Google y no un authorization code ni un access token; GIS es la forma mas directa y alineada con ese contrato.
- Alternatives considered:
  - Authorization Code + PKCE: mas robusto para un backend propietario, pero no encaja con el contrato ya existente de `token = <id_token>`.
  - Libreria .NET de terceros para OAuth en cliente: introduce mas abstraccion y dependencias innecesarias para un flujo que ya depende de JS del proveedor.

2. Nuevo metodo explicito de login social en `IAuthService`
- Decision: ampliar `IAuthService` con una operacion especifica para login social y modelar el request con proveedor y token.
- Rationale: evita sobrecargar `LoginAsync` con caminos condicionales, mantiene el contrato claro y facilita tests unitarios del componente.
- Alternatives considered:
  - Reutilizar `LoginAsync` con un request mixto: rechazado por mezclar credenciales y proveedores en una misma operacion.

3. Configuracion frontend separada para el Google Client ID
- Decision: definir una configuracion frontend especifica para el `Client ID` de Google, desacoplada de la audiencia backend aunque ambos valores deban corresponderse.
- Rationale: el cliente necesita conocer su propio `Client ID` para inicializar GIS; el frontend no debe depender de leer configuracion interna del API.
- Alternatives considered:
  - Hardcodear el Client ID en JS o markup: rechazado por mala trazabilidad y despliegue.
  - Intentar derivarlo desde el backend: rechazado para no acoplar el arranque del login a un endpoint adicional.

4. Reutilizacion del pipeline actual de sesion tras login social exitoso
- Decision: tras una respuesta exitosa de `/api/auth/social-login`, el componente debe seguir exactamente el mismo flujo que el login clasico: construir `AuthSession`, llamar a `SessionService.SetSession(...)` y disparar `OnLoginSucceeded`.
- Rationale: reduce riesgo de divergencia funcional y asegura coherencia con autorizacion y navegacion ya existentes.
- Alternatives considered:
  - Flujo de sesion separado para social login: rechazado por duplicacion.

5. Facebook permanece visible pero no operativo en esta propuesta
- Decision: el boton de Facebook se mantiene sin integracion funcional y con comportamiento explicitamente no disponible o inert, segun la implementacion UX final.
- Rationale: el backend no ofrece soporte real para Facebook, y simular paridad en frontend induciria a error.
- Alternatives considered:
  - Ocultarlo: posible, pero cambia innecesariamente el layout actual.

## Risks / Trade-offs

- [Dependencia externa de GIS y carga de script] → Mitigacion: cargar el script de forma controlada, detectar fallo de inicializacion y mostrar error de disponibilidad sin bloquear el login clasico.
- [Desajuste entre el Google Client ID del frontend y `SocialAuth:GoogleAudience` del backend] → Mitigacion: documentar que ambos deben corresponderse y validar el flujo manualmente en entorno.
- [Complejidad de JS interop en render interactivo Blazor] → Mitigacion: encapsular la integracion JS en una superficie pequena y bien definida, disparada solo desde el boton de Google.
- [Experiencia inconsistente si el popup se cancela] → Mitigacion: tratar cancelacion o cierre como operacion abortada sin mostrar error generico agresivo.
- [Confusion del usuario con Facebook visible] → Mitigacion: dejar claro que solo Google esta activo en esta iteracion.

## Migration Plan

- Añadir configuracion frontend para Google Client ID y el recurso JS necesario para GIS.
- Extender la capa de autenticacion frontend con login social y conectarla a `LoginView`.
- Validar manualmente que el `id_token` obtenido por el cliente es aceptado por el backend configurado.
- Rollback simple: retirar la integracion JS y el metodo de login social, manteniendo intacto el login clasico.

## Open Questions

- ¿El boton de Facebook debe quedar deshabilitado visualmente o simplemente sin accion mientras no exista soporte backend?
- ¿La configuracion del Google Client ID se almacenara en `appsettings` del frontend, secrets o variables de entorno segun entorno?
