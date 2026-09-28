## 1. Reorganizacion del codigo de persistencia

- [x] 1.1 Mover `ApplicationDbContext` y su factoria de diseno a `Persistence/DbContext` y verificar que sus namespaces se resuelven en Infrastructure.
- [x] 1.2 Mover `SqlQueryConnectionFactory` a `Persistence/Querying` y verificar que la implementacion de `IQueryConnectionFactory` sigue registrada.
- [x] 1.3 Mover las configuraciones EF Core a los directorios de sus modulos junto a los repositorios y verificar que no queda el directorio horizontal `Persistence/Configurations`.
- [x] 1.4 Actualizar imports, namespaces y el registro de dependencias, y verificar que no quedan referencias a las ubicaciones anteriores.

## 2. Verificacion de regresion

- [ ] 2.1 Compilar la solucion y verificar que no hay errores de compilacion ni referencias de namespace rotas.
- [x] 2.2 Ejecutar los tests de arquitectura y verificar que se preservan los limites entre capas.
- [x] 2.3 Comprobar las migraciones pendientes sin generarlas y verificar que el cambio no introduce modificaciones de modelo EF Core.
