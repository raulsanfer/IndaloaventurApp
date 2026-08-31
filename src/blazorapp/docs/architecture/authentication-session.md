# Autenticacion y sesion

La autenticacion se modela con contratos compartidos y servicios concretos del cliente.

## Contratos

- `IAuthService` define login tradicional, login social, recuperacion y reseteo de password.
- `ISessionService` representa la sesion activa, persistencia, restauracion y cierre.
- `AuthSession` concentra token, tipo de token, expiracion, pertenencia a socio, roles y usuario.

## Implementaciones

- `AuthApiClient` llama a `/api/auth/login`, `/api/auth/social-login`, `/api/auth/passrecovery` y reseteo de password.
- `SessionService` mantiene sesion en memoria y usa almacenamiento del navegador para sobrevivir a recargas.
- `SessionInitializationGate` retrasa decisiones de navegacion protegida hasta que termina la restauracion de sesion.

## Paginas y componentes

- `LoginPage` aloja `LoginView`.
- `ForgotPasswordPage` aloja el flujo de solicitud de recuperacion.
- `ResetPasswordPage` aloja `ResetPasswordView`.
- Varias paginas protegidas validan `ISessionService` y redirigen si no hay sesion utilizable.

## Estados esperados

Los flujos de autenticacion deben distinguir:

- credenciales incorrectas;
- token social invalido;
- sesion ausente o caducada;
- errores HTTP o payload invalido;
- exito con persistencia de sesion.

Los tests relevantes viven en `IndaloaventurApp.Frontend.Tests/Features/Auth`.
