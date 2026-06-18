## Why

La app web/PWA pierde la sesi�n al hacer una recarga completa del navegador, incluyendo `F5` en escritorio o el gesto de refresco en m�vil, porque el estado autenticado solo vive en memoria dentro de `SessionService`. Eso rompe una expectativa b�sica de uso en una PWA y expulsa al usuario al login aunque su token siga siendo v�lido.

## What Changes

- Persistir la sesi�n autenticada del frontend en almacenamiento del navegador para poder rehidratarla tras una recarga completa de la app.
- Restaurar autom�ticamente la sesi�n al arrancar la aplicaci�n si existe una sesi�n persistida y su token todav�a no ha expirado.
- Evitar redirecciones prematuras al login mientras el cliente est� resolviendo si puede rehidratar una sesi�n persistida.
- Limpiar la sesi�n persistida cuando el usuario cierre sesi�n o cuando el token restaurado ya no sea v�lido por expiraci�n.
- Mantener fuera de alcance la renovaci�n silenciosa de credenciales mientras el backend solo exponga `accessToken` sin `refresh token`.

## Capabilities

### New Capabilities
- `frontend-session-persistence`: Persistencia y restauraci�n de sesi�n autenticada en la app web/PWA despu�s de recargas completas del navegador.

### Modified Capabilities

## Impact

- `IndaloaventurApp.SharedUI`: evoluci�n de `ISessionService` y de los flujos de login para soportar sesi�n persistida y estados de inicializaci�n.
- `IndaloaventurApp.Web.Client`: `SessionService`, arranque de la app y p�ginas protegidas para rehidratar sesi�n antes de decidir la navegaci�n.
- Navegaci�n protegida de la PWA/web: cambio de comportamiento al refrescar manualmente o mediante gesto en m�vil.
- Tests frontend de sesi�n y navegaci�n para cubrir restauraci�n, expiraci�n y cierre de sesi�n.
