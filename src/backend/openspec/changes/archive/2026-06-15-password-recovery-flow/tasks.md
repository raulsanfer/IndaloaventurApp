## 1. Flujo de aplicacion y seguridad

- [x] 1.1 Definir comandos, validadores y contratos API para solicitar recuperacion y confirmar nueva contrasena.
- [x] 1.2 Integrar el flujo nativo de ASP.NET Identity para generar y validar tokens de reseteo con respuestas neutras y mensajes en espanol.

## 2. Correo y configuracion

- [x] 2.1 Introducir la abstraccion de envio de correo y la configuracion necesaria para la URL publica del frontend y el proveedor de email.
- [x] 2.2 Implementar la plantilla amigable de correo de recuperacion con enlace seguro hacia frontend.

## 3. API y documentacion

- [x] 3.1 Exponer el endpoint publico `passrecovery` o equivalente para iniciar la recuperacion y el endpoint publico final para confirmar la nueva contrasena.
- [x] 3.2 Crear en `docs/` el markdown con instrucciones de integracion para frontend sobre la pagina publica de reseteo.

## 4. Verificacion

- [x] 4.1 Crear pruebas automatizadas para solicitud neutra, envio de correo, reseteo correcto, token invalido y validaciones de contrasena.
- [x] 4.2 Ejecutar `dotnet test` y dejar verificado el flujo antes de marcar las tareas como completadas.
