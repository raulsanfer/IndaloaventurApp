# Feature: PWA y navegacion

## Proposito

La aplicacion puede ejecutarse desde navegador y comportarse como PWA instalable. La navegacion organiza las areas principales para usuarios autenticados.

## Componentes y paginas

- `IndaloaventurApp.Web.Client/Routes.razor`: resolucion de rutas cliente.
- `IndaloaventurApp.SharedUI/Components/Shell/AuthenticatedShell.razor`: estructura de layout autenticado.
- `IndaloaventurApp.SharedUI/Components/Shell/AppHeader.razor`: cabecera.
- `IndaloaventurApp.SharedUI/Components/Shell/BottomNav.razor`: navegacion inferior.
- `IndaloaventurApp.Web.Client/Pages/HomePage.razor`: ruta `/home`.
- `IndaloaventurApp.SharedUI/Components/Home/HomeDashboard.razor`: dashboard de inicio.
- `IndaloaventurApp.Web/IndaloaventurApp.Web/wwwroot/manifest.webmanifest`: manifiesto PWA cuando este presente en el host.

## Servicios y contratos

- `ISessionService` decide visibilidad y acceso segun sesion.
- `IStringLocalizer<SharedTexts>` proporciona textos de navegacion.
- `NavigationManager` se usa en paginas y componentes que redirigen.

## Estados

- sesion inicial pendiente;
- usuario autenticado;
- usuario no autenticado redirigido al login;
- navegacion inferior con items principales;
- instalabilidad PWA validada por manifest y assets.

## Tests relacionados

- `IndaloaventurApp.Frontend.Tests/Features/Navigation/RoutesTests.cs`
- `IndaloaventurApp.Frontend.Tests/Features/Navigation/BottomNavTests.cs`
- `IndaloaventurApp.Frontend.Tests/Features/Navigation/HomeDashboardTests.cs`
- `IndaloaventurApp.Frontend.Tests/Features/Pwa/PwaManifestTests.cs`

Ver tambien [Testing](../architecture/testing-strategy.md).
