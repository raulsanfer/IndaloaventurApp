## 1. Domain Refactor

- [x] 1.1 Refactorizar `Cargo` para creación sin parámetro `Id` y mantener validaciones de descripción.
- [x] 1.2 Refactorizar `SignalType` para creación sin parámetro `Id` y mantener validaciones de nombre/icono.
- [x] 1.3 Ajustar pruebas unitarias de dominio para reflejar generación de `Id` en persistencia.

## 2. Application and API Contracts

- [x] 2.1 Actualizar comandos de creación (`CreateCargoCommand`, `CreateSignalTypeCommand`) para eliminar `Id` de entrada.
- [x] 2.2 Actualizar handlers de creación para usar las nuevas factorías de dominio.
- [x] 2.3 Actualizar controladores `CargosController` y `SignalTypesController` para recibir payloads sin `Id` y devolver el `Id` generado.
- [x] 2.4 Revisar procesos relacionados (`Users` con `CargoId`, `Signals` con `Tipo`) para asegurar coherencia tras la creación autoincremental.

## 3. Persistence and Migration

- [x] 3.1 Configurar EF Core para `ValueGeneratedOnAdd` en `Cargo.Id` y `SignalType.Id`.
- [x] 3.2 Generar migración SQL Server para habilitar autoincremento/identidad en ambas tablas.
- [x] 3.3 Verificar que relaciones FK (`AspNetUsers.CargoId`, `Signals.Tipo`) continúan funcionando sin cambios de semántica.

## 4. Testing and Verification

- [x] 4.1 Actualizar pruebas unitarias de aplicación para nuevas firmas de comandos y flujos de creación.
- [x] 4.2 Actualizar pruebas de integración que usan IDs hardcodeados para capturar IDs generados dinámicamente.
- [x] 4.3 Ejecutar pruebas de dominio, aplicación e integración afectadas.
- [x] 4.4 Ejecutar suite completa del backend y corregir regresiones antes de cerrar tareas.
