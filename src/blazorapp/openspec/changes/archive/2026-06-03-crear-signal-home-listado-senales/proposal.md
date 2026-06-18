## Why

La app ya comunica la existencia de `Señales` desde el dashboard de inicio, pero todavía no ofrece una pantalla de entrada donde el socio pueda consultar incidencias con una presentación coherente, navegable y alineada con el diseño definido para esta funcionalidad. Esta primera fase desbloquea la base visual y funcional de `SignalHome`, dejando preparada la navegación, el listado y los filtros principales sobre contratos API ya existentes o previstos para completarse, y la posiciona además como acceso persistente del menú footer para cualquier usuario autenticado.

## What Changes

- Crear una nueva pantalla frontend `SignalHome` como punto de entrada de la funcionalidad de señales.
- Añadir `Signals` al menú footer autenticado con iconografía de advertencia, presencia visible para todos los roles de usuario y posición estable entre `Home` y `Mi Cuenta`, preferiblemente antes de `Mi Club`.
- Añadir una vista de listado de señales basada en el diseño de `openspec/design/signal/list`, incluyendo breadcrumb, buscador, filtros por categoría en formato píldora y cards de resultado.
- Cargar las señales desde el API de signals y resolver las categorías a partir de `signal-types`, manteniendo una capa de servicio frontend desacoplada de la UI.
- Definir el comportamiento de scroll horizontal para las categorías cuando el número de filtros exceda el ancho visible.
- Usar DaisyUI como base para los nuevos componentes visuales de Signals, manteniendo los ajustes SCSS complementarios que ya se vienen aplicando en otras vistas.
- Establecer estados de carga, vacío y error para la primera fase del listado.
- Dejar fuera en esta iteración los flujos de alta, edición, comentarios y detalle de una señal individual.

## Capabilities

### New Capabilities
- `frontend-signal-home-page`: Define la nueva página `SignalHome`, su navegación desde Home y desde el menú footer, la carga del listado de señales y la experiencia inicial de búsqueda y filtrado por categorías.

### Modified Capabilities

None.

## Impact

- Afecta a `IndaloaventurApp.SharedUI` con nuevos componentes Razor, modelos de vista, recursos localizados y estilos SCSS.
- Afecta a `IndaloaventurApp.Web.Client` con una nueva página y un cliente HTTP para `signals` / `signal-types`.
- Afecta a la navegación autenticada global mediante cambios en `BottomNav` y sus recursos visibles para todos los roles.
- Afecta a la composición visual del módulo Signals, que deberá montarse sobre base DaisyUI con refinado SCSS complementario.
- Depende de los contratos `GET /api/signals`, `GET /api/signal-types` y, si la imagen no se incorpora al listado principal, de `GET /api/signals/{id}/images`.
- Puede requerir alineación con backend si el contrato final del listado necesita exponer de forma directa todos los campos visuales del diseño.
