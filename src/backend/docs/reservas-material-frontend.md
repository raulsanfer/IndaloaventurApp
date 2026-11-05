# Reservas de material: guía de diseño de vistas

## Propósito

Este documento complementa [openapi.json](./openapi.json) y define cómo debe comportarse la interfaz de reservas de material para respetar las reglas de negocio implementadas. El contrato OpenAPI es la referencia para cuerpos, tipos y respuestas HTTP; esta guía concreta qué mostrar, qué acciones habilitar y qué validaciones anticipar en la experiencia de usuario.

La interfaz no debe sustituir al servidor: la disponibilidad, los permisos, la titularidad y los cambios de estado siempre se vuelven a validar en la API.

## Conceptos que deben conservarse en la interfaz

- Una reserva corresponde a **un solo material**, pero puede solicitar varias unidades mediante `cantidadSolicitada`.
- `Retirada` significa que el club ya entregó el material al socio. `Entregada` significa que el material ha regresado al club; no significa que se haya entregado al socio.
- Los estados `Entregada` y `Cancelada` son finales. No se deben ofrecer acciones de modificación sobre ellos.
- El campo de inventario `activo` controla si se puede crear una nueva reserva. Desactivar un material no elimina ni cancela reservas ya existentes.
- El estado maestro del material no determina por sí solo si es reservable. La interfaz debe respetar `activo` y la validación de disponibilidad de la API.

## Acceso y navegación

Todas las rutas requieren `Authorization: Bearer <accessToken>`.

- La zona **Mis reservas** está pensada para socios reales: el token debe reflejar `isMember=true` y el usuario debe tener una ficha de socio. Tener únicamente el rol técnico `Member` no basta.
- La zona **Administración de reservas** requiere el rol `Admin`.
- El rol y los datos de sesión pueden utilizarse para ocultar accesos y acciones, pero una respuesta `401` o `403` debe seguir gestionándose. En particular, un usuario marcado como socio pero sin ficha puede recibir `403` al usar las rutas personales.
- La API es la fuente de autorización: no se debe intentar seleccionar otro `userId` para consultar o modificar reservas ajenas. Las rutas `me` siempre operan sobre la identidad del token.

## Modelo de presentación

El recurso `ReservaMaterialDto` contiene los datos necesarios para una tarjeta de detalle y para las tablas. Como mínimo, mostrar:

- material (`materialDescripcion`) y unidades solicitadas (`cantidadSolicitada`);
- estado, retirada prevista y devolución estimada;
- retirada y devolución efectivas cuando existan;
- las dos confirmaciones de devolución;
- observaciones del socio y, si existe, `observacionAdministracion`;
- en administración, los datos de `socio` (nombre completo y correo).

Las fechas `fechaRetiradaPrevista`, `fechaRetirada`, `fechaDevolucionEstimada` y `fechaDevolucionEfectiva` son fechas sin hora: deben enviarse y mostrarse como `YYYY-MM-DD`. `fechaSolicitudUtc` es una marca de tiempo UTC y debe convertirse a la zona horaria de presentación elegida por la aplicación.

OpenAPI representa `estado` como entero. La interfaz debe mapear de forma estable los siguientes valores:

| Valor | Etiqueta para mostrar | Significado |
| --- | --- | --- |
| `1` | Solicitada | Petición pendiente; el material aún pertenece al inventario. |
| `2` | Retirada | El administrador lo ha entregado al socio. |
| `3` | Entregada | Devuelto al club y confirmado por las dos partes. |
| `4` | Cancelada | Reserva anulada; no se puede reabrir. |

No se debe deducir el estado a partir de las fechas o de los booleanos: se debe usar siempre el campo `estado` recibido.

## Vistas para el socio

### Listado y detalle de “Mis reservas”

Usar `GET /api/inventario-reservas/me` para el listado y `GET /api/inventario-reservas/me/{reservaId}` para recargar un detalle. El listado llega ordenado desde el servidor por fecha de solicitud descendente.

Conviene separar visualmente las reservas activas (`Solicitada`, `Retirada`) de las históricas (`Entregada`, `Cancelada`), pero sin ocultar el historial. Una etiqueta de estado y un indicador independiente para cada confirmación evitan confundir “en devolución” con “devuelta”.

En el detalle se puede usar `disponibilidad` como información contextual del material:

