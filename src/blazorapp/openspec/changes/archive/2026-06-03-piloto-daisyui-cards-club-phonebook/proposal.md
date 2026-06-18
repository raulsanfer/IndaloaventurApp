## Why

La pantalla `ClubPhonebookView` muestra hoy los datos de contacto con separación insuficiente entre registros y con una composición demasiado plana, por lo que el listado no transmite visualmente un formato claro de ficha independiente. Esta vista es un buen candidato para validar si un patrón `card` inspirado en DaisyUI puede mejorar el escaneo del contenido manteniendo intacto el nombre actual del contacto y reforzando la presentación de teléfono y email justo debajo.

## What Changes

- Aplicar un piloto de DaisyUI únicamente sobre los items de la lista de fichas de contacto en `ClubPhonebookView`.
- Refactorizar cada elemento del listado para que se perciba como una card independiente, con borde sencillo y separación visual clara respecto al resto.
- Mantener el nombre del contacto con la jerarquía actual y reorganizar debajo los datos de teléfono y email con una presentación más elaborada.
- Medir la convivencia entre el nuevo patrón de cards y el sistema actual basado en SCSS global.
- Dejar explícito que este cambio no implica adoptar DaisyUI para el resto del frontend todavía.

## Capabilities

### New Capabilities

None.

### Modified Capabilities

- `frontend-club-phonebook-page`: cambia el patrón visual de las fichas de contacto para probar una implementación basada en cards de DaisyUI dentro de `ClubPhonebookView`.

## Impact

- Afecta a `IndaloaventurApp.SharedUI/Components/Club/ClubPhonebookView`.
- Requiere revisar cómo convivirían clases DaisyUI/Tailwind con los estilos SCSS actuales del módulo `Mi Club`.
- Puede requerir activar o consolidar soporte real de Tailwind/DaisyUI en el frontend si aún no está operativo.
- No cambia APIs, modelos de datos ni la lógica de carga de contactos.
