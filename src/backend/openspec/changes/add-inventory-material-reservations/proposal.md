## Why

El inventario registra existencias, pero el club no puede asignar unidades a socios ni conocer quien las tiene, cuando deben devolverse o si su devolucion ha sido confirmada. Se necesita un flujo trazable de reservas que impida comprometer mas unidades de las disponibles y conserve el historico del material retirado, devuelto o cancelado.

## What Changes

- Incorporar reservas de material por socio y cantidad solicitada, asociadas a un unico material por reserva.
- Calcular la disponibilidad a partir de la cantidad total menos las unidades comprometidas por reservas solicitadas o retiradas, con proteccion ante operaciones concurrentes.
- Incorporar el campo administrativo `Activo` al material de inventario para excluirlo de nuevas reservas sin eliminar su historico.
- Definir el ciclo de vida controlado `Solicitada`, `Retirada`, `Entregada` y `Cancelada`, con fechas efectivas y estimadas de retirada y devolucion.
- Permitir que el socio cree, consulte, edite o cancele sus solicitudes solo antes de la retirada; reservar las transiciones de retirada, cancelacion posterior y devolucion a administracion.
- Exigir dos confirmaciones de devolucion —socio y administrador— y completar la entrega al club solamente cuando ambas consten.
- Enviar correos trazables al socio y al buzon administrativo al crear una reserva, cambiar su estado o registrar una confirmacion de devolucion.
- Exponer consultas personales aisladas por propietario y operaciones administrativas de gestion, con autorizacion basada en socio real (`IsMember`) y ficha de socio existente, no solo en el rol tecnico `Member`.

## Capabilities

### New Capabilities

- `inventory-material-reservations`: Gestion de reservas de unidades de material de inventario, disponibilidad, ciclo de retirada y devolucion, confirmaciones, autorizacion y notificaciones.

### Modified Capabilities

<!-- Ninguna especificacion canonica existente cambia. La incorporacion de Activo se coordina con el cambio activo de material de inventario. -->

## Impact

- Domain: nuevo agregado de reserva, estado controlado, reglas de disponibilidad y extension `Activo` de `InventarioMaterial`.
- Application e Infrastructure: comandos, consultas, repositorios, configuracion EF Core, indices y migracion SQL Server para reservas y material activo.
- API: endpoints de autoservicio para socios y gestion para administracion; actualizacion de la matriz de autorizacion.
- Email: nuevas opciones de destinatario administrativo y plantillas genericas mediante el servicio de correo existente; los fallos de envio no revertiran una reserva persistida y deberan quedar registrables o reintentables.
- Dependencia: la implementacion requiere que `add-inventory-item-management` este integrado antes de crear las reservas sobre `InventarioMaterial`.
