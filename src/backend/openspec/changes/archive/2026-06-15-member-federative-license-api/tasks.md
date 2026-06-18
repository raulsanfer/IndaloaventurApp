## 1. Contratos de aplicacion y seguridad

- [x] 1.1 Definir los DTOs, comandos y queries de autoservicio para crear, listar y consultar detalle de solicitudes de licencia federativa.
- [x] 1.2 Implementar la resolucion del usuario autenticado y la validacion del claim `IsMember` para el alta, con mensajes de error en espanol.

## 2. Endpoints y handlers

- [x] 2.1 Crear los endpoints autenticados `me` para alta de solicitud y consulta de mis solicitudes siguiendo los patrones del API actual.
- [x] 2.2 Implementar el comando de creacion reutilizando tarifa, repositorio de solicitudes y controlando duplicados por temporada.
- [x] 2.3 Implementar las queries Dapper para listado y detalle de solicitudes propias incluyendo la informacion de tarifa y estado.

## 3. Verificacion

- [x] 3.1 Crear pruebas automatizadas de autorizacion y comportamiento para alta, listado propio, detalle propio y aislamiento entre usuarios.
- [x] 3.2 Ejecutar `dotnet test` y dejar verificado el cambio antes de marcar las tareas como completadas.
