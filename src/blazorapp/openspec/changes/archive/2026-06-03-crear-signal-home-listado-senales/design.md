## Context

El proyecto ya muestra una tarjeta `Señales` en `HomeDashboard`, pero no existe aún una página dedicada para explorar este dominio. El material de diseño en `openspec/design/signal/list` define una pantalla móvil centrada en listado con breadcrumb, buscador, chips de categorías y cards visuales con imagen, tipología, fecha y resumen.

La arquitectura actual del frontend separa las páginas en `IndaloaventurApp.Web.Client` y las vistas reutilizables en `IndaloaventurApp.SharedUI`. Los datos remotos se consumen mediante interfaces y clientes HTTP desacoplados, como ya ocurre con agenda telefónica o licencias federativas. Además, el proyecto exige localización vía recursos y estilos centralizados en SCSS, no inline. Para esta funcionalidad, los nuevos patrones visuales deben apoyarse en DaisyUI como base y completarse con SCSS del módulo cuando hagan falta ajustes de integración.

La navegación inferior autenticada se construye hoy en `BottomNav` con tres destinos fijos (`Home`, `Mi Club` y `Mi Cuenta`). La ampliación solicitada para Signals implica tratar esta funcionalidad como acceso de primer nivel, no solo como tarjeta secundaria dentro del dashboard.

El contrato OpenAPI actual expone:

- `GET /api/signals` con filtros `descripcion`, `tipo`, `tags` y `activo`.
- `GET /api/signal-types` para obtener categorías.
- `GET /api/signals/{id}/images` para recuperar imágenes por señal.

Ese contrato cubre bien el listado y el filtrado, pero hoy no documenta explícitamente todos los campos visuales exactos del mockup, como un título corto independiente de la descripción o una imagen ya embebida en el DTO base.

## Goals / Non-Goals

**Goals:**
- Crear `SignalHome` como nueva entrada de señales accesible desde Home.
- Exponer `SignalHome` también como acceso persistente del menú footer autenticado.
- Implementar en fase 1 una experiencia de listado fiel al diseño base, incluyendo breadcrumb.
- Consumir señales y categorías desde servicios frontend desacoplados del markup.
- Permitir búsqueda textual y filtrado por categoría usando controles preparados para móvil.
- Garantizar que la fila de chips pueda desplazarse horizontalmente cuando existan muchas categorías.
- Definir fallbacks razonables cuando algunos campos visuales del diseño no lleguen todavía resueltos por el backend.
- Garantizar que el acceso a `Signals` sea visible para cualquier rol autenticado sin depender de permisos específicos de navegación.

**Non-Goals:**
- No implementar en esta fase creación, edición o borrado de señales.
- No implementar comentarios ni gestión de imágenes en detalle.
- No cerrar todavía la arquitectura completa de siguientes subflujos de Signals.
- No redefinir en esta fase una estrategia visual distinta a la combinación ya deseada de DaisyUI como base y SCSS complementario para ajustes del módulo.

## Decisions

### 1. Separar página, vista y servicio como en el resto del frontend

`SignalHomePage.razor` vivirá en `IndaloaventurApp.Web.Client` y delegará la UI en un componente compartido tipo `SignalHomeView`. La carga de datos se encapsulará en una abstracción como `ISignalService`, con un cliente HTTP específico.

Rationale:
- Mantiene coherencia con la arquitectura ya usada en `ClubPhonebookPage` y vistas equivalentes.
- Facilita reutilización futura en otras superficies si Signals evoluciona a app híbrida o móvil.

Alternatives considered:
- Consumir `HttpClient` directamente desde el componente Razor.
  Rechazado porque acopla demasiado la UI al transporte y rompe el patrón actual del repositorio.

### 2. Tratar Signals como destino global del footer autenticado

`BottomNav` incorporará un nuevo item `Signals` con iconografía semántica de advertencia, usando la misma mecánica declarativa del resto de destinos y sin condicionar su visibilidad por rol. La ruta de `SignalHome` pasará a ser un destino primario para cualquier usuario autenticado que vea el shell. Su orden en la navegación inferior deberá mantenerse estable entre `Home` y `Mi Cuenta`, priorizando además colocarlo antes de `Mi Club` para reforzar su descubribilidad.

Rationale:
- Responde al objetivo de producto de dar a `Signals` una presencia constante y visible.
- Encaja con la implementación actual del footer, que se define mediante una colección fija de items en SharedUI.
- Evita que el destino cambie de posición entre builds o roles, lo que mejora memoria de uso en móvil.

Alternatives considered:
- Mantener `Signals` solo como acceso desde la tarjeta de Home.
  Rechazado porque haría la funcionalidad menos descubrible y no cumpliría la ampliación solicitada.
- Mostrar `Signals` solo a algunos roles.
  Rechazado porque el requisito actual pide visibilidad para todos los roles de usuario.

### 3. Resolver categorías desde `signal-types` y filtros de listado desde `signals`

La fila de categorías se construirá con `GET /api/signal-types`. La selección de una categoría filtrará el listado de `signals` por `tipo`. La búsqueda textual utilizará el parámetro `descripcion`. La opción `Todas` será un filtro local adicional del frontend.

