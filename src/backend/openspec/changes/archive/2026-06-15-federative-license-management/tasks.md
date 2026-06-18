## 1. Modelo de dominio

- [x] 1.1 Crear el modelo de catalogo `TarifaLicenciaFederativa` con los campos de temporada, licencia, categoria, importes y territorio.
- [x] 1.2 Crear el agregado `SolicitudLicenciaFederativa` con `UserId`, `Temporada`, `TarifaLicenciaFederativaId` y estado `Pendiente`/`Confirmada`/`Cancelada`.
- [x] 1.3 Definir invariantes de dominio para historico de solicitudes, unicidad funcional por usuario y temporada y coherencia de temporada entre solicitud y tarifa.

## 2. Persistencia e infraestructura

- [x] 2.1 Anadir contratos de repositorio y exponer los nuevos `DbSet` en `ApplicationDbContext`.
- [x] 2.2 Crear configuraciones EF Core para catalogo y solicitudes con indices y restricciones unicas.
- [x] 2.3 Generar la migracion SQL Server con las tablas nuevas y la carga inicial de las 25 tarifas del Excel 2026.

## 3. Datos fuente y verificacion

- [x] 3.1 Transcribir y validar los registros de `docs/Tarifas_Federacion_2026_Estructurado.xlsx` en el mecanismo de seed o migracion elegido, asignando `Temporada = 2026`.
- [x] 3.2 Crear pruebas de persistencia o integracion que verifiquen el catalogo cargado, la unicidad por temporada y la restriccion de una solicitud por usuario y temporada.
- [x] 3.3 Ejecutar `dotnet test` y verificar que la solucion queda lista para una siguiente fase de endpoints.
