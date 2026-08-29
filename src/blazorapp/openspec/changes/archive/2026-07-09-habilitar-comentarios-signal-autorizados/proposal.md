## Why

La pagina de detalle de `Signal` ya expone un bloque de `Comentarios`, pero hoy no ofrece una forma guiada de publicar nuevos comentarios desde la interfaz. Hace falta cerrar ese flujo para que perfiles autorizados puedan anadir contexto operativo sobre una signal sin salir del detalle ni depender de operaciones externas.

## What Changes

- Habilitar una accion `Anadir comentario` dentro de la zona de `Comentarios` del detalle de signal.
- Restringir esa accion y el envio del alta a usuarios autenticados con rol `Admin` o con rol `Member` cuando el claim `IsMember = true`.
- Incorporar un popup modal con un campo de texto largo, accion de confirmacion, validacion basica y estados de envio.
- Asociar el comentario creado a la signal abierta y refrescar el bloque de comentarios tras un alta correcta.
- Mantener fuera de alcance la edicion, eliminacion o moderacion de comentarios existentes.

## Capabilities

### New Capabilities
- `frontend-signal-comment-create-flow`: flujo de creacion de comentarios de signal con modal, validacion, guardado y refresco del detalle.

### Modified Capabilities
- `frontend-signal-detail-page`: el detalle cambia para mostrar la accion de alta de comentarios solo a perfiles autorizados dentro del bloque de comentarios.

## Impact

- `IndaloaventurApp.SharedUI`: `SignalDetailView`, componentes auxiliares del modal, estados visuales del bloque de comentarios y modelos de formulario.
- `IndaloaventurApp.Web.Client`: servicios frontend y cliente API de `signals` o comentarios para soportar el alta asociada a una signal.
- Capa de sesion/autenticacion frontend para evaluar roles y claim `IsMember` al decidir la visibilidad y habilitacion del flujo.
- Tests frontend del detalle de signal y del nuevo flujo de alta de comentarios.
- Dependencia del contrato del API adyacente para persistir el comentario y devolver o permitir recargar la coleccion actualizada.
