## ADDED Requirements

### Requirement: El histórico del socio MUST identificar la modalidad de cada licencia federativa solicitada
El sistema MUST mostrar en el detalle de cada solicitud de `Licencias Federativas` del socio si la licencia solicitada corresponde a `Temporada Completa` o a `Media Temporada`, además de la temporada asociada.

#### Scenario: Solicitud de temporada completa en el histórico
- **WHEN** el socio visualiza una solicitud existente cuyo dato `MediaTemporada = false`
- **THEN** el sistema MUST mostrar en el detalle de esa solicitud la temporada acompañada de la modalidad `Temporada Completa`

#### Scenario: Solicitud de media temporada en el histórico
- **WHEN** el socio visualiza una solicitud existente cuyo dato `MediaTemporada = true`
- **THEN** el sistema MUST mostrar en el detalle de esa solicitud la temporada acompañada de la modalidad `Media Temporada`

#### Scenario: Nueva solicitud visible con modalidad tras el refresco
- **WHEN** el socio crea correctamente una nueva solicitud y el listado se recarga
- **THEN** el sistema MUST mostrar la nueva solicitud en el histórico indicando tanto la temporada como la modalidad realmente solicitada
