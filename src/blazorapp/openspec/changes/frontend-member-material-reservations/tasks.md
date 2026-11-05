## 1. Contratos y acceso HTTP de reservas personales

- [x] 1.1 Crear los modelos de lectura y de alta de solicitud Member, el modelo de formulario con validación cruzada de fechas/observaciones y `IMemberMaterialReservationService` documentado con XML; verificar las reglas con pruebas unitarias de validación.
- [x] 1.2 Implementar y registrar el cliente HTTP de reservas personales con Bearer de la sesión para listar, consultar detalle, cargar materiales activos, crear, cancelar y confirmar devolución; verificar con pruebas de cliente las rutas, métodos, cuerpos y mapeos del contrato.
- [x] 1.3 Implementar el mapeo seguro de `ProblemDetails`, errores de red, tiempo de espera y respuestas JSON inválidas; verificar con pruebas de cliente los casos `400`, `401`, `403`, `404`, `409` y fallo transitorio, sin filtrar datos ni reintentar mutaciones.

## 2. Navegación y protección de la experiencia Member

- [x] 2.1 Incorporar una comprobación reutilizable de sesión para reservas propias que exija rol `Member` e `IsMember=true`, y usarla para mostrar `Reservas de material` inmediatamente después de `Teléfonos de interés` en Mi Club; verificar con pruebas de componente las combinaciones de rol y condición de socio.
- [x] 2.2 Crear las rutas de listado, nueva solicitud y detalle bajo `/mi-club/reservas-material`, con guardas que no inicien consultas para sesiones no autorizadas; verificar con pruebas de página las redirecciones y la ausencia de llamadas de datos.
- [x] 2.3 Gestionar `401` mediante el flujo de sesión existente y `403` bloqueando acciones con una explicación segura; verificar ambos recorridos de interfaz sin exponer datos de reservas.

## 3. Listado y detalle de reservas propias

- [x] 3.1 Implementar el listado de tarjetas verticales que ordene `RequestedAtUtc` de forma descendente y muestre solo material/unidades, fecha de solicitud, recogida prevista y estado explícito; verificar con pruebas de componente el orden, los campos renderizados y los estados de carga, vacío y error.
- [x] 3.2 Añadir al listado el botón flotante accesible con símbolo `+` que navegue a nueva solicitud; verificar con una prueba de componente o de página que la acción sea alcanzable.
- [x] 3.3 Implementar el detalle recargando la reserva mediante la ruta `me/{reservaId}` y limpiando la selección ante `404`; verificar con pruebas que no se conserven datos de una reserva inexistente o no disponible.
- [x] 3.4 Renderizar acciones dependientes de estado: cancelar solo `Solicitada`, confirmar devolución solo `Retirada` y lectura sola en estados finales; verificar con pruebas de componente toda la matriz de estados y la ausencia de controles de edición.
- [x] 3.5 Añadir confirmaciones explícitas para cancelación y devolución, deshabilitar la acción durante el envío y recargar detalle/listado tras `204`; verificar con pruebas que no haya actualización optimista y que un `409` preserve la entrada y refresque el estado remoto.

## 4. Alta de solicitud y diseño responsive

- [x] 4.1 Implementar el formulario reutilizable de nueva solicitud con selector de materiales activos, cantidad entera positiva, recogida prevista, devolución estimada y observaciones opcionales; verificar validaciones locales y que materiales inactivos no sean seleccionables.
- [x] 4.2 Enviar exclusivamente el cuerpo de `POST /api/inventario-reservas/me`, conservar los valores y errores de campo ante `400`/`409`, y recargar el listado tras `201`; verificar con pruebas de componente y cliente los recorridos satisfactorios y de conflicto.
- [x] 4.3 Añadir recursos localizados y estilos SCSS globales con DaisyUI para tarjetas, estados, formulario, confirmaciones y botón flotante; verificar manualmente en anchos móvil y escritorio que no exista desbordamiento horizontal y que la información esencial sea accesible.
- [ ] 4.4 Ampliar el catálogo autenticado con la cantidad disponible calculada por el servidor y mostrarla para el material seleccionado como una fotografía informativa; corregir la expresión Razor que deja el envío deshabilitado y verificar ambos casos con pruebas de cliente y componente.

## 5. Documentación y verificación integral

- [ ] 5.1 Revisar el impacto en `docs/architecture` y `docs/features`, documentar el cliente y el flujo Member si procede, mantener XML docs de contratos públicos y verificar con `docfx docs/docfx.json`.
- [ ] 5.2 Ejecutar las pruebas de frontend afectadas y `dotnet test` para la solución o proyectos aplicables, corregir regresiones de este cambio y verificar manualmente el flujo de socio real desde Mi Club hasta cancelación o confirmación de devolución.
