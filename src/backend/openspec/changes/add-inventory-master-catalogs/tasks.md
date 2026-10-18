## 1. Dominio y contratos de persistencia

- [x] 1.1 Crear el modulo `Domain/Inventario` con la regla reutilizable de normalizacion y validacion de nombres; verificar con pruebas de dominio que recorta espacios y rechaza vacios, controles no permitidos y longitudes superiores a 120.
- [x] 1.2 Implementar el agregado `InventarioCategoria`, su relacion padre opcional y sus invariantes de identidad/nombre; verificar con pruebas de dominio que una categoria y una subcategoria validas se pueden construir.
- [x] 1.3 Implementar el agregado `InventarioEstado` usando la misma regla de nombre; verificar con pruebas de dominio que no duplica reglas de validacion y conserva el nombre normalizado.
- [x] 1.4 Definir las abstracciones de repositorio de comando de categorias y estados, incluyendo las comprobaciones necesarias de existencia e hijos; verificar que Application no referencia Infrastructure mediante las pruebas de arquitectura existentes.

## 2. Casos de uso CQRS

- [x] 2.1 Implementar comandos y validadores de alta, actualizacion y borrado de categorias; verificar con pruebas de Application los casos de padre inexistente, autoreferencia, padre subcategoria y movimiento que crearia un tercer nivel.
- [x] 2.2 Implementar el manejo de unicidad, inexistencia y borrado de categoria con hijos; verificar con pruebas de Application los resultados de conflicto, no encontrado y ausencia de borrado en cascada.
- [x] 2.3 Implementar DTOs y consultas `GetAll`/`GetById` de categorias a traves de una abstraccion de lectura; verificar con pruebas que el arbol resultante contiene solo dos niveles y queda ordenado por nombre.
- [x] 2.4 Implementar comandos, validadores y DTOs de alta, actualizacion, borrado y consulta de estados; verificar con pruebas de Application los flujos correctos, nombre duplicado, validacion y no encontrado.
- [x] 2.5 Asegurar que todos los mensajes de validacion y errores funcionales nuevos quedan en espanol y se mapean a los resultados esperados; verificar las aserciones de mensajes y codigos en las pruebas de Application.

## 3. Infraestructura y migracion

- [x] 3.1 Incorporar los `DbSet` y configuraciones EF Core de `InventarioCategorias` e `InventarioEstados`; verificar mediante pruebas de mapeo o migracion que las claves son autogeneradas, `Nombre` es obligatorio con longitud 120 y la FK de categorias usa borrado restrictivo.
- [x] 3.2 Configurar los indices unicos sobre la clave normalizada de estado y sobre padre más clave normalizada de categoria; verificar contra SQL Server aislado que se rechazan duplicados concurrentes o por diferencias de mayusculas/espacios.
- [x] 3.3 Implementar los repositorios EF Core de comando y registrarlos en la inyeccion de dependencias; verificar con pruebas de infraestructura que crear, actualizar y borrar una hoja confirma los cambios correctamente.
- [x] 3.4 Implementar los servicios Dapper parametrizados de consulta y registrarlos en DI; verificar con pruebas de integracion que las listas y detalles devuelven las proyecciones esperadas sin materializar agregados.
- [x] 3.5 Generar la migracion EF Core descriptiva y actualizar el snapshot; verificar que `dotnet ef migrations list --project src/IndaloAventurApi.Infrastructure --startup-project src/IndaloAventurApi.Api` la detecta y que su `Up`/`Down` solo afecta a las tablas nuevas.

## 4. Superficie HTTP y autorizacion

- [x] 4.1 Crear `InventarioCategoriasController` con contratos de solicitud/respuesta, rutas de lista y detalle, y respuestas `200`, `201`, `204`, `400`, `404` y `409` segun el contrato; verificar mediante pruebas de integracion los flujos CUD y de lectura.
- [x] 4.2 Crear `InventarioEstadosController` con los mismos patrones de contrato para el maestro plano; verificar mediante pruebas de integracion los flujos CUD y de lectura.
- [x] 4.3 Aplicar `Authenticated` a las lecturas y `Admin` unicamente al CUD de ambos controladores; actualizar `ApiAuthorizationMatrix` y verificar mediante pruebas de integracion los rechazos anonimos y los `403` para usuarios autenticados sin rol `Admin`.
- [ ] 4.4 Confirmar el mapeo de validacion, no encontrado, conflicto de jerarquia/duplicidad y violaciones de indice a ProblemDetails en espanol; verificar los codigos y contenidos de respuesta mediante pruebas de integracion.

## 5. Verificacion integral

- [x] 5.1 Ejecutar `dotnet test IndaloAventurApi.sln` y corregir cualquier regresion hasta que la suite completa finalice correctamente.
- [x] 5.2 Ejecutar `dotnet build IndaloAventurApi.sln --no-restore` y `openspec validate add-inventory-master-catalogs --strict`; verificar compilacion sin errores y especificacion valida antes de marcar tareas como completadas.
