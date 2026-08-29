## ADDED Requirements

### Requirement: El sistema MUST permitir iniciar el alta de comentario solo a perfiles autorizados
El sistema MUST permitir iniciar el flujo de alta de comentario de una signal unicamente a usuarios autenticados con rol `Admin` o con rol `Member` y claim `IsMember = true`.

#### Scenario: Admin autorizado inicia el flujo
- **WHEN** un usuario autenticado con rol `Admin` visualiza el detalle de una signal y activa `Anadir comentario`
- **THEN** el sistema MUST abrir el flujo de alta de comentario para esa signal
- **AND** el sistema MUST mantener el contexto de la signal abierta

#### Scenario: Member activo autorizado inicia el flujo
- **WHEN** un usuario autenticado con rol `Member` y claim `IsMember = true` visualiza el detalle de una signal y activa `Anadir comentario`
- **THEN** el sistema MUST abrir el flujo de alta de comentario para esa signal

#### Scenario: Usuario no autorizado sin flujo operativo
- **WHEN** un usuario autenticado que no cumple la regla anterior accede al detalle o intenta forzar el alta
- **THEN** el sistema MUST no ofrecer un flujo operativo de creacion de comentarios
- **AND** el sistema MUST mantener el detalle en modo de solo lectura para ese bloque

### Requirement: El flujo de alta MUST usar un popup modal con texto requerido
El sistema MUST abrir el alta de comentario dentro de un popup modal asociado al detalle de signal y MUST exigir un texto no vacio antes de permitir la confirmacion.

#### Scenario: Apertura del modal con controles esperados
- **WHEN** un usuario autorizado inicia el alta de comentario
- **THEN** el sistema MUST mostrar un popup modal con un campo de texto largo y una accion `Confirmar`
- **AND** el sistema MUST permitir cancelar o cerrar el popup sin persistir cambios

#### Scenario: Confirmacion bloqueada con texto vacio
- **WHEN** el usuario autorizado deja el comentario vacio o formado solo por espacios
- **THEN** el sistema MUST mantener no operativa la accion `Confirmar`
- **AND** el sistema MUST indicar que el texto es obligatorio

### Requirement: La confirmacion MUST crear el comentario y refrescar la coleccion asociada
El sistema MUST persistir el comentario usando el endpoint de alta asociado a la signal y MUST refrescar la coleccion de comentarios del detalle despues de una creacion satisfactoria.

#### Scenario: Alta correcta del comentario
- **WHEN** un usuario autorizado confirma un comentario valido
- **THEN** el sistema MUST enviar el nuevo comentario asociado a la signal abierta
- **AND** el sistema MUST cerrar el popup al completarse correctamente la operacion
- **AND** el sistema MUST volver a cargar la coleccion de comentarios para mostrar el nuevo elemento

#### Scenario: Error durante el alta
- **WHEN** falla la validacion remota o la creacion del comentario
- **THEN** el sistema MUST mantener operativo el detalle de signal
- **AND** el sistema MUST informar del error dentro del flujo de alta sin perder el texto introducido por el usuario
