## Context

Los agregados `Cargo` y `SignalType` están modelados con identificador entero asignado desde el exterior (`Crear(int id, ...)`). Los comandos y endpoints de alta también exponen ese `Id`, y las pruebas de integración usan valores fijos (p.ej., 901, 101).

Como ambas entidades participan en relaciones (`AspNetUsers.CargoId`, `Signals.Tipo`), el cambio a autoincremento debe cubrir no solo el modelo, sino también los procesos de alta y el consumo posterior de IDs generados.

## Goals / Non-Goals

**Goals:**
- Eliminar asignación manual de `Id` en creación de `Cargo` y `SignalType`.
- Delegar generación de IDs a SQL Server mediante identidad/autoincremento.
- Mantener contratos de actualización/eliminación/listado usando `Id` como clave de ruta.
- Ajustar tests y flujos para consumir `Id` devuelto en creación.

**Non-Goals:**
- Cambiar tipo de `Id` (se mantiene `int`).
- Modificar reglas de autorización de cargos o tipos de señal.
- Introducir endpoints nuevos para búsqueda avanzada.

## Decisions

1. Factorías de dominio sin parámetro `Id`
- Decision: `Cargo.Crear(string descripcion)` y `SignalType.Crear(string nombre, string icono)`.
- Rationale: el `Id` deja de ser un input de negocio y pasa a ser responsabilidad de persistencia.
- Alternative considered: mantener sobrecargas con `Id` opcional. Rechazada para evitar ambigüedad y uso accidental.

2. Contratos de creación sin `Id` en request
- Decision: comandos/DTOs de creación para cargos y signal types no reciben `Id`; response devuelve `int` generado.
- Rationale: alinea API con fuente única de identidad y evita colisiones por cliente.
- Alternative considered: aceptar `Id` opcional en API. Rechazada por complejidad innecesaria y riesgo de inconsistencias.

3. Persistencia con `ValueGeneratedOnAdd`
- Decision: mapear `Id` como generado en alta para ambas tablas y migrar esquema.
- Rationale: permite autoincremento nativo y simplifica creación concurrente.
- Alternative considered: generar IDs en aplicación. Rechazada por pérdida de coherencia con patrón actual y riesgos de race conditions.

4. Actualizar pruebas y procesos que asumen IDs fijos
- Decision: capturar `Id` retornado por creación en tests y reutilizarlo para updates/referencias (`CargoId`, `Tipo`).
- Rationale: evita dependencia de valores concretos y reduce fragilidad de tests.
- Alternative considered: reinicializar seed con IDs conocidos. Rechazada por acoplamiento al estado de datos.

## Risks / Trade-offs

- [Risk] Migración de identidad en tablas con datos existentes puede fallar o requerir recreación de PK -> Mitigation: diseñar migración explícita y validar en entorno de prueba con datos.
- [Risk] Flujos clientes que hoy envían `Id` en POST quedarán incompatibles -> Mitigation: documentar breaking change y ajustar contratos en paralelo con frontend/consumidores.
- [Risk] Tests de integración inestables por asumir orden de creación -> Mitigation: usar IDs retornados por API en la misma prueba.

## Migration Plan

1. Refactor dominio (`Cargo`, `SignalType`) para alta sin `Id`.
2. Actualizar comandos/handlers/controladores de creación.
3. Ajustar configuración EF Core para identidad/autoincremento y generar migración.
4. Adaptar pruebas de aplicación e integración dependientes de IDs fijos.
5. Ejecutar suite completa y verificar relaciones (`CargoId`, `Tipo`) sin regresión.

Rollback strategy:
- Revertir código y migración del cambio en caso de incompatibilidad crítica.
- Restaurar contratos previos de creación con `Id` explícito si se decide diferir breaking change.

## Open Questions

- Confirmar si el cambio de contrato API (quitar `Id` en POST) requiere ventana de compatibilidad temporal o puede aplicarse directamente.
