## MODIFIED Requirements

### Requirement: Registrar comentarios simples sobre una signal
El sistema MUST permitir que cualquier usuario autenticado registre comentarios simples de texto plano sobre una `Signal` existente, almacenando para cada comentario el identificador de la incidencia, el identificador del usuario autor, la fecha y hora del comentario y el texto introducido. El comentario MUST ser de un unico nivel, sin respuestas anidadas y sin fotos o adjuntos. Antes de persistir el comentario, el sistema SHALL normalizar el texto admitido y MUST rechazar contenido vacio tras normalizacion, caracteres de control no permitidos o texto que supere la longitud maxima definida para comentarios.

#### Scenario: Alta valida de comentario en una signal existente
- **WHEN** un usuario autenticado envia una solicitud valida de comentario sobre una `Signal` existente con texto informado
- **THEN** el sistema crea el comentario asociado a la `Signal` y persiste `SignalId`, `UserId`, `FechaComentario` y `Texto` ya normalizado

#### Scenario: Alta valida con normalizacion de texto
- **WHEN** un usuario autenticado envia un comentario con espacios de borde o finales de linea admitidos
- **THEN** el sistema acepta la solicitud y almacena el comentario con el texto normalizado segun el contrato de comentario simple

#### Scenario: Alta rechazada sobre una signal inexistente
- **WHEN** un usuario autenticado intenta comentar una `Signal` que no existe
- **THEN** el sistema rechaza la solicitud por referencia invalida a la `Signal`

#### Scenario: Alta rechazada por contenido no permitido
- **WHEN** un usuario autenticado envia un comentario sin texto valido, con caracteres de control no permitidos o que supera la longitud maxima
- **THEN** el sistema rechaza la solicitud por incumplir el contrato de comentario simple
