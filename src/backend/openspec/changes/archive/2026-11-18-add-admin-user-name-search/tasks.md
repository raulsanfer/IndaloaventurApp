## 1. Contrato de API y Application

- [x] 1.1 Añadir `nombre` opcional a `UsersController.List` y `ListManagedUsersQuery`, conservando `email` y la respuesta actual. Actualizar los comentarios XML en español con la semántica de ambos filtros.
- [x] 1.2 Normalizar ambos filtros en `ListManagedUsersQueryHandler` con recorte de espacios extremos y conversión de valores nulos, vacíos o de solo espacios a ausencia de filtro.
- [x] 1.3 Extender `IIdentityService.ListUsersAsync` y actualizar su implementación y todos los dobles de pruebas afectados; comprobar consumidores con `rg -n 'ListUsersAsync|new IdentityService|ListManagedUsersQuery' src tests`.

## 2. Consulta de nombres y apellidos

- [x] 2.1 Inyectar `ApplicationDbContext` en `IdentityService` y aplicar, únicamente cuando `nombre` sea efectivo, una subconsulta por `FichaSocio.UserId == Usuario.Id` con coincidencia parcial literal en `Nombre` OR `Apellidos`, sin distinguir mayúsculas/minúsculas.
- [x] 2.2 Combinar el filtro `email` existente con el nuevo filtro mediante AND, sin exigir `IsMember` ni roles al usuario buscado, y excluir cuentas sin ficha solo cuando haya filtro de nombre efectivo.
- [x] 2.3 Mantener los filtros parametrizados y ejecutados en base de datos antes de materializar la colección, sin cargar fichas completas ni consultar una ficha por cada usuario; conservar orden por email, contrato, cancelación y política administrativa.

## 3. Pruebas de Application

- [x] 3.1 Ampliar `UserManagementHandlersTests` para verificar propagación y recorte de `nombre`, nulo/vacío/espacios, ambos filtros independientes y combinados, y conservación del resultado del servicio. Ejecutar `dotnet test tests/IndaloAventurApi.Application.Tests/IndaloAventurApi.Application.Tests.csproj` antes de marcar las tareas de Application verificadas.

## 4. Pruebas de integración y aceptación

- [x] 4.1 Añadir pruebas HTTP con fichas vinculadas para coincidencias al principio, en el interior, al final y completas, tanto en nombre como en primer y segundo apellido, con exclusión de usuarios sin coincidencia.
- [x] 4.2 Verificar mayúsculas mezcladas, espacios extremos, filtros ausentes/vacíos/solo espacios y caracteres `%`, `_`, `[` tratados literalmente.
- [x] 4.3 Verificar AND al combinar `email` y `nombre`, incluyendo usuarios que coincidan únicamente con uno de ellos y búsquedas sin resultados que devuelvan `200 OK` con `[]`.
- [x] 4.4 Verificar usuarios sin ficha en el listado completo y en búsqueda solo por email, su exclusión con `nombre` activo, y coincidencia por ficha aunque `IsMember` sea `false`.
- [x] 4.5 Verificar orden por email, una sola aparición por usuario, los campos de respuesta existentes y regresión de las búsquedas parciales por email.
- [x] 4.6 Verificar `401` sin autenticación y `403` sin autorización administrativa al consultar con `nombre`.
- [x] 4.7 Añadir y ejecutar una prueba de integración con SQL Server en una base aislada que confirme traducción y ejecución del filtro, coincidencia interior sin distinguir mayúsculas, combinación con email y tratamiento literal de comodines. Registrar el resultado; si SQL Server no está disponible, mantener esta tarea pendiente y reportar la limitación.

## 5. Verificación final

- [x] 5.1 Ejecutar `dotnet test tests/IndaloAventurApi.IntegrationTests/IndaloAventurApi.IntegrationTests.csproj` y `dotnet test IndaloAventurApi.sln`; comprobar las regresiones y registrar cualquier prueba omitida o dependiente de servicios externos.
- [x] 5.2 Validar el cambio con `openspec validate add-admin-user-name-search --strict` y comprobar `openspec status --change add-admin-user-name-search`. Marcar las tareas de implementación como completadas únicamente cuando sus pruebas hayan pasado.

## Resultados de verificación (2026-11-18)

- `dotnet test tests/IndaloAventurApi.Application.Tests/IndaloAventurApi.Application.Tests.csproj --no-restore --verbosity minimal`: 101 pruebas correctas, ninguna omitida.
- Pruebas específicas de la búsqueda: 25 casos HTTP en `ManagedUserNameSearchTests` y 1 caso de integración real en `ManagedUserNameSearchSqlServerTests`, todos correctos. La prueba SQL Server utiliza una base aislada que elimina al finalizar.
- `dotnet test tests/IndaloAventurApi.IntegrationTests/IndaloAventurApi.IntegrationTests.csproj --no-build --no-restore --verbosity minimal`: 166 pruebas correctas, ninguna omitida.
- `dotnet test IndaloAventurApi.sln --no-restore --verbosity minimal`: 311 pruebas correctas (101 de Application, 166 de integración, 41 de Domain y 3 de arquitectura), sin fallos ni omisiones.
- Las pruebas de SQL Server y las suites que las incluyen se ejecutaron fuera del sandbox para permitir la conexión cifrada; no fue necesario cambiar su configuración de seguridad.
- `openspec validate add-admin-user-name-search --strict`: correcto. CLI ejecutado mediante Node desde `.vscode/npm-cache/_npx/abab5bd700860149/node_modules/@fission-ai/openspec/bin/openspec.js`; `status` confirma los cuatro artefactos disponibles.
