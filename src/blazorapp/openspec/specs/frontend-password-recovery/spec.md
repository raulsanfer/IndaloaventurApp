# frontend-password-recovery Specification

## Purpose
TBD - created by archiving change activar-recuperacion-contrasena-frontend. Update Purpose after archive.
## Requirements
### Requirement: Acceso público al inicio de recuperación de contraseña
El sistema MUST ofrecer desde la pantalla de login un acceso operativo a una página pública de `He olvidado mi contraseña` para iniciar la recuperación mediante email.

#### Scenario: Usuario abre el flujo desde login
- **WHEN** un usuario pulsa `He olvidado mi contraseña` desde la pantalla de login
- **THEN** el sistema MUST navegar a una pantalla pública de solicitud de recuperación

#### Scenario: Pantalla accesible sin sesión iniciada
- **WHEN** un usuario no autenticado abre directamente la URL de recuperación
- **THEN** el sistema MUST permitir el acceso sin exigir autenticación previa

### Requirement: Solicitud neutra de recuperación por email
La pantalla de recuperación MUST enviar el email informado a `POST /api/auth/passrecovery` y MUST mostrar siempre una respuesta neutra tras el envío.

#### Scenario: Solicitud enviada
- **WHEN** el usuario informa su email y confirma la recuperación
- **THEN** el sistema MUST invocar `POST /api/auth/passrecovery` con el email introducido
- **THEN** el sistema MUST mostrar un mensaje neutro indicando que, si existe una cuenta asociada, se han enviado instrucciones

#### Scenario: Reapertura de la pantalla tras solicitar
- **WHEN** el usuario vuelve a la pantalla de recuperación después de una solicitud previa
- **THEN** el sistema MUST permitir iniciar una nueva solicitud sin depender de estado persistido del navegador

### Requirement: Página pública de restablecimiento con token del enlace
El sistema MUST ofrecer una página pública de restablecimiento que lea `email` y `token` desde la URL y los use como entrada del formulario de cambio de contraseña.

#### Scenario: Usuario abre el enlace del correo
- **WHEN** el usuario accede a la URL pública de restablecimiento con `email` y `token` en query string
- **THEN** el sistema MUST cargar la pantalla de restablecimiento sin exigir autenticación previa
- **THEN** el sistema MUST conservar `email` y `token` para el envío al backend sin persistirlos en almacenamiento local

#### Scenario: Datos incompletos en la URL
- **WHEN** la página se abre sin `email` o sin `token`
- **THEN** el sistema MUST resolver un estado no operativo o guiado que permita reiniciar el flujo de recuperación

### Requirement: Confirmación de nueva contraseña
La pantalla de restablecimiento MUST permitir informar `Nueva contraseña` y `Confirmar nueva contraseña`, validar su presencia y enviar `email`, `token`, `newPassword` y `confirmPassword` a `POST /api/auth/reset-password`.

#### Scenario: Reseteo correcto
- **WHEN** el usuario informa una nueva contraseña válida y confirma el formulario
- **THEN** el sistema MUST invocar `POST /api/auth/reset-password` con `email`, `token`, `newPassword` y `confirmPassword`
- **THEN** el sistema MUST redirigir al login mostrando una confirmación de cambio completado

#### Scenario: Confirmación no coincide
- **WHEN** el usuario introduce valores distintos en `Nueva contraseña` y `Confirmar nueva contraseña`
- **THEN** el sistema MUST impedir el envío y mostrar una validación clara de discrepancia

### Requirement: Tratamiento de errores del reset y reinicio del flujo
La pantalla de restablecimiento MUST mostrar los mensajes de error devueltos por backend sin traducir y MUST ofrecer una vía clara para volver a iniciar la recuperación cuando el token no sea válido o haya expirado.

#### Scenario: Token inválido o expirado
- **WHEN** el backend rechaza `POST /api/auth/reset-password` por token inválido o expirado
- **THEN** el sistema MUST mostrar el mensaje devuelto por backend
- **THEN** el sistema MUST ofrecer un acceso visible para volver a `He olvidado mi contraseña`

#### Scenario: Política de contraseña no cumplida
- **WHEN** el backend rechaza el reset porque la nueva contraseña no cumple la política requerida
- **THEN** el sistema MUST mostrar el mensaje devuelto por backend sin traducirlo

