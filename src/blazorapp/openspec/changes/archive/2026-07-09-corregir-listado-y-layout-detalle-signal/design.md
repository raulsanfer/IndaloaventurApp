## Context

La funcionalidad `Signals` ya dispone de listado y detalle, pero ambos presentan problemas de acabado y una regresión funcional visible. En `SignalHome`, la card contempla imagen principal o placeholder, pero el frontend actual mapea `ImageUrl: null` al construir `SignalCardItem`, lo que fuerza siempre la variante de placeholder aunque exista una foto útil en origen. En `SignalDetail`, el layout sigue una estructura de hero, overview cards y tabs, pero en móvil el resumen de cabecera pesa demasiado, la rejilla `signal-detail__overview-grid` colapsa a una columna y la navegación por tres tabs no cabe con comodidad. El problema se hace visible en viewports de `412px`, donde el espacio consumido por márgenes, paddings o gaps de cada `article` impide sostener dos bloques por fila.

El rediseño pedido no introduce nuevas capacidades de dominio; reorganiza jerarquía visual y secuencia de lectura en componentes ya existentes. La implementación deberá seguir usando Razor parcial + SCSS organizado y apoyarse preferentemente en DaisyUI sin resolver el problema con estilos inline o hacks puntuales.

## Goals / Non-Goals

**Goals:**
- Mostrar la imagen principal real de cada signal en el listado cuando exista.
- Mantener el placeholder solo para signals sin imagen disponible.
- Reducir el protagonismo visual del texto devuelto por `GetHeaderSummary()` en la cabecera del detalle.
- Mantener `signal-detail__overview-grid` en móvil con una densidad mínima de dos cards por fila, incluyendo anchos de `412px` equivalentes a Samsung Galaxy.
- Simplificar tabs del detalle a `Datos de la signal` y `Mapa`.
- Renderizar `Comentarios` fuera de tabs, debajo del bloque tabulado, y dejar `Etiquetas` al final de la página.

**Non-Goals:**
- Rediseñar completamente la identidad visual de `Signals`.
- Añadir edición de comentarios, nuevas acciones sociales o nuevos bloques funcionales en el detalle.
- Alterar el breadcrumb actual o los estados base de carga/error/no encontrado.
- Introducir una dependencia externa nueva para layouts masonry reales con JavaScript.

## Decisions

1. Resolver la imagen principal del listado desde el contrato frontend de signals y no desde lógica visual ad hoc.
Rationale: el fallo actual nace en el mapeo de `SignalApiClient`, no en el markup de la card. La solución debe asegurar que `SignalCardItem.ImageUrl` reciba una URL útil cuando el payload la ofrezca, manteniendo el placeholder solo como fallback legítimo.
Alternatives considered:
- Cambiar solo SCSS o markup del listado: descartado porque no corrige que `ImageUrl` llegue nulo de forma sistemática.
- Forzar una imagen fija por categoría: descartado porque enmascara la regresión y rompe la expectativa de foto principal real.

2. Mantener una solución CSS de alta densidad en móvil para `signal-detail__overview-grid`, con dos columnas mínimas y reducción controlada de gaps, márgenes y paddings auxiliares en cada card.
Rationale: el objetivo del usuario es evitar una pila vertical de cards, especialmente en `412px`, no introducir un masonry con alturas independientes complejas. Un grid de dos columnas con cards compactadas satisface la densidad esperada y encaja mejor con DaisyUI/SCSS existente.
Alternatives considered:
- Integrar un masonry JavaScript completo: descartado por complejidad innecesaria y por depender de scripting adicional para un bloque pequeño.
- Mantener `auto-fit` actual: descartado porque ya está permitiendo una sola columna en móvil.

3. Ajustar primero el espaciado interno y externo de las overview cards antes de permitir una degradación a una sola columna.
Rationale: el problema reportado no es solo el número de columnas; también hay espacio desperdiciado dentro de cada `article` que obliga al colapso prematuro del grid. Compactar spacing preserva legibilidad y cumple la expectativa visual sin trucos específicos por dispositivo.
Alternatives considered:
- Forzar anchos fijos por card: descartado porque puede provocar overflow o recortes en dispositivos más estrechos.
- Reducir solo tipografía manteniendo gaps actuales: descartado porque no libera suficiente ancho útil para sostener dos columnas.

4. Sacar `Comentarios` del sistema de tabs y convertirlo en un bloque fijo posterior al panel tabulado.
Rationale: los tres tabs no caben bien en móvil y `Comentarios` es contenido secundario que se puede leer en flujo continuo sin competir con `Datos` y `Mapa`, que sí son modos alternativos del contenido principal.
Alternatives considered:
- Mantener tres tabs y reducir tipografía o padding: descartado porque sigue tensionando el ancho disponible y empeora usabilidad táctil.
- Convertir también `Etiquetas` en un tab: descartado porque el usuario ha pedido explícitamente moverlas al final.

5. Reordenar el detalle en la secuencia `hero -> tabs (datos/mapa) -> comentarios -> etiquetas`.
Rationale: esa jerarquía separa contenido principal, navegación y contenido complementario con una lectura más estable en móvil y escritorio.
Alternatives considered:
- Mantener etiquetas dentro de `Datos de la signal`: descartado porque ya compite con descripción y bloques de lectura principales.

## Risks / Trade-offs

- [El API puede no exponer aún una URL de imagen principal utilizable] -> Mitigación: dejar el impacto explicitado sobre `SignalApiClient` y el contrato consumido; si falta el dato, habrá que ajustar el DTO/frontend contract en la implementación.
- [Forzar dos cards por fila en móvil puede comprimir demasiado texto largo] -> Mitigación: reducir spacing, márgenes y tipografías auxiliares, pero mantener títulos y valores con truncado o wrap controlado.
- [Mover comentarios fuera de tabs aumenta la longitud total de la página] -> Mitigación: conservar comentarios como bloque posterior y no dentro de la parte superior de la experiencia, donde hoy interfieren con la navegación.
- [Cambiar la estructura del detalle puede requerir ajustar tests visuales/markup existentes] -> Mitigación: actualizar pruebas de componente orientadas a estructura y no a una secuencia obsoleta de tabs.

## Migration Plan

1. Ajustar el contrato frontend o mapeo de `SignalCardItem` para habilitar imagen principal real en listado.
2. Reestructurar `SignalDetailView` para dejar solo dos tabs y recolocar comentarios y etiquetas.
3. Adaptar SCSS de `signals` para la nueva jerarquía, tamaño del resumen y grid denso en móvil, compactando spacing de overview cards.
4. Verificar con tests de componente y revisión manual responsive que listado y detalle conservan estabilidad con y sin imagen, con y sin comentarios, y con tags vacías, incluyendo validación en `412px`.

## Open Questions

- Falta confirmar el nombre exacto del campo de imagen principal disponible en el payload de `signals`, o si será necesario ampliar el DTO frontend consumido.
- Si el contenido de comentarios crece mucho, podría ser útil estudiar paginación o colapsado en una iteración posterior, pero queda fuera de este cambio.
