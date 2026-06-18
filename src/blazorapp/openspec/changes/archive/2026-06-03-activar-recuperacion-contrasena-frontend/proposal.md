## Why

La app ya ofrece autenticación, pero todavía no permite que un usuario recupere su contraseña cuando pierde el acceso. El backend ya ha definido el flujo y los endpoints necesarios, así que ahora toca activar la operativa en frontend para cerrar una necesidad básica de autoservicio y soporte.

## What Changes

- Añadir una pantalla pública de `He olvidado mi contraseña` accesible desde login para solicitar la recuperación por email.
- Conectar esa pantalla con `POST /api/auth/passrecovery`, mostrando siempre una respuesta neutra al usuario.
- Añadir una pantalla pública de `Restablecer contraseña` que lea `email` y `token` desde la URL del enlace recibido por correo.
- Conectar el formulario de restablecimiento con `POST /api/auth/reset-password`, permitiendo definir nueva contraseña y confirmación.
- Mostrar errores devueltos por backend sin traducir en la confirmación del reseteo y ofrecer reinicio del flujo cuando el token no sea válido o haya expirado.
- Redirigir al login tras un reseteo correcto y mostrar una confirmación visible dentro de la experiencia.
- Mantener el estilo visual actual de la app, sin exigir sesión iniciada ni persistir el token en almacenamiento local.

## Capabilities

### New Capabilities
- `frontend-password-recovery`: Define el flujo público de solicitud y confirmación de recuperación de contraseña desde login hasta el restablecimiento final.

### Modified Capabilities

## Impact

- Afecta a `IndaloaventurApp.SharedUI/Components/Login` para activar el acceso real desde la pantalla de inicio de sesión.
- Afecta a `IndaloaventurApp.Web.Client/Pages` para incorporar páginas públicas nuevas del flujo de recuperación.
- Requiere nuevas abstracciones o ampliaciones en los servicios de autenticación HTTP del frontend para consumir `POST /api/auth/passrecovery` y `POST /api/auth/reset-password`.
- Afecta a recursos localizados ES, validaciones de formularios y estilos SCSS globales de la experiencia de autenticación.
- Requiere tests de frontend para navegación, formularios, manejo de token y estados de éxito/error.
