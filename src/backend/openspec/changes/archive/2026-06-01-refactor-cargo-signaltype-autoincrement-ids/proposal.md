## Why

Actualmente `Cargo` y `SignalType` exigen que el cliente proporcione el `Id` al crear entidades. Este enfoque incrementa el riesgo de colisiones, acopla el contrato API al detalle de persistencia y complica la coherencia entre entornos.

Se requiere refactorizar ambos modelos para que su identificador sea autoincremental en base de datos y revisar los procesos asociados (creación, pruebas, validaciones, relaciones y contratos) para mantener consistencia funcional.

## What Changes

- Refactorizar `Cargo` y `SignalType` para creación sin `Id` explícito en dominio/aplicación/API.
- Configurar persistencia EF Core para `ValueGeneratedOnAdd` en `Cargos.Id` y `SignalTypes.Id`.
- Ajustar endpoints de alta para que no reciban `Id` en request y devuelvan el `Id` generado en respuesta.
- Revisar flujos dependientes:
  - Gestión de usuarios con `CargoId` (referencia y validaciones de existencia).
  - Gestión de señales que referencian `SignalType` por `Tipo`.
  - Pruebas unitarias/integración que hoy usan IDs fijos en altas.
- Añadir migración para alinear esquema SQL Server con identidad autoincremental.

## Capabilities

### Modified Capabilities
- `signal-type-management`: la creación de `SignalType` pasa a usar `Id` generado automáticamente por persistencia.
- `admin-user-management-api`: se preserva coherencia de asignación `CargoId` tras crear cargos con IDs generados.

## Impact

- `IndaloAventurApi.Domain`: factorías de `Cargo` y `SignalType`.
- `IndaloAventurApi.Application`: comandos de creación y handlers asociados para cargos/tipos de señal.
- `IndaloAventurApi.Api`: contratos HTTP de `POST /api/cargos` y `POST /api/signal-types`.
- `IndaloAventurApi.Infrastructure`: configuraciones EF, migración y potenciales ajustes de relaciones.
- `tests/*`: adaptación de tests que dependen de IDs hardcodeados.
