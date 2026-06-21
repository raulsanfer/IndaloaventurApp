## Context

El backend ya dispone de una abstraccion `IEmailSender` y de una implementacion SMTP usada en recuperacion de contrasena, por lo que no hace falta introducir una nueva dependencia externa. El nuevo cambio afecta al flujo de `CreateSolicitudLicenciaFederativa`, que hoy valida la solicitud, la persiste y devuelve la respuesta sin ningun aviso adicional al club.

## Goals / Non-Goals

**Goals:**
- Enviar un correo informativo en castellano cada vez que se cree una nueva solicitud de licencia federativa.
- Permitir configurar el destinatario del aviso en `appsettings`, con valor inicial `club@indaloaventura.com`.
- Reutilizar la infraestructura SMTP existente y mantener el contenido del mensaje simple, incluyendo al menos el email del usuario solicitante.
- Mantener intacto el contrato HTTP del endpoint de creacion.

**Non-Goals:**
- Crear plantillas HTML complejas o branding especifico para este correo.
- Enviar correos al propio usuario solicitante o a multiples destinatarios con reglas avanzadas.
- Introducir reintentos asincronos, colas o trazabilidad persistente de envios en esta iteracion.

## Decisions

### 1. Integrar el envio en el handler de creacion de solicitud

El correo se disparara desde `CreateSolicitudLicenciaFederativaCommandHandler` despues de persistir correctamente la solicitud. Asi se garantiza que solo se notifiquen altas reales y se evita enviar correos para operaciones que luego fallen antes de guardar.

Alternativas consideradas:
- Enviar antes de persistir. Rechazada porque puede generar avisos sobre solicitudes que finalmente no existan.
- Crear un proceso independiente o cola. Rechazada en esta iteracion por complejidad innecesaria para un aviso simple.

### 2. Añadir una opcion de destinatario especifica dentro de la configuracion de email

La propuesta usara una nueva propiedad de configuracion bajo el bloque `Email`, orientada especificamente a notificaciones de solicitudes federativas. El valor inicial en configuracion sera `club@indaloaventura.com`.

Alternativas consideradas:
- Reutilizar `FromAddress` como destinatario. Rechazada porque mezcla remitente tecnico con buzon operativo.
- Crear una seccion de configuracion completamente nueva. Rechazada porque el cambio sigue perteneciendo al subsistema de correo existente.

### 3. Mantener un mensaje sencillo en texto y HTML minimo

El correo incluira un asunto claro y un cuerpo breve en castellano indicando que se ha recibido una solicitud de licencia federativa y el email del usuario que la ha realizado. Se puede reutilizar el patron actual de `EmailMessage` con cuerpo plano y HTML basico.

Alternativas consideradas:
- Correo solo en texto plano. Valida, pero se prefiere mantener el patron dual ya usado por el sistema.
- Incluir mas datos de solicitud como licencia o temporada. Se deja fuera para respetar el alcance pedido.

### 4. Tratar el envio como aviso no bloqueante

Si el correo falla despues de persistir la solicitud, la operacion de alta seguira siendo valida y el sistema registrara el incidente en logs. Esto evita responder error al cliente cuando la solicitud ya ha quedado creada correctamente y el fallo se limita al canal operativo de notificacion.

Alternativas consideradas:
- Propagar el error SMTP al cliente. Rechazada porque puede dejar solicitudes creadas con respuestas erroneamente fallidas.
- Deshacer la solicitud al fallar el correo. Rechazada porque no existe una transaccion fiable entre SQL Server y SMTP en este flujo.

## Risks / Trade-offs

- [Fallo SMTP tras persistir la solicitud] -> Mitigar definiendo con claridad si el fallo debe interrumpir el caso de uso o solo registrarse; este punto debe cerrarse en implementacion.
- [Configuracion incompleta del destinatario] -> Mitigar validando la opcion al enviar y dejando un valor inicial explicito en `appsettings`.
- [Acoplamiento del handler a infraestructura de correo] -> Mitigar apoyandose en la abstraccion `IEmailSender` y en opciones tipadas, sin dependencias directas a SMTP.
- [Correo no enviado por fallo puntual de SMTP] -> Mitigar registrando la incidencia y preservando la solicitud ya creada para gestion posterior.

## Migration Plan

- Añadir la nueva clave de configuracion del destinatario en `appsettings.json` y, si procede, en `appsettings.Development.json`.
- Implementar el envio reutilizando `IEmailSender`.
- Verificar el flujo con pruebas de aplicacion/integracion y, en entornos reales, completar el valor del destinatario si se sobreescribe por secretos o variables.

## Open Questions

Ninguna por ahora.
