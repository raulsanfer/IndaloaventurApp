## Context

La `BottomNav` actual renderiza texto abreviado como sustituto de iconos, lo que resuelve la estructura de navegación pero no la intención visual mostrada en la referencia adjunta. Además, el proyecto hoy no tiene una librería de iconos cargada globalmente: se usa tipografía `Outfit` y algunos `svg` inline en otras vistas, pero no existe todavía un sistema reutilizable para iconografía de interfaz.

La introducción de `Bootstrap Icons` afecta a varias capas: carga global de assets, representación de iconos en `SharedUI`, estilos de la botonera y criterio de selección de iconos equivalentes. También conviene fijar de antemano qué hacer cuando el catálogo no tenga una equivalencia suficientemente fiel para no bloquear la implementación ni improvisar un icono visualmente erróneo.

## Goals / Non-Goals

**Goals:**
- Incorporar `Bootstrap Icons` al frontend como librería de iconos reutilizable.
- Sustituir las abreviaturas actuales de `BottomNav` por iconos + etiqueta de texto.
- Aproximar la `BottomNav` a la referencia adjunta para `Home`, `Mi Club` y `Mi Cuenta`.
- Dejar un criterio explícito para iconos no localizables con suficiente fidelidad.

**Non-Goals:**
- Reemplazar en esta iteración todos los `svg` inline existentes en el resto de la aplicación.
- Diseñar una librería de iconos propia del proyecto.
- Rediseñar el comportamiento de navegación o las rutas de la `BottomNav`.
- Introducir iconografía de marca o assets personalizados si `Bootstrap Icons` no cubre un caso.

## Decisions

1. `Bootstrap Icons` como sistema base de iconografía
- Decision: cargar `Bootstrap Icons` globalmente y consumir sus clases desde componentes Razor.
- Rationale: evita `svg` inline repetidos, facilita mantenimiento visual y encaja con la necesidad de cambiar iconos con rapidez.
- Alternatives considered:
  - Mantener `svg` inline: rechazado por menor editabilidad y peor reutilización.
  - `Font Awesome`: rechazado por mayor peso y exceso de catálogo para esta necesidad puntual.

2. `BottomNav` debe modelar icono y label por separado
- Decision: actualizar el modelo de item de navegación para que cada entrada defina un identificador de icono y su literal localizado.
- Rationale: separa semántica de navegación y presentación visual, y facilita cambiar iconos sin tocar la estructura del componente.
- Alternatives considered:
  - Mantener `Icon` como texto libre: rechazado porque mezclaría iconos reales y abreviaturas de fallback.

3. Aproximación por equivalencia razonable, no por clon visual exacto
- Decision: buscar en `Bootstrap Icons` equivalentes razonables a la referencia adjunta:
  - `Home`: icono de cohete o despegue si existe equivalencia suficientemente clara.
  - `Mi Club`: icono de grupo/personas.
  - `Mi Cuenta`: icono de usuario/perfil.
- Rationale: la referencia parece mezclar iconografía custom con iconos de personas estándar; el objetivo de esta iteración es una aproximación coherente dentro del set elegido.
- Alternatives considered:
  - Intentar replicar exactamente la referencia con iconos externos o propios: rechazado por ampliar demasiado el alcance.

4. Falta de equivalencia iconográfica debe quedar explicitada
- Decision: si un icono no puede localizarse con fidelidad aceptable en `Bootstrap Icons`, la implementación debe documentar el faltante y avisar al usuario para que aporte el recurso definitivo.
- Rationale: evita cerrar el cambio con un icono engañoso o claramente peor que la referencia.
- Alternatives considered:
  - Elegir cualquier icono aproximado sin avisar: rechazado por riesgo de desalineación visual.

## Risks / Trade-offs

- [La referencia usa un icono custom no disponible en `Bootstrap Icons`] → Mitigacion: documentar el gap y solicitar el icono final al usuario.
- [Introducir una librería global de iconos añade dependencia visual] → Mitigacion: limitar su uso inicial a `BottomNav` y dejar el resto fuera de alcance.
- [El estilo de `Bootstrap Icons` puede no coincidir exactamente con la referencia] → Mitigacion: ajustar tamaño, color, peso visual y espaciado desde CSS para acercar el resultado.
- [Mezcla temporal de `Bootstrap Icons` y `svg` inline en la app] → Mitigacion: aceptar coexistencia en esta iteración y evaluar una migración más amplia solo si aporta valor.

## Migration Plan

- Registrar la librería de `Bootstrap Icons` en la aplicación web para que esté disponible globalmente.
- Actualizar `BottomNav` para usar iconos del set en sus tres entradas actuales.
- Ajustar estilos de la botonera para reflejar composición vertical y estado activo similares a la referencia.
- Si falta un icono equivalente, dejar constancia del hueco antes de dar por cerrada la implementación visual.

## Open Questions

- ¿Quieres que, si el icono de `Home` tipo cohete no existe con suficiente fidelidad en `Bootstrap Icons`, lo sustituyamos por una casa temporal o prefieres dejar pendiente ese item hasta que aportes el recurso?
