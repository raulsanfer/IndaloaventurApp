# Feature: licencias federativas

## Proposito

Permite al socio consultar y solicitar licencias federativas, y al administrador revisar solicitudes y actualizar su estado.

## Componentes y paginas

- `IndaloaventurApp.Web.Client/Pages/FederativeLicensesPage.razor`: ruta `/mi-club/licencias-federativas`.
- `IndaloaventurApp.SharedUI/Components/Club/FederativeLicensesView.razor`: flujo de socio.
- `IndaloaventurApp.Web.Client/Pages/SettingsFederativeLicensesPage.razor`: ruta `/configuracion/licencias-federativas`.
- `IndaloaventurApp.SharedUI/Components/Settings/AdminFederativeLicenseManagementView.razor`: gestion admin.

## Servicios y contratos

- `IFederativeLicenseService`
- `IAdminFederativeLicenseService`
- `FederativeLicenseApiClient`
- `AdminFederativeLicenseApiClient`
- `FederativeLicenseRequest`, `FederativeLicenseRate`, `CreateFederativeLicenseRequest`
- `AdminFederativeLicenseRequest`, `AdminFederativeLicenseQuery`, `UpdateAdminFederativeLicenseStatusRequest`

## Estados

- carga de solicitudes y tarifas;
- seleccion de temporada y media temporada;
- alta de solicitud;
- filtros admin por usuario, temporada y estado;
- forbidden para sesiones no admin;
- actualizacion de estado.

## Tests relacionados

- `IndaloaventurApp.Frontend.Tests/Features/Licenses/FederativeLicenseApiClientTests.cs`
- `IndaloaventurApp.Frontend.Tests/Features/Licenses/FederativeLicensesViewTests.cs`
- `IndaloaventurApp.Frontend.Tests/Features/Licenses/AdminFederativeLicenseApiClientTests.cs`
- `IndaloaventurApp.Frontend.Tests/Features/Licenses/AdminFederativeLicenseManagementViewTests.cs`

Ver tambien [Clientes HTTP](../architecture/http-api-clients.md).
