## Why

La presentación actual de las fichas de contacto ya separa la información por card, pero no aprovecha un patrón de grid más claro y reconocible para escaneo rápido, especialmente cuando hay varios registros en pantalla. Tomar como referencia el estilo de `Grid Lists` de Tailwind permite ordenar mejor la información básica y modernizar la lectura visual sin cambiar la fuente de datos.

## What Changes

- Refactorizar el layout de `Teléfonos de interés` para mostrar las fichas de contacto en un grid visual inspirado en los bloques `Grid Lists` y `Contact cards` de Tailwind.
- Reorganizar el contenido de cada ficha para destacar la información básica del contacto en una estructura más compacta y consistente.
- Excluir explícitamente el avatar o icono de contacto de ese patrón visual, manteniendo una tarjeta limpia basada solo en texto y metadatos.
- Mantener la carga desde `GET /api/agenda-telefonica`, el breadcrumb actual y los estados de carga/error existentes.

## Capabilities

### New Capabilities

None.

### Modified Capabilities

- `frontend-club-phonebook-page`: cambia el layout de las fichas de contacto para renderizarlas como un grid de tarjetas de información básica inspirado en Tailwind, sin icono de contacto.

## Impact

- Afecta a la UI de `Teléfonos de interés` en `IndaloaventurApp.SharedUI`.
- Requiere ajustar los estilos SCSS globales del módulo de `Mi Club` / agenda telefónica.
- Puede requerir ajustar textos auxiliares o labels visibles de las fichas si el nuevo layout necesita secciones más compactas.
- No cambia endpoints, contratos backend ni navegación principal de la pantalla.
