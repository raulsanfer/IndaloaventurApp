# PWA y navegacion

La aplicacion se sirve como Blazor Web App con cliente WebAssembly y capacidad PWA. La navegacion combina rutas de `Web.Client/Pages`, shell compartido y restauracion de sesion.

## Rutas

Las paginas con `@page` viven en `IndaloaventurApp.Web/IndaloaventurApp.Web.Client/Pages`. Estas paginas deben ser delgadas: declaran rutas, reciben parametros y delegan la experiencia principal a componentes de `SharedUI`.

Ejemplos:

- `/home` usa `HomePage` y `HomeDashboard`.
- `/mi-club` usa `ClubPage` y `ClubIndexView`.
- `/signals/{SignalId:guid}` usa `SignalDetailPage` y `SignalDetailView`.
- `/configuracion` usa `SettingsPage` y `SettingsView`.

## Shell

Los elementos comunes estan en `IndaloaventurApp.SharedUI/Components/Shell`:

- `AuthenticatedShell`: layout para zonas autenticadas.
- `AppHeader`: cabecera de aplicacion.
- `BottomNav`: navegacion inferior.

## Sesion y rutas protegidas

Las paginas protegidas consultan `ISessionService`. Si no hay sesion utilizable, redirigen al login. `SessionInitializationGate` evita tomar esa decision antes de completar la restauracion de sesion persistida.

## PWA

La instalabilidad PWA se valida con tests en `IndaloaventurApp.Frontend.Tests/Features/Pwa/PwaManifestTests.cs`. Los assets estaticos y el manifiesto pertenecen al host web bajo `IndaloaventurApp.Web/IndaloaventurApp.Web/wwwroot`.

Ver tambien [Autenticacion y sesion](authentication-session.md) y [Feature: PWA y navegacion](../features/pwa-navigation.md).
