## Context

`SettingsCargoPage` solo actúa como contenedor autenticado de `CargoManagementView`, por lo que el problema de usabilidad se concentra en el componente compartido y en sus estilos SCSS globales. La implementación actual ya usa algunas clases DaisyUI como `breadcrumbs`, `fieldset` y `list`, pero sigue dependiendo de clases personalizadas para la mayor parte de la apariencia, y eso deja el input, las acciones y el ritmo visual en un punto inconsistente con el resto de la aplicación.

La referencia inmediata dentro del proyecto es `Mi Cuenta`, que presenta una jerarquía visual más clara, mejor ritmo vertical y patrones de interacción más legibles. El objetivo aquí no es copiar ese layout, sino alcanzar el mismo nivel de claridad aprovechando mejor DaisyUI y asegurando una adaptación sólida a móvil.

## Goals / Non-Goals

**Goals:**
- Hacer que la gestión de cargos se perciba como una pantalla administrativa utilizable en móvil y escritorio.
- Alinear breadcrumb, títulos, fieldset, input, botones y listado con patrones DaisyUI ya presentes en la app.
- Mejorar el espaciado, anchura y reflujo de la composición para evitar bloques apretados o desalineados.
- Mantener intacto el flujo principal de carga, alta y edición de cargos.
- Cubrir el nuevo layout con tests de componente de alto valor.

**Non-Goals:**
- Introducir una nueva dependencia visual distinta de DaisyUI/Tailwind ya disponibles.
- Cambiar los contratos API o la lógica de negocio del CRUD de cargos.
- Reintroducir el borrado operativo en esta iteración.
- Rediseñar el shell autenticado o la página `Configuración` completa.

## Decisions

1. Reforzar DaisyUI directamente en el markup del componente.
Rationale: el componente ya usa parcialmente DaisyUI; completar ese patrón con `input`, `btn`, `join`, `badge`, `card` y utilidades responsivas reduce CSS ad hoc y mejora consistencia.
Alternatives considered:
- Resolver todo desde SCSS personalizado: descartado porque reproduce el problema actual y desaprovecha el sistema ya elegido.

2. Organizar la pantalla como una columna principal con dos superficies claras: editor superior y listado inferior.
Rationale: es el patrón más robusto para móvil y evita saltos visuales innecesarios en una tarea CRUD simple.
Alternatives considered:
- Volver a un layout de dos columnas: descartado por empeorar la adaptación en pantallas estrechas y por competir con el flujo principal.

3. Usar SCSS global solo para reforzar identidad visual, espaciado y responsive, no para reemplazar componentes DaisyUI.
Rationale: así el HTML conserva semántica y utilidades de DaisyUI, mientras SCSS personaliza la presencia a la identidad del proyecto.

4. Mantener la cobertura de tests en el nivel de componente.
Rationale: permite validar rápidamente breadcrumb, bloques principales y flujos de alta/edición sin depender del shell completo.

## Risks / Trade-offs

- [Un uso más intenso de clases DaisyUI puede chocar con reglas SCSS antiguas] → Mitigación: simplificar selectores heredados y dejar que DaisyUI lleve el peso de inputs, botones y listas.
- [Mejorar el layout sin revisar pantallas vecinas podría generar pequeñas diferencias visuales] → Mitigación: tomar `Mi Cuenta` y `Configuración` como referencia inmediata y mantener la misma paleta/base.
- [Los tests pueden quedar demasiado acoplados al markup] → Mitigación: validar estructuras y señales de UX clave, no cada clase exacta.
