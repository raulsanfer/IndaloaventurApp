## MODIFIED Requirements

### Requirement: Listado completo inicial de usuarios administrativos
La página administrativa de `Usuarios` MUST cargar el listado completo al abrirse y MUST usar `GET /api/users` sin parámetros de filtro cuando no haya criterios efectivos de email ni nombre o apellidos. Un criterio vacío o compuesto únicamente por espacios MUST tratarse como ausente.

#### Scenario: Admin entra en la página de usuarios
- **WHEN** un administrador navega a `Configuración -> Usuarios`
- **THEN** el sistema MUST invocar `GET /api/users` sin query de `email` ni `nombre`
- **AND** el sistema MUST mostrar el listado completo recibido como estado inicial de la página

#### Scenario: No hay usuarios que mostrar
- **WHEN** `GET /api/users` sin filtros devuelve una colección vacía
- **THEN** el sistema MUST mostrar un estado vacío general controlado sin ocultar la capacidad de filtrado

#### Scenario: Ambos campos contienen solo espacios
- **WHEN** un administrador ejecuta la búsqueda con email y nombre vacíos o compuestos solo por espacios
- **THEN** el sistema MUST solicitar `GET /api/users` sin parámetros de filtro
- **AND** el sistema MUST tratar la respuesta como un listado sin filtros

### Requirement: Filtrado administrativo de usuarios por email
La página administrativa de `Usuarios` MUST permitir refinar el listado por email y MUST consultar `GET /api/users` con el email informado como filtro opcional, conservando su normalización actual. El filtro MUST funcionar por sí solo o combinado mediante AND con el criterio opcional `nombre`, y MUST permitir el texto parcial admitido por el API sin exigir un nombre ni una dirección completa.

#### Scenario: Admin aplica un filtro por email
- **WHEN** un administrador informa un email y ejecuta la búsqueda sin nombre efectivo
- **THEN** el sistema MUST invocar `GET /api/users` con el email como query string de filtro y sin `nombre`
- **AND** el sistema MUST reemplazar el listado visible por los resultados recibidos

#### Scenario: Admin busca un fragmento de email
- **WHEN** un administrador informa `club.es` en el campo email, deja vacío el nombre y ejecuta la búsqueda
- **THEN** el formulario MUST permitir el envío
- **AND** el sistema MUST consultar `GET /api/users` con `email=club.es` y sin `nombre`

#### Scenario: El filtro no encuentra coincidencias
- **WHEN** la búsqueda por email no devuelve usuarios
- **THEN** el sistema MUST mostrar un estado vacío específico para el filtro aplicado

## ADDED Requirements

### Requirement: Búsqueda administrativa parcial por nombre o apellidos
La página administrativa MUST ofrecer un campo opcional `Nombre o apellidos` y enviar el texto recortado mediante `nombre` a `GET /api/users`. MUST mostrar la respuesta del backend, que busca texto parcial literal en nombre O apellidos de la ficha vinculada sin distinguir mayúsculas. MUST NOT filtrar localmente ni consultar fichas individuales para buscar.

#### Scenario: Admin busca un fragmento interior de un nombre
- **GIVEN** el API contiene un usuario con ficha `Nombre = Ana Marina`
- **WHEN** un administrador introduce `ari` como nombre, deja vacío el email y ejecuta la búsqueda
- **THEN** el sistema MUST consultar `GET /api/users?nombre=ari`
- **AND** el sistema MUST mostrar el usuario devuelto por el API aunque el texto no sea un nombre completo ni un prefijo

#### Scenario: Admin busca un fragmento de cualquiera de los apellidos
- **GIVEN** el API contiene un usuario con ficha `Apellidos = García Montes`
- **WHEN** un administrador busca `arc` o `ont` en `Nombre o apellidos`
- **THEN** el sistema MUST enviar ese fragmento mediante `nombre`
- **AND** el sistema MUST mostrar el usuario devuelto por coincidir con cualquiera de los apellidos

#### Scenario: Se recortan espacios y se respeta la búsqueda sin distinguir mayúsculas
- **GIVEN** el API contiene un usuario con ficha `Nombre = Ana Marina`
- **WHEN** un administrador introduce `  aRI  ` en el campo de nombre y ejecuta la búsqueda
- **THEN** el sistema MUST enviar `nombre=aRI` sin espacios exteriores
- **AND** el sistema MUST mostrar el usuario devuelto por la búsqueda insensible a mayúsculas del backend

#### Scenario: Nombre vacío no restringe la búsqueda por email
- **WHEN** un administrador aplica un email y deja el nombre vacío o solo con espacios
- **THEN** el sistema MUST omitir `nombre` y aplicar exclusivamente el filtro email

#### Scenario: No hay coincidencias por nombre
- **WHEN** una búsqueda con nombre efectivo devuelve una colección vacía
- **THEN** el sistema MUST mostrar el estado vacío de resultados filtrados
- **AND** el sistema MUST mantener accesibles los campos y la acción de limpiar filtros

#### Scenario: El usuario no tiene ficha vinculada
- **GIVEN** el API contiene un usuario sin ficha cuyo email incluye el fragmento buscado
- **WHEN** se ejecuta una búsqueda por `nombre` y el servidor no devuelve ese usuario
- **THEN** el frontend MUST respetar la respuesta sin añadir coincidencias locales por email
- **AND** el usuario MUST seguir siendo localizable mediante una búsqueda solo por email o el listado completo

