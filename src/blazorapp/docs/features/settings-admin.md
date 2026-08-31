# Feature: settings y admin

## Proposito

Agrupa pantallas administrativas para usuarios, fichas de socio, cargos, licencias federativas y categorias de signals.

## Componentes y paginas

- `IndaloaventurApp.Web.Client/Pages/SettingsPage.razor`: ruta `/configuracion`.
- `IndaloaventurApp.SharedUI/Components/Settings/SettingsView.razor`: hub de configuracion.
- `IndaloaventurApp.Web.Client/Pages/SettingsUsersPage.razor`: ruta `/configuracion/usuarios`.
- `IndaloaventurApp.SharedUI/Components/Settings/AdminUsersManagementView.razor`: listado y acciones de usuarios.
- `IndaloaventurApp.Web.Client/Pages/SettingsCargoPage.razor`: ruta `/configuracion/cargos`.
- `IndaloaventurApp.SharedUI/Components/Settings/CargoManagementView.razor`: gestion de cargos.
- `IndaloaventurApp.Web.Client/Pages/SettingsSignalsPage.razor`: ruta `/configuracion/signals`.
- `IndaloaventurApp.SharedUI/Components/Settings/SignalSettingsHubView.razor`: hub admin de signals.

## Servicios y contratos

- `IAdminUserManagementService`
- `ICargoAdminService`
- `IAdminFederativeLicenseService`
- `ISignalService`
- `AdminUserManagementApiClient`
- `CargoAdminApiClient`
- `AdminFederativeLicenseApiClient`
- `ManagedUserItem`, `CargoItem`, `CreateCargoRequest`, `UpdateCargoRequest`

## Estados

- acceso permitido solo para sesiones admin;
- filtros de usuarios;
- activacion y desactivacion de usuarios;
- creacion, edicion y borrado de cargos;
- gestion de categorias de signals;
- errores de permisos, validacion o conectividad.

## Tests relacionados

- `IndaloaventurApp.Frontend.Tests/Features/Admin/AdminUsersManagementViewTests.cs`
- `IndaloaventurApp.Frontend.Tests/Features/Admin/AdminUserManagementApiClientTests.cs`
- `IndaloaventurApp.Frontend.Tests/Features/Admin/CargoManagementViewTests.cs`
- `IndaloaventurApp.Frontend.Tests/Features/Admin/CargoAdminApiClientTests.cs`
- `IndaloaventurApp.Frontend.Tests/Features/Settings/SettingsViewTests.cs`

Ver tambien [Autenticacion y sesion](../architecture/authentication-session.md).
