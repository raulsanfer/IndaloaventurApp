## Context

La app ya cuenta con una pantalla operativa de `Licencias Federativas` para socios autenticados con rol `Member` y `IsMember = true`, incluyendo listado agrupado por `Temporada` y un CTA visual de solicitud. El backend ya expone `GET /api/licencias-federativas/tarifas` para obtener el catálogo de tarifas disponibles y `POST /api/licencias-federativas/me/solicitudes`, cuyo contrato requiere `Temporada` y `TarifaLicenciaFederativaId`, y valida tanto la pertenencia del usuario al área de socios como la unicidad de la solicitud por temporada.

El nuevo flujo necesita cerrar la brecha entre la UI y ese contrato backend: el usuario no conoce el `TarifaLicenciaFederativaId`, por lo que el frontend debe obtener un catálogo de tarifas disponibles, deduplicar la experiencia visible y resolver la tarifa final a partir de la combinación `Temporada` + `Licencia` + `Categoría`. Además, negocio ha fijado que la temporada seleccionable no vendrá inicialmente del API: el modal ofrecerá solo `año actual` y `año actual + 1`, y la consulta de tarifas se hará después de elegir una de esas temporadas. La implementación debe seguir los patrones del proyecto: Razor + clase partial separada, estilos en SCSS global, literales localizados y diseño responsive sin lógica dispersa dentro del `.razor`.

## Goals / Non-Goals

**Goals:**
- Permitir que el socio abra un modal responsive desde `Licencias Federativas` para registrar una nueva solicitud.
- Precargar el combo de `Temporada` con `año actual` y `año actual + 1`, y cargar `Tipología` y `Categoría` desde licencias disponibles de la temporada elegida, ocultando duplicados y guiando la selección de una combinación válida.
- Mostrar `PrecioClub` cuando la selección obligatoria esté completa y usar la tarifa resuelta para confirmar la solicitud.
- Ejecutar el `POST` de creación y recargar inmediatamente el listado del usuario cuando la operación finalice correctamente.
- Mantener estados de carga, error, cancelación y acceso restringido coherentes con la experiencia actual.
- Mostrar un estado vacío controlado cuando una temporada permitida todavía no tenga tarifas publicadas, especialmente para `año actual + 1`.

**Non-Goals:**
- Rediseñar la página completa de `Licencias Federativas` o alterar la agrupación del histórico existente.
- Cambiar el contrato de creación actual del backend (`Temporada` + `TarifaLicenciaFederativaId`).
- Implementar administración de tarifas o edición/cancelación de solicitudes ya creadas.
- Introducir estilos inline o lógica compleja incrustada directamente en el `.razor`.

## Decisions

1. Reutilizar la página actual y convertir el CTA existente en disparador real de un modal de solicitud.
Rationale: evita duplicar navegación o crear una pantalla separada para una acción breve, manteniendo al usuario dentro del contexto del histórico de licencias.
Alternatives considered:
- Navegar a una página nueva de alta: descartado porque añade fricción y rompe el patrón natural de “consultar y actuar” en una misma vista.

2. Precargar `Temporada` localmente con `DateTime.UtcNow.Year` y `DateTime.UtcNow.Year + 1`, y consultar tarifas solo tras seleccionar una temporada.
Rationale: la regla de negocio indica que no se puede solicitar para más de un año futuro ni para un año anterior al actual; fijar esos dos valores en frontend simplifica la UI, evita una llamada inicial innecesaria y garantiza que el modal arranque con un rango permitido.
Alternatives considered:
- Obtener temporadas desde el API: descartado porque para este flujo el rango permitido ya está totalmente acotado por negocio.
- Permitir escritura libre o más años en el selector: descartado porque abriría combinaciones inválidas que el usuario no debe poder intentar.

3. Modelar la carga del resto del formulario desde un catálogo de tarifas disponibles filtrado por temporada y proyectarlo a opciones de UI deduplicadas.
Rationale: el contrato de guardado exige un `TarifaLicenciaFederativaId`, pero la experiencia pedida por negocio se expresa en términos de `Temporada`, `Licencia` y `Categoría`; por tanto, conviene mapear el catálogo filtrado de la temporada elegida y generar conjuntos únicos por nivel.
Alternatives considered:
- Hardcodear opciones en frontend: descartado porque rompería la fuente de verdad del backend y dificultaría futuras temporadas.
- Exponer directamente IDs en la UI: descartado porque no es comprensible para el usuario y empeora la experiencia.