- `cantidadTotal`: existencias registradas;
- `cantidadComprometida`: unidades comprometidas por reservas solicitadas o retiradas;
- `cantidadDisponible`: diferencia entre ambas;
- `activo`: indica si actualmente admite nuevas reservas.

Estos valores son una fotografía de la consulta. No deben utilizarse para garantizar una operación ni actualizarse de forma optimista.

### Formulario de solicitud y edición

Para crear se usa `POST /api/inventario-reservas/me`; para editar, `PUT /api/inventario-reservas/me/{reservaId}`. Ambos reciben el objeto completo:

```json
{
  "inventarioMaterialId": 42,
  "cantidadSolicitada": 2,
  "fechaRetiradaPrevista": "2026-11-12",
  "fechaDevolucionEstimada": "2026-11-15",
  "observaciones": "Recogeré el material por la tarde."
}
```

Reglas que el formulario debe anticipar:

| Campo | Regla de interfaz |
| --- | --- |
| Material | Identificador entero positivo. Cargar el catálogo con `GET /api/inventario-materiales` y excluir de nuevas solicitudes los materiales con `activo: false`. |
| Cantidad solicitada | Entero positivo, sin decimales. No usar `cantidad` del material como disponibilidad garantizada. |
| Retirada prevista | Obligatoria, formato `YYYY-MM-DD`. |
| Devolución estimada | Obligatoria, formato `YYYY-MM-DD` e igual o posterior a la retirada prevista. |
| Observaciones | Opcionales; máximo 2.000 caracteres. Permitir saltos de línea, pero no otros caracteres de control. Los espacios extremos se normalizan en el servidor. |

El contrato actual no ofrece una consulta pública de disponibilidad para cada material antes de crear una reserva. Por ello, el selector puede mostrar materiales activos y su cantidad total, pero no debe presentar esa cantidad como “disponible”. La comprobación definitiva se realiza al enviar la solicitud. Si se necesita mostrar disponibilidad en tiempo real para todos los materiales, debe acordarse un endpoint específico antes de construir esa funcionalidad.

Solo habilitar **Editar** y **Cancelar** cuando `estado` sea `Solicitada`. Al editar se puede cambiar material, cantidad, fechas previstas y observaciones; se debe volver a enviar todo el cuerpo. Tras un `200`, sustituir o recargar el detalle con la respuesta recibida.

La cancelación de una solicitud se ejecuta con `DELETE /api/inventario-reservas/me/{reservaId}` y devuelve `204 No Content`. Debe mostrar una confirmación previa y, al terminar, recargar la reserva o el listado: el resultado es estado `Cancelada`, no un borrado físico.

### Confirmación del socio en la devolución

Cuando una reserva está en `Retirada`, el socio no puede editarla ni cancelarla. Su única acción es confirmar que ha devuelto el material mediante:

```text
POST /api/inventario-reservas/me/{reservaId}/confirmacion-devolucion
```

La acción no lleva cuerpo y devuelve `204`. Debe mostrarse como una confirmación explícita, no como un interruptor que el usuario pueda desmarcar. Tras la respuesta, recargar el detalle: si Administración aún no ha confirmado, el estado seguirá siendo `Retirada`; cuando ambas confirmaciones consten, el estado será `Entregada`.

## Vistas de administración

Usar `GET /api/inventario-reservas/admin` y `GET /api/inventario-reservas/admin/{reservaId}`. Esta vista debe mostrar además el socio, sus datos de contacto, material, unidades, todas las fechas y ambas confirmaciones. Es aconsejable priorizar visualmente las reservas `Solicitada` y `Retirada`.

La siguiente matriz determina las acciones que se deben ofrecer:

| Estado recibido | Acción de Administración | Ruta y cuerpo | Resultado esperado |
| --- | --- | --- | --- |
| `Solicitada` | Registrar retirada | `POST /api/inventario-reservas/admin/{id}/retirada` con `{ "fechaRetirada": "YYYY-MM-DD" }` | `204`; pasa a `Retirada`. |
| `Solicitada` | Cancelar | `POST /api/inventario-reservas/admin/{id}/cancelacion` con `{ "observacionAdministracion": "..." }` | `204`; la observación es opcional en este estado. |
| `Retirada` | Confirmar devolución administrativa | `POST /api/inventario-reservas/admin/{id}/confirmacion-devolucion` con `{ "fechaDevolucionEfectiva": "YYYY-MM-DD" }` | `204`; se completa solo si el socio ya confirmó. |
| `Retirada` | Cancelar | `POST /api/inventario-reservas/admin/{id}/cancelacion` con `{ "observacionAdministracion": "..." }` | `204`; la observación es obligatoria. |
| `Entregada` o `Cancelada` | Ninguna | — | Estado final, solo lectura. |

