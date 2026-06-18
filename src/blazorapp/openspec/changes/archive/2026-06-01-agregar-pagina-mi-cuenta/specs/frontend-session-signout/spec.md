## ADDED Requirements

### Requirement: Acción de cerrar sesión operativa en Mi Cuenta
El sistema MUST ofrecer una acción "Cerrar sesión" en la página "Mi cuenta" que esté operativa para usuarios autenticados.

#### Scenario: Disponibilidad de acción de cierre
- **WHEN** el usuario visualiza la página "Mi cuenta"
- **THEN** el sistema MUST mostrar la acción "Cerrar sesión" como elemento interactivo

### Requirement: Limpieza de sesión local al cerrar sesión
El sistema MUST limpiar el estado local de autenticación (token y cualquier dato de sesión asociado) cuando el usuario ejecuta "Cerrar sesión".

#### Scenario: Ejecución de signout
- **WHEN** el usuario pulsa "Cerrar sesión"
- **THEN** el sistema MUST eliminar el estado de sesión local del usuario

### Requirement: Redirección al login tras cerrar sesión
El sistema MUST redirigir al usuario a la pantalla de login inmediatamente después de completar el cierre de sesión.

#### Scenario: Retorno a login
- **WHEN** finaliza la operación de "Cerrar sesión"
- **THEN** el sistema MUST navegar a la ruta de login

### Requirement: Protección de navegación tras signout
El sistema MUST evitar que el usuario permanezca en área autenticada usando el estado previo tras cerrar sesión.

#### Scenario: Reacceso inmediato tras signout
- **WHEN** un usuario intenta volver a una ruta autenticada justo después de cerrar sesión
- **THEN** el sistema MUST exigir nuevo login o redirigir a pantalla de acceso
