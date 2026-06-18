## Context

La API ya aplica Clean Architecture + DDD + CQRS con autenticacion JWT y autorizacion por roles. No existe una capacidad de Agenda Telefonica, por lo que se requiere introducir un nuevo agregado de dominio y su operativa completa hasta endpoints, manteniendo SQL Server como persistencia y el estilo del proyecto.

El caso de uso exige que solo `Admin` pueda mutar fichas (crear, editar, eliminar), mientras que cualquier usuario autenticado pueda consultar. Ademas, se pide fortalecer el modelo con Value Objects y validaciones de dominio.

## Goals / Non-Goals

**Goals:**
- Introducir el agregado `FichaContacto` con invariantes de dominio para nombre, telefonos y observaciones.
- Implementar CRUD completo usando el patron CQRS ya existente.
- Garantizar autorizacion por endpoint: lectura para cualquier rol autenticado, escritura solo para `Admin`.
- Persistir el agregado con EF Core/SQL Server y repositorio dedicado.
- Cubrir reglas de dominio, aplicacion y API con pruebas.

**Non-Goals:**
- Sincronizacion con agendas externas o importacion/exportacion masiva.
- Auditoria de usuario creador/modificador.
- Busqueda avanzada (filtros complejos, paginacion sofisticada, full-text).

## Decisions

1. Modelar `FichaContacto` como agregado raiz con `Guid` como identidad
- Decision: la entidad incluye `Id`, `FechaAlta`, `Nombre`, `Telefono1`, `Telefono2` y `Observaciones`, con metodos de fabrica/actualizacion que preserven invariantes.
- Rationale: concentra reglas de negocio en dominio y evita mutaciones invalidas desde capas superiores.
- Alternative considered: usar entidad anemica con validacion solo en validators de aplicacion. Rechazada por debilitar el modelo de dominio.

2. Usar Value Objects para campos de texto con semantica de negocio
- Decision: crear al menos `NombreContacto`, `TelefonoContacto` y `ObservacionesContacto` (opcional para `Telefono2` y `Observaciones`), aplicando normalizacion y longitudes maximas.
- Rationale: encapsula reglas y evita duplicar validaciones en handlers/controladores.
- Alternative considered: strings primitivas en toda la cadena. Rechazada por mayor riesgo de inconsistencias.

3. Separar escritura/lectura con CQRS
- Decision: comandos para crear/actualizar/eliminar y consultas para listado/detalle; cada caso de uso con handler propio.
- Rationale: mantiene consistencia con arquitectura existente y facilita pruebas aisladas.
- Alternative considered: servicio CRUD unico. Rechazada por romper convencion del proyecto.

4. Autorizacion explicita por endpoint
- Decision: endpoints GET requieren usuario autenticado; POST/PUT/DELETE requieren rol `Admin`.
- Rationale: satisface requerimiento funcional y evita sobreexposicion de operaciones de mutacion.
- Alternative considered: proteger todo el controlador con `Admin`. Rechazada porque bloquearia consultas para otros roles.

5. Persistencia mediante repositorio de agregado + configuracion EF Core
- Decision: agregar contrato de repositorio para `FichaContacto`, implementacion Infrastructure y mapeo EF Core con restricciones de esquema acordes a Value Objects.
- Rationale: respeta especificaciones actuales del patron repositorio y permite evolucion controlada de persistencia.
- Alternative considered: acceso directo DbContext desde handlers. Rechazada por saltar frontera de arquitectura.

## Risks / Trade-offs

- [Risk] Validaciones de telefono demasiado estrictas bloqueen datos reales -> Mitigation: definir reglas pragmaticas (digitos, espacios, prefijo +) y cubrir casos reales con tests.
- [Risk] Divergencia entre validacion de dominio y validacion de API -> Mitigation: centralizar invariantes en Value Objects/entidad y reutilizar mensajes consistentes.
- [Risk] Cambios de autorizacion rompan clientes existentes -> Mitigation: documentar matriz de permisos por endpoint y añadir pruebas de autorizacion.
- [Risk] Migracion de esquema falle en despliegue -> Mitigation: incluir migracion EF Core deterministica y validacion en pipeline.

## Migration Plan

1. Crear agregado y Value Objects de Agenda Telefonica en Domain.
2. Definir contratos de aplicacion (comandos/consultas/repositorio) y handlers.
3. Implementar persistencia Infrastructure (entidad EF, configuraciones, repositorio, migracion).
4. Exponer endpoints API con autorizacion por metodo.
5. Añadir/actualizar pruebas unitarias e integracion.
6. Ejecutar suite de pruebas y validar criterios de aceptacion.

Rollback strategy:
- Revertir el cambio completo y restaurar la ultima migracion estable si se detecta regresion.
- Al ser funcionalidad nueva, el rollback no requiere transformaciones de datos legacy.

## Open Questions

- Confirmar politica exacta de longitud maxima para `Nombre`, `Telefono1/2` y `Observaciones` segun convenciones actuales del dominio.
- Confirmar si el listado debe salir ordenado por `Nombre` o por `FechaAlta` por defecto.