## ADDED Requirements

### Requirement: Opción de alta para usuarios Member no socios
La pantalla `Mi Cuenta` MUST mostrar una opción `Hazte socio` a usuarios autenticados con rol `Member` cuyo claim `IsMember` sea `false`.

#### Scenario: Usuario Member no socio abre Mi Cuenta
- **WHEN** un usuario autenticado con rol `Member` y `IsMember = false` accede a `Mi Cuenta`
- **THEN** el sistema MUST mostrar una opción visible para iniciar el alta como socio

#### Scenario: Usuario que ya es socio abre Mi Cuenta
- **WHEN** un usuario autenticado con `IsMember = true` accede a `Mi Cuenta`
- **THEN** el sistema MUST no mostrar la opción `Hazte socio`

### Requirement: Apertura externa del proceso de alta
La opción `Hazte socio` MUST abrir `https://indaloaventura.com/hazte-socio/` fuera de la navegación interna de la app.

#### Scenario: Usuario pulsa Hazte socio en navegador
- **WHEN** el usuario pulsa la opción `Hazte socio`
- **THEN** el sistema MUST abrir `https://indaloaventura.com/hazte-socio/` en una nueva pestaña del navegador

#### Scenario: Usuario pulsa Hazte socio en app instalada
- **WHEN** el usuario pulsa la opción `Hazte socio` desde la app instalada
- **THEN** el sistema MUST delegar la apertura de la URL en un navegador nuevo o equivalente externo a la app

### Requirement: Coherencia visual en Mi Cuenta
La nueva opción `Hazte socio` MUST mantener el mismo lenguaje visual y jerarquía que el resto de acciones mostradas en `Mi Cuenta`.

#### Scenario: Renderizado del CTA
- **WHEN** la opción `Hazte socio` se muestra en pantalla
- **THEN** el sistema MUST integrarla dentro del layout de `Mi Cuenta` sin romper el ritmo visual de la sección
