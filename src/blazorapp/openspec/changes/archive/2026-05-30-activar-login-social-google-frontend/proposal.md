## Why

El backend ya expone autenticacion social con Google mediante `POST /api/auth/social-login`, pero el frontend todavia no ofrece un flujo operativo para obtener el `id_token` de Google, enviarlo al API y convertir la respuesta en una sesion autenticada de la app. Activarlo ahora permite aprovechar una capacidad ya disponible en backend y cerrar el recorrido real del boton social presente en `LoginView`.

## What Changes

- Se activa el boton social de Google en `LoginView` para iniciar un flujo real de autenticacion con Google desde el frontend.
- Se define que el frontend MUST obtener un `id_token` de Google compatible con la audiencia configurada en backend y enviarlo a `/api/auth/social-login`.
- Se amplian los contratos de autenticacion del frontend para soportar login social ademas del login por credenciales.
- Se define el manejo de estados de carga y error del login social dentro de la experiencia actual de `LoginView`, sin alterar el layout principal ni el resto de proveedores sociales.
- Se deja el boton de Facebook fuera de alcance funcional en esta iteracion, manteniendolo sin comportamiento o con estado no disponible segun se implemente.

## Capabilities

### New Capabilities
- `frontend-google-social-login`: Define el flujo de autenticacion social con Google en el frontend, desde la obtencion del `id_token` hasta la creacion de la sesion autenticada local.

### Modified Capabilities
- Ninguna.

## Impact

- Afecta a `IndaloaventurApp.SharedUI` en `LoginView`, sus eventos de UI y los mensajes mostrados durante el login social.
- Afecta a `IndaloaventurApp.Web.Client` en el contrato `IAuthService` y su implementacion HTTP para consumir `/api/auth/social-login`.
- Afecta a la integracion cliente con Google Identity Services, previsiblemente mediante JS interop y configuracion frontend del `Client ID`.
- Depende del contrato ya existente en backend para `provider = "google"` y `token = <id_token>`, asi como de la configuracion backend de `SocialAuth:GoogleAudience`.
