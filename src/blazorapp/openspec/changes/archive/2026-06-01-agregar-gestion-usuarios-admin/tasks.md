## 1. Navegación y contrato administrativo

- [x] 1.1 Añadir la opción `Usuarios` dentro de `Configuración -> Administración` y crear la nueva ruta/página administrativa
- [x] 1.2 Crear o extender servicios frontend para buscar usuarios en `GET /api/users`
- [x] 1.3 Crear o extender servicios frontend para cargar, crear y actualizar fichas administrativas con `GET/POST/PUT /api/fichas-socio/{userId}`

## 2. Flujo de usuarios y ficha administrativa

- [x] 2.1 Implementar la pantalla de `Usuarios` con buscador por email, listado y estados operativos
- [x] 2.2 Implementar la lógica condicional `Editar` para `IsMember = true` y `Crear Ficha` para `IsMember = false`
- [x] 2.3 Implementar la ficha administrativa de socio y la redirección tras crear o editar

## 3. Recursos, estilos y validación

- [x] 3.1 Añadir recursos localizados ES y estilos SCSS globales para `Usuarios` y la ficha administrativa
- [x] 3.2 Añadir tests de servicio y componente para búsqueda, visibilidad de acciones y redirecciones principales
- [x] 3.3 Confirmar el mecanismo de búsqueda por email en `GET /api/users` y el payload mínimo aceptado por `POST /api/fichas-socio/{userId}` antes de cerrar la implementación
