## Context

El flujo actual de solicitud de `Licencias Federativas` permite elegir `Temporada`, `Tipología` y `Categoría`, y carga las tarifas disponibles filtrando solo por temporada. Mientras tanto, el backend ya distingue variantes de tarifa con el flag `MediaTemporada` y acepta el parámetro `mediaTemporada` en la consulta del catálogo. El frontend todavía no expone esa decisión al usuario, por lo que mezcla en el mismo flujo tarifas de temporada completa y de media temporada.

## Goals / Non-Goals

**Goals:**
- Incorporar una selección explícita entre `Temporada Completa` y `Media Temporada` en el modal de solicitud.
- Hacer que el catálogo de tarifas se consulte siempre con la combinación de `Temporada` y `MediaTemporada`.
- Mantener `Temporada Completa` como valor inicial y como comportamiento por defecto del flujo.
- Garantizar que las opciones visibles de `Tipología`, `Categoría` y la tarifa resuelta pertenecen solo a la modalidad elegida.

**Non-Goals:**
- Cambiar el flujo de creación de la solicitud para enviar un booleano adicional en el alta; la selección seguirá resolviéndose mediante `TarifaLicenciaFederativaId`.
- Rediseñar el histórico de solicitudes ya creadas.
- Modificar el comportamiento del flujo administrativo de licencias federativas.

## Decisions

### 1. Modelar la modalidad como estado propio del modal
Se añadirá un nuevo estado de UI con dos valores permitidos:
- `TemporadaCompleta` → `mediaTemporada = false`
- `MediaTemporada` → `mediaTemporada = true`

Rationale:
- La modalidad condiciona la consulta del catálogo, no solo el texto mostrado.
- Tener un estado explícito simplifica el reseteo del formulario y evita inferencias a partir de los resultados del catálogo.

Alternativas consideradas:
- Inferir la modalidad a partir del nombre de la tarifa o de la categoría. Rechazada porque el backend ya expone `MediaTemporada` como dato funcional y el nombre no es fiable.

### 2. Reconsultar tarifas al cambiar la modalidad cuando ya hay temporada seleccionada
Si el usuario cambia entre `Temporada Completa` y `Media Temporada` después de haber elegido una temporada, el frontend recargará el catálogo con la nueva combinación de filtros y limpiará las selecciones dependientes.

Rationale:
- La modalidad cambia el conjunto válido de tarifas y hace inválidas las selecciones previas de licencia/categoría.
- Forzar la recarga mantiene consistente la resolución de `SelectedRate`.

Alternativas consideradas:
- Mantener las selecciones previas si coinciden por texto. Rechazada porque podría resolver otra tarifa distinta con la misma `Licencia` y `Categoría`.

### 3. Ampliar el contrato frontend de tarifas para incluir `MediaTemporada`
El modelo de tarifa y el cliente API del frontend deben aceptar el flag `MediaTemporada`, aunque el modal filtre por él antes de construir las opciones visibles.

Rationale:
- Alinea el contrato frontend con el API real.
- Facilita validar en pruebas que la respuesta recibida corresponde a la modalidad solicitada.

Alternativas consideradas:
- No mapear `MediaTemporada` en el frontend y confiar solo en el filtro de la URL. Rechazada porque deja el contrato incompleto y reduce capacidad de diagnóstico.

## Risks / Trade-offs

- [El usuario puede cambiar modalidad después de empezar a rellenar el formulario] → Mitigación: limpiar `Tipología`, `Categoría`, tarifa resuelta y mensajes dependientes cuando cambie la modalidad.
- [La UI gana un control extra y aumenta la complejidad visual del modal] → Mitigación: usar un combo simple con valor por defecto ya seleccionado y mantener el orden actual de campos.
- [El backend puede devolver tarifas de ambas modalidades si no se envía bien el filtro] → Mitigación: cubrir con pruebas del cliente API que la query incluya `mediaTemporada=true|false`.

## Migration Plan

1. Añadir el estado y el selector de modalidad al modal de solicitud.
2. Extender el cliente/frontend model para mapear `MediaTemporada` y enviar el filtro en la consulta de tarifas.
3. Ajustar la lógica del modal para recargar y recalcular opciones cuando cambie la modalidad.
4. Actualizar pruebas de componente y de API client para la nueva combinación de filtros.

## Open Questions

- Si el histórico de solicitudes debería mostrar también la modalidad `Media Temporada` en una iteración posterior.
- Si el orden visual definitivo del combo de modalidad debe ir antes o después del selector de `Temporada`; por defecto esta propuesta lo sitúa antes para reflejar que ambos definen el catálogo.
