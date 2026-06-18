# password-recovery-management Specification

## Purpose
TBD - created by archiving change password-recovery-flow. Update Purpose after archive.
## Requirements
### Requirement: El sistema MUST permitir iniciar la recuperacion de contrasena por email
El sistema MUST exponer un endpoint publico de recuperacion de contrasena que reciba el email del usuario y, cuando proceda, genere un token seguro de reseteo usando ASP.NET Identity. La respuesta expuesta al cliente SHALL ser neutra y en espanol, sin revelar si el email existe o no en el sistema.

#### Scenario: Solicitud valida con usuario existente
- **WHEN** un cliente envia un email registrado al endpoint de recuperacion de contrasena
- **THEN** el sistema SHALL generar un token de reseteo y SHALL enviar un correo con un enlace seguro hacia la pagina publica del frontend

#### Scenario: Solicitud con email no registrado
- **WHEN** un cliente envia un email que no existe en Identity al endpoint de recuperacion de contrasena
- **THEN** el sistema SHALL devolver la misma respuesta neutra que para un email existente y SHALL no exponer si la cuenta existe

### Requirement: El correo de recuperacion MUST incluir un enlace seguro y amigable para el frontend
El sistema MUST enviar un correo en espanol con una plantilla amigable que incluya un enlace seguro hacia una pagina publica del frontend para definir una nueva contrasena. El enlace SHALL transportar al menos el email del usuario y el token de reseteo codificado de forma segura para su transporte por URL.

#### Scenario: Correo de recuperacion contiene enlace al frontend
- **WHEN** el sistema prepara un correo de recuperacion para un usuario valido
- **THEN** el correo SHALL incluir una URL publica del frontend configurada y los datos necesarios para completar el reseteo

### Requirement: El sistema MUST permitir confirmar una nueva contrasena mediante token de reseteo
El sistema MUST exponer un endpoint publico para confirmar una nueva contrasena recibiendo email, token de reseteo y la nueva contrasena. El sistema SHALL validar el token con ASP.NET Identity y SHALL aplicar la politica de contrasenas ya configurada.

#### Scenario: Reseteo correcto de contrasena
- **WHEN** un cliente envia email, token valido y una nueva contrasena que cumple la politica
- **THEN** el sistema SHALL actualizar la contrasena del usuario y devolver una respuesta satisfactoria en espanol

#### Scenario: Token invalido o expirado
- **WHEN** un cliente envia un token invalido, alterado o expirado al endpoint de reseteo
- **THEN** el sistema SHALL rechazar la operacion con un error de validacion o conflicto en espanol

#### Scenario: Nueva contrasena invalida
- **WHEN** un cliente envia una nueva contrasena que no cumple la politica de Identity
- **THEN** el sistema SHALL rechazar la operacion y SHALL devolver los mensajes de validacion en espanol

### Requirement: El sistema MUST documentar el contrato de integracion con el frontend para la pagina publica de reseteo
El sistema MUST mantener en `docs/` un markdown de integracion para frontend explicando la URL de destino esperada, los parametros del enlace, los campos de la pantalla publica de reseteo y el payload del endpoint final de confirmacion.

#### Scenario: Documentacion disponible para frontend
- **WHEN** el equipo frontend consulta la documentacion del flujo de recuperacion
- **THEN** el repositorio SHALL ofrecer una guia markdown con instrucciones suficientes para implementar la experiencia publica de reseteo

