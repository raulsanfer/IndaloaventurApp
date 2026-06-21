## Context

El modulo de licencias federativas ya persiste un catalogo de `TarifaLicenciaFederativa` y expone varios flujos de lectura sobre esas tarifas: el catalogo autenticado para eleccion de licencia, las consultas de autoservicio del propio usuario y las consultas administrativas de solicitudes. Hoy todas esas lecturas asumen un unico precio de club (`PrecioClub`) de temporada completa y un precio opcional para independientes.

El cambio introduce una nueva variante funcional de precio para club en media temporada. Eso afecta a varias capas a la vez: dominio, persistencia, seeds, DTOs de lectura, handlers CQRS y el endpoint del catalogo, que ademas debe poder resolver el precio aplicable a partir de un booleano `MediaTemporada`.

## Goals / Non-Goals

**Goals:**
- Incorporar `PrecioClubMediaTemporada` al modelo persistido de `TarifaLicenciaFederativa`.
- Hacer visible el nuevo importe en los contratos de lectura de licencias y solicitudes que exponen datos funcionales de tarifa.
- Permitir que `GET /api/licencias-federativas/tarifas` acepte `MediaTemporada` opcional y devuelva el precio de club aplicable segun ese contexto.
- Mantener compatibilidad semantica con el significado actual de `PrecioClub` como precio de temporada completa.

**Non-Goals:**
- Cambiar la logica de `PrecioIndependiente` o introducir variante de media temporada para independientes.
- Alterar el flujo de alta de solicitudes para que reciba el booleano `MediaTemporada` en esta iteracion.
- Crear un endpoint nuevo separado para media temporada si el comportamiento puede resolverse dentro de la consulta existente.
- Introducir reglas automaticas para decidir media temporada a partir de fecha actual o temporada activa.

## Decisions

### 1. `PrecioClubMediaTemporada` se almacenara como dato propio de la tarifa
La entidad `TarifaLicenciaFederativa`, su configuracion EF Core y la tabla `TarifasLicenciasFederativas` se ampliaran con un nuevo campo `PrecioClubMediaTemporada`.

Rationale: el importe de media temporada es parte del catalogo oficial de negocio para una combinacion concreta de licencia y categoria. Modelarlo como campo persistido evita reglas derivadas opacas y permite que cualquier lectura reutilice el mismo dato.

Alternativas consideradas:
- Calcular media temporada mediante un porcentaje sobre `PrecioClub`: descartado porque el usuario ha definido un importe propio y no una formula.
- Reutilizar `PrecioClub` y sobrescribirlo segun contexto: descartado porque destruye la distincion entre temporada completa y media temporada en el modelo.

### 2. El nuevo campo sera expuesto en todos los contratos funcionales de tarifa
Los DTOs que hoy devuelven datos de tarifa (`TarifaLicenciaFederativaDto`, `SolicitudLicenciaFederativaDto`, `AdminSolicitudLicenciaFederativaDto`) incluiran `PrecioClubMediaTemporada` como dato adicional.

Rationale: el usuario ha pedido que el nuevo importe se refleje en los flujos de consulta de licencias. Hacer el cambio aditivo en todos los contratos funcionales evita incoherencias entre listados del catalogo, consultas del propio usuario y vistas administrativas.

Alternativas consideradas:
- Limitar el nuevo campo solo al catalogo: descartado porque dejaria incompletas las lecturas de solicitudes que ya exponen detalle de tarifa.
- Crear DTOs distintos solo para media temporada: descartado porque anade complejidad sin un cambio funcional equivalente.

### 3. La consulta del catalogo resolvera el precio aplicable en un campo derivado
`GET /api/licencias-federativas/tarifas` aceptara un parametro booleano opcional `MediaTemporada`, con valor por defecto `false`. La respuesta del catalogo mantendra `PrecioClub` como precio de temporada completa, incluira `PrecioClubMediaTemporada` como dato bruto y anadira un campo derivado `PrecioClubAplicable` que valdra:
- `PrecioClub` cuando `MediaTemporada = false`
- `PrecioClubMediaTemporada` cuando `MediaTemporada = true`

