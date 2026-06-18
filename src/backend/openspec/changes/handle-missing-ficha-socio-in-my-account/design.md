## Context

Actualmente `GetFichaSocioQueryHandler` lanza `KeyNotFoundException` cuando no existe una ficha para el usuario consultado. En la pagina "Mi Cuenta" esto bloquea la carga general porque la ausencia de ficha no representa un error funcional, sino un estado inicial valido.

## Goals / Non-Goals

**Goals:**
- Evitar excepcion cuando no exista ficha de socio para el usuario objetivo.
- Permitir que el flujo de cliente continue cargando el resto de componentes.
- Mantener controles de autorizacion actuales para acceso a ficha.

**Non-Goals:**
- Cambiar permisos de consulta/edicion de ficha.
- Crear ficha automaticamente cuando no exista.
- Redisenar contratos de otros modulos no relacionados con ficha de socio.

## Decisions

1. El handler devolvera resultado vacio (nullable) en lugar de lanzar `KeyNotFoundException` cuando no haya ficha.
- Rationale: modela la ausencia de ficha como estado esperado y evita cortar el flujo de "Mi Cuenta".
- Alternative: devolver 404 desde API. Se descarta porque el cliente necesita continuar carga sin tratarlo como fallo.

2. Se conservara la excepcion de autorizacion (`ForbiddenAccessException`) para accesos no permitidos.
- Rationale: el cambio solo aplica a no-encontrado, no a seguridad.

3. Ajustar contrato de respuesta del endpoint para tolerar `null` en payload de ficha.
- Rationale: contrato explicito y estable para consumidores.

## Risks / Trade-offs

- [Riesgo] Consumidores actuales esperan siempre objeto no nulo -> Mitigacion: actualizar contratos/documentacion y pruebas de integracion.
- [Trade-off] Menos señal explicita de "ficha inexistente" a nivel HTTP -> Mitigacion: cliente interpreta `null` como "pendiente de crear ficha".

## Migration Plan

1. Cambiar query/handler y DTO de respuesta para permitir valor nulo en lectura de ficha.
2. Ajustar controlador y `ProducesResponseType` acorde al nuevo contrato.
3. Agregar pruebas de aplicacion/integracion para "usuario sin ficha".
4. Desplegar sin migraciones de base de datos.

Rollback:
- Revertir cambios de handler/controlador para restaurar comportamiento previo (lanza excepcion).

## Open Questions

- Confirmar si el endpoint debe responder `200` con body `null` o `204 No Content` cuando no haya ficha (propuesta actual: `200` + `null`).
