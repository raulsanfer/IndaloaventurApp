## Context

El flujo de `Licencias Federativas` del socio ya distingue en el alta entre `Temporada Completa` y `Media Temporada` al consultar tarifas, pero el histórico renderiza cada solicitud únicamente con el literal `Temporada {año}`. En el frontend actual, el modelo `FederativeLicenseRequest` no expone `MediaTemporada`, y el cliente `FederativeLicenseApiClient` tampoco lo mapea desde `GET /api/licencias-federativas/me/solicitudes`, por lo que la vista no puede construir un detalle más preciso.

## Goals / Non-Goals

**Goals:**
- Hacer que cada solicitud del histórico del socio informe si corresponde a `Temporada Completa` o `Media Temporada`.
- Ampliar el contrato frontend del histórico para mapear `MediaTemporada` desde el API.
- Mantener la presentación coherente con el estilo textual actual del detalle de cada licencia.

**Non-Goals:**
- Rediseñar el layout general de `Licencias Federativas`.
- Cambiar el comportamiento del flujo administrativo de licencias.
- Alterar el proceso de alta o la resolución de tarifas en el modal de solicitud.

## Decisions

### 1. Extender el modelo del histórico del socio con `MediaTemporada`
`FederativeLicenseRequest` pasará a incluir un booleano `MediaTemporada`, y `FederativeLicenseApiClient` lo mapeará desde la respuesta de solicitudes del socio.

Rationale:
- La modalidad es un dato propio de la solicitud ya creada, no una inferencia visual.
- Evita reconstruir la modalidad a partir del texto de licencia o de la tarifa resuelta en otro momento.

Alternativas consideradas:
- Inferir la modalidad desde la temporada o desde el nombre de licencia. Rechazada porque ambos datos pueden coincidir entre variantes y no son una fuente fiable.

### 2. Mostrar una etiqueta de temporada enriquecida en la vista del socio
La vista `FederativeLicensesView` dejará de mostrar sólo `Temporada {año}` y pasará a construir un texto que incluya también la modalidad, por ejemplo `Temporada 2026 · Media Temporada`.

Rationale:
- Reutiliza el punto visual donde el usuario ya busca la información temporal de su solicitud.
- Minimiza el cambio visual al no añadir nuevos bloques ni metadatos redundantes.

Alternativas consideradas:
- Añadir una badge independiente junto al estado. Rechazada porque compite visualmente con el estado operativo y añade ruido en cada fila.

### 3. Introducir literales localizados específicos para modalidad y formato combinado
Los textos de `Temporada Completa`, `Media Temporada` y el formato combinado del detalle se resolverán mediante recursos.

Rationale:
- Mantiene la estrategia del proyecto de centralizar literales en recursos.
- Evita concatenaciones rígidas o textos hardcodeados en el componente.

Alternativas consideradas:
- Formar el texto completo inline en C#. Rechazada porque dificulta ajustes posteriores de copy.

## Risks / Trade-offs

- [El API de solicitudes del socio puede no estar devolviendo todavía `MediaTemporada`] → Mitigación: reflejar explícitamente en tareas la validación y el mapeo del DTO; si faltase en backend, el cambio quedará bloqueado hasta alinear contrato.
- [El nuevo texto puede ocupar más espacio en pantallas estrechas] → Mitigación: reutilizar el bloque ya existente y mantener un formato breve, sin introducir una línea extra obligatoria salvo que el CSS actual la envuelva de forma natural.
- [La modalidad puede quedar incoherente entre histórico y modal si no se usa el mismo vocabulario] → Mitigación: reutilizar las mismas claves de recursos o una nomenclatura alineada con el modal.

## Migration Plan

1. Extender el modelo frontend y el DTO del cliente de solicitudes del socio para mapear `MediaTemporada`.
2. Añadir los literales necesarios para modalidad y/o formato de detalle.
3. Actualizar `FederativeLicensesView` para renderizar la temporada con modalidad.
4. Cubrir con pruebas de cliente y componente que el histórico muestra la modalidad correcta.

## Open Questions

- Si el backend del endpoint `me/solicitudes` ya expone `MediaTemporada` con el mismo nombre exacto que el catálogo de tarifas.
- Si el copy final debe ser una sola cadena combinada (`Temporada 2026 · Media Temporada`) o una temporada con subtítulo separado.
