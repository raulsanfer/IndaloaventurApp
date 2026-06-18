## Why

El proyecto declara Tailwind 4.3 en su contexto, pero la implementación visible del frontend está organizada principalmente alrededor de SCSS modular y CSS compilado, sin un uso significativo de utilidades Tailwind ni de un sistema de componentes asociado. Antes de introducir DaisyUI por su catálogo de componentes gratuitos, conviene evaluar su encaje real y definir una estrategia que no rompa el layout actual ni multiplique paradigmas de estilos sin control.

## What Changes

- Evaluar formalmente el impacto de adoptar DaisyUI frente a continuar con el enfoque actual basado en SCSS modular.
- Definir una estrategia de decisión para el sistema de estilos del frontend, aclarando si Tailwind está realmente activo, residual o pendiente de integración efectiva.
- Proponer una adopción incremental de DaisyUI solo mediante piloto acotado, en lugar de una sustitución completa del layout actual.
- Establecer criterios de aceptación y restricciones para cualquier uso futuro de DaisyUI: compatibilidad con Blazor, coexistencia con SCSS global y coste de refactor sobre layouts existentes.

## Capabilities

### New Capabilities

- `frontend-ui-styling-strategy`: Define la política de adopción o descarte de DaisyUI frente al stack actual, incluyendo alcance permitido, restricciones de convivencia con SCSS y condiciones para un piloto controlado.

### Modified Capabilities

None.

## Impact

- Afecta a la estrategia frontend transversal del proyecto `IndaloaventurApp.Web` y a los componentes reutilizables de `IndaloaventurApp.SharedUI`.
- Puede requerir introducir toolchain real de Tailwind si se decide usar DaisyUI, ya que DaisyUI opera como plugin del ecosistema Tailwind.
- Impacta en el layout y mantenibilidad: una migración total obligaría a revisar clases, componentes y convenciones SCSS ya existentes.
- No cambia APIs backend ni contratos funcionales, pero sí condiciona cómo se diseñarán los próximos componentes UI.
