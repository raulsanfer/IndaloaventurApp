## Purpose

Permite que administracion cambie de forma segura la contraseña de una cuenta sin exponer secretos ni conservar sesiones que ya no sean válidas.

## ADDED Requirements

### Requirement: Administrador puede cambiar la contraseña de un usuario
El sistema MUST exponer `PUT /api/users/{userId}/password` exclusivamente a una sesión autenticada con rol `Admin` y reautenticada recientemente. El identificador del usuario objetivo SHALL proceder únicamente del parámetro de ruta `userId` como GUID válido y el administrador podrá elegir incluida su propia cuenta.

#### Scenario: Cambio administrativo correcto
- **WHEN** un administrador reautenticado recientemente envía una contraseña válida para un usuario existente
- **THEN** el sistema SHALL actualizar la credencial mediante el proveedor de identidad y responder `204 No Content` sin incluir secretos, hashes, tokens ni datos de la cuenta

#### Scenario: Petición anónima o sin permisos
- **WHEN** una petición sin autenticación válida o con un rol distinto de `Admin` invoca el endpoint
- **THEN** el sistema SHALL responder respectivamente `401 Unauthorized` o `403 Forbidden` y SHALL mantener la credencial sin cambios

#### Scenario: Reautenticación expirada
- **WHEN** un administrador autenticado invoca el endpoint sin una reautenticación reciente
- **THEN** el sistema SHALL responder `403 Forbidden` sin cambiar la credencial

### Requirement: Entrada de contraseña administrativa es validada sin exponer secretos
El sistema MUST aceptar un cuerpo que contenga solamente `newPassword`, obligatorio, ASCII alfanumérico y de longitud entre 1 y 20 caracteres. El sistema SHALL rechazar propiedades no reconocidas, entradas no válidas y políticas de contraseña de identidad no satisfechas con `400 Bad Request` genérico, sin recortar, normalizar, registrar ni devolver el valor de la contraseña.

#### Scenario: Cuerpo no válido no modifica la credencial
- **WHEN** la solicitud omite `newPassword`, usa símbolos, supera 20 caracteres o contiene propiedades adicionales
- **THEN** el sistema SHALL responder `400 Bad Request` genérico y SHALL mantener la credencial anterior

### Requirement: Cambio revoca las sesiones de la cuenta objetivo
Tras un cambio de contraseña correcto, el sistema MUST actualizar el sello de seguridad de la cuenta objetivo y SHALL invalidar los tokens de acceso emitidos previamente para esa cuenta. Si Identity no acepta la contraseña o no puede persistir el cambio, el sistema SHALL responder con un error seguro y no SHALL dejar una credencial parcialmente actualizada.

#### Scenario: Token emitido antes del cambio queda invalidado
- **WHEN** un usuario presenta un token de acceso emitido antes de que un administrador cambie su contraseña
- **THEN** el sistema SHALL rechazar la solicitud protegida con `401 Unauthorized`

### Requirement: Operación administrativa queda limitada y auditada sin secretos
El sistema MUST limitar intentos por combinación de administrador actor y usuario objetivo con una respuesta genérica `429 Too Many Requests`. El sistema SHALL registrar estructuradamente los intentos exitosos, fallidos y limitados con el identificador del actor, usuario objetivo, resultado, código HTTP, fecha/hora y correlación de petición, y MUST NOT registrar contraseñas, cuerpos, hashes, tokens, cookies ni cabeceras de autorización.

#### Scenario: Exceso de intentos queda limitado
- **WHEN** un administrador supera la frecuencia configurada de cambios contra la misma cuenta objetivo
- **THEN** el sistema SHALL responder `429 Too Many Requests`, SHALL mantener la credencial sin cambios y SHALL registrar el evento sin secretos

#### Scenario: Resultado queda auditado
- **WHEN** un intento de cambio termina correctamente o es rechazado tras alcanzar el flujo de aplicación
- **THEN** el sistema SHALL registrar los identificadores de actor y objetivo, resultado, código HTTP y correlación sin datos de contraseña
