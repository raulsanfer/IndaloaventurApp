## Why

La aplicación ya depende de `IsMember` para decidir qué mostrar en `Mi Cuenta`, pero hoy esa señal no está formalizada como parte de la identidad autenticada del usuario en este frontend. Eso genera dos problemas: por un lado, backend y frontend no tienen un contrato explícito y verificable sobre cómo se propaga `IsMember`; por otro, cuando un usuario no socio entra en `MyAccount`, el flujo actual sigue intentando cargar la ficha de socio y puede terminar en error.

## What Changes

- Se define que backend MUST emitir `IsMember` como claim del usuario autenticado y mantenerlo coherente con la propiedad `AspNetUser.IsMember`.
- Se define que el contrato de autenticación MUST exponer `IsMember` de forma consistente para que el frontend pueda poblar `AuthSession` durante la validación.
- Se documenta un handoff técnico para el equipo backend con los cambios requeridos en generación de claims/token y contratos de respuesta.
- Se ajusta el frontend para mantener `IsMember` en `AuthSession` a partir de la identidad autenticada.
- Se redefine el comportamiento de `MyAccount` para que, cuando `IsMember = false`, no trate la ausencia de ficha de socio como error y renderice únicamente las secciones que no dependan de `Profile`.

## Capabilities

### New Capabilities
- `auth-ismember-claim-alignment`: Alinea la propagación de `IsMember` entre autenticación, sesión cliente y reglas de UI.
- `my-account-nonmember-graceful-render`: Permite renderizar `Mi Cuenta` sin error para usuarios autenticados no socios.

### Modified Capabilities
- `frontend-mi-cuenta-layout-alignment`: Se amplía para tolerar el caso autenticado/no socio sin depender de una ficha de socio disponible.

## Impact

- Afecta a backend de autenticación/autorización para incluir `IsMember` en claims y, si aplica, en el payload de login.
- Afecta a `IndaloaventurApp.Web.Client` en el flujo de inicio de sesión y carga de `AuthSession`.
- Afecta a `IndaloaventurApp.SharedUI` en el manejo de estado y render de `MyAccountView`.
- Requiere actualizar la documentación de contratos (`openspec/endpoints.json` o fuente equivalente) para reflejar `IsMember`.
