## Why

La aplicación está planteada en `project.md` como una `Blazor Web App + InteractiveWebAssembly + PWA`, pero hoy no cumple todas las condiciones para que el navegador la ofrezca como instalable. Necesitamos cerrar esa parte de arquitectura ahora para que la experiencia web móvil se pueda instalar realmente en dispositivo.

## What Changes

- Completar la configuración PWA del frontend web con `manifest`, `service worker` y registro en cliente.
- Incorporar iconos de aplicación reales derivados del logo ubicado en `IndaloaventurApp.SharedUI/wwwroot/assets/images/logo.png`.
- Definir el comportamiento esperado para que la app sea elegible para instalación desde navegadores compatibles en HTTPS.
- Añadir validaciones manuales y técnicas mínimas para comprobar instalación y persistencia del modo standalone.

## Capabilities

### New Capabilities
- `frontend-pwa-installability`: Define los requisitos del frontend para que la aplicación pueda ser reconocida e instalada como PWA desde el navegador.

### Modified Capabilities

## Impact

- `IndaloaventurApp.Web/IndaloaventurApp.Web/Components/App.razor`
- `IndaloaventurApp.Web/IndaloaventurApp.Web/wwwroot/manifest.webmanifest`
- Nuevos assets e iconos PWA en `IndaloaventurApp.Web/IndaloaventurApp.Web/wwwroot/`
- Nuevo `service worker` y su registro en cliente
- Validación manual en navegador móvil/escritorio sobre HTTPS
