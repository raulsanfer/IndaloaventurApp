## ADDED Requirements

### Requirement: Acceso del socio a la página de Licencias Federativas
El sistema MUST ofrecer una página de `Licencias Federativas` accesible desde `Mi Club` únicamente para usuarios autenticados con rol `Member` y `IsMember = true`.

#### Scenario: Socio navega desde Mi Club
- **WHEN** un usuario autenticado con rol `Member` y `IsMember = true` pulsa el acceso `Licencias Federativas` en `Mi Club`
- **THEN** el sistema MUST navegar a la pantalla de licencias federativas del propio usuario

#### Scenario: Usuario no socio intenta abrir la ruta
- **WHEN** un usuario con `IsMember = false` usa la app o intenta abrir manualmente la URL de licencias federativas
- **THEN** el sistema MUST no mostrar desde `Mi Club` un acceso operativo a esa página
- **THEN** el sistema MUST resolver un estado no operativo o una redirección consistente con las reglas de acceso de la app

### Requirement: Mi Club MUST mostrar Licencias Federativas como opción del área de miembros
La página `Mi Club` MUST incluir una opción `Licencias Federativas` ubicada debajo de `Teléfonos de interés` y MUST mantener un aspecto visual homogéneo con el resto de opciones del índice.

#### Scenario: Orden de opciones en Mi Club
- **WHEN** un usuario miembro visualiza la página `Mi Club`
- **THEN** el sistema MUST mostrar `Licencias Federativas` debajo de `Teléfonos de interés`

#### Scenario: Consistencia visual del acceso
- **WHEN** la opción `Licencias Federativas` se renderiza en `Mi Club`
- **THEN** el sistema MUST usar una presentación visual similar a `Teléfonos de interés`

### Requirement: Carga del listado propio de licencias federativas
La página de `Licencias Federativas` MUST cargar las solicitudes del usuario autenticado mediante `GET /api/licencias-federativas/me/solicitudes`.

#### Scenario: Carga correcta del listado
- **WHEN** un socio abre la pantalla de licencias federativas
- **THEN** el sistema MUST invocar `GET /api/licencias-federativas/me/solicitudes`
- **THEN** el sistema MUST mostrar las solicitudes devueltas para ese usuario

#### Scenario: Usuario sin solicitudes registradas
- **WHEN** el endpoint devuelve una colección vacía
- **THEN** el sistema MUST mostrar un estado vacío claro y visualmente integrado en la pantalla

### Requirement: Agrupación visual por Temporada
El sistema MUST mostrar el listado en una tabla con filas de grupo ancladas por `Temporada`, y dentro de cada grupo MUST representar cada licencia en formato lista mostrando `Licencia`, `Categoria` y `Ambito/Territorio`.

#### Scenario: Varias temporadas en el listado
- **WHEN** el usuario tiene solicitudes pertenecientes a más de una `Temporada`
- **THEN** el sistema MUST agrupar las solicitudes por `Temporada`
- **THEN** el sistema MUST renderizar una cabecera o fila anclada por cada temporada

#### Scenario: Varias solicitudes en una misma temporada
- **WHEN** una misma `Temporada` contiene varias solicitudes
- **THEN** el sistema MUST mostrarlas dentro del grupo de esa temporada en formato lista
- **THEN** cada elemento MUST incluir `Licencia`, `Categoria` y `Ambito/Territorio`

### Requirement: Botón Solicitar preparado para la siguiente iteración
La pantalla de `Licencias Federativas` MUST mostrar un botón `Solicitar` en la zona superior derecha, manteniendo la coherencia visual de la app aunque el flujo de creación no forme parte de esta iteración.

#### Scenario: Renderizado de la cabecera principal
- **WHEN** la pantalla de licencias federativas se muestra al usuario
- **THEN** el sistema MUST renderizar el botón `Solicitar` en la parte superior derecha del listado
- **THEN** el sistema MUST integrarlo con el mismo lenguaje visual y jerarquía que el resto de acciones principales de la app

#### Scenario: Iteración actual sin flujo de alta implementado
- **WHEN** el usuario visualiza la pantalla en esta iteración
- **THEN** el sistema MUST no requerir todavía la implementación del flujo completo de creación de solicitud para considerar válida esta capability

### Requirement: Estados operativos coherentes en la pantalla
La página de `Licencias Federativas` MUST mostrar estados de carga, vacío y error sin romper la composición principal ni el estilo visual de la app.

#### Scenario: Carga en progreso
- **WHEN** la pantalla inicia la petición del listado
- **THEN** el sistema MUST mostrar un estado de carga integrado en la vista

#### Scenario: Error al recuperar licencias
- **WHEN** falla la carga de `GET /api/licencias-federativas/me/solicitudes`
- **THEN** el sistema MUST informar del error al usuario con un estado controlado y coherente con la UI existente
