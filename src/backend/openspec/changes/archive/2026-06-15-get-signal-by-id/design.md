## Context

La API de `signals` ya expone tres consultas diferenciadas: busqueda con filtros (`GET /api/signals`), imagenes (`GET /api/signals/{id}/images`) y comentarios (`GET /api/signals/{id}/comments`). El front necesita una consulta adicional para cargar la informacion principal de una signal concreta sin depender del listado de busqueda ni de las consultas auxiliares.

El repositorio ya dispone de `GetByIdAsync`, por lo que no es necesario introducir cambios de persistencia ni nuevos modelos de dominio. Tambien existe `SignalDto`, que ya representa exactamente los campos principales que se devuelven en la busqueda.

## Goals / Non-Goals

**Goals:**
- Exponer `GET /api/signals/{id}` para usuarios autenticados.
- Reutilizar `SignalDto` como contrato de respuesta para evitar duplicidad entre listado y detalle principal.
- Devolver `404` cuando la signal no exista.

**Non-Goals:**
- Incluir imagenes binarias en la respuesta principal de detalle.
- Incluir comentarios en la respuesta principal de detalle.
- Cambiar el comportamiento de `GET /api/signals` ni su forma de filtrado.

## Decisions

- Se anadira una nueva query `GetSignalById` en Application y una nueva accion en `SignalsController`.
  Rationale: mantiene la separacion CQRS ya presente y deja el contrato HTTP alineado con el resto de operaciones de `signals`.
  Alternative considered: reutilizar `SearchSignals` filtrando por `Id`. Se descarta porque `SearchSignals` no contempla ese filtro y mezclar busqueda y detalle haria menos explicito el contrato del front.

- La respuesta reutilizara `SignalDto`.
  Rationale: el front necesita la informacion principal ya disponible en el listado, y reutilizar el DTO evita crear dos contratos casi identicos.
  Alternative considered: crear un `SignalDetailDto`. Se descarta por ahora porque no hay campos adicionales respecto al DTO actual.

- La obtencion se apoyara en `ISignalRepository.GetByIdAsync`.
  Rationale: el repositorio ya ofrece esta operacion y evita ampliar innecesariamente la abstraccion.
  Alternative considered: anadir un metodo especifico para detalle. Se descarta porque no aporta valor con el modelo actual.

## Risks / Trade-offs

- [Acoplamiento entre listado y detalle] -> Mitigacion: reutilizar `SignalDto` solo mientras ambos necesiten el mismo bloque de datos; si el detalle crece, se podra introducir un DTO especifico en un cambio posterior.
- [Duplicidad parcial con consultas auxiliares] -> Mitigacion: mantener imagenes y comentarios fuera del detalle principal para que cada endpoint conserve una responsabilidad clara.
