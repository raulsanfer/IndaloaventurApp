## Context

Vease `proposal.md` para la motivacion. `ApplicationDbContext` aplica todas las configuraciones desde el ensamblado de Infrastructure, por lo que las configuraciones EF Core pueden cambiar de directorio sin cambiar el modelo. Actualmente, los repositorios estan organizados por modulo mientras que todas las configuraciones residen en `Persistence/Configurations`.

## Goals / Non-Goals

**Goals:**

- Colocar cada configuracion EF Core junto al repositorio de su modulo funcional.
- Hacer explicitas las responsabilidades transversales con los directorios `DbContext` y `Querying`.
- Mantener `Migrations` como el historico unico y global del esquema de `ApplicationDbContext`.
- Preservar el comportamiento, los contratos de Application, el modelo EF y la salida de las migraciones.

**Non-Goals:**

- No introducir unit of work ni modificar los contratos de repositorio.
- No cambiar el uso actual de EF Core para comandos ni Dapper para consultas.
- No crear proyectos adicionales, dividir el contexto, regenerar migraciones ni modificar la base de datos.

## Decisions

### Organizacion vertical por modulo

Cada modulo de persistencia contendra su configuracion EF Core y sus repositorios:

```text
Persistence/
  ClubPositions/        CargoConfiguration, CargoRepository
  FichasSocio/          FichaSocioConfiguration, FichaSocioRepository
  LicenciasFederativas/ SolicitudLicenciaFederativaConfiguration,
                         SolicitudLicenciaFederativaRepository,
                         TarifaLicenciaFederativaConfiguration,
                         TarifaLicenciaFederativaRepository
  Phonebook/            FichaContactoConfiguration, FichaContactoRepository
  TrailSignals/         SignalConfiguration, SignalRepository,
                         SignalCommentConfiguration, SignalTypeConfiguration,
                         SignalTypeRepository
```

Esto sigue la organizacion vertical que ya usan los repositorios y reduce la distancia entre el mapeo y su implementacion. Se descarta conservar una carpeta global `Configurations` porque escala peor para navegar por un modulo. Se descarta un proyecto por modulo porque no aporta aislamiento de dependencias para este tamano de aplicacion.

### Detalles transversales separados

`ApplicationDbContext` y `ApplicationDbContextFactory` se moveran a `Persistence/DbContext`; `SqlQueryConnectionFactory` a `Persistence/Querying`; `Migrations` permanecera directamente en `Persistence/Migrations`.

Las migraciones no se agrupan por modulo: representan cambios ordenados sobre un unico modelo y contexto, y EF Core las descubre mediante el ensamblado, no mediante una agrupacion funcional.

### Actualizacion mecanica de referencias

Se actualizaran los namespaces, imports de `DependencyInjection` y las referencias directas al contexto en Infrastructure. `ApplyConfigurationsFromAssembly` se conserva tal cual, por lo que el descubrimiento de configuraciones permanece basado en el ensamblado y no en la ruta fisica.

## Risks / Trade-offs

- [Un namespace o registro de DI sin actualizar puede romper compilacion] -> Mitigar con busqueda de referencias residuales y compilacion completa.
- [Mover archivos podria afectar la deteccion de EF] -> Mitigar conservando el ensamblado, el tipo de configuracion y verificando migraciones pendientes sin generarlas.
- [El refactor puede confundirse con un cambio de esquema] -> Mitigar sin editar clases de entidad, `OnModelCreating` ni archivos de migracion.

## Migration Plan

El cambio se despliega como una actualizacion de binarios sin migracion de base de datos. El rollback consiste en revertir los movimientos y los cambios de namespace/importacion del mismo commit; la base de datos no requiere ninguna accion.
