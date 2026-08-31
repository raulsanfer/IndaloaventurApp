# Feature: socios y perfil

## Proposito

Gestiona datos de cuenta, ficha de socio propia y ficha de socio administrada desde settings.

## Componentes y paginas

- `IndaloaventurApp.Web.Client/Pages/MyAccountPage.razor`: ruta `/mi-cuenta`.
- `IndaloaventurApp.SharedUI/Components/MyAccount/MyAccountView.razor`: vista principal de cuenta.
- `IndaloaventurApp.Web.Client/Pages/MemberProfilePage.razor`: ruta `/mi-cuenta/ficha-socio`.
- `IndaloaventurApp.SharedUI/Components/MyAccount/MemberSelfProfileView.razor`: ficha propia.
- `IndaloaventurApp.Web.Client/Pages/AdminMemberProfilePage.razor`: ruta `/configuracion/usuarios/{UserId:guid}/ficha`.
- `IndaloaventurApp.SharedUI/Components/Settings/AdminMemberProfileView.razor`: ficha admin.

## Servicios y contratos

- `IMemberProfileService`
- `IAdminUserManagementService`
- `MemberProfileApiClient`
- `AdminUserManagementApiClient`
- `MemberProfile`, `MemberSelfProfile`, `MemberSelfProfileFormModel`, `UpdateMemberSelfProfileRequest`
- `ManagedUserItem`

## Estados

- usuario con o sin ficha de socio;
- edicion de datos propios;
- edicion admin de ficha;
- cargo asignado, cargo nulo o seleccion pendiente;
- errores de validacion o persistencia.

## Tests relacionados

- `IndaloaventurApp.Frontend.Tests/Features/Profile/MyAccountPageTests.cs`
- `IndaloaventurApp.Frontend.Tests/Features/Profile/MemberSelfProfileViewTests.cs`
- `IndaloaventurApp.Frontend.Tests/Features/Profile/MemberProfileApiClientTests.cs`
- `IndaloaventurApp.Frontend.Tests/Features/Admin/AdminMemberProfileViewTests.cs`
- `IndaloaventurApp.Frontend.Tests/Features/Admin/AdminUserManagementApiClientTests.cs`

Ver tambien [Inyeccion de dependencias](../architecture/dependency-injection.md).
