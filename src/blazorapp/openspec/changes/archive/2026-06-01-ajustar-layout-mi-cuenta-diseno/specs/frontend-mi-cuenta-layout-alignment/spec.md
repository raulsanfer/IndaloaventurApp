## ADDED Requirements

### Requirement: Estructura visual de Mi Cuenta alineada al screen de referencia
El sistema MUST renderizar `MyAccountView` con la misma estructura de bloques visible en `openspec/design/mi_cuenta/screen.png`.

#### Scenario: Render de bloques principales
- **WHEN** el usuario abre la pantalla `Mi Cuenta`
- **THEN** el sistema MUST mostrar bloque de perfil superior, tarjeta de cargo (si aplica), sección `MIS DATOS`, sección `AJUSTES Y SOPORTE` y botonera inferior

### Requirement: Sección MIS DATOS con enlaces específicos del diseño
El sistema MUST incluir en la sección `MIS DATOS` los accesos `Ficha Socio` y `Licencias Federativas` con estilo de lista del diseño.

#### Scenario: Visualización de enlaces de datos
- **WHEN** se renderiza `Mi Cuenta` para un usuario con `IsMember = true`
- **THEN** el sistema MUST mostrar `Ficha Socio` y `Licencias Federativas` como ítems de la lista `MIS DATOS`

#### Scenario: Ocultación de enlaces de socio para no miembro
- **WHEN** se renderiza `Mi Cuenta` para un usuario con `IsMember = false`
- **THEN** el sistema MUST ocultar `Ficha Socio` y `Licencias Federativas`

### Requirement: Visibilidad condicional del bloque Cargo
El sistema MUST mostrar la tarjeta/badge de Cargo únicamente cuando `IsMember = true` y la ficha de socio indique cargo; en cualquier otro caso, ese bloque MUST ocultarse.

#### Scenario: Usuario miembro con cargo
- **WHEN** el usuario tiene `IsMember = true` y el perfil del miembro tiene cargo informado
- **THEN** el sistema MUST renderizar el bloque de Cargo

#### Scenario: Usuario no miembro
- **WHEN** el usuario tiene `IsMember = false`
- **THEN** el sistema MUST no mostrar el bloque de Cargo

#### Scenario: Usuario miembro sin cargo
- **WHEN** el usuario tiene `IsMember = true` y el perfil del miembro no tiene cargo
- **THEN** el sistema MUST no mostrar el bloque de Cargo

### Requirement: Ajustes y soporte con cierre de sesión operativo
El sistema MUST mostrar `Configuración`, `Ayuda` y `Cerrar Sesión` en la sección `AJUSTES Y SOPORTE`, y la acción `Cerrar Sesión` MUST continuar operativa.

#### Scenario: Cierre de sesión desde Mi Cuenta
- **WHEN** el usuario pulsa `Cerrar Sesión`
- **THEN** el sistema MUST cerrar sesión local y redirigir al login

### Requirement: Botonera inferior con Mi Cuenta activa
El sistema MUST representar la botonera inferior con tres elementos (`Home`, `Mi Club`, `Mi Cuenta`) y marcar `Mi Cuenta` como activo en esta ruta.

#### Scenario: Estado activo de navegación
- **WHEN** el usuario está en `/mi-cuenta`
- **THEN** el sistema MUST mostrar `Mi Cuenta` en estado visual activo

### Requirement: Exclusión de bloques no presentes en el screen
El sistema MUST no renderizar en `Mi Cuenta` bloques adicionales ausentes en la referencia visual actual.

#### Scenario: Validación de contenido visible
- **WHEN** se compara la pantalla renderizada con `screen.png`
- **THEN** el sistema MUST omitir módulos no visibles en el diseño objetivo (por ejemplo, bloque de puntos o tarjetas extras)
