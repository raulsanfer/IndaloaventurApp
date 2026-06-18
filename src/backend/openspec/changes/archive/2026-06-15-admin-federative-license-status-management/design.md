## Context

El backend ya dispone del agregado `SolicitudLicenciaFederativa` con estados `Pendiente`, `Confirmada` y `Cancelada`, y del API de autoservicio para alta y consulta de solicitudes propias. Falta la pieza administrativa que permita a usuarios con rol `Admin` supervisar el conjunto de solicitudes y tramitar solicitudes ajenas sin tocar el resto de datos funcionales de la solicitud.

El proyecto sigue un patron claro: controladores protegidos con JWT, CQRS con MediatR, comandos persistidos mediante EF Core y mensajes user-facing en espanol. Para coherencia con otros modulos administrativos, la operativa debe quedar separada de las rutas `me` y expresar claramente el `UserId` sobre el que se actua.

## Goals / Non-Goals

**Goals:**
- Permitir a un usuario con rol `Admin` listar y filtrar todas las solicitudes de licencia federativa creadas.
- Permitir a un usuario con rol `Admin` actualizar el `Estado` de una solicitud concreta de licencia federativa.
- Validar que la solicitud pertenece al `UserId` indicado y evitar cambios sobre otros campos.
- Reutilizar el agregado existente para aplicar transiciones de estado sin duplicar reglas de negocio.
- Cubrir el comportamiento con pruebas de integracion y autorizacion.

**Non-Goals:**
- Crear, eliminar o reasignar solicitudes desde la operativa administrativa.
- Editar tarifa, temporada o usuario propietario de la solicitud.
- Incorporar historico de cambios de estado, auditoria avanzada o comentarios administrativos.
- Abrir estas operaciones a roles distintos de `Admin`.

## Decisions

### 1. Ruta administrativa separada de `me`
La operativa administrativa se expondrá bajo rutas separadas de `me`: una ruta de listado global para administracion y una ruta de actualizacion con `userId` y `solicitudId`, manteniendo separado el autoservicio del usuario de la gestion por administracion.

Alternativas consideradas:
- Reutilizar la misma ruta de detalle `me`: descartado porque mezcla responsabilidades y no expresa la actuacion sobre terceros.
- Actualizar solo por `solicitudId`: descartado porque el caso de uso pide gestionar solicitudes de un usuario concreto y conviene que la ruta lo haga explicito.

### 2. Listado administrativo con filtros por query string
La consulta global se resolvera mediante query Dapper y aceptara filtros opcionales, al menos por `UserId`, `Temporada` y `Estado`, para que administracion pueda acotar la bandeja de solicitudes sin descargar siempre el conjunto completo.

Alternativas consideradas:
- Exponer siempre el listado completo sin filtros: descartado porque escala peor y complica el uso administrativo.
- Reutilizar el query de autoservicio: descartado porque el alcance y los criterios de filtrado administrativos son distintos.

### 3. Cambio limitado al campo `Estado`
El contrato de escritura solo aceptara el nuevo estado. La temporada, tarifa y usuario seguiran siendo inmutables desde este caso de uso.

Alternativas consideradas:
- Permitir editar toda la solicitud desde un DTO amplio: descartado para reducir riesgo y mantener el cambio pequeno y determinista.

### 4. Aplicacion de transiciones a traves del agregado
El handler recuperara la solicitud mediante repositorio EF Core, comprobara que pertenece al `UserId` indicado y aplicara `Confirmar`, `Cancelar` o una vuelta controlada a `Pendiente` con un metodo de dominio si fuera necesario incorporarlo. El resultado devolvera la solicitud actualizada.

Alternativas consideradas:
- Actualizar el enum directamente desde infraestructura: descartado porque saltaria el modelo de dominio.
- Implementar la mutacion como SQL directo: descartado por la especificacion CQRS del proyecto, que reserva EF Core para comandos.

### 5. Autorizacion por rol `Admin` en endpoints
La restriccion principal residira en el controlador mediante `[Authorize(Roles = "Admin")]`, complementada por tests de integracion que verifiquen denegacion para usuarios no administradores tanto en lectura global como en cambio de estado.

Alternativas consideradas:
- Resolver la autorizacion solo dentro del handler: descartado porque el proyecto ya expresa las reglas administrativas en la capa API.

## Risks / Trade-offs

- [Necesidad de volver una solicitud a `Pendiente`] -> Mitigar decidiendo explicitamente si el agregado admite esa transicion y documentandolo en la implementacion.
- [Listado administrativo puede crecer con demasiados criterios] -> Mitigar arrancando con filtros funcionales minimos y DTOs de lectura dedicados.
- [Ruta con `userId` y `solicitudId` puede resultar redundante] -> Mitigar validando la coherencia entre ambos para evitar actualizaciones sobre recursos mal direccionados.
- [Cambio de estado sin auditoria detallada] -> Mitigar manteniendo el alcance pequeno y dejando el historico como mejora futura separada.

## Migration Plan

- No requiere cambios de esquema si el estado ya se persiste y la solicitud ya contiene la informacion necesaria.
- Desplegar sobre el cambio base de persistencia y sobre el API de autoservicio ya aplicado.
- Si fuera necesario rollback, bastaria con retirar el endpoint y el comando administrativo sin afectar al modelo persistido.

## Open Questions

- Confirmar si administracion debe poder devolver una solicitud desde `Confirmada` o `Cancelada` a `Pendiente`, o si solo se admiten transiciones hacia estados finales.
- Confirmar si el listado administrativo necesita algun filtro adicional de salida, como licencia o categoria, en esta primera iteracion.
