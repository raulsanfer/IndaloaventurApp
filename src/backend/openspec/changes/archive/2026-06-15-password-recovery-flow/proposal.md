## Why

Actualmente un usuario que olvida su contrasena no tiene ninguna via segura de recuperacion sin intervencion manual de administracion. Necesitamos habilitar un flujo completo de recuperacion basado en email y token seguro para reducir friccion de acceso y alinearnos con las capacidades nativas de ASP.NET Identity.

## What Changes

- Exponer un endpoint publico de inicio de recuperacion de contrasena que reciba el email del usuario.
- Generar un token seguro de reseteo usando ASP.NET Identity y enviar un email con plantilla amigable y un enlace hacia una pagina publica del frontend.
- Exponer un segundo endpoint publico para confirmar la nueva contrasena usando email, token de recuperacion y nueva contrasena.
- Endurecer la seguridad del flujo evitando enumeracion de usuarios y aplicando expiracion/validacion del token segun Identity.
- Documentar en `docs/` el comportamiento esperado del frontend para la pagina publica de reseteo y su integracion con el backend.

## Capabilities

### New Capabilities
- `password-recovery-management`: flujo de recuperacion y reseteo de contrasena por email con token seguro y documentacion de integracion para frontend.

### Modified Capabilities

## Impact

- API: nuevos endpoints publicos para solicitar recuperacion y confirmar nueva contrasena.
- Application/Infrastructure: generacion de token de reseteo con Identity, envio de correo, configuracion de URL del frontend y plantilla de email.
- Seguridad: respuestas neutrales, token seguro de un solo uso y validaciones de contrasena alineadas con Identity.
- Documentacion: nuevo markdown en `docs/` con instrucciones para implementar la pagina publica de reseteo en frontend.
