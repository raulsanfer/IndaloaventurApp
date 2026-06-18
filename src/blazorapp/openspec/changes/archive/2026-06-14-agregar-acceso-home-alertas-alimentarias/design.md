## Context

`Alertas Alimentarias` ya dispone de su propia experiencia con página de entrada y listados por categoría, pero `HomeDashboard` todavía no la expone como acceso visible en el punto principal de entrada del usuario. En paralelo, el literal `home_card_activities_title` sigue representando una intención antigua y el bloque de cards de `HomeDashboard.razor` aparece actualmente desactivado en el markup, por lo que este cambio debe reintroducir el acceso correcto sin reabrir decisiones más amplias sobre el resto del dashboard.

La petición es concreta: sustituir el acceso asociado a `Actividades` por uno de `Alertas Alimentarias`, con icono `bi bi-basket2` a la izquierda y el título a la derecha, apuntando a la entrada principal ya existente de alertas alimentarias. No hay cambios de datos, rutas backend ni integración remota; el alcance es de discoverability y composición visual dentro de Home.

## Goals / Non-Goals

**Goals:**
- Hacer accesible `Alertas Alimentarias` desde `HomeDashboard`.
- Sustituir semántica y destino de la antigua tarjeta `Actividades` por el nuevo acceso.
- Mostrar un layout simple y escaneable con icono a la izquierda y título a la derecha.
- Reutilizar la ruta principal existente de `Alertas Alimentarias` sin introducir una categoría por defecto arbitraria.

**Non-Goals:**
- No rediseñar el resto completo de `HomeDashboard`.
- No reintroducir otras tarjetas antiguas del dashboard más allá del acceso solicitado.
- No modificar la funcionalidad interna de `Alertas Alimentarias`, sus listados o su detalle.
- No crear nuevos contratos de API ni nuevas rutas de navegación aparte de reutilizar la ya existente.

## Decisions

### 1. El acceso de Home navegará a la página principal de `Alertas Alimentarias`

El CTA de `HomeDashboard` apuntará a `/alertas-alimentarias`, es decir, a la entrada principal de la funcionalidad que ya presenta las categorías disponibles.

Rationale:
- Evita elegir una categoría por defecto no pedida por el usuario.
- Mantiene coherencia con la arquitectura ya propuesta para `Alertas Alimentarias`, donde la selección de categoría ocurre dentro de la propia funcionalidad.

Alternatives considered:
- Navegar directamente a una categoría concreta como `general`.
  Rechazado porque introduce una prioridad funcional no pedida y oculta el resto de categorías.

### 2. Se reutilizará el hueco conceptual de `Actividades` en vez de añadir una tarjeta nueva

El cambio debe interpretar `sustituir home_card_activities_title` como reemplazo del acceso anterior, no como suma de una tarjeta adicional dentro de Home.

Rationale:
- Respeta la petición exacta del usuario.
- Evita crecer el dashboard con un elemento nuevo cuando el objetivo es reemplazar uno anterior.

Alternatives considered:
- Añadir una cuarta tarjeta o una tarjeta nueva separada para `Alertas Alimentarias`.
  Rechazado porque altera la jerarquía actual de Home sin necesidad.

### 3. La composición visual será icono a la izquierda y título a la derecha

El acceso usará `bi bi-basket2` como icono principal, alineado a la izquierda del contenedor, con el título de `Alertas Alimentarias` alineado a la derecha dentro del mismo row o bloque horizontal.

Rationale:
- Responde literalmente al requisito visual pedido.
- Permite un CTA compacto y reconocible sin depender de texto descriptivo largo.

Alternatives considered:
- Mantener la composición vertical clásica de título y subtítulo.
  Rechazado porque no coincide con la composición solicitada para este acceso.

## Risks / Trade-offs

- [El bloque de cards de Home está hoy comentado] -> Mitigación: reactivar solo la superficie mínima necesaria para el nuevo acceso, sin arrastrar markup obsoleto no validado.
- [Cambiar `Actividades` por `Alertas Alimentarias` reduce visibilidad de un acceso histórico] -> Mitigación: dejar el cambio acotado contractualmente como sustitución explícita, no como eliminación accidental.
- [El acceso puede quedar visualmente inconsistente con `HomeNewsSection`] -> Mitigación: reutilizar el lenguaje visual actual de Home y limitar el SCSS a ajustes locales del CTA.

## Migration Plan

1. Actualizar la spec de `frontend-food-alerts-pages` para añadir la discoverability desde Home.
2. Ajustar `HomeDashboard.razor` para sustituir el acceso de `Actividades` por `Alertas Alimentarias`.
3. Añadir o ajustar los literales y estilos mínimos del CTA en Home.
4. Validar navegación desde Home hasta `/alertas-alimentarias` y composición visual del icono y título.

## Open Questions

- Confirmar si el acceso debe mostrar únicamente icono y título, o si en una iteración posterior volverá a necesitar subtítulo o copy de apoyo.
