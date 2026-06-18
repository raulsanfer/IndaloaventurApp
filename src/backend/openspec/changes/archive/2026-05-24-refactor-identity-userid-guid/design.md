## Context

El backend usa ASP.NET Identity con clave de usuario en `string`, mientras que el dominio y muchos contratos de aplicación trabajan con `Guid`. Esta diferencia obliga a parseos frecuentes, incrementa complejidad y puede provocar errores de conversión en autenticación, autorización y gestión de usuarios.

## Goals / Non-Goals

**Goals:**
- Migrar identidad a claves `Guid` en `AspNetUsers` y tablas relacionadas.
- Eliminar conversiones innecesarias de `string` a `Guid` en capas de aplicación e infraestructura.
- Mantener comportamiento funcional de login, registro, social login y administración de usuarios.
- Actualizar pruebas y migraciones para validar el nuevo esquema.

**Non-Goals:**
- Rediseñar flujos de autenticación o cambiar proveedores sociales.
- Cambiar el contrato externo de endpoints más allá de lo estrictamente necesario para soportar `Guid`.

## Decisions

1. Cambiar Identity a genéricos `IdentityUser<Guid>` y `IdentityRole<Guid>` en toda la solución.
2. Usar `ApplicationDbContext : IdentityDbContext<IdentityUser<Guid>, IdentityRole<Guid>, Guid>` como fuente única.
3. Conservar JWT con `sub` y `nameidentifier` serializados como texto de `Guid` para compatibilidad de claims.
4. Refactorizar servicios/DTOs internos para usar `Guid` fuertemente tipado donde corresponda.

## Risks / Trade-offs

- [Riesgo] Migración de Identity puede romper datos existentes de usuarios. -> Mitigación: migración explícita con recreación de esquema Identity en entornos de desarrollo y validación en tests.
- [Riesgo] Cambios transversales en tests e integración. -> Mitigación: ejecutar suite completa y ajustar factories/seeders.
- [Trade-off] Mayor esfuerzo inicial de refactor, menor deuda técnica y más seguridad de tipos a medio plazo.

## Migration Plan

1. Refactor de tipos de Identity en infraestructura y servicios.
2. Ajuste de contratos de aplicación y handlers dependientes.
3. Generación de migración EF para esquema Identity Guid.
4. Actualización de tests unitarios e integración.
5. Ejecución completa de tests.

## Open Questions

- ¿Se necesita estrategia de migración de datos legacy en producción o basta con recreación de esquema en esta fase?
