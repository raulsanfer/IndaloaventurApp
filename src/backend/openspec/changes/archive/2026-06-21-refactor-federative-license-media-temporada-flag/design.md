## Context

El codigo actual ya introdujo `PrecioClubMediaTemporada` en `TarifaLicenciaFederativa`, en sus DTOs y en varias consultas. Ese enfoque obliga a resolver el precio aplicable en tiempo de lectura y hace que una misma tarifa represente simultaneamente temporada completa y media temporada. Con el nuevo modelo, cada variante comercial debe ser una tarifa distinta para que la seleccion, el historico y las respuestas de consulta sean inequívocos.

## Goals / Non-Goals

**Goals:**
- Representar temporada completa y media temporada como tarifas distintas mediante `MediaTemporada`.
- Mantener `MediaTemporada = false` como valor por defecto para la creacion de tarifas que no informen el nuevo flag.
- Preservar los precios de media temporada ya cargados transformandolos en nuevas filas catalogadas, sin perder historico ni romper solicitudes existentes.
- Exponer `MediaTemporada` en las consultas que devuelven informacion de tarifas para que el cliente pueda diferenciar variantes con la misma licencia y categoria.

**Non-Goals:**
- Redisenar el modelo de `PrecioIndependiente` o introducir nuevas reglas de calculo de precio.
- Cambiar las reglas de negocio de solicitud de licencia distintas de la identificacion de la tarifa seleccionada.
- Automatizar una carga externa nueva del Excel de tarifas mas alla de adaptar las semillas y la migracion actual.

## Decisions

### 1. Sustituir `PrecioClubMediaTemporada` por filas diferenciadas con `MediaTemporada`

Cada fila de `TarifaLicenciaFederativa` representara una unica variante comercial. La tarifa de temporada completa seguira usando `PrecioClub` con `MediaTemporada = false`, y la tarifa de media temporada se persistira como otro registro con `MediaTemporada = true`.

Alternativas consideradas:
- Mantener `PrecioClubMediaTemporada` y solo agregar el booleano. Rechazada porque seguiria habiendo dos variantes de precio en una sola fila y el flag seria redundante.
- Crear una tabla hija de variantes tarifarias. Rechazada por sobredimensionar un caso que puede resolverse con una clave compuesta mas simple.

### 2. Transformar los datos existentes mediante duplicacion controlada

La migracion agregara `MediaTemporada` con valor por defecto `false`, convertira cada valor existente de `PrecioClubMediaTemporada` en una nueva fila con el mismo contenido funcional y `PrecioClub` igual al precio de media temporada, y despues eliminara la columna antigua. Asi se conserva el comportamiento funcional sin recalculo en lectura.

Alternativas consideradas:
- Copiar el valor de `PrecioClubMediaTemporada` sobre `PrecioClub` en la misma fila. Rechazada porque destruiria la tarifa de temporada completa.
- Mantener ambas columnas de forma indefinida. Rechazada porque contradice el objetivo del refactor.

### 3. Incluir `MediaTemporada` en contratos de consulta y unicidad

El indice unico del catalogo pasara a depender de `Temporada`, `Licencia`, `Categoria` y `MediaTemporada`. Las respuestas del catalogo, de autoservicio y administrativas incluiran `MediaTemporada` para que el cliente pueda distinguir filas que por el resto de atributos visibles serian iguales.

Alternativas consideradas:
- No exponer `MediaTemporada` y confiar en el `Id` de la tarifa. Rechazada porque las respuestas funcionales perderian capacidad de explicarse por si mismas.
- Mantener un campo derivado tipo `PrecioClubAplicable`. Rechazada porque el precio correcto quedara determinado por la propia tarifa seleccionada.

## Risks / Trade-offs

- [Migracion de datos duplicando tarifas] -> Mitigar con una migracion determinista que inserte primero las filas `MediaTemporada = true`, recalcule claves si es necesario y solo despues elimine la columna antigua.
- [Contratos API incompatibles con clientes que esperan `PrecioClubMediaTemporada`] -> Mitigar versionando el cambio en OpenSpec, actualizando DTOs de forma coordinada y validando el front que consume las consultas.
- [Solicitudes existentes que apunten a tarifas antiguas] -> Mitigar conservando los registros originales como temporada completa y creando nuevas tarifas para media temporada en lugar de reemplazarlas.
- [Aumento de cardinalidad del catalogo] -> Mitigar manteniendo filtros por temporada y `MediaTemporada`, y un orden determinista en consultas.
