## Context

El modulo de licencias federativas ya dispone de persistencia para `TarifaLicenciaFederativa` y de endpoints autenticados para solicitudes del usuario actual y gestion administrativa de solicitudes. El hueco pendiente es la lectura del catalogo de tarifas para que el cliente pueda mostrar las opciones disponibles antes de crear una solicitud.

El proyecto sigue una separacion CQRS clara: comandos y mutaciones con EF Core, consultas con handlers dedicados y controladores protegidos con JWT. Tambien existe el matiz funcional de que el rol tecnico `Member` no representa privilegios especiales de negocio, por lo que esta consulta debe apoyarse solo en autenticacion y no en autorizacion por rol.

## Goals / Non-Goals

**Goals:**
- Exponer una consulta autenticada del catalogo de tarifas de licencias federativas para cualquier usuario con JWT valido.
- Permitir recuperar todas las tarifas disponibles y, opcionalmente, acotar por `Temporada`.
- Devolver un contrato de lectura estable y suficiente para que el cliente renderice la seleccion de tarifa sin llamadas adicionales.
- Mantener coherencia con los patrones actuales del modulo `LicenciasFederativas`.

**Non-Goals:**
- Crear, editar o eliminar tarifas federativas desde el API.
- Introducir reglas de autorizacion basadas en rol o en el claim `IsMember`.
- Cambiar el modelo persistido del catalogo o la carga inicial de tarifas.
- Resolver logica de "temporada activa" automatica en esta primera iteracion.

## Decisions

### 1. Endpoint GET autenticado dentro de `LicenciasFederativasController`
La consulta se expondra en una ruta GET del mismo modulo de licencias federativas, previsiblemente `api/licencias-federativas/tarifas`, protegida solo con `[Authorize]`.

Alternativas consideradas:
- Colgarla de una ruta `me`: descartado porque el recurso es un catalogo global y no depende de la identidad concreta del usuario.
- Marcarla como publica: descartado porque el requisito pide acceso solo a usuarios autenticados.

### 2. Query de aplicacion dedicada para lectura de catalogo
Se introducira una query MediatR especifica para listar tarifas y devolver DTOs planos con los campos funcionales del catalogo. El handler podra reutilizar `ITarifaLicenciaFederativaRepository` si cubre el filtro requerido o incorporar una proyeccion de lectura equivalente.

Alternativas consideradas:
- Leer directamente desde el controlador: descartado para mantener el patron CQRS del proyecto.
- Reutilizar el DTO de solicitudes: descartado porque mezcla datos de solicitud con datos del catalogo.

### 3. Filtro opcional por temporada
La query aceptara `Temporada` opcional. Sin filtro devolvera todo el catalogo disponible; con filtro devolvera solo las tarifas de esa temporada.

Alternativas consideradas:
- Exigir `Temporada` siempre: descartado porque el requisito pide consultar todas las tarifas y algunos clientes pueden necesitar el catalogo completo.
- No permitir ningun filtro: descartado porque complica al cliente cuando solo necesita la campana activa.

### 4. Ordenacion estable orientada a cliente
La respuesta se devolvera con un orden determinista, priorizando temporada descendente y despues criterios funcionales como licencia y categoria, para que distintas ejecuciones den el mismo resultado y el cliente no tenga que reordenar obligatoriamente.

Alternativas consideradas:
- Dejar el orden implicito de base de datos: descartado por no ser estable ni contractual.

## Risks / Trade-offs

- [El catalogo completo puede crecer con nuevas temporadas] -> Mitigar con filtro opcional por `Temporada` y dejando paginacion fuera hasta que exista necesidad real.
- [Reutilizar repositorio EF Core en una query puede ser menos consistente con otras lecturas Dapper] -> Mitigar aceptandolo si el volumen es pequeno o creando una proyeccion dedicada si el equipo prefiere mantener lectura especializada.
- [Exponer todas las temporadas puede mostrar tarifas historicas no operables] -> Mitigar dejando claro en el contrato que el endpoint lista catalogo y que la seleccion funcional puede filtrarse por temporada desde el cliente.

## Migration Plan

- No requiere migraciones ni cambios de esquema porque reutiliza `TarifasLicenciasFederativas`.
- Desplegar junto con el resto del modulo de licencias federativas ya existente.
- Si fuera necesario rollback, bastara con retirar el endpoint, la query y sus pruebas.

## Open Questions

- Confirmar si el cliente quiere recibir tambien un indicador derivado de "temporada vigente" en una fase futura.
- Confirmar si el contrato inicial necesita algun filtro adicional, como `Licencia` o `Territorio`, o si `Temporada` es suficiente para esta iteracion.
