## 1. API y aplicacion

- [x] 1.1 Extender `GET /api/users` y `ListManagedUsersQuery` para aceptar un filtro opcional `email`.
- [x] 1.2 Propagar el filtro a `IIdentityService` y aplicar la busqueda por email manteniendo el orden actual.
- [x] 1.3 Tratar valores nulos, vacios o con espacios como ausencia de filtro.

## 2. Pruebas

- [x] 2.1 Agregar prueba de aplicacion o integracion para listar todos los usuarios cuando no se informe `email`.
- [x] 2.2 Agregar prueba para filtrar usuarios por coincidencia parcial de email sin sensibilidad a mayusculas/minusculas.
- [x] 2.3 Agregar prueba para devolver una coleccion vacia cuando no existan coincidencias.

## 3. Verificacion

- [x] 3.1 Ejecutar `dotnet test` y verificar que no hay regresiones.
- [x] 3.2 Validar manualmente `GET /api/users?email=<valor>` y `GET /api/users` sin filtro.
