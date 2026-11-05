## 1. Contratos y acceso HTTP administrativo

- [x] 1.1 Crear los modelos compartidos de reserva administrativa, socio, disponibilidad, estados y solicitudes de retirada, cancelación y devolución, con documentación XML pública; verificar que la solución compila sin advertencias nuevas de documentación.
- [x] 1.2 Definir `IInventoryReservationAdminService` con operaciones de listado, detalle y tres transiciones, y verificar mediante pruebas de compilación que los componentes consumen el contrato sin depender de DTO de transporte.
- [x] 1.3 Implementar y registrar el cliente HTTP tipado administrativo con Bearer de sesión y mapeo de `ServiceResult`/`ProblemDetails`; verificar con pruebas de cliente los métodos, rutas, cuerpos JSON, respuesta `204 No Content` y encabezado Authorization de los cinco endpoints administrativos.
- [x] 1.4 Cubrir el cliente con pruebas de respuestas `400`, `401`, `403`, `404`, `409`, error de red, timeout, `5xx` y carga JSON inválida; verificar que no revela detalles internos ni reintenta automáticamente operaciones de transición.

## 2. Entrada y ruta protegida de Configuración

- [x] 2.1 Añadir el recurso localizado y la entrada `Reservas de material` a Configuración, visible solo para Admin; verificar con prueba de componente que un Admin la ve y un usuario sin ese rol no.
- [x] 2.2 Crear la página `/configuracion/reservas-material` y aplicar el guardia de sesión/rol antes de renderizar la herramienta; verificar con pruebas de ruta que sesiones anónimas y no Admin se redirigen sin invocar el servicio de reservas.

## 3. Consulta y presentación responsive

- [x] 3.1 Implementar el componente parcial de gestión administrativa con estados accesibles de carga, vacío, error, lista y detalle bajo demanda; verificar con pruebas bUnit que usa los endpoints de colección y detalle y muestra los campos de reserva y socio definidos por la especificación.
- [x] 3.2 Representar los estados de reserva exclusivamente desde el valor devuelto por la API y dejar `Entregada` y `Cancelada` en solo lectura; verificar con pruebas de matriz de estado que no se deducen acciones de fechas o confirmaciones.
- [x] 3.3 Añadir textos localizados y estilos SCSS globales que compongan lista/detalle y formularios con DaisyUI de forma responsive; verificar manualmente o con prueba de renderizado que a viewport estrecho no hay desbordamiento horizontal y se mantienen visibles datos y acciones esenciales.

## 4. Decisiones administrativas y consistencia remota

- [x] 4.1 Implementar el formulario confirmado de retirada para reservas `Solicitada`, enviando únicamente `fechaRetirada`; verificar con prueba de componente que una respuesta `204` recarga listado y detalle sin actualización optimista.
- [x] 4.2 Implementar la cancelación confirmada, permitiendo observación opcional en `Solicitada` y exigiendo observación no vacía en `Retirada`; verificar ambas reglas y el cuerpo enviado con pruebas de componente.
- [x] 4.3 Implementar la confirmación administrativa de devolución para `Retirada`, exigiendo fecha y rechazando localmente una fecha anterior a la retirada efectiva; verificar que tras `204` el estado mostrado procede de una nueva consulta, incluido el caso en que aún falta la confirmación del socio.
- [x] 4.4 Implementar el manejo de errores en la vista: conservar formularios y errores de campo en `400`, bloquear reintentos en `403`, limpiar detalle y datos de socio en `404`, y recargar antes de reintentar en `409`; verificar cada flujo con pruebas bUnit, incluidos error de red y `5xx` recuperables.

## 5. Documentación y validación final

- [x] 5.1 Revisar si el cambio afecta `docs/architecture` o `docs/features`, actualizar la documentación conceptual si procede y verificarla con `docfx docs/docfx.json`; verificar también que los comentarios XML de los nuevos contratos públicos se incluyen correctamente.
- [ ] 5.2 Ejecutar `dotnet test` para la solución o los proyectos frontend afectados y corregir los fallos introducidos; verificar que las pruebas de cliente, página y componentes de reservas pasan.
- [x] 5.3 Ejecutar `openspec validate frontend-admin-material-reservations --strict` y verificar que la propuesta, diseño, especificación y tareas son válidas antes de iniciar la implementación.
