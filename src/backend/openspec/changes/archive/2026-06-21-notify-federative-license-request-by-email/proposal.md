## Why

Actualmente una solicitud nueva de licencia federativa solo queda registrada en el sistema, pero no dispara ningun aviso operativo al club. Necesitamos una notificacion automatica y sencilla por correo para que el club reciba de inmediato el email del solicitante y pueda iniciar su gestion sin depender de consultas manuales.

## What Changes

- Enviar un correo informativo cuando un usuario cree una nueva `SolicitudLicenciaFederativa`.
- Configurar la direccion destinataria del aviso en `appsettings`, con valor inicial `club@indaloaventura.com`.
- Mantener el mensaje del correo en castellano, con contenido simple orientado a operativa interna del club.
- Integrar el envio en el flujo de alta de solicitud propia sin cambiar el contrato publico del endpoint.

## Capabilities

### New Capabilities
Ninguna.

### Modified Capabilities
- `member-federative-license-api`: crear una solicitud propia de licencia pasa a requerir el envio de un correo informativo al club con el email del solicitante.

## Impact

- Application: ampliacion del caso de uso de creacion de solicitudes para componer y enviar la notificacion.
- Infrastructure: reutilizacion del servicio SMTP existente y alta de una nueva opcion de configuracion para el destinatario del aviso.
- API/Config: actualizacion de `appsettings` para incluir el correo operativo por defecto `club@indaloaventura.com`.
