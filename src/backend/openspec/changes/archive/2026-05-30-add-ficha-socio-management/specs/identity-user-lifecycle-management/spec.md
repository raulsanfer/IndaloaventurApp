## ADDED Requirements

### Requirement: Identity user MUST soportar vinculo univoco con FichaSocio
El sistema MUST soportar la vinculacion univoca de cada usuario de Identity con una entidad `FichaSocio` mediante `UserId`, preservando la identidad del usuario como fuente de autenticacion/autorizacion.

#### Scenario: Usuario activo con vinculo de ficha
- **WHEN** un usuario de Identity existe en el sistema
- **THEN** el sistema SHALL poder resolver su `FichaSocio` por `UserId` sin ambigüedad