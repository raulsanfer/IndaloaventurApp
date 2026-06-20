## Why

El modal de solicitud de licencias federativas aún no permite distinguir entre tarifas de `Temporada Completa` y `Media Temporada`, aunque el backend ya expone esa variante mediante el parámetro `mediaTemporada`. Hace falta incorporar ese selector en la UI para que el socio pueda consultar y solicitar la tarifa correcta sin ambigüedad.

## What Changes

- Añadir al modal de solicitud de licencias federativas un nuevo combo de modalidad con las opciones `Temporada Completa` y `Media Temporada`.
- Establecer `Temporada Completa` como valor inicial del combo, equivalente a `mediaTemporada = false`.
- Hacer que la carga del catálogo de tarifas filtre por `Temporada` y por el valor de `mediaTemporada` seleccionado.
- Ajustar la resolución de opciones visibles y la tarifa seleccionada para que trabajen solo con la variante de temporada elegida.

## Capabilities

### New Capabilities

### Modified Capabilities
- `frontend-member-federative-license-request-flow`: el modal de solicitud pasa a incluir selección explícita entre temporada completa y media temporada, y usa ese valor al consultar tarifas.

## Impact

- Componentes compartidos del flujo de solicitud de `Licencias Federativas`.
- Cliente frontend que consulta `GET /api/licencias-federativas/tarifas`.
- Modelos de tarifas del frontend si todavía no exponen `MediaTemporada`.
- Pruebas del modal de solicitud y del cliente API de licencias federativas.
