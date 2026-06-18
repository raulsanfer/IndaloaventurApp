## 1. Contratos y servicios administrativos

- [x] 1.1 Extender el contrato de usuario gestionado consumido por frontend para incluir el estado activo/deactivado de la cuenta.
- [x] 1.2 Adaptar `IAdminUserManagementService` y `AdminUserManagementApiClient` para cargar `GET /api/users` sin filtro por defecto y mantener el filtro opcional por email.
- [x] 1.3 Añadir en el servicio administrativo frontend las operaciones para `POST /api/users/{userId}/deactivate` y `POST /api/users/{userId}/reactivate`.

## 2. Página administrativa de usuarios

- [x] 2.1 Actualizar `AdminUsersManagementView` para que cargue el listado completo al inicializarse y reutilice el formulario actual como filtro por email.
- [x] 2.2 Mostrar en cada fila el estado administrativo del usuario y mantener las acciones `Editar` o `Crear ficha` desde su registro.
- [x] 2.3 Ajustar literales y estados vacíos/carga/error para distinguir entre listado inicial, filtro sin resultados y errores de consulta.

## 3. Ficha administrativa y ciclo de vida

- [x] 3.1 Extender `AdminMemberProfileView` para mostrar el estado actual de la cuenta administrada.
- [x] 3.2 Incorporar la acción `Desactivar` o `Reactivar` según corresponda, con feedback de éxito/error y refresco del estado tras la operación.
- [x] 3.3 Verificar que la navegación desde el listado a la ficha administrativa conserva el contexto operativo esperado para usuarios socio y no socio.

## 4. Verificación

- [x] 4.1 Añadir o actualizar tests frontend para el listado inicial completo, el filtrado por email y las acciones por fila.
- [x] 4.2 Añadir o actualizar tests frontend para desactivación/reactivación desde la ficha administrativa y sus estados de feedback.
