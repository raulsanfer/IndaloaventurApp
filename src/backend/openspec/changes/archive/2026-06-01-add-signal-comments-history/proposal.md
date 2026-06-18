## Why

Las `Signal` permiten publicar y editar incidencias, pero hoy no existe un mecanismo para que el resto de usuarios aporte contexto adicional o actualizaciones sobre una incidencia ya publicada. Incorporar comentarios simples ahora permite mantener un historico colaborativo de cada `Signal` sin introducir la complejidad de conversaciones anidadas ni adjuntos.

## What Changes

- Anadir la capacidad de registrar comentarios asociados a una `Signal` existente por parte de cualquier usuario autenticado con acceso de lectura a signals.
- Definir cada comentario con `Id`, `SignalId`, `UserId`, `FechaComentario` y `Texto`, sin soporte para respuestas anidadas ni fotos.
- Exponer un flujo de lectura del historial de comentarios de una `Signal`, ordenado cronologicamente para facilitar el seguimiento de la incidencia.
- Exponer un flujo de alta de comentarios con validaciones de existencia de la `Signal`, texto obligatorio y auditoria de autor/fecha.
- Adaptar persistencia, dominio, CQRS y API para almacenar y recuperar el historial de comentarios de forma consistente.
- Anadir pruebas unitarias e integracion para alta y consulta de comentarios, incluyendo casos de autorizacion y validacion.

## Capabilities

### New Capabilities
- `signal-comments-history`: Gestion de comentarios simples asociados a una `Signal`, incluyendo alta y consulta del historial cronologico.

### Modified Capabilities
- Ninguna.

## Impact

- API: nuevos endpoints o acciones en `SignalsController` para crear y consultar comentarios por `Signal`.
- Application: nuevos comandos/queries, DTOs, validadores y handlers de comentarios de `Signal`.
- Domain: modelado de comentario de `Signal` y reglas de asociacion a una incidencia existente.
- Infrastructure: configuracion EF Core, repositorios y migracion SQL para persistir comentarios relacionados con `Signals`.
- Tests: suites unitarias e integracion de `TrailSignals` para cubrir historial, autorizacion y validaciones.
