## 1. Rediseño de estructura y flujo

- [x] 1.1 Reorganizar `CargoManagementView` para incluir breadcrumb, nueva cabecera y fieldset superior alineados con `SettingsView`
- [x] 1.2 Adaptar el estado del formulario para alternar entre `Nuevo Cargo` y edición desde la lista sin duplicar componentes
- [x] 1.3 Sustituir la presentación actual por un listado administrativo con acciones `Editar` y `Borrar`

## 2. Comportamiento CRUD en alcance

- [x] 2.1 Mantener operativas la carga inicial y el alta de cargos desde el fieldset con refresco de lista
- [x] 2.2 Mantener operativa la edición de cargos cargando el elemento seleccionado en el fieldset y guardando mediante `PUT`
- [x] 2.3 Retirar de esta iteración la eliminación efectiva y dejar la acción `Borrar` como capacidad visual no operativa

## 3. Recursos, estilos y validación

- [x] 3.1 Añadir o ajustar claves de recursos ES para breadcrumb, `Nuevo Cargo`, modo edición y mensajes auxiliares
- [x] 3.2 Actualizar los parciales SCSS globales de `Settings` para soportar fieldset, lista y nueva jerarquía tipográfica sin estilos inline
- [x] 3.3 Verificar manualmente la experiencia de creación y edición, el refresco del listado y la no ejecución del borrado