La fecha de devolución efectiva no puede ser anterior a la fecha de retirada efectiva. La interfaz debe validarlo antes del envío y conservar el error de la API si el dato que muestra está desactualizado.

La confirmación administrativa tampoco es reversible. Si Administración confirma primero, la reserva continúa en `Retirada` hasta que confirme el socio; si el socio confirma primero, la acción administrativa la cambiará a `Entregada`. Después de cualquiera de las dos operaciones hay que recargar el detalle, porque el estado final depende de ambas confirmaciones.

## Materiales y disponibilidad

La disponibilidad se calcula en el servidor así:

```text
cantidad disponible = cantidad total - unidades de reservas Solicitada y Retirada
```

Las reservas `Entregada` y `Cancelada` dejan de comprometer unidades. Este cálculo es común para todos los socios y se protege contra solicitudes simultáneas; por tanto, la validación de cliente solo mejora la experiencia y no evita un `409 Conflict`.

Para las vistas de inventario de Administración:

- incluir y persistir `activo` en el formulario de `POST` y `PUT /api/inventario-materiales`;
- al desactivar un material, explicar que se conserva el histórico y que solo se impiden nuevas solicitudes;
- no permitir que la interfaz considere segura una reducción de `cantidad` si existen reservas activas. La API rechazará reducirla por debajo de las unidades comprometidas;
- los materiales inactivos pueden y deben seguir mostrándose en detalles e históricos de reservas.

## Errores, concurrencia y actualización de pantalla

No actualizar de forma optimista el estado de una reserva ni la disponibilidad. Deshabilitar temporalmente el botón mientras la petición está en curso y, tras una respuesta satisfactoria, recargar el recurso afectado.

| Código | Tratamiento recomendado |
| --- | --- |
| `400 Bad Request` | Mostrar el detalle de validación junto al campo correspondiente. Mantener el formulario y sus valores. |
| `401 Unauthorized` | Aplicar el flujo de renovación o inicio de sesión. |
| `403 Forbidden` | Ocultar o deshabilitar la acción y explicar que el usuario no es socio válido o no tiene permisos administrativos. No reintentar. |
| `404 Not Found` | La reserva o material ya no está disponible para esa identidad. Volver al listado y no revelar ni conservar datos de una reserva ajena. |
| `409 Conflict` | Puede indicar falta de unidades, material inactivo, cambio de estado concurrente o una transición no permitida. Mostrar `detail`, conservar el formulario y recargar listado/detalle antes de permitir reintentar. |
| `500` o error de red | Mostrar un mensaje recuperable; no confirmar visualmente la operación hasta que una consulta posterior la verifique. |

Las notificaciones por correo se intentan después de persistir la operación. La interfaz debe tomar la respuesta HTTP como resultado de la reserva y no mostrar el envío de correo como condición de éxito: un fallo de correo no revierte una creación, retirada, cancelación o confirmación realizada correctamente.

## Lista de comprobación para implementación

- [ ] El frontend usa los tipos y rutas de `openapi.json`; no construye cuerpos con campos de solo lectura.
- [ ] El selector de creación solo permite materiales activos y la cantidad es un entero positivo.
- [ ] El formulario bloquea una devolución estimada anterior a la retirada prevista.
- [ ] Solo una reserva `Solicitada` ofrece edición o cancelación al socio.
- [ ] Una reserva `Retirada` ofrece al socio únicamente su confirmación de devolución.
- [ ] La administración registra la retirada, la cancelación posterior con observación y su confirmación de devolución con fecha efectiva.
- [ ] Las reservas finales son de solo lectura.
- [ ] Tras `204`, `200` o `201`, la pantalla recarga el detalle o listado para obtener estado, disponibilidad y confirmaciones vigentes.
- [ ] Los conflictos `409` no se interpretan como éxito ni se resuelven únicamente con datos locales.
