## Why

Las solicitudes de licencia federativa ya pueden ser creadas y consultadas, pero todavia no existe una operativa API para que administracion gestione su ciclo de tramitacion ni supervise el conjunto de solicitudes registradas. Necesitamos permitir que un usuario con rol `Admin` consulte y filtre todas las solicitudes, ademas de actualizar el estado de una solicitud concreta para reflejar el avance real del proceso.

## What Changes

- Exponer un endpoint administrativo autenticado para editar una solicitud de licencia federativa de un usuario concreto.
- Exponer un endpoint administrativo autenticado para listar todas las solicitudes de licencia federativa creadas en el sistema.
- Permitir filtros administrativos sobre la lista para acotar resultados por criterios funcionales de la solicitud.
- Limitar la edicion administrativa al campo `Estado`, sin abrir cambios sobre temporada, usuario o tarifa asociada.
- Validar que el `Admin` solo pueda actualizar solicitudes existentes que pertenezcan al `UserId` indicado en la ruta.
- Permitir unicamente valores de estado soportados por el dominio (`Pendiente`, `Confirmada`, `Cancelada`) devolviendo errores en espanol ante valores invalidos o solicitudes inexistentes.
- Mantener el contrato actual de autoservicio del usuario sin mezclar rutas `me` con operaciones administrativas.

## Capabilities

### New Capabilities
- `admin-federative-license-status-management`: API CQRS para que un usuario con rol `Admin` consulte, filtre y actualice solicitudes de licencia federativa de cualquier usuario.

### Modified Capabilities

## Impact

- API: nuevas rutas protegidas por rol `Admin` para consulta global y gestion administrativa de solicitudes de licencia federativa.
- Application: nuevo comando, nuevas queries, validaciones y autorizacion para listado filtrado y actualizacion controlada del estado de una solicitud.
- Domain: uso explicito de las transiciones de estado del agregado `SolicitudLicenciaFederativa`.
- Infrastructure: nuevas consultas de lectura administrativa para recuperar solicitudes con filtros.
- Tests: cobertura de autorizacion admin-only, filtros de listado, solicitud inexistente, solicitud ajena al `UserId` indicado y cambio correcto de estado.
