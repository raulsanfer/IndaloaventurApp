## Context

El backend actual no dispone de una entidad de perfil de socio separada del usuario de Identity, ni de casos de uso para consulta/edicion autocontenida. El proyecto usa Clean Architecture + CQRS, autenticacion JWT y mensajes user-facing en espanol. La nueva funcionalidad debe respetar el control de acceso por usuario autenticado y rol Administrador.

## Goals / Non-Goals

**Goals:**
- Definir un modelo `FichaSocio` con relacion 1:1 respecto a `User` por `UserId`.
- Permitir consulta y edicion de ficha por el propio usuario autenticado.
- Permitir consulta y edicion de cualquier ficha por usuarios con rol Administrador.
- Aplicar validaciones de entrada para campos personales y consentimientos.
- Encajar la solucion en el patron actual (Domain + Application + Infrastructure + Api).

**Non-Goals:**
- No se implementa gestion de multiples fichas por usuario.
- No se incorpora versionado historico de cambios de ficha.
- No se añaden flujos de aprobacion manual ni auditoria avanzada fuera del logging existente.

## Decisions

1. Entidad dedicada `FichaSocio` en dominio con clave propia y `UserId` unico.
Rationale: separa responsabilidades entre identidad/autenticacion y datos de socio, facilita evolucion independiente.
Alternativa considerada: ampliar tabla de usuario de Identity con campos de ficha. Se descarta por acoplamiento y menor mantenibilidad.

2. Casos de uso CQRS: `GetFichaSocioQuery` y `UpdateFichaSocioCommand`.
Rationale: mantiene coherencia con arquitectura existente y separa lectura/escritura.
Alternativa considerada: servicio CRUD unico. Se descarta por romper convencion del proyecto.

3. Regla de autorizacion central: acceso permitido si `request.UserId == authenticatedUserId` o rol `Administrador`.
Rationale: simplifica verificaciones, reutilizable en handlers/controlador.
Alternativa considerada: politicas por endpoint sin comprobacion de ownership en aplicacion. Se descarta por riesgo de bypass al reutilizar handlers.

4. Validacion explicita por FluentValidation (o mecanismo equivalente actual) para formato y obligatoriedad.
Rationale: feedback temprano y contratos consistentes.
Alternativa considerada: validaciones solo en capa dominio/EF. Se descarta por menor calidad de errores API.

5. Endpoints REST bajo recurso de ficha de socio, protegidos por JWT.
Rationale: contrato claro para app movil y alineado con estilo API actual.
Alternativa considerada: incrustar datos en endpoints de usuario. Se descarta por mezclar bounded contexts.

## Risks / Trade-offs

- [Reglas de formato ambiguas para DNI/telefono/codigo postal] -> Mitigacion: definir validaciones minimas seguras ahora (no vacio, max length, formato basico) y cerrar reglas exactas antes de merge final.
- [Migracion de base de datos en entorno con datos existentes] -> Mitigacion: crear migracion idempotente, indice unico por `UserId` y probar en base de staging.
- [Escalada de privilegios por chequeo incompleto de ownership] -> Mitigacion: pruebas de autorizacion negativas en capa aplicacion e integracion API.
- [Actualizaciones parciales inconsistentes] -> Mitigacion: contrato de actualizacion explicito (PUT completo o PATCH definido) y pruebas de regresion.

## Migration Plan

1. Crear entidad de dominio y configuracion EF con relacion 1:1 e indice unico por `UserId`.
2. Generar y aplicar migracion SQL.
3. Implementar repositorio/consultas de ficha.
4. Implementar query/command + validadores + mapeos DTO.
5. Exponer endpoints API protegidos y mapear errores a ProblemDetails.
6. Ejecutar pruebas unitarias e integracion.
7. Desplegar junto con migracion; rollback mediante revert de migracion y despliegue de version previa.

## Open Questions

- Confirmar restriccion de unicidad y formato exacto de `DNI` (ej. 8 digitos + letra mayuscula).
- Confirmar normalizacion de `Tlf` (prefijo internacional permitido, longitud minima/maxima).
- Confirmar longitudes maximas para `Direccion`, `Poblacion`, `Provincia`, `Alergias`.
- Confirmar si `Email` debe coincidir obligatoriamente con email de `User` o puede ser distinto.
- Confirmar si `Fecha_nacimiento` debe rechazar fechas futuras y edad minima.