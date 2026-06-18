## 1. Sincronizacion de membresia

- [x] 1.1 Ampliar la abstraccion de identidad con una operacion especifica para actualizar `IsMember` del usuario vinculado sin reutilizar el flujo general de actualizacion de email y roles.
- [x] 1.2 Implementar en infraestructura la actualizacion de `Usuario.IsMember` para que pueda participar en la misma unidad de persistencia que la creacion de `FichaSocio`.

## 2. Alta de ficha de socio

- [x] 2.1 Actualizar `CreateFichaSocioCommandHandler` para marcar `IsMember = true` cuando la ficha se cree correctamente y para abortar la operacion si la sincronizacion de identidad no puede completarse.
- [x] 2.2 Revisar los mensajes de error y el flujo de guardado para asegurar que las validaciones existentes siguen evitando cambios parciales en duplicados o cargos inexistentes.

## 3. Verificacion

- [x] 3.1 Anadir o actualizar pruebas de aplicacion para cubrir creacion de ficha con sincronizacion de `IsMember`, y los casos donde la creacion se rechaza sin modificar la membresia.
- [x] 3.2 Ejecutar la bateria relevante de pruebas automatizadas y dejar documentado el resultado antes de marcar las tareas como completadas.
