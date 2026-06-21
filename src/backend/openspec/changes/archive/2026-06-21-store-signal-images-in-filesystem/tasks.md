## 1. Modelo y configuracion de almacenamiento

- [x] 1.1 Definir la configuracion tipada para la raiz de almacenamiento de imagenes de `Signal` y validar su arranque
- [x] 1.2 Diseñar y añadir la abstraccion de almacenamiento de medios para guardar, leer, reemplazar y borrar fotos en filesystem
- [x] 1.3 Refactorizar el modelo de `Signal` y su persistencia para almacenar referencias de fichero en lugar de binarios en SQL Server

## 2. Migracion y persistencia hibrida

- [x] 2.1 Crear la migracion EF Core para sustituir las columnas binarias de fotos por referencias persistidas
- [x] 2.2 Implementar la estrategia de backfill/migracion de fotos existentes desde base de datos a filesystem
- [x] 2.3 Asegurar limpieza compensatoria y consistencia entre escritura de ficheros y persistencia relacional en create/update

## 3. Flujos de aplicacion y API

- [x] 3.1 Adaptar comandos, handlers y consultas de `Signal` para usar el almacenamiento de ficheros manteniendo el comportamiento funcional acordado
- [x] 3.2 Revisar el endpoint de imagenes y sus contratos para resolver contenido desde filesystem
- [x] 3.3 Ajustar documentacion/configuracion operativa sobre ruta persistente, permisos y backup

## 4. Pruebas y verificacion

- [x] 4.1 Actualizar pruebas unitarias e integracion de `TrailSignals` para el nuevo almacenamiento de imagenes
- [x] 4.2 Ejecutar `dotnet test IndaloAventurApi.sln` y confirmar la regresion completa antes de marcar el cambio como implementable