Rationale: esto satisface la necesidad de "devolver las licencias y su precio segun media temporada" sin romper la semantica actual de `PrecioClub`. El cliente obtiene el precio efectivo listo para usar y, a la vez, conserva ambos importes del catalogo.

Alternativas consideradas:
- Sobrecargar `PrecioClub` para que cambie segun `MediaTemporada`: descartado porque hace ambiguo el contrato y complica a clientes que necesiten mostrar ambos importes.
- Crear un endpoint nuevo solo para precios aplicables: descartado porque el cambio cabe en la consulta existente con un parametro adicional.

### 4. Las consultas de solicitudes no resolveran precio dinamico por parametro
Las lecturas de solicitudes del usuario y las administrativas expondran `PrecioClub` y `PrecioClubMediaTemporada`, pero no aceptaran `MediaTemporada` ni calcularan un `PrecioClubAplicable`.

Rationale: esas consultas representan una solicitud ya creada y hoy no tienen un contexto de consulta que deba alterar la semantica del precio. Mantener ambos importes visibles es suficiente para el cliente y evita propagar un parametro transversal a endpoints cuyo objetivo no es recalcular el catalogo.

Alternativas consideradas:
- Propagar `MediaTemporada` a todas las consultas de solicitudes: descartado por ampliar innecesariamente la superficie del API sin requerimiento explicito del usuario.

### 5. La migracion debe incluir esquema y datos del catalogo
El cambio necesitara una migracion que agregue la columna nueva y actualice los seeds del catalogo de tarifas federativas para que cada tarifa tenga un valor valido de `PrecioClubMediaTemporada`.

Rationale: el campo forma parte del modelo persistido y las consultas dependen de que el dato exista para todas las tarifas disponibles.

Alternativas consideradas:
- Introducir la columna como nullable y completar valores en otro cambio: descartado porque dejaria ambigua la consulta de `MediaTemporada`.
- Rellenar temporalmente con `PrecioClub`: solo aceptable como estrategia tecnica de despliegue si no se dispone aun del catalogo definitivo, pero no como comportamiento final esperado.

## Risks / Trade-offs

- [El valor real de media temporada puede no estar disponible para todos los seeds actuales] -> Mitigacion: exigir que la migracion incluya todos los importes definitivos o documentar explicitamente una estrategia transitoria controlada.
- [Ampliar varios DTOs incrementa el alcance del cambio en clientes y pruebas] -> Mitigacion: hacer el cambio de forma aditiva y mantener la semantica actual de campos existentes.
- [Introducir `PrecioClubAplicable` solo en el catalogo puede crear asimetria con DTOs de solicitudes] -> Mitigacion: documentar que el campo derivado responde al parametro `MediaTemporada` del catalogo y que las solicitudes exponen importes brutos.
- [El parametro `MediaTemporada` podria usarse en el futuro para mas reglas de negocio] -> Mitigacion: limitarlo ahora a una resolucion de lectura sin alterar creacion ni estado de solicitudes.

## Migration Plan

1. Agregar la columna `PrecioClubMediaTemporada` a `TarifasLicenciasFederativas`.
2. Actualizar configuracion EF Core, snapshot y datos semilla del catalogo para persistir el nuevo importe en todas las tarifas cargadas.
3. Ampliar DTOs, mappings y queries de lectura para exponer el nuevo campo.
4. Extender `GET /api/licencias-federativas/tarifas` con `MediaTemporada` y `PrecioClubAplicable`.
5. Verificar con pruebas de dominio, aplicacion e integracion que:
   - el nuevo campo se valida y persiste;
   - el catalogo devuelve el precio aplicable correcto;
   - las lecturas de solicitudes incluyen el nuevo importe.
6. En rollback, retirar la nueva columna y revertir contratos solo si no se han desplegado clientes dependientes; si ya hay consumo del nuevo campo, coordinar rollback con versionado de cliente.

## Open Questions

- Confirmar el valor definitivo de `PrecioClubMediaTemporada` para todas las tarifas iniciales de la temporada ya cargada.
- Confirmar si en una fase posterior la creacion de solicitudes debera registrar tambien si la solicitud corresponde a media temporada o solo necesita elegir una tarifa con ambos importes visibles.
