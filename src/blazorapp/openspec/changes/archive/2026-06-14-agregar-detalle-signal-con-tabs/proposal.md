## Why

La pagina de detalle de una signal ya tiene una direccion funcional definida, pero necesita un rediseño visual mas cercano al boceto aprobado para que la lectura sea mas clara, la jerarquia de la informacion sea mas util en movil y el conjunto mantenga el lenguaje visual actual de la aplicacion.

El usuario ha dejado un diseño de referencia en `openspec/design/signal/detail` y ha acotado dos restricciones importantes: el breadcrumb existente debe conservarse tal como funciona hoy y la solucion debe construirse preferentemente con DaisyUI, evitando rediseños apoyados en CSS propio.

## What Changes

- Redefinir la composicion visual de la pagina de detalle de signal siguiendo el boceto de `openspec/design/signal/detail`.
- Mantener el breadcrumb actual sin alterar su estructura, comportamiento ni posicion funcional dentro de la pagina.
- Reorganizar el contenido del detalle con componentes DaisyUI para priorizar cabecera, chips de estado/categoria, bloques de metadatos y tabs de contenido.
- Mantener las dos tabs funcionales del detalle, adaptando su presentacion al nuevo diseño visual.
- Limitar los estilos especificos a los ya existentes en la aplicacion y evitar introducir CSS ad hoc para suplir componentes que DaisyUI ya resuelve.

## Capabilities

### Modified Capabilities
- `frontend-signal-detail-page`: el detalle de signal MUST adoptar el nuevo layout visual basado en el boceto, preservando breadcrumb, estados de carga y estructura por tabs.

## Impact

- `IndaloaventurApp.SharedUI`: ajuste de los componentes Razor y del modelo visual del detalle.
- `IndaloaventurApp.Web`: posible ajuste menor de ensamblado de pagina si el breadcrumb actual vive fuera del componente principal.
- `openspec/design/signal/detail`: pasa a ser la referencia visual explicita de la implementacion.
- `openspec/specs`: actualizacion de la spec del detalle para recoger restricciones visuales y de composicion.
