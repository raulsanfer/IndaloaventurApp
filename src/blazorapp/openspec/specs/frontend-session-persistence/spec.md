# frontend-session-persistence Specification

## Purpose
TBD - created by archiving change evitar-cierre-sesion-al-refrescar-pwa. Update Purpose after archive.
## Requirements
### Requirement: Persistencia de sesi�n autenticada en navegador
El frontend MUST persistir una sesi�n autenticada en almacenamiento del navegador al completar un login correctamente, de forma que una recarga completa no elimine por s� sola la sesi�n mientras el token siga siendo v�lido.

#### Scenario: Login sin `Recordarme`
- **WHEN** un usuario completa un login correcto sin activar `Recordarme`
- **THEN** el frontend MUST guardar la sesi�n en un almacenamiento que sobreviva a una recarga completa del navegador dentro de la sesi�n actual

#### Scenario: Login con `Recordarme`
- **WHEN** un usuario completa un login correcto con `Recordarme` activado
- **THEN** el frontend MUST guardar la sesi�n en un almacenamiento persistente que permita reabrir la app sin volver a autenticarse mientras el token siga siendo v�lido

### Requirement: Rehidrataci�n de sesi�n antes de proteger rutas
El frontend MUST intentar restaurar la sesi�n persistida al arrancar la app antes de decidir la redirecci�n de las p�ginas protegidas.

#### Scenario: Recarga en ruta protegida con sesi�n v�lida
- **WHEN** un usuario autenticado hace `F5` o un gesto de refresco sobre una ruta protegida y existe una sesi�n persistida todav�a v�lida
- **THEN** la app MUST restaurar la sesi�n y mantener al usuario en la misma ruta protegida sin enviarlo al login

#### Scenario: Inicio interactivo pendiente
- **WHEN** la app todav�a no ha terminado de comprobar si existe una sesi�n persistida en el navegador
- **THEN** una p�gina protegida MUST aplazar la redirecci�n al login hasta que la comprobaci�n de restauraci�n haya finalizado

### Requirement: Rechazo de sesiones persistidas caducadas
El frontend MUST validar la vigencia temporal de la sesi�n persistida antes de reutilizarla y MUST descartarla cuando ya haya expirado.

#### Scenario: Sesi�n persistida expirada al arrancar
- **WHEN** la app encuentra una sesi�n persistida cuya expiraci�n ya ha pasado
- **THEN** el frontend MUST eliminar esa sesi�n almacenada
- **THEN** el frontend MUST tratar al usuario como no autenticado

#### Scenario: Acceso protegido sin sesi�n reutilizable
- **WHEN** un usuario abre una ruta protegida y no existe una sesi�n persistida reutilizable
- **THEN** la app MUST redirigir al login una vez finalizada la comprobaci�n de restauraci�n

### Requirement: Cierre de sesi�n limpia
El frontend MUST eliminar tanto la sesi�n en memoria como la sesi�n persistida cuando el usuario cierre sesi�n de forma expl�cita.

#### Scenario: Usuario cierra sesi�n
- **WHEN** un usuario autenticado pulsa `Cerrar Sesi�n`
- **THEN** el frontend MUST borrar la sesi�n activa de memoria
- **THEN** el frontend MUST borrar cualquier sesi�n persistida en navegador asociada a ese usuario
- **THEN** el frontend MUST navegar al login

