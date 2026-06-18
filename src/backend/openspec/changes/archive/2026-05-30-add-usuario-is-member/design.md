## Context

La API ya gestiona ciclo de vida del usuario (alta, activación/desactivación y autenticación), pero no modela explícitamente si el usuario es miembro del club. Se requiere incorporar este estado como dato de dominio persistente y gestionable por administradores, sin incluir cambios de frontend en este alcance.

## Goals / Non-Goals

**Goals:**
- Incorporar `IsMember` como propiedad booleana persistida en la entidad de usuario.
- Exponer `IsMember` en consultas y respuestas de usuario que alimentan el cliente.
- Permitir que administradores puedan actualizar `IsMember` en flujos de gestión de usuario.
- Mantener compatibilidad con usuarios existentes mediante migración con valor por defecto.

**Non-Goals:**
- Implementar interfaces de usuario cliente para activar/desactivar membresía.
- Cambiar reglas de autorización existentes fuera del caso de edición administrativa.
- Añadir nuevas integraciones externas basadas en membresía.

## Decisions

1. Modelar `IsMember` en la entidad `Usuario` (Identity user)
- Rationale: evita duplicidad de estado y mantiene la membresía junto al resto de atributos de cuenta.
- Alternative considered: tabla separada de perfil de membresía. Se descarta por sobreingeniería en este alcance.

2. Default `false` para registros existentes y nuevos cuando no se informe valor
- Rationale: minimiza riesgo de conceder membresía accidentalmente.
- Alternative considered: `true` por defecto. Se descarta por riesgo funcional.

3. Actualización de `IsMember` solo en flujos administrativos existentes de edición de usuario
- Rationale: reutiliza controles de seguridad y evita crear endpoints adicionales.
- Alternative considered: endpoint dedicado de membresía. Se puede evaluar en cambios futuros.

4. Incluir `IsMember` en DTOs/queries de lectura de usuario
- Rationale: permite consumo inmediato por cliente y procesos internos sin llamadas extra.
- Alternative considered: exponerlo en endpoint separado; se descarta por complejidad innecesaria.

## Risks / Trade-offs

- [Riesgo] Inconsistencia de mapeos (entidad vs DTOs) -> Mitigación: actualizar todos los contratos de lectura/escritura de usuario con pruebas de integración/aplicación.
- [Riesgo] Regresión en migraciones sobre entorno con datos previos -> Mitigación: definir migración explícita con `default false` y validar apply/rollback en local.
- [Trade-off] Ampliar contratos API puede requerir actualización de consumidores -> Mitigación: mantener backward compatibility agregando campo opcional/no disruptivo.

## Migration Plan

1. Añadir propiedad `IsMember` a la entidad de usuario.
2. Crear y aplicar migración EF Core con columna booleana no nula y valor por defecto `false`.
3. Ajustar comandos/queries y DTOs para lectura y edición administrativa.
4. Ejecutar pruebas automáticas y validar escenarios de alta/consulta/edición de usuario.
5. Despliegue gradual con monitoreo de errores de serialización y acceso a datos.

Rollback:
- Revertir despliegue de aplicación a versión anterior.
- Aplicar rollback de migración si no existen dependencias operativas del campo.

## Open Questions

- Confirmar si `IsMember` debe aparecer en todos los endpoints de usuario o solo en endpoints administrativos.
- Confirmar si se requiere auditoría explícita (quién cambió membresía y cuándo) en este cambio o en uno posterior.
