## 1. Dominio y contratos de escritura

- [ ] 1.1 Integrar la politica comun de validacion textual aplicable a descripcion, marca/modelo, procedencia, observaciones y numero de serie; verificar con pruebas de dominio que normaliza los valores permitidos y rechaza vacios obligatorios, controles y excesos de longitud sin duplicar reglas por campo.
- [ ] 1.2 Crear el agregado `InventarioMaterial` con sus campos, identificador autogenerado e invariantes de cantidad positiva, fecha no futura y numero de serie solo para una unidad; verificar con pruebas de dominio los casos validos y cada rechazo.
- [ ] 1.3 Definir `IInventarioMaterialRepository` para carga, alta, eliminacion, comprobacion de numero de serie y confirmacion de cambios; verificar mediante pruebas de arquitectura que Application no referencia Infrastructure.

## 2. Casos de uso CQRS

- [ ] 2.1 Implementar comandos y validadores de creacion y actualizacion que comprueben categoria y estado existentes, fecha, cantidad y numero de serie; verificar con pruebas de Application los flujos correctos, maestros inexistentes, fecha futura y cantidad no positiva.
- [ ] 2.2 Implementar la deteccion funcional de numeros de serie duplicados en altas y cambios, incluida la traduccion uniforme de una violacion concurrente de indice; verificar con pruebas de Application que devuelve conflicto y no modifica el material existente.
- [ ] 2.3 Implementar el comando de borrado y el resultado de no encontrado; verificar con pruebas de Application que elimina solo el registro solicitado y que un identificador inexistente no altera datos.
- [ ] 2.4 Definir DTOs y contratos de lectura, y crear `GetAll` y `GetById` para proyectar todos los datos del material junto con categoria, padre de categoria cuando exista y estado; verificar con pruebas que el listado tiene orden estable y el detalle inexistente se resuelve como no encontrado.

## 3. Persistencia, consultas y migracion

- [ ] 3.1 Incorporar el `DbSet` y la configuracion EF Core de `InventarioMateriales`, con columnas, longitudes, tipo `date`, claves foraneas restrictivas e indices de categoria, estado y numero de serie normalizado unico filtrado; verificar el modelo o una migracion contra SQL Server aislado.
- [ ] 3.2 Implementar el repositorio EF Core de comandos, registrarlo en DI y traducir restricciones de persistencia a errores funcionales; verificar con pruebas de infraestructura altas, actualizaciones, borrados y unicidad de numero de serie.
- [ ] 3.3 Implementar y registrar las consultas Dapper parametrizadas para lista y detalle con joins de categoria, categoria padre y estado; verificar con pruebas de integracion que devuelve las proyecciones sin materializar el agregado y sin concatenar SQL de entrada.
- [ ] 3.4 Ajustar los flujos de borrado de categorias y estados para devolver conflicto cuando una clave foranea de material impida el borrado; verificar con pruebas de integracion que se mantienen maestro y material.
- [ ] 3.5 Generar la migracion EF Core descriptiva y actualizar el snapshot; verificar con `dotnet ef migrations list --project src/IndaloAventurApi.Infrastructure --startup-project src/IndaloAventurApi.Api` que se detecta y que `Up` y `Down` solo afectan a la tabla de material, sus indices y sus claves foraneas.

## 4. API y autorizacion

- [ ] 4.1 Crear `InventarioMaterialesController` y los contratos HTTP de lista, detalle, alta, edicion y borrado, con respuestas `200`, `201`, `204`, `400`, `404` y `409`; verificar mediante pruebas de integracion el CRUD completo y los ProblemDetails en Espanol.
- [ ] 4.2 Proteger el controlador con `Authenticated` y sus operaciones CUD con `Admin`, y actualizar `ApiAuthorizationMatrix`; verificar mediante pruebas de integracion los rechazos anonimos y los `403` para usuarios autenticados sin rol `Admin`.
- [ ] 4.3 Verificar extremo a extremo que un numero de serie duplicado y la eliminacion de un maestro referenciado devuelven `409 Conflict` sin detalles de SQL; cubrir ambos casos en pruebas de integracion HTTP.

## 5. Verificacion integral

- [ ] 5.1 Ejecutar `dotnet test IndaloAventurApi.sln` y corregir regresiones hasta que toda la suite finalice correctamente.
- [ ] 5.2 Ejecutar `dotnet build IndaloAventurApi.sln --no-restore` y `openspec validate add-inventory-item-management --strict`; verificar compilacion sin errores y especificacion valida antes de marcar tareas como completadas.
