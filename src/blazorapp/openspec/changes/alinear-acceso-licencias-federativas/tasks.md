## 1. Alinear acceso y alcance del flujo de socio

- [x] 1.1 Centralizar en sesión o autorización compartida la regla de acceso a `Licencias Federativas` para distinguir `Member + IsMember=true`, `Member + IsMember=false` y `Admin`.
- [x] 1.2 Ajustar la navegación y la vista de autoservicio para que un `Member + IsMember=false` no tenga visualización ni flujo operativo de `Licencias Federativas`.
- [x] 1.3 Ajustar el cliente y la carga de datos del flujo de socio para que solo consulte y cree solicitudes del usuario autenticado, sin edición de solicitudes existentes.

## 2. Incorporar gestión administrativa de licencias federativas

- [x] 2.1 Confirmar y adaptar el contrato de frontend para operaciones administrativas sobre un usuario objetivo distinto de `me`.
- [x] 2.2 Implementar la entrada y el contenedor del flujo administrativo para que un `Admin` acceda independientemente de `IsMember`.
- [x] 2.3 Implementar consulta de solicitudes de cualquier usuario desde el flujo administrativo, incluyendo el contexto del propio admin.
- [x] 2.4 Implementar edición administrativa de solicitudes usando los campos autorizados por el backend y refresco coherente del resultado.

## 3. Validación y regresión

- [x] 3.1 Añadir o actualizar pruebas de componente para cubrir visibilidad y operatividad de `Member + IsMember=false`, `Member + IsMember=true` y `Admin`.
- [x] 3.2 Añadir o actualizar pruebas de cliente/API para validar el alcance `self` frente a `any-user` y los bloqueos por rol.
- [x] 3.3 Ejecutar la batería de pruebas afectada y dejar marcadas como completadas solo las tareas verificadas en verde.
