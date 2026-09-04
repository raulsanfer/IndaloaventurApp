# Feature: autenticacion

## Proposito

Permite iniciar sesion, iniciar sesion social, recuperar password, resetear password y mantener la sesion entre recargas cuando corresponde.

## Componentes y paginas

- `IndaloaventurApp.Web.Client/Pages/LoginPage.razor`: ruta `/`.
- `IndaloaventurApp.SharedUI/Components/Login/LoginView.razor`: formulario principal.
- `IndaloaventurApp.Web.Client/Pages/ForgotPasswordPage.razor`: ruta `/forgot-password`.
- `IndaloaventurApp.SharedUI/Components/Login/PasswordRecoveryRequestView.razor`: solicitud de recuperacion.
- `IndaloaventurApp.Web.Client/Pages/ResetPasswordPage.razor`: ruta `/reset-password`.
- `IndaloaventurApp.SharedUI/Components/Login/ResetPasswordView.razor`: reset de password.

## Servicios y contratos

- `IAuthService`
- `ISessionService`
- `AuthApiClient`
- `SessionService`
- `AuthSession`, `LoginRequest`, `SocialLoginRequest`, `PasswordRecoveryRequest`, `ResetPasswordRequest`

## Estados

- carga durante llamada HTTP;
- error de credenciales o token social;
- error de recuperacion/reset devuelto por backend;
- exito con sesion almacenada;
- redireccion a home tras login correcto.

## Tests relacionados

- `IndaloaventurApp.Frontend.Tests/Features/Auth/AuthApiClientTests.cs`
- `IndaloaventurApp.Frontend.Tests/Features/Auth/LoginViewTests.cs`
- `IndaloaventurApp.Frontend.Tests/Features/Auth/PasswordRecoveryViewTests.cs`
- `IndaloaventurApp.Frontend.Tests/Features/Auth/SessionServiceTests.cs`
- `IndaloaventurApp.Frontend.Tests/Features/Auth/SessionInitializationGateTests.cs`

Ver tambien [Autenticacion y sesion](../architecture/authentication-session.md).
