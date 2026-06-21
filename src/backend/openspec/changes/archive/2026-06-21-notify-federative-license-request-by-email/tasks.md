## 1. Configuracion y flujo de aplicacion

- [x] 1.1 Definir una nueva opcion de configuracion para el destinatario de avisos de solicitudes federativas y establecer `club@indaloaventura.com` como valor inicial en `appsettings`.
- [x] 1.2 Ampliar `CreateSolicitudLicenciaFederativaCommandHandler` para enviar el correo informativo reutilizando `IEmailSender` y el email del usuario solicitante.

## 2. Verificacion

- [x] 2.1 Actualizar o crear pruebas de aplicacion para validar que una solicitud creada dispara el correo al destinatario configurado con el contenido esperado.
- [x] 2.2 Ejecutar la bateria de pruebas relevante del flujo de licencias federativas y del envio de correo antes de cerrar el cambio.
