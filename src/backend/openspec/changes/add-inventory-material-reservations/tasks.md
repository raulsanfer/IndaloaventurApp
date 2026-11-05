## 1. Dominio y contratos

- [x] 1.1 Integrar la extension `Activo` en `InventarioMaterial`, con valor inicial compatible para materiales existentes; verificar con pruebas de dominio los cambios de activo y los limites de cantidad.
- [x] 1.2 Crear el agregado `ReservaMaterial`, su enum de estados y reglas de fechas, cantidades, transiciones, cancelacion y doble confirmacion de devolucion; verificar con pruebas de dominio todos los flujos validos y cada transicion o fecha invalida.
- [x] 1.3 Definir contratos de repositorio y DTOs de reserva, disponibilidad, socio/ficha y notificaciones sin introducir dependencias de Infrastructure en Application; verificar con pruebas de arquitectura y compilacion de los contratos.

## 2. Casos de uso de reservas

- [x] 2.1 Implementar creacion y edicion de reservas propias, validando `IsMember`, ficha de socio, material activo, fechas y cantidad disponible; verificar con pruebas de Application los casos validos, sin ficha, material inactivo, cantidad excedida y fechas invalidas.
- [x] 2.2 Implementar consulta aislada por propietario, cancelacion logica antes de retirada y confirmacion propia de devolucion; verificar con pruebas de Application que no se muestran ni modifican reservas ajenas y que el socio no puede alterar una retirada.
- [x] 2.3 Implementar consultas y transiciones administrativas de retirada, cancelacion y entrega, incluida la observacion obligatoria al cancelar una retirada; verificar con pruebas de Application permisos, estados finales, fechas efectivas y liberacion de unidades.
- [x] 2.4 Crear el servicio y configuracion de notificaciones de reserva con plantillas HTML y texto plano para solicitud, estado y ambas confirmaciones; verificar con pruebas que avisa a socio y administracion tras persistir y que un fallo de correo no revierte el cambio.
- [x] 2.5 Implementar proyecciones Dapper para reservas propias y administrativas con socio, material, cantidad, fechas, estado, confirmaciones y disponibilidad; verificar mediante pruebas que los listados estan aislados, ordenados y no materializan agregados.

## 3. Persistencia y concurrencia

- [x] 3.1 Incorporar `DbSet`, configuracion EF Core e indices de `ReservasMaterial`, con claves foraneas restrictivas, campos requeridos, estados y concurrencia; verificar el modelo contra SQL Server aislado.
- [x] 3.2 Implementar los repositorios EF Core que calculan y bloquean unidades disponibles dentro de una transaccion serializable para altas, ediciones, cancelaciones, devoluciones y cambios de cantidad del material, impidiendo reducir el stock por debajo de los compromisos activos; verificar con pruebas de infraestructura que dos operaciones concurrentes no sobreasignan unidades.
- [x] 3.3 Generar la migracion descriptiva para `Activo` y `ReservasMaterial`, actualizar el snapshot y revisar que `Up` inicializa los materiales existentes como activos; verificar con `dotnet ef migrations list --project src/IndaloAventurApi.Infrastructure --startup-project src/IndaloAventurApi.Api`.

## 4. API y seguridad

- [x] 4.1 Crear los contratos HTTP y el controlador bajo `/api/inventario-reservas`, con operaciones `me` y administrativas, respuestas `200`, `201`, `204`, `400`, `403`, `404` y `409` mediante ProblemDetails en Espanol; verificar con pruebas de integracion el flujo completo desde solicitud a entrega.
- [x] 4.2 Aplicar `ClubMember` y la identidad del token a las rutas de socio, `Admin` a las administrativas, y actualizar `ApiAuthorizationMatrix`; verificar con pruebas de integracion rechazos anonimos, `IsMember=false`, falta de ficha, acceso cruzado y rol insuficiente.
- [x] 4.3 Extender el contrato de material para exponer y editar `Activo` solo por administracion; verificar con pruebas de integracion que un material desactivado no admite nuevas reservas y conserva su historico.

## 5. Verificacion integral

- [x] 5.1 Ejecutar `dotnet test IndaloAventurApi.sln` y corregir regresiones hasta que la suite completa pase, incluyendo concurrencia, propiedad, estados, correo y autorizacion.
- [x] 5.2 Ejecutar `dotnet build IndaloAventurApi.sln --no-restore` y `openspec validate add-inventory-material-reservations --strict`; verificar compilacion sin errores y especificacion valida antes de marcar las tareas como completadas.
