## Why

Actualmente un socio puede crear `Signals`, pero no puede corregir después errores de título, descripción o estado en una señal propia. Eso obliga a depender de terceros para ajustes básicos y deja visible información desactualizada en una funcionalidad ya abierta a usuarios autenticados.

## What Changes

- Añadir en el detalle de una signal un botón `Editar` visible únicamente cuando la signal pertenezca al usuario autenticado.
- Incorporar un flujo de edición limitada para signals propias que permita cambiar solo `Título`, `Descripción` y `Estado`.
- Mantener como no editables desde este flujo la categoría, coordenadas, fotos, tags, comentarios y cualquier otro campo fuera del alcance pedido.
- Impedir la edición de signals ajenas tanto a nivel de experiencia frontend como de contrato/autorización del servicio que ejecute el guardado.

## Capabilities

### New Capabilities
- `frontend-signal-own-edit-flow`: edición limitada de una signal propia desde la acción `Editar` del detalle.

### Modified Capabilities
- `frontend-signal-detail-page`: el detalle cambia para exponer una entrada de edición solo al propietario autenticado de la signal.

## Impact

- `IndaloaventurApp.SharedUI`: detalle de signal, nuevos estados o componentes de edición y modelos frontend asociados al formulario limitado.
- `IndaloaventurApp.Web.Client`: `SignalApiClient`, resolución de ownership en sesión/detalle y operación de guardado de cambios.
- `IndaloaventurApp.SharedUI.Models.Auth`: posible ampliación de la sesión frontend para conocer el identificador del usuario autenticado.
- Tests frontend de detalle y edición de signals.
- Dependencia sobre el API adyacente para garantizar autorización por propietario y un contrato de actualización compatible con edición limitada.
