## Context

La pantalla `Teléfonos de interés` ya muestra cards y un breadcrumb compacto, pero la estructura actual organiza el contenido más como bloques apilados que como una cuadrícula de fichas homogéneas. La nueva petición busca acercar la experiencia al patrón visual de `Grid Lists` de Tailwind, concretamente a variantes de `Contact cards`, manteniendo el proyecto en Blazor + SCSS global y sin replicar elementos que no aplican aquí, como el avatar o icono del contacto.

El componente actual ya dispone del modelo de datos necesario (`DisplayName`, teléfonos, `Email`, `Direccion`, `Observaciones`), por lo que el cambio afecta principalmente a jerarquía visual, distribución del contenido y comportamiento responsive del grid.

## Goals / Non-Goals

**Goals:**
- Refactorizar el listado para que las fichas se presenten como un grid claro y consistente de tarjetas de contacto.
- Priorizar la información básica de cada contacto en el cuerpo principal de la card: nombre, teléfonos y email cuando exista.
- Mantener compatibilidad con datos opcionales sin impedir una apariencia limpia y compacta.
- Excluir iconos o avatares del patrón final aunque la referencia de Tailwind contemple variantes con retrato.

**Non-Goals:**
- No cambiar la API `agenda-telefonica` ni el modelo `PhonebookContact`.
- No introducir acciones nuevas, filtros, búsqueda ni menús contextuales.
- No reemplazar el breadcrumb ni los estados de carga/error implementados recientemente.
- No migrar estilos a clases Tailwind; la referencia se usa solo como dirección visual.

## Decisions

1. Adoptar un patrón de grid de cards uniforme inspirado en `Grid Lists`, no una réplica literal.
Rationale: permite capturar la claridad visual de la referencia manteniendo coherencia con el sistema SCSS actual y evitando dependencias de Tailwind UI.
Alternatives considered:
- Copiar la referencia más fielmente: descartado porque el proyecto no consume Tailwind UI como fuente directa de componentes.
- Mantener el layout actual y solo retocar espaciados: descartado por insuficiente frente al objetivo explícito de grid.

2. Definir la información básica como cabecera textual + bloque principal de contacto.
Rationale: para este dominio, el valor principal es localizar rápidamente a quién llamar o escribir; por eso `DisplayName`, teléfonos y `Email` deben quedar en la zona principal de la ficha.
Alternatives considered:
- Dar el mismo peso visual a dirección y observaciones: descartado porque recarga cada card y la aleja del patrón de lectura rápida.

3. Mantener los datos opcionales en una zona secundaria dentro de la card cuando existan.
Rationale: se conserva toda la información disponible del contrato sin sacrificar limpieza visual cuando esos campos no están presentes.

4. Excluir avatar/icono de contacto y compensar con tipografía, borde, espaciado y alineación.
Rationale: responde exactamente a la petición y evita introducir un hueco visual o placeholder artificial para un dato inexistente.

## Risks / Trade-offs

- [Las cards pueden perder densidad si se simplifica demasiado] → Mitigar manteniendo visibles teléfonos y email como mínimo, y relegando solo los campos secundarios.
- [Un grid demasiado rígido puede generar alturas desiguales] → Mitigar diseñando tarjetas que toleren ausencia de campos opcionales sin romper alineación general.
- [La referencia de Tailwind incluye elementos no aplicables al dominio] → Mitigar tratándola como inspiración de composición, no como plantilla cerrada.
