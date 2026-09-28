## Purpose

Permite que un administrador restablezca de forma segura la contraseña de cualquier cuenta, incluida la propia, desde la gestión administrativa de usuarios.

## ADDED Requirements

### Requirement: Restablecimiento administrativo de contraseña
El sistema MUST permitir que un usuario autenticado con rol `Admin` establezca una nueva contraseña para cualquier usuario identificado por `userId`, incluida su propia cuenta, mediante una pantalla administrativa dedicada.

#### Scenario: Admin abre el restablecimiento de otra cuenta
- **WHEN** un administrador inicia el cambio desde la ficha de un usuario administrado
- **THEN** el sistema MUST mostrar la identidad legible de la cuenta objetivo, un campo `Nueva contraseña` y la acción `Enviar`

#### Scenario: Admin restablece su propia contraseña
- **WHEN** un administrador inicia el cambio desde su propia ficha administrativa
- **THEN** el sistema MUST permitir completar el mismo flujo sin excluir su identificador de usuario

### Requirement: Validación de entrada de la nueva contraseña
El sistema MUST aceptar en la interfaz únicamente valores alfanuméricos de entre 1 y 20 caracteres para `Nueva contraseña`, y el servicio receptor MUST volver a validar el mismo contrato antes de modificar credenciales.

#### Scenario: Contraseña con caracteres no permitidos o longitud excesiva
- **WHEN** el administrador introduce una contraseña que contiene caracteres no alfanuméricos o supera 20 caracteres
- **THEN** el sistema MUST impedir el envío y comunicar un error de validación controlado sin exponer el valor introducido

#### Scenario: Contraseña válida
- **WHEN** el administrador informa un valor alfanumérico de hasta 20 caracteres y pulsa `Enviar`
- **THEN** el sistema MUST enviar la solicitud al contrato administrativo dedicado de restablecimiento

### Requirement: Operación de backend segura y auditable
El contrato de backend para el restablecimiento MUST exigir autenticación y autorización de rol `Admin`, comprobar la existencia de la cuenta objetivo y usar únicamente las primitivas de identidad de la plataforma para actualizar la credencial. El servicio MUST registrar el evento administrativo sin incluir la contraseña, invalidar o rotar las sesiones de la cuenta objetivo conforme al mecanismo de identidad disponible, y devolver errores genéricos y seguros.

#### Scenario: Solicitud de un usuario no autorizado
- **WHEN** una solicitud al endpoint dedicado no procede de una sesión autenticada con rol `Admin`
- **THEN** el servicio MUST rechazarla sin cambiar credenciales ni revelar información de la cuenta objetivo

#### Scenario: Restablecimiento correcto
- **WHEN** el endpoint recibe una solicitud válida de un administrador para una cuenta existente
- **THEN** el servicio MUST actualizar la credencial mediante el proveedor de identidad, auditar el cambio sin secretos y devolver una respuesta de éxito sin incluir datos de contraseña
