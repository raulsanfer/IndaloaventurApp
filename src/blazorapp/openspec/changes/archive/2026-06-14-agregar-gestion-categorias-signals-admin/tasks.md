## 1. Navegación administrativa de Signals

- [x] 1.1 Añadir la opción administrativa `Signals` en `Configuración` visible solo para administradores
- [x] 1.2 Crear la ruta y vista privada intermedia `Signals` con breadcrumb y acceso a `Categorías`
- [x] 1.3 Crear la ruta privada `Signals -> Categorías` con la misma protección por rol `Admin` que el resto de páginas administrativas

## 2. Integración frontend con signal-types

- [x] 2.1 Extender la abstracción y el cliente frontend de `signals` para soportar crear, actualizar y eliminar `SignalTypes`
- [x] 2.2 Añadir los modelos de edición necesarios para nombre e icono y mapear correctamente los errores de carga y guardado

## 3. Interfaz de gestión de categorías

- [x] 3.1 Implementar la vista administrativa de `Categorías` con listado, estado vacío, carga y reintento
- [x] 3.2 Implementar el formulario de alta y edición con validación de nombre obligatorio
- [x] 3.3 Implementar la acción de borrado con feedback de éxito y manejo controlado cuando la categoría no pueda eliminarse
- [x] 3.4 Añadir los recursos localizados y estilos SCSS necesarios para `Signals` y `Categorías` siguiendo el patrón administrativo existente

## 4. Verificación

- [x] 4.1 Añadir o actualizar pruebas automatizadas de navegación administrativa, control de acceso y CRUD de categorías si la cobertura existente lo permite
- [ ] 4.2 Verificar manualmente el recorrido admin completo: Configuración -> Signals -> Categorías, alta, edición y borrado rechazado cuando corresponda
