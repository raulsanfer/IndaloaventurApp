## MODIFIED Requirements

### Requirement: Administrar tipos de signal
El sistema MUST permitir que un usuario con rol `Admin` cree, edite y elimine registros `Signal_Type` con identificador entero autogenerado, y con campos `Nombre` (cadena) e `Icono` (cadena).

#### Scenario: Alta de tipo de signal correcta con identificador autogenerado
- **WHEN** un usuario con rol `Admin` envía una solicitud válida de creación de `Signal_Type` con `Nombre` e `Icono`
- **THEN** el sistema crea el tipo de señal, genera automáticamente su `Id` y devuelve dicho identificador en la respuesta

#### Scenario: Cliente no puede imponer Id en la creación
- **WHEN** un cliente intenta forzar un `Id` manual al crear un `Signal_Type`
- **THEN** el sistema SHALL ignorar o rechazar ese valor según el contrato API vigente y SHALL mantener la generación automática de `Id`

#### Scenario: Edición de tipo de signal correcta
- **WHEN** un usuario con rol `Admin` envía una actualización válida sobre un `Signal_Type` existente
- **THEN** el sistema actualiza los campos `Nombre` e `Icono` del tipo indicado

#### Scenario: Eliminación de tipo de signal correcta
- **WHEN** un usuario con rol `Admin` solicita eliminar un `Signal_Type` existente sin restricciones de integridad
- **THEN** el sistema elimina el tipo de signal indicado

#### Scenario: Acceso denegado para rol no administrador
- **WHEN** un usuario sin rol `Admin` intenta crear, editar o eliminar `Signal_Type`
- **THEN** el sistema rechaza la operación por autorización insuficiente