#### Scenario: La ficha coincide aunque el usuario no sea socio
- **GIVEN** el API devuelve una ficha coincidente con el nombre buscado para un usuario con `IsMember = false`
- **WHEN** el administrador ejecuta la búsqueda por nombre
- **THEN** el sistema MUST mostrar ese usuario sin exigir condición de socio ni un rol concreto al usuario buscado

### Requirement: Combinación de filtros administrativos
El sistema MUST enviar `email` y `nombre` en una única solicitud cuando ambos criterios sean efectivos. MUST usar la intersección que devuelve el backend, sin fusionar resultados de solicitudes separadas ni aplicar OR entre email y nombre. La presencia de cualquiera de los dos criterios en la petición MUST activar los estados de carga y vacío filtrados.

#### Scenario: Admin combina email y nombre
- **GIVEN** existen usuarios que cumplen solo el email, solo el nombre y ambos criterios
- **WHEN** un administrador informa ambos campos y ejecuta la búsqueda
- **THEN** el sistema MUST invocar una sola vez `GET /api/users` con ambos parámetros codificados
- **AND** el sistema MUST mostrar exclusivamente la respuesta del API con los usuarios que cumplen ambos criterios

#### Scenario: Admin envía mediante Enter
- **WHEN** un administrador pulsa Enter en cualquiera de los campos del formulario
- **THEN** el sistema MUST ejecutar la misma búsqueda que el botón de buscar con ambos criterios actuales

#### Scenario: La combinación no tiene coincidencias
- **WHEN** el API devuelve una colección vacía para ambos filtros combinados
- **THEN** el sistema MUST mostrar el estado vacío filtrado y permitir modificar o limpiar ambos campos

### Requirement: Limpieza explícita de filtros de usuarios
El sistema MUST ofrecer una acción localizada `Limpiar filtros` que vacíe simultáneamente email y nombre, restablezca el estado de filtros aplicados, retire los mensajes anteriores y vuelva a solicitar `GET /api/users` sin filtros. La acción MUST estar disponible tras resultados, búsquedas vacías y errores, y MUST NOT enviar el formulario como una búsqueda filtrada.

#### Scenario: Admin limpia uno o ambos filtros
- **GIVEN** el administrador ha aplicado email, nombre o ambos
- **WHEN** pulsa `Limpiar filtros`
- **THEN** el sistema MUST vaciar ambos campos y solicitar el listado sin parámetros de filtro
- **AND** el sistema MUST reemplazar los resultados por la respuesta completa del servidor

#### Scenario: Admin limpia después de una búsqueda vacía o fallida
- **GIVEN** la última búsqueda filtrada no obtuvo coincidencias o mostró un error
- **WHEN** el administrador pulsa `Limpiar filtros`
- **THEN** el sistema MUST retirar el feedback anterior y solicitar todos los usuarios
- **AND** si no hay usuarios MUST mostrar el estado vacío general

#### Scenario: Falla la recarga al limpiar
- **WHEN** falla la consulta sin filtros iniciada por `Limpiar filtros`
- **THEN** el sistema MUST mantener ambos campos vacíos y el estado sin filtros
- **AND** el sistema MUST mostrar un error controlado que permita repetir la carga o buscar de nuevo

### Requirement: Consulta segura y estado coherente de los filtros administrativos
La búsqueda MUST conservar las restricciones administrativas y la autenticación del cliente existente. Cada criterio MUST codificarse como un valor de query independiente sin introducir parámetros adicionales. El sistema MUST NOT registrar ni persistir los valores de búsqueda o exponer detalles internos en errores. Buscar y limpiar MUST impedir consultas solapadas mientras haya una carga en curso; el estado de resultados MUST corresponder a los criterios enviados.

#### Scenario: El nombre incluye caracteres reservados o Unicode
- **WHEN** se busca el texto `  Muñoz & Ana+?#%_[  `
- **THEN** el valor decodificado de `nombre` MUST ser exactamente `Muñoz & Ana+?#%_[`
- **AND** la solicitud MUST conservar una única clave `nombre`, sin fragmento ni parámetros inyectados

#### Scenario: El filtro intenta añadir un parámetro
- **WHEN** se busca `Ana&email=otro@club.es` como nombre sin informar email
- **THEN** el sistema MUST enviarlo íntegramente como valor de `nombre`
- **AND** la solicitud MUST NOT contener una clave `email` procedente de ese texto

#### Scenario: El API rechaza autenticación o autorización
- **WHEN** una búsqueda con `nombre`, solo o combinado, recibe `401` o `403`
- **THEN** el cliente MUST devolver su error controlado de sesión inválida o acceso denegado respectivamente
- **AND** la interfaz MUST mostrar feedback seguro sin resultados nuevos ni detalles internos de la respuesta

#### Scenario: Fallo del servicio durante una búsqueda por nombre
- **WHEN** ocurre un fallo de red, timeout o respuesta JSON inválida al buscar por nombre
- **THEN** el sistema MUST mostrar un error localizado y recuperable
- **AND** al terminar la petición MUST permitir editar, volver a buscar o limpiar filtros

#### Scenario: Se intenta buscar o limpiar mientras se carga
- **WHEN** una petición de usuarios sigue pendiente y se intenta otra búsqueda o limpieza
- **THEN** el sistema MUST impedir una segunda consulta simultánea
- **AND** al terminar MUST presentar el estado correspondiente a los criterios de la petición enviada