4. Aplicar selección dependiente en cascada y resolver la tarifa final en memoria.
Rationale: una vez elegida `Temporada`, el conjunto de `Licencias` y `Categorías` válidas puede reducirse; mantener la colección filtrada de esa temporada en memoria del modal permite recalcular opciones sin nuevas llamadas y localizar la tarifa concreta antes del `POST`.
Alternatives considered:
- Llamadas remotas por cada combo: descartado por añadir latencia, complejidad y riesgo de estados parciales.

5. Mostrar `PrecioClub` solo cuando los tres campos obligatorios estén resueltos a una tarifa concreta.
Rationale: el precio pertenece a la tarifa final, no a selecciones parciales; retrasar su render hasta tener una combinación válida evita mostrar importes ambiguos.
Alternatives considered:
- Mostrar precio provisional por temporada o licencia: descartado porque puede inducir a error si existen múltiples categorías con importes distintos.

6. Refrescar el listado mediante una nueva lectura tras la creación satisfactoria, cerrando el modal únicamente cuando la operación haya concluido.
Rationale: una recarga real desde el origen garantiza consistencia con el backend y refleja el estado exacto persistido, incluyendo orden, agrupación y validaciones de unicidad por temporada.
Alternatives considered:
- Insertar la nueva solicitud de forma optimista en memoria: descartado porque el backend puede enriquecer o normalizar datos y porque el flujo ya dispone de un `GET` consolidado.

7. Consumir `GET /api/licencias-federativas/tarifas?temporada=<seleccionada>` como fuente de verdad del catálogo para el modal.
Rationale: el contrato ya está documentado en `openspec/endpoints.json`, acepta filtro por `temporada`, devuelve `TarifaLicenciaFederativaDto` y encaja de forma directa con la necesidad de limitar datos al año permitido que haya elegido el usuario.
Alternatives considered:
- Reutilizar el histórico de solicitudes del usuario para poblar combos: descartado porque no representa el catálogo disponible y mezclaría datos operativos con datos de referencia.

8. Tratar una respuesta vacía del catálogo como estado funcional esperado y no como error.
Rationale: puede ocurrir que `año actual + 1` ya sea seleccionable por regla de negocio pero que aún no existan tarifas cargadas para esa temporada; en ese caso la UX debe informar claramente y mantener la interacción bajo control.
Alternatives considered:
- Bloquear la selección del año siguiente hasta tener datos: descartado porque la disponibilidad temporal del selector responde a la regla de negocio, no al estado de carga del catálogo.
- Mostrar un error genérico: descartado porque la ausencia de tarifas no implica fallo técnico.

## Risks / Trade-offs

- [La deduplicación visible puede ocultar que varias tarifas compartan el mismo trío `Temporada` + `Licencia` + `Categoría`] -> Mitigación: definir que la combinación usada para confirmar debe resolver una única tarifa válida; si el catálogo no garantiza unicidad, ampliar la proyección con `Territorio` u otro discriminador antes de implementar.
- [El cálculo del año actual puede variar entre cliente y servidor en cambios de fecha o zona horaria] -> Mitigación: calcular la lista inicial de temporadas con una única referencia temporal consistente en la app y seguir validando en backend cualquier combinación enviada.
- [El usuario puede interpretar como fallo que una temporada permitida no devuelva tarifas] -> Mitigación: mostrar un mensaje explícito indicando que aún no hay tarifas disponibles para esa temporada y mantener inactiva la confirmación.
- [La recarga inmediata tras crear puede hacer más visible la latencia] -> Mitigación: mantener feedback de confirmación/cargando en el modal y refrescar el listado de manera controlada una sola vez.
- [El usuario puede tener ya una solicitud de la misma temporada y recibir error del backend al confirmar] -> Mitigación: anticipar esa restricción en validación/UI cuando el histórico ya contenga esa temporada o mostrar el error backend de forma clara si la carrera se detecta al enviar.
