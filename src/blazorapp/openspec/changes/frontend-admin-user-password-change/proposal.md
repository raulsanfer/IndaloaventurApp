## Why

Los administradores pueden gestionar las fichas de usuario, pero no disponen de un flujo controlado para restablecer una contraseña cuando un usuario lo necesita. Se necesita una operación administrativa explícita, con límites de entrada y un contrato de backend seguro.

## What Changes

- Añadir a la ficha administrativa de usuario una acción `Cambiar contraseña` accesible también para la propia cuenta del administrador.
- Incorporar una vista de restablecimiento con la identidad legible del usuario administrado, un campo de nueva contraseña y envío validado.
- Restringir el valor de la contraseña en cliente a caracteres alfanuméricos y un máximo de 20 caracteres, sin registrar ni mostrar el secreto.
- Documentar el contrato de un endpoint administrativo dedicado para que el backend aplique autorización, validación, almacenamiento mediante Identity y auditoría.

## Capabilities

### New Capabilities
- `frontend-admin-user-password-change`: Restablecimiento administrativo de contraseñas desde la gestión de usuarios.

### Modified Capabilities
- `frontend-admin-users-management`: La ficha administrativa incorpora la acción que inicia el restablecimiento de contraseña.

## Impact

- `IndaloaventurApp.SharedUI`: componentes, modelo de formulario, recursos localizados, estilos SCSS y servicio de gestión administrativa.
- `IndaloaventurApp.Web.Client`: nueva ruta administrativa protegida.
- `.vscode`: especificación de integración para el endpoint de backend dedicado.
- No se añaden dependencias ni se implementa almacenamiento, hash o endpoint de contraseña en el frontend.
