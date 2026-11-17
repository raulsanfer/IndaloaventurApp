## MODIFIED Requirements

### Requirement: El sistema MUST gestionar una solicitud de licencia federativa por usuario y temporada
El sistema MUST disponer de solicitudes asociadas a `UserId`, `Temporada` y una tarifa concreta. Cada solicitud SHALL persistir estado y fechas anulables `FechaInicio` y `FechaFin`; su tipo SHALL derivarse de la tarifa. Un usuario SHALL poder registrar varias solicitudes `Día` en la misma temporada.

#### Scenario: Registro de solicitud valida
- **WHEN** existe un usuario y se registra una solicitud para una temporada con una tarifa válida y las fechas requeridas según su tipo
- **THEN** el sistema SHALL persistir la solicitud vinculada a ese usuario, temporada y tarifa con su cobertura

#### Scenario: Varias solicitudes diarias en una temporada
- **WHEN** un usuario con una solicitud `Día` registra otra para distintas fechas de la misma temporada
- **THEN** el sistema SHALL conservar ambas solicitudes con identificadores y coberturas independientes

### Requirement: El sistema MUST conservar el historico de solicitudes por usuario
El sistema MUST mantener las solicitudes históricas vinculadas a sus tarifas al incorporar nuevas temporadas o migrar los tipos. SHALL conservar identificadores, usuario, temporada, estado y fecha de creación y SHALL no inventar coberturas para solicitudes anteriores a la migración.

#### Scenario: Consulta de solicitudes historicas tras nueva vigencia
- **WHEN** se cargan tarifas de una nueva temporada y existen solicitudes de temporadas anteriores
- **THEN** las solicitudes históricas SHALL seguir existiendo y vinculadas a sus tarifas originales

#### Scenario: Solicitudes históricas después de migrar tipos
- **WHEN** se consulta una solicitud anterior a la migración
- **THEN** SHALL conservar todos sus datos originales, mostrar el tipo obtenido de su tarifa migrada y mantener `FechaInicio` y `FechaFin` nulas

## REMOVED Requirements

### Requirement: El sistema MUST impedir mas de una solicitud por usuario y temporada
**Reason**: La exclusividad global impide solicitar varias licencias `Día` en una temporada.
**Migration**: Sustituirla por exclusividad solo entre tipos no diarios, preservando todas las solicitudes existentes y serializando la validación y el alta para el mismo usuario y temporada.

## ADDED Requirements

### Requirement: El sistema MUST limitar a una las solicitudes no diarias por usuario y temporada
El sistema MUST impedir más de una solicitud de tipo distinto de `Día` para un usuario y temporada, considerando todos los estados. Las solicitudes `Día` SHALL no consumir ese límite y SHALL poder coexistir con una solicitud `Completa` o `Media`.

#### Scenario: Segunda solicitud no diaria
- **WHEN** un usuario con una solicitud `Completa` o `Media` intenta registrar otra de cualquiera de esos dos tipos en la misma temporada
- **THEN** el sistema SHALL rechazarla con conflicto funcional y conservar la solicitud existente

#### Scenario: Solicitud diaria coexistiendo con una completa
- **WHEN** un usuario con una solicitud `Completa` o `Media` registra una solicitud `Día` válida para esa temporada
- **THEN** el sistema SHALL aceptar la solicitud diaria y conservar la no diaria

#### Scenario: Solicitud completa después de una diaria
- **WHEN** un usuario que solo tiene solicitudes `Día` registra una solicitud `Completa` o `Media` válida para la misma temporada
- **THEN** el sistema SHALL aceptar la solicitud no diaria

#### Scenario: Solicitud no diaria cancelada
- **WHEN** ya existe una solicitud no diaria cancelada para un usuario y temporada y se intenta crear otra no diaria
- **THEN** el sistema SHALL rechazar la nueva solicitud, manteniendo el criterio previo que incluye solicitudes canceladas

#### Scenario: Altas no diarias concurrentes
- **WHEN** se reciben simultáneamente dos solicitudes no diarias para un usuario y temporada sin una previa
- **THEN** el sistema SHALL persistir como máximo una y SHALL devolver conflicto funcional para la otra

#### Scenario: Altas diarias concurrentes para fechas distintas
- **WHEN** se reciben simultáneamente dos solicitudes `Día` válidas con fechas distintas para un usuario y temporada
- **THEN** el sistema SHALL poder persistir ambas sin aplicarles la exclusividad de solicitudes no diarias

### Requirement: La solicitud diaria MUST incluir una cobertura válida
Una solicitud con tarifa `Día` MUST informar `FechaInicio` y `FechaFin` como fechas sin hora. El inicio SHALL ser menor o igual al fin y ambas fechas SHALL pertenecer al año indicado por `Temporada`. El tipo SHALL determinarse desde la tarifa persistida, sin aceptar una clasificación alternativa del cliente.

#### Scenario: Licencia para un único día
- **WHEN** se solicita `Día` con inicio y fin iguales y dentro de la temporada
- **THEN** el sistema SHALL aceptar la cobertura

#### Scenario: Licencia para un intervalo de días
- **WHEN** se solicita `Día` con inicio anterior al fin y ambas fechas dentro de la temporada
- **THEN** el sistema SHALL persistir ambas fechas sin modificar su valor

#### Scenario: Falta una o ambas fechas
- **WHEN** se solicita `Día` omitiendo el inicio, el fin o ambos
- **THEN** el sistema SHALL rechazar la solicitud con validación en español y SHALL no persistirla

#### Scenario: Intervalo invertido
- **WHEN** el inicio es posterior al fin
- **THEN** el sistema SHALL rechazar la solicitud con validación en español

#### Scenario: Cobertura fuera de temporada
- **WHEN** alguna fecha de una solicitud `Día` pertenece a un año distinto de `Temporada`
- **THEN** el sistema SHALL rechazar la solicitud con validación en español

### Requirement: Las solicitudes no diarias MUST admitir fechas de cobertura nulas
Las solicitudes `Completa` y `Media` MUST poder crearse con ambas fechas nulas. Si se informa cobertura, SHALL exigirse la pareja completa con inicio menor o igual al fin y ambas fechas dentro de la temporada. El sistema SHALL no rellenar automáticamente fechas a partir del nombre del tipo.

#### Scenario: Solicitud completa o media sin fechas
- **WHEN** se solicita `Completa` o `Media` omitiendo ambas fechas
- **THEN** el sistema SHALL aceptar la solicitud y persistir ambas como nulas

#### Scenario: Cobertura opcional informada
- **WHEN** una solicitud no diaria incluye ambas fechas válidas dentro de la temporada
- **THEN** el sistema SHALL conservar las fechas informadas

#### Scenario: Cobertura opcional incompleta o inválida
- **WHEN** una solicitud no diaria incluye solo una fecha, un intervalo invertido o fechas fuera de temporada
- **THEN** el sistema SHALL rechazar la solicitud con validación en español
