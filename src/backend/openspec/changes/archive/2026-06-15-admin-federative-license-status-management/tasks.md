## 1. Contratos y reglas de negocio

- [x] 1.1 Definir el comando administrativo, las queries de listado filtrado y los DTOs de respuesta para la operativa administrativa sobre solicitudes de licencia federativa.
- [x] 1.2 Ajustar el agregado o la logica de aplicacion para soportar las transiciones de estado permitidas con mensajes en espanol.

## 2. API administrativa

- [x] 2.1 Crear el endpoint autenticado solo para `Admin` que lista todas las solicitudes con filtros administrativos opcionales.
- [x] 2.2 Crear el endpoint autenticado solo para `Admin` que actualiza el estado de una solicitud de un usuario concreto.
- [x] 2.3 Implementar la query Dapper administrativa para recuperar solicitudes globales aplicando filtros por `UserId`, `Temporada` y `Estado`.
- [x] 2.4 Implementar el handler EF Core que valide existencia, coherencia entre `userId` y solicitud, y persistencia del nuevo estado.

## 3. Verificacion

- [x] 3.1 Crear pruebas automatizadas para listado administrativo sin filtros, filtros funcionales, acceso denegado a no administradores, cambio correcto de estado, solicitud inexistente y solicitud asociada a otro usuario.
- [x] 3.2 Ejecutar `dotnet test` y marcar las tareas solo cuando toda la verificacion pase.
