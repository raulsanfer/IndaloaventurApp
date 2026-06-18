## Why

La ficha de socio sigue mostrando `CargoId` como un valor numérico, lo que no resulta útil ni para usuarios `Member` ni para administradores. Además, hoy no existe una forma guiada de asignar o corregir el cargo de una ficha usando el catálogo real de cargos disponible en la aplicación.

## What Changes

- Ajustar `MemberSelfProfileView` para que el campo de cargo no exponga el identificador numérico directamente.
- Permitir que un usuario autenticado con perfil `Admin` vea y edite el cargo mediante un selector alimentado desde el catálogo de cargos.
- Hacer que un usuario autenticado con perfil `Member` vea el cargo con su nombre legible, pero en modo no editable.
- Aplicar el mismo criterio al flujo administrativo de edición de ficha (`AdminMemberProfileView`) para mantener consistencia entre fichas de socio.
- Añadir el manejo de carga y resolución del nombre de cargo cuando la ficha solo dispone de `CargoId`.

## Capabilities

### New Capabilities
- `frontend-member-file-cargo-selection`: Gestiona cómo se visualiza y edita el cargo dentro de la ficha de socio según el rol autenticado y usando el catálogo de cargos del API.

### Modified Capabilities
- None.

## Impact

- Afecta a `IndaloaventurApp.SharedUI` en los componentes `MemberSelfProfileView` y `AdminMemberProfileView`, junto con sus modelos y lógica partial.
- Reutiliza el cliente existente del catálogo de cargos (`GET /api/cargos`) para obtener opciones y resolver descripciones legibles.
- Puede requerir extender servicios o contratos de UI para exponer la lista de cargos al flujo de ficha de socio.