Rationale:
- Aprovecha el contrato existente sin hardcodear categorías en la UI.
- Permite que las píldoras sigan el modelo visual pedido y escalen cuando backend añada nuevos tipos.

Alternatives considered:
- Derivar categorías solo a partir de los resultados de `signals`.
  Rechazado porque impide mostrar el conjunto completo de filtros disponibles y hace más inestable la UI.

### 4. Modelar una card de señal con view-model adaptado al diseño

El frontend transformará la respuesta remota en un modelo de presentación con campos como categoría visible, fecha visible, texto principal, extracto, imagen principal y línea de metadato destacado. Ese adaptador podrá combinar `SignalDto`, `SignalTypeDto` y, si sigue siendo necesario, `SignalImagesDto`.

Rationale:
- El diseño requiere una estructura de card más rica que el DTO base expuesto hoy.
- Centralizar el mapeo evita lógica condicional dispersa en el componente Razor.

Alternatives considered:
- Pintar directamente `SignalDto` en la vista.
  Rechazado porque complica el markup y no resuelve bien la posible composición con categorías e imágenes.

### 5. Construir la experiencia visual de Signals sobre DaisyUI con ajustes SCSS del módulo

La pantalla `SignalHome` usará DaisyUI como base para cards, inputs, chips y superficies interactivas. La fila de categorías usará un contenedor horizontal con `overflow-x: auto`, `flex-wrap: nowrap` y ocultación discreta del scrollbar donde sea viable. El look final se completará con SCSS del módulo Signals para adaptar spacing, composición y acabados al lenguaje visual ya introducido en otras vistas del proyecto.

Rationale:
- Cumple el requisito visual pedido y alinea Signals con la base DaisyUI que se quiere reutilizar en nuevos diseños.
- Permite aprovechar patrones ya usados en otras vistas sin renunciar a ajustes finos del proyecto en SCSS.

Alternatives considered:
- Resolver toda la pantalla exclusivamente con SCSS artesanal.
  Rechazado porque contradice la directriz actual para nuevos diseños y reduce coherencia con las vistas donde DaisyUI ya se está usando.

### 6. Tratar como opcionales los campos visuales aún no garantizados por el contrato base

La fase 1 asumirá como obligatorio el render de texto, categoría y fechas disponibles. Las imágenes y cualquier metadato destacado deberán mostrarse cuando el backend los proporcione; en caso contrario la card conservará estructura consistente sin romper el layout.

Rationale:
- Evita bloquear la entrega de la primera fase por lagunas del contrato.
- Hace visible el punto de integración con backend sin inventar datos en frontend.

Alternatives considered:
- Bloquear la fase hasta ampliar formalmente el DTO de `signals`.
  Rechazado porque la propuesta pide avanzar ya con la pantalla base y el listado.

## Risks / Trade-offs

- [El DTO actual de `signals` no documenta todos los campos exactos del diseño] -> Mitigación: crear un view-model con fallbacks y dejar explícita la dependencia de backend en la implementación.
- [La obtención de imágenes por endpoint individual puede multiplicar llamadas] -> Mitigación: priorizar imagen embebida si backend la añade al listado y, si no, evaluar carga diferida o placeholder en fase 1.
- [Búsqueda y filtro simultáneos pueden producir una UI nerviosa si cada cambio dispara red] -> Mitigación: aplicar debounce corto en búsqueda y recarga controlada por estado del componente.
- [La nueva pantalla puede quedar aislada si no se define bien la navegación desde Home] -> Mitigación: incluir la ruta y el breadcrumb dentro del propio alcance de esta fase.
- [Añadir un cuarto acceso al footer puede tensionar el espacio disponible en móvil] -> Mitigación: ajustar el layout del `BottomNav` para cuatro items con icono y label sin perder legibilidad.
- [Un orden ambiguo del nuevo destino puede degradar la encontrabilidad] -> Mitigación: fijar contractualmente que `Signals` quede entre `Home` y `Mi Cuenta`, preferentemente antes de `Mi Club`.
- [Combinar DaisyUI con overrides SCSS puede generar solapes visuales] -> Mitigación: usar DaisyUI como capa base y reservar el SCSS del módulo para ajustes concretos y consistentes.

## Migration Plan

1. Añadir la nueva capability de `SignalHome` en OpenSpec.
2. Implementar página, componente, servicio y modelos de presentación.
3. Registrar la ruta de navegación desde la tarjeta `Señales` del dashboard Home y desde el menú footer autenticado.
4. Añadir recursos localizados, iconografía y ajustes visuales del `BottomNav` si son necesarios para cuatro destinos.
5. Implementar la base visual con DaisyUI y añadir los estilos SCSS complementarios del módulo.
6. Validar manualmente la experiencia de búsqueda, chips con scroll horizontal, cards en móvil y la nueva entrada del footer.

## Open Questions

- Confirmar si el listado inicial debe mostrar todas las señales o solo las activas por defecto usando `activo=true`.
- Confirmar si backend va a exponer en breve un campo de imagen/título optimizado para listado o si fase 1 debe componerlo con endpoints auxiliares.
