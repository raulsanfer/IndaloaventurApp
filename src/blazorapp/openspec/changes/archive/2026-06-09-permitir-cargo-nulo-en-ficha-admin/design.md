## Context

La app ya resolvió que el campo `Cargo` sea legible para usuarios y editable solo para administradores mediante un `InputSelect` alimentado por el catálogo de cargos. En la ficha administrativa actual existe una primera opción vacía, pero su significado funcional no está definido como parte del contrato de UX ni de la spec, y tampoco queda explícito que el administrador pueda usarla para dejar el cargo en `null`.

El modelo y el payload actuales ya admiten `CargoId` nullable, así que el ajuste es principalmente de comportamiento y claridad: el selector debe ofrecer una opción vacía explícita y el flujo de guardado debe conservar `null` cuando se elige esa opción.

## Goals / Non-Goals

**Goals:**
- Definir que el cargo no es obligatorio en edición administrativa.
- Hacer explícita una opción vacía/“sin cargo” en el selector admin.
- Permitir limpiar un cargo existente y persistir `CargoId = null`.

**Non-Goals:**
- Cambiar las reglas de visibilidad del campo para usuarios `Member`.
- Añadir lógica nueva de validación de cargos en backend.
- Rediseñar el formulario de ficha más allá del selector de cargo.

## Decisions

1. Tratar la opción vacía del selector como la forma canónica de representar “sin cargo asignado”.
Rationale: aprovecha el binding nullable ya existente y evita introducir controles adicionales o acciones de borrado separadas.
Alternatives considered:
- Añadir un botón independiente “Quitar cargo”: descartado por complejidad innecesaria para una acción que ya cabe en el selector.

2. Usar una etiqueta explícita para la opción vacía en modo editable, en lugar de reutilizar el nombre del cargo actual o un texto ambiguo.
Rationale: el administrador debe entender claramente que esa opción elimina la asignación vigente.
Alternatives considered:
- Mantener el texto actual implícito: descartado porque no comunica con suficiente precisión la acción de dejar el valor a `null`.

## Risks / Trade-offs

- [La opción vacía puede confundirse con un error de carga del catálogo] -> Mitigación: usar una etiqueta explícita tipo “Sin cargo” o equivalente y mantener el resto del estado del selector.
- [Un admin puede eliminar un cargo por error] -> Mitigación: mantener el comportamiento de guardado explícito, con feedback de éxito ya existente en la ficha.
