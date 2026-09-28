## Why

La capa de persistencia ya agrupa los repositorios por modulo funcional, pero las configuraciones de Entity Framework Core permanecen en un directorio horizontal separado. Esta disposicion dificulta localizar todos los detalles de almacenamiento de una entidad y aumentara el coste de mantenimiento a medida que se incorporen modulos.

## What Changes

- Reorganizar los archivos de configuracion EF Core junto a los repositorios de su modulo funcional dentro de `Infrastructure/Persistence`.
- Agrupar el `ApplicationDbContext` y su factoria de diseno bajo una ubicacion explicita de contexto.
- Agrupar la factoria de conexiones de consultas bajo una ubicacion explicita de lectura.
- Mantener las migraciones de EF Core en una ubicacion global y estable asociada al unico `ApplicationDbContext`.
- Actualizar namespaces y el registro de dependencias para preservar la resolucion de las implementaciones existentes.

## Capabilities

### New Capabilities

Ninguna. Es un refactor interno sin comportamiento nuevo.

### Modified Capabilities

Ninguna. No cambian requisitos funcionales ni contratos de API.

## Impact

- Afecta exclusivamente a la estructura interna, namespaces y referencias de `IndaloAventurApi.Infrastructure`.
- No modifica el modelo EF Core, las migraciones, la base de datos, los contratos de Application ni las rutas HTTP.
- Requiere verificar compilacion y los tests de arquitectura e integracion relevantes.
