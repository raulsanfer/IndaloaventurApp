## Context

La solucion actual contempla autenticacion JWT e Identity a nivel de especificacion, pero no existe una estrategia cerrada para persistencia evolutiva con migraciones, bootstrap de roles/administrador ni federacion social. La API usa .NET 9 y SQL Server; el entorno de desarrollo ya dispone de la cadena `api_ContextConnection`, que debe convertirse en la fuente de verdad para migraciones y actualizacion de esquema en desarrollo.

## Goals / Non-Goals

**Goals:**
- Configurar Identity + EF Core sobre SQL Server y reflejar su esquema via migraciones versionadas.
- Garantizar que roles base y usuario administrador por defecto existen al iniciar en desarrollo.
- Definir endpoints de gestion de usuarios para administradores con autorizacion por rol.
- Habilitar login social con proveedores externos y consolidar respuesta JWT local.

**Non-Goals:**
- Implementar interfaz de administracion en frontend.
- Cubrir autoservicio avanzado de cuentas (registro publico, recuperacion password, MFA).
- Incluir integraciones con todos los proveedores sociales desde el dia uno (se priorizara una lista inicial configurable).

## Decisions

1. Identity persiste en el mismo SQL Server de la aplicacion usando `api_ContextConnection`.
- Rationale: simplifica despliegue inicial y garantiza consistencia transaccional entre identidad y dominio cuando aplique.
- Alternativa considerada: base separada para Identity; descartada por complejidad operativa temprana.

2. Las migraciones EF Core seran el unico mecanismo permitido para cambios de esquema.
- Rationale: trazabilidad, reproducibilidad y rollback controlado.
- Alternativa considerada: `Database.EnsureCreated`; descartada por no soportar evolucion controlada.

3. Inicializacion idempotente de roles y admin por defecto en startup (entorno desarrollo).
- Rationale: facilita onboarding y evita entornos inconsistentes.
- Alternativa considerada: script SQL manual; descartada por riesgo de deriva y baja automatizacion.

4. Gestion de usuarios via endpoints dedicados bajo politica `RequireAdmin`.
- Rationale: encapsula operaciones sensibles y mantiene reglas de acceso coherentes.
- Alternativa considerada: reutilizar endpoints de autenticacion existentes; descartada por mezclar responsabilidades.

5. Login social mediante flujo de validacion de token externo + emision de JWT local.
- Rationale: mantiene un unico contrato de sesion para clientes de la API.
- Alternativa considerada: sesion delegada completamente al proveedor social; descartada por fragmentar autorizacion interna.

## Risks / Trade-offs

- [Credenciales por defecto inseguras] -> Mitigar con secretos por entorno, rotacion obligatoria y bandera que fuerce cambio inicial.
- [Dependencia de disponibilidad del proveedor social] -> Mitigar con timeouts, manejo de errores uniforme y fallback a login local.
- [Cambios de esquema sensibles en Identity] -> Mitigar con migraciones pequenas, revisadas y con plan de rollback.
- [Escalada de privilegios por mala configuracion de Authorize] -> Mitigar con politicas centralizadas y pruebas de autorizacion positiva/negativa.

## Migration Plan

1. Configurar DbContext de Identity y proveedor SQL Server con `api_ContextConnection`.
2. Generar migracion inicial de Identity (o incremental si ya existe contexto) y aplicarla a desarrollo.
3. Implementar seeding idempotente de roles (`Admin` y no-admin requerido por negocio) y admin por defecto.
4. Introducir endpoints de gestion de usuarios y decorarlos con `Authorize(Roles = "Admin")` o politica equivalente.
5. Integrar proveedores sociales configurables, mapear identidad externa a usuario local y emitir JWT.
6. Ejecutar suite de pruebas y pruebas manuales de autorizacion antes de promover.

Rollback:
- Revertir despliegue de API a build previa.
- Aplicar migracion `down` correspondiente si el cambio de esquema genera fallo operativo.
- Deshabilitar temporalmente login social desde configuracion en caso de incidencia externa.

## Open Questions

- Que proveedores sociales son obligatorios en la primera entrega (Google, Facebook, Apple, Microsoft)?
- El usuario admin por defecto debe crearse solo en desarrollo o tambien en otros entornos iniciales controlados?
- La gestion administrativa requiere activacion/desactivacion de usuarios o solo alta/edicion basica?
