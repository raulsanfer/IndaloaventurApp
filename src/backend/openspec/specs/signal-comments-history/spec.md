# signal-comments-history Specification

## Purpose
TBD - created by archiving change add-signal-comments-history. Update Purpose after archive.
## Requirements
### Requirement: Registrar comentarios simples sobre una signal
El sistema MUST permitir que cualquier usuario autenticado registre comentarios simples sobre una `Signal` existente, almacenando para cada comentario el identificador de la incidencia, el identificador del usuario autor, la fecha y hora del comentario y el texto introducido. El comentario MUST ser de un unico nivel, sin respuestas anidadas y sin fotos o adjuntos.

#### Scenario: Alta valida de comentario en una signal existente
- **WHEN** un usuario autenticado envia una solicitud valida de comentario sobre una `Signal` existente con texto informado
- **THEN** el sistema crea el comentario asociado a la `Signal` y persiste `SignalId`, `UserId`, `FechaComentario` y `Texto`

#### Scenario: Alta rechazada sobre una signal inexistente
- **WHEN** un usuario autenticado intenta comentar una `Signal` que no existe
- **THEN** el sistema rechaza la solicitud por referencia invalida a la `Signal`

#### Scenario: Alta rechazada por contenido no permitido
- **WHEN** un usuario autenticado envia un comentario sin texto valido o intenta incluir estructura anidada o fotos
- **THEN** el sistema rechaza la solicitud por incumplir el contrato de comentario simple

### Requirement: Consultar el historico de comentarios de una signal
El sistema MUST permitir que cualquier usuario autenticado consulte el historico de comentarios de una `Signal` existente mediante una lectura dedicada, devolviendo una coleccion cronologica de comentarios simples con `UserId`, `FechaComentario` y `Texto`.

#### Scenario: Consulta de historico con comentarios existentes
- **WHEN** un usuario autenticado solicita el historico de comentarios de una `Signal` que tiene comentarios registrados
- **THEN** el sistema devuelve la coleccion cronologica completa de comentarios asociados a esa `Signal`

#### Scenario: Consulta de historico sin comentarios
- **WHEN** un usuario autenticado solicita el historico de comentarios de una `Signal` existente que aun no tiene comentarios
- **THEN** el sistema devuelve una coleccion vacia

#### Scenario: Consulta rechazada para signal inexistente
- **WHEN** un usuario autenticado solicita el historico de comentarios de una `Signal` que no existe
- **THEN** el sistema rechaza la solicitud indicando que la incidencia no fue encontrada

