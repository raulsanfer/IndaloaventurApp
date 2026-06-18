## Context

El endpoint administrativo `GET /api/users` devuelve hoy todos los usuarios y el servicio `IIdentityService.ListUsersAsync` no recibe ningun criterio de busqueda. La necesidad es permitir localizar usuarios por correo sin romper el contrato actual para consumidores que siguen necesitando el listado completo.

## Goals / Non-Goals

**Goals:**
- Permitir filtrar usuarios por email desde el endpoint administrativo existente.
- Mantener `GET /api/users` compatible cuando no se informe filtro.
- Hacer que la busqueda sea predecible para administracion: sin sensibilidad a mayusculas/minusculas y con recorte de espacios extremos.
- Seguir devolviendo resultados ordenados por email ascendente.

**Non-Goals:**
- Introducir paginacion, ordenacion dinamica o filtros por otros campos.
- Crear un endpoint nuevo independiente del listado actual.
- Cambiar el modelo de respuesta `ManagedUserDto`.

## Decisions

1. El filtro se expondra como query string opcional `email` en `GET /api/users`.
- Rationale: extiende el endpoint ya existente sin romper clientes actuales y sigue una semantica natural de listado filtrable.
- Alternative: crear `GET /api/users/search`. Se descarta por duplicar responsabilidad del listado.

2. La coincidencia por email sera parcial y case-insensitive usando el email normalizado del usuario cuando el proveedor de datos lo permita.
- Rationale: facilita localizar cuentas con fragmentos del correo y reduce friccion en la UI administrativa.
- Alternative: coincidencia exacta. Se descarta porque obliga a conocer el email completo y ofrece menos valor operativo.

3. Si el parametro `email` llega vacio o solo con espacios, el sistema lo tratara como ausencia de filtro.
- Rationale: evita falsos vacios por errores menores del cliente y preserva el comportamiento actual.

4. Cuando no existan coincidencias, la API respondera `200 OK` con una coleccion vacia.
- Rationale: sigue la semantica de un listado filtrado y simplifica consumo del frontend.

## Risks / Trade-offs

- [Riesgo] La busqueda parcial puede traducirse a consultas menos eficientes si el volumen de usuarios crece mucho.
  Mitigacion: mantener el filtro sobre `IQueryable` en base de datos y revisar paginacion en un cambio futuro si hiciera falta.

- [Trade-off] Un filtro parcial puede devolver varias coincidencias cuando el administrador esperaba una sola.
  Mitigacion: documentar que se trata de un listado filtrado y no de una resolucion unica.

## Migration Plan

1. Extender la query y el controlador para aceptar `email` opcional.
2. Actualizar `IIdentityService` e `IdentityService` para aplicar el filtro sobre la consulta de usuarios.
3. Anadir pruebas de aplicacion/integracion para filtro por email, sin filtro y sin resultados.
4. Desplegar sin migraciones de base de datos.

Rollback:
- Revertir la aceptacion del parametro `email` y restaurar el listado sin filtros.

## Open Questions

- Ninguna por ahora; se asume que la UI administrativa consumira el mismo endpoint con query string opcional.
