## Context

La pagina de detalle de signal ya esta definida funcionalmente dentro del change, con carga por identificador y dos tabs principales. El nuevo trabajo no cambia ese alcance funcional base, pero si redefine la composicion visual para alinearla con el boceto ubicado en `openspec/design/signal/detail`.

El proyecto usa Tailwind y DaisyUI sobre componentes Razor compartidos. Ademas, `openspec/project.md` fija una restriccion general: los estilos propios deben vivir en SCSS global organizado, no inline ni embebidos a nivel de componente. En esta propuesta, la prioridad es resolver el layout con utilidades y componentes DaisyUI existentes, minimizando CSS nuevo.

## Goals / Non-Goals

**Goals:**
- Aplicar al detalle de signal un layout equivalente al boceto de referencia.
- Mantener el breadcrumb actual exactamente como patron de navegacion persistente del detalle.
- Reforzar la jerarquia visual de cabecera, estado, metadatos y tabs usando DaisyUI.
- Garantizar que el detalle siga funcionando bien en movil y escritorio sin depender de CSS artesanal.

**Non-Goals:**
- No rediseñar el breadcrumb ni sustituirlo por una cabecera alternativa con boton de volver.
- No introducir un sistema de estilos paralelo al design system actual.
- No ampliar el alcance funcional con nuevas acciones de negocio, edicion o flujos adicionales.
- No reemplazar la estructura de tabs por acordeones, secciones infinitas o modales.

## Decisions

### 1. Mantener el breadcrumb existente fuera del rediseño

El breadcrumb actual forma parte de la navegacion ya aceptada en la aplicacion y el usuario ha pedido conservarlo. La implementacion del rediseño debe tratarlo como una pieza fija: se puede reacomodar el contenido que aparece debajo, pero no cambiar el breadcrumb ni su logica.

Alternativas consideradas:
- Sustituir el breadcrumb por una cabecera del mock con icono de volver: descartado porque contradice el requisito explicito del usuario.
- Duplicar breadcrumb y boton de volver: descartado porque introduce ruido y jerarquia redundante.

### 2. Resolver la estructura visual principalmente con DaisyUI

La pagina debe componerse con primitives ya presentes o faciles de mapear a DaisyUI: `tabs`, `card`, `badge`, `stats`, `alert`, `loading`, `divider` y superficies equivalentes. Esto reduce CSS especifico, mantiene consistencia y acelera una implementacion reversible.

Alternativas consideradas:
- Recrear el mock con contenedores y clases SCSS ad hoc: descartado salvo para ajustes minimos no cubiertos por DaisyUI.
- Copiar el HTML del boceto tal cual: descartado porque no respeta la arquitectura Razor ni el sistema de estilos del proyecto.

### 3. Reorganizar la informacion en tres bandas visuales

Debajo del breadcrumb, el detalle se distribuira en:

1. Una cabecera de signal con titulo, chips de categoria/estado y metadato temporal principal.
2. Un bloque resumen tipo card con los metadatos mas relevantes en disposicion compacta.
3. La zona de tabs para `Datos de la signal` y `Mapa/Ubicacion`.

Esta estructura respeta la idea del boceto y a la vez preserva la funcionalidad ya definida en la spec actual.

Alternativas consideradas:
- Mezclar resumen y tabs en una sola card larga: descartado porque pierde jerarquia.
- Llevar el mapa al encabezado: descartado porque compite con la lectura del contenido principal.

### 4. Mantener los estados del detalle dentro de la misma gramatica visual

Los estados de carga, error, no encontrado y ausencia de coordenadas deben renderizarse con componentes DaisyUI consistentes con el nuevo layout, sin pantallas visualmente desconectadas del detalle.

Alternativas consideradas:
- Usar placeholders genericos fuera del layout: descartado porque rompe continuidad visual.

## Implementation Outline

1. Revisar la pagina y componentes actuales de detalle para identificar donde se renderiza hoy el breadcrumb y congelar esa pieza.
2. Ajustar la composicion principal del detalle para introducir:
   - cabecera con titulo y badges;
   - card resumen con metadatos;
   - tabs DaisyUI ya previstas por el change.
3. Reutilizar componentes DaisyUI o wrappers existentes antes de crear nuevas clases SCSS.
4. Si algun ajuste visual no sale solo con DaisyUI/utilidades, limitar SCSS a layout estructural pequeno y compartible.
5. Validar que los estados especiales siguen encajando en la misma pagina sin romper el diseño.

## Risks / Trade-offs

- [El boceto incluye detalles visuales mas libres que DaisyUI puro] -> Puede hacer falta algun ajuste menor de layout, pero se debe evitar convertirlo en CSS bespoke.
- [El breadcrumb actual puede imponer una separacion o ancho distinto al mock] -> Se prioriza mantener breadcrumb intacto aunque obligue a adaptar el resto del layout.
- [El detalle actual puede tener componentes internos ya muy acoplados] -> Conviene rediseñar por composicion y no por reescritura completa para limitar regresiones.

## Validation Plan

- Verificar visualmente que el breadcrumb sigue presente y sin cambios funcionales.
- Verificar que la cabecera, resumen y tabs se corresponden con el boceto de `openspec/design/signal/detail`.
- Verificar que `Datos de la signal` y `Mapa/Ubicacion` siguen accesibles en movil y escritorio.
- Verificar que no se introduce CSS inline ni estilos de componente fuera de la convencion global del proyecto.

## Open Questions

- Si el detalle actual ya usa algun wrapper interno para cards o badges, durante la implementacion convendra decidir si se reutiliza tal cual o se alinea con DaisyUI de forma mas directa.
