## Context

La pantalla `Teléfonos de interés` ya existe y consume correctamente `GET /api/agenda-telefonica`, pero la cabecera actual apila un enlace de retorno, un eyebrow y un resumen descriptivo que ocupan espacio vertical sin aportar suficiente valor en móvil. Además, aunque cada contacto ya se renderiza dentro de una estructura visual, la lectura rápida de teléfonos y datos complementarios puede mejorar con una separación más marcada y una jerarquía más compacta.

La solución debe respetar los patrones del proyecto: textos desde recursos localizados, estilos en SCSS global y lógica de UI desacoplada en componentes SharedUI.

## Goals / Non-Goals

**Goals:**
- Compactar la navegación superior con un breadcrumb accionable que conserve el retorno a `Mi Club` y muestre el contexto actual `Área del socio`.
- Eliminar la descripción bajo el título para priorizar contenido útil above the fold en móvil.
- Reforzar la separación visual entre contactos mediante cards más claras y agrupación legible de sus datos.
- Mantener intactos el contrato backend, la carga de datos y los estados de loading/error.

**Non-Goals:**
- No cambiar rutas, endpoints ni modelo de datos de `agenda-telefonica`.
- No introducir filtros, búsqueda, acciones extra o edición de contactos.
- No rediseñar la página índice `Mi Club` más allá del enlace de retorno ya consumido desde la pantalla hija.

## Decisions

1. Sustituir el bloque `back link + eyebrow + summary` por un breadcrumb compacto.
Rationale: reduce altura de cabecera y mantiene contexto de navegación sin duplicar información.
Alternatives considered:
- Mantener el enlace actual y recolocar `Área del socio` debajo: descartado porque sigue ocupando dos líneas de cabecera con poco beneficio.
- Usar solo un enlace de retorno sin contexto actual: descartado porque la captura pide reflejar explícitamente `Área del socio`.

2. Mantener el título principal `Teléfonos de interés` como ancla semántica única de la pantalla.
Rationale: el título ya orienta al usuario; el texto descriptivo inferior es redundante una vez existen cards diferenciadas.
Alternatives considered:
- Conservar una descripción abreviada: descartado para maximizar espacio visible en móvil.

3. Presentar cada contacto como card independiente con bloques internos consistentes para teléfonos y metadatos.
Rationale: mejora el escaneo rápido y separa visualmente registros consecutivos sin alterar el contenido mostrado.
Alternatives considered:
- Usar una lista compacta de filas: descartado porque vuelve a mezclar campos y empeora la lectura táctil.
- Añadir iconografía por campo: descartado por no ser necesaria para cumplir el objetivo y por introducir ruido visual adicional.

4. Reutilizar literales y estilos existentes siempre que sea posible, ajustando clases SCSS del módulo `_mi-club.scss`.
Rationale: limita el alcance del cambio y preserva consistencia visual con el resto del área autenticada.

## Risks / Trade-offs

- [Breadcrumb demasiado verboso en pantallas estrechas] → Mitigar con composición en una sola línea flexible o salto controlado sin perder accionabilidad.
- [El rediseño de cards puede aumentar altura por registro] → Mitigar compactando la cabecera y agrupando campos para compensar el espacio consumido.
- [Ambigüedad entre `Mi Club` y `Área del socio`] → Mitigar usando `Volver a Mi Club` como enlace y `Área del socio` como elemento actual no accionable, tal como pide la captura.
