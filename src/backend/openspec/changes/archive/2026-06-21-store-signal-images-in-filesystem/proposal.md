## Why

Las fotos de `Signal` se almacenan hoy como binarios dentro de SQL Server, lo que encarece el tamano de la base de datos, complica copias/restauraciones y mezcla metadatos funcionales con contenido pesado de imagen. Queremos refactorizar ahora el modulo para conservar el comportamiento funcional de `Signals`, pero mover las imagenes a almacenamiento en fichero bajo una ruta configurada del servidor.

## What Changes

- Introducir una capacidad de almacenamiento de imagenes en filesystem, configurada por `appsettings`, para guardar, leer, reemplazar y borrar las fotos de `Signal` fuera de la base de datos.
- Refactorizar el modelo de persistencia de `Signal` para que SQL Server mantenga metadatos y referencias de fichero, en lugar de persistir `Foto1` y `Foto2` como `varbinary(max)`.
- Modificar el flujo de alta, edicion y lectura de imagenes de `Signal` para resolver las fotos a traves de una abstraccion de almacenamiento de archivos, manteniendo la autorizacion y los casos de uso actuales.
- Definir estrategia de migracion para trasladar fotos existentes desde base de datos a disco y limpiar el esquema relacional obsoleto.
- **BREAKING** Cambiar la persistencia fisica y el esquema de datos de `Signal`, con impacto en despliegue, migraciones y operacion del entorno.

## Capabilities

### New Capabilities
- `filesystem-media-storage`: almacenamiento en sistema de ficheros configurado para contenidos binarios de `Signal`, incluyendo reglas de ubicacion, nombres, reemplazo y eliminacion segura.

### Modified Capabilities
- `signal-binary-photos`: las fotos de `Signal` siguen expuestas por los casos de uso existentes, pero dejan de persistirse como binarios relacionales y pasan a resolverse mediante referencias de fichero.
- `repository-pattern-persistence`: la persistencia de `Signal` pasa a ser hibrida, manteniendo metadatos en SQL Server y contenido binario en filesystem gestionado por infraestructura.

## Impact

- API: endpoints de `Signals` y contrato del endpoint de imagenes, si se decide mantener o ajustar el transporte actual.
- Application: comandos, handlers y consultas de `Signal` con una nueva abstraccion para almacenamiento de medios.
- Domain: entidad `Signal` y/o value objects relacionados con referencias de imagen en lugar de bytes persistidos.
- Infrastructure: configuracion tipada, servicio de filesystem, migraciones EF Core y backfill de datos existentes.
- Operacion: necesidad de aprovisionar una ruta persistente, permisos de escritura y estrategia de backup para imagenes fuera de la base de datos.
- Tests: adaptacion de pruebas unitarias e integracion para cubrir almacenamiento, lectura y migracion de imagenes.
