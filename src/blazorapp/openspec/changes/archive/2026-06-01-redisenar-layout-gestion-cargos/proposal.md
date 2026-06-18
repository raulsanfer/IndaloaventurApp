## Why

La vista actual de `CargoManagementView` ya cubre el CRUD base, pero su composición en dos cards no sigue del todo la línea visual introducida en `Configuración` ni prioriza el flujo principal de trabajo sobre una lista administrativa simple. Este cambio redefine la experiencia para que crear y editar cargos resulte más directa, consistente y preparada para crecer sin introducir todavía la eliminación real.

## What Changes

- Rediseñar `CargoManagementView` para alinearla visualmente con `SettingsView` usando `breadcrumb`, jerarquía tipográfica más marcada y una composición vertical más simple.
- Reemplazar el layout actual por un flujo basado en `fieldset` superior para alta/edición y una lista de cargos inspirada en el patrón `list` de DaisyUI.
- Cambiar el formulario para que el modo por defecto sea `Nuevo Cargo`, con un único campo de texto para el nombre y un botón `Guardar`.
- Permitir que la acción `Editar` cargue el cargo seleccionado en el `fieldset`, cambie el formulario a modo edición y, al guardar, actualice el registro y refresque la lista.
- Mantener visible una acción `Borrar` por cada elemento de la lista, pero dejar su comportamiento operativo fuera de alcance en esta iteración.
- Ajustar recursos localizados y estilos SCSS globales necesarios para reflejar la nueva estructura visual y estados del formulario.

## Capabilities

### New Capabilities
- `frontend-admin-cargos-management-layout`: Define la experiencia visual y de interacción de la página de gestión de cargos con breadcrumb, fieldset superior y listado administrativo orientado a alta/edición rápida.

### Modified Capabilities

## Impact

- Afecta a `IndaloaventurApp.SharedUI/Components/Settings/CargoManagementView.razor` y su clase partial asociada para reorganizar la vista y los estados de edición.
- Afecta a los parciales SCSS globales de `Settings` para incorporar el nuevo layout, tipografía y patrón de lista.
- Afecta a recursos localizados ES para literales de breadcrumb, `Nuevo Cargo`, estados de edición y mensajes asociados.
- Reutiliza el cliente/API actual de cargos para `GET`, `POST` y `PUT`, dejando `DELETE` fuera de la implementación operativa de este cambio.
