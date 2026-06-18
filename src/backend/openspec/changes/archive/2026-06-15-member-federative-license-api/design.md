## Context

El cambio `federative-license-management` ya ha introducido el catalogo de tarifas federativas y la persistencia de `SolicitudLicenciaFederativa`, dejando preparada la base para una fase de API. En la capa API actual existen patrones reutilizables para operaciones del usuario autenticado con rutas `me`, autenticacion JWT y separacion CQRS con EF Core para comandos y Dapper para consultas.

La nueva necesidad es abrir una experiencia de autoservicio donde el usuario autenticado pueda registrar una solicitud y consultar solo sus propios datos. El principal condicionante funcional es que la creacion solo esta permitida a usuarios marcados como socios reales del club mediante el claim `IsMember`, que no equivale al rol tecnico `Member`.

## Goals / Non-Goals

**Goals:**
- Exponer un flujo API para crear una solicitud de licencia federativa del usuario autenticado.
- Permitir listar y consultar el detalle de las solicitudes del propio usuario con informacion suficiente para pintar el cliente.
- Reutilizar los patrones existentes del proyecto para seguridad, CQRS y mensajes en espanol.
- Mantener el aislamiento entre usuarios y apoyar la unicidad por temporada ya definida en dominio/persistencia.

**Non-Goals:**
- Gestion administrativa de solicitudes de otros usuarios.
- Cambio de estados `Pendiente`, `Confirmada` o `Cancelada` desde esta fase.
- Edicion o cancelacion de solicitudes ya creadas.
- Gestion del catalogo de tarifas federativas desde el API publico.

## Decisions

### 1. Rutas de autoservicio basadas en `me`
Se usaran rutas autenticadas bajo una convencion `me` para evitar pasar `userId` desde el cliente y alinear el API con `FichasSocioController`.

Alternativas consideradas:
- Exponer `userId` en la ruta: descartado porque introduce riesgo de acceso cruzado y no aporta valor en un flujo de autoservicio.
- Colgar las rutas de un modulo administrativo existente: descartado porque mezcla responsabilidades de administracion con experiencia de usuario final.

### 2. Autorizacion combinando autenticacion JWT y claim `IsMember`
El endpoint de creacion validara el claim `IsMember` resuelto desde el token autenticado y rechazara la operacion si vale `false`. Las consultas de "mis solicitudes" se limitaran al `NameIdentifier` del token para que el usuario solo vea su propio historial.

Alternativas consideradas:
- Basarse en el rol `Member`: descartado porque el proyecto define que ese rol tecnico no representa la condicion de socio real.
- Validar `IsMember` tambien en lectura: no se adopta de inicio porque el requisito diferencial del negocio se centra en la capacidad de solicitar; leer el propio historial no necesita una regla distinta si ya existen datos previos.

### 3. Comandos con EF Core y consultas con Dapper
La creacion se implementara como comando MediatR que recupere la tarifa elegida, construya el agregado y persista mediante repositorio/`UnitOfWork`. Las lecturas de lista y detalle se implementaran como queries Dapper devolviendo DTOs listos para el cliente.

Alternativas consideradas:
- Usar EF Core tambien en lectura: descartado para mantener consistencia con la especificacion CQRS del proyecto.
- Exponer entidades de dominio directamente: descartado para no acoplar el contrato del API a la persistencia.

### 4. Contrato de lectura con detalle de tarifa y estado
Las respuestas incluiran identificadores y metadatos funcionales de la solicitud: temporada, estado, fecha de creacion y datos de la tarifa asociada como licencia, categoria, territorio e importes. La lista reutilizara el mismo modelo o uno equivalente suficientemente rico para evitar llamadas adicionales innecesarias del cliente.

Alternativas consideradas:
- Devolver solo ids y forzar una segunda llamada por cada solicitud: descartado porque empeora la experiencia del cliente sin beneficio claro en este caso.

## Risks / Trade-offs

- [Dependencia de un cambio OpenSpec aun no archivado] -> Mitigar implementando este cambio solo cuando `federative-license-management` este aplicado o integrando ambos en la misma rama.
- [El claim `IsMember` puede quedar desactualizado respecto a base de datos hasta el siguiente login] -> Mitigar documentando que la autorizacion usa el token emitido y exigiendo renovacion de sesion tras cambios administrativos sensibles.
- [Lista con mucho detalle puede crecer de forma rigida] -> Mitigar definiendo DTOs propios y manteniendo separada la proyeccion de lista y la de detalle si el cliente evoluciona.
- [Unicidad por temporada puede aflorar como error de persistencia poco amigable] -> Mitigar anadiendo validacion previa de existencia y cobertura de errores de conflicto con mensaje en espanol.

## Migration Plan

- No requiere cambios de esquema nuevos si se apoya en las tablas y repositorios ya introducidos por `federative-license-management`.
- Desplegar junto con el cambio base de persistencia y verificar que el catalogo de tarifas de la temporada activa ya esta cargado.
- Si fuera necesario rollback, bastara con retirar los endpoints y handlers porque no se introduce una nueva migracion propia en esta fase.

## Open Questions

- Confirmar si la consulta de "mis solicitudes" debe devolver una coleccion completa con todo el detalle o si el cliente prefiere lista resumida mas endpoint de detalle.
- Confirmar si debe existir una regla adicional para bloquear tambien la lectura cuando `IsMember` sea `false`.
