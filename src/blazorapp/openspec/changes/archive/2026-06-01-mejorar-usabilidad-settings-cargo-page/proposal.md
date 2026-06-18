## Why

La funcionalidad de gestión de cargos ya existe, pero la presentación actual de `SettingsCargoPage` y `CargoManagementView` no resulta usable: la jerarquía visual es débil, el formulario no aprovecha bien DaisyUI y la composición se rompe en pantallas estrechas. Este cambio busca convertir la página en una interfaz administrativa clara, consistente con `Mi Cuenta` y responsive sin alterar el comportamiento CRUD ya disponible.

## What Changes

- Revisar `SettingsCargoPage` y `CargoManagementView` para asegurar un uso coherente de DaisyUI en breadcrumb, títulos, fieldset, input, botones, estados y listado.
- Rediseñar la composición visual de la pantalla con una cabecera más clara, bloques con mejor separación y un formulario de alta/edición más usable.
- Mejorar el aspecto del listado de cargos para que cada elemento tenga acciones legibles y un patrón visual estable en móvil y escritorio.
- Ajustar los estilos SCSS globales de `Configuración` para incorporar un layout responsive con mejor espaciado, anchuras y reflujo.
- Añadir o ajustar cobertura de tests de componente para validar el marcado esperado del nuevo layout usable.

## Capabilities

### New Capabilities
- `frontend-admin-cargos-usable-layout`: Define una experiencia visual usable y responsive para la página de gestión de cargos, alineada con DaisyUI y con el lenguaje visual del resto de componentes.

### Modified Capabilities

## Impact

- Afecta a `IndaloaventurApp.SharedUI/Components/Settings/CargoManagementView.razor` y a su clase partial solo en lo necesario para soportar la nueva composición.
- Afecta a `IndaloaventurApp.Web/IndaloaventurApp.Web/wwwroot/scss/components/_configuracion.scss` para reforzar el uso de DaisyUI y el comportamiento responsive.
- Afecta a recursos localizados ES relacionados con títulos, ayudas visuales y estados del formulario/listado.
- Afecta a `IndaloaventurApp.Frontend.Tests` con tests de componente para proteger el layout y la interacción principal.
