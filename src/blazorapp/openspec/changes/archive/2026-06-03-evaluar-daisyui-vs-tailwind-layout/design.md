## Context

La documentación del proyecto declara `Tailwind 4.3`, pero el frontend actual se apoya principalmente en parciales SCSS (`wwwroot/scss/`) y en un `app.css` compilado a partir de esa estructura. No se observa una adopción efectiva de utilidades Tailwind ni una base existente de componentes semánticos tipo DaisyUI en los componentes Razor actuales.

DaisyUI es compatible con Tailwind y se distribuye como plugin del ecosistema Tailwind, por lo que su entrada real en el proyecto no sería un simple reemplazo visual: implicaría introducir o consolidar toolchain, convenciones de clase y una segunda forma de componer UI frente al enfoque SCSS ya establecido.

## Goals / Non-Goals

**Goals:**
- Determinar si DaisyUI aporta valor real al proyecto frente al enfoque SCSS actual.
- Documentar la diferencia entre el stack declarado y el stack realmente usado en el frontend.
- Definir una estrategia segura: descarte razonado o piloto controlado, evitando una migración total prematura.
- Establecer restricciones para que cualquier adopción futura no rompa el layout ni la mantenibilidad.

**Non-Goals:**
- No migrar de golpe el frontend completo a DaisyUI.
- No eliminar el sistema SCSS existente en esta iteración.
- No introducir una reescritura global de componentes compartidos sin validar antes su impacto.
- No tratar DaisyUI como sustituto directo de Tailwind, porque DaisyUI depende del propio ecosistema Tailwind.

## Decisions

1. Tratar DaisyUI como opción de capa de componentes sobre Tailwind, no como reemplazo directo del SCSS actual.
Rationale: técnicamente DaisyUI complementa el flujo de Tailwind; no sustituye por sí mismo el sistema modular SCSS hoy dominante.
Alternatives considered:
- Plantear una “migración DaisyUI” total: descartado porque el repo no parte de una adopción fuerte de Tailwind sobre la que DaisyUI pueda encajar sin fricción.

2. Recomendar primero una auditoría/normalización del stack real antes de cualquier adopción.
Rationale: hoy existe una divergencia entre documentación (`Tailwind 4.3`) y práctica real (SCSS compilado), y esa ambigüedad debe resolverse antes de sumar otra abstracción.
Alternatives considered:
- Introducir DaisyUI inmediatamente para futuros componentes: descartado porque consolidaría una mezcla de paradigmas sin contrato claro.

3. Si se aprueba DaisyUI, limitar su entrada a un piloto aislado.
Rationale: permite medir impacto en productividad, HTML, theming y convivencia con SCSS sin comprometer todo el layout.
Alternatives considered:
- Adoptarlo como nuevo estándar desde el primer cambio: descartado por riesgo alto de inconsistencia visual y deuda de migración.

4. Mantener SCSS global como baseline del proyecto hasta que un piloto pruebe lo contrario.
Rationale: es el sistema realmente operativo hoy y el que cumple las reglas del proyecto sobre organización de estilos.

## Risks / Trade-offs

- [Añadir DaisyUI sin Tailwind realmente operativo genera complejidad innecesaria] → Mitigar aclarando primero si el proyecto va a usar Tailwind de forma efectiva o si debe corregirse la documentación.
- [Convivencia SCSS + DaisyUI puede fragmentar el lenguaje visual] → Mitigar restringiendo DaisyUI a un piloto con criterios de salida claros.
- [Migración total futura puede tener coste alto de refactor] → Mitigar evitando compromiso global hasta medir impacto en uno o pocos componentes.
- [Percepción de “componentes gratis” puede ocultar coste de integración] → Mitigar documentando explícitamente impacto en toolchain, clases, theming y mantenimiento.

## Migration Plan

- Auditar el stack frontend actual y documentar si Tailwind está activo o solo declarado.
- Decidir entre dos caminos:
  - mantener SCSS como estrategia principal y no adoptar DaisyUI por ahora, o
  - habilitar un piloto acotado de DaisyUI sobre un componente/página concreta.
- Si se hace piloto, definir criterios de evaluación: velocidad de desarrollo, legibilidad del markup, coherencia con diseño actual y coste de convivencia con SCSS.
- Solo después del piloto decidir si conviene ampliar, limitar o descartar DaisyUI.

## Open Questions

- ¿Quieres DaisyUI como biblioteca complementaria para acelerar algunos componentes nuevos, o estás valorando una sustitución progresiva del sistema actual?
- Si se hace piloto, ¿qué pantalla sería mejor candidata: login, cards simples de dashboard o una vista nueva aún no implementada?
