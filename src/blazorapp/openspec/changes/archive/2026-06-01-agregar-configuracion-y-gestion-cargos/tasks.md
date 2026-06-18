## 1. Navegación y autorización de Configuración

- [x] 1.1 Añadir la ruta/página `Configuración` en `IndaloaventurApp.Web.Client` y conectar la opción `Configuración` desde `Mi cuenta`
- [x] 1.2 Incorporar componentes reutilizables en `IndaloaventurApp.SharedUI` para la pantalla `Configuración` y el bloque `Administración`
- [x] 1.3 Resolver la comprobación de rol `Admin` en la sesión/autorización actual para controlar visibilidad del panel y de la opción `Cargos`

## 2. Gestión administrativa de Cargos

- [x] 2.1 Crear modelos y cliente API desacoplado para consumir `GET/POST /api/cargos` y `PUT/DELETE /api/cargos/{id}`
- [x] 2.2 Implementar la vista administrativa de `Cargos` con listado, formulario de alta/edición y acción de borrado solo para `Admin`
- [x] 2.3 Añadir estados de carga, éxito y error controlado incluyendo el caso de conflicto al eliminar

## 3. Recursos, estilos y validación

- [x] 3.1 Añadir claves de recursos ES para `Configuración`, `Administración`, `Cargos` y los mensajes operativos asociados
- [x] 3.2 Crear parciales SCSS globales y registrarlos en `style.scss` para las nuevas pantallas sin estilos inline
- [x] 3.3 Verificar navegación desde `Mi cuenta`, visibilidad para `Admin` vs no `Admin` y operaciones CRUD de `Cargos` contra los endpoints definidos
