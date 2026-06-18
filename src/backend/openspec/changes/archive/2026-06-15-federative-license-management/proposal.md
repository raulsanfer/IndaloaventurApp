## Why

La aplicacion necesita gestionar licencias federativas como una capacidad propia del dominio en lugar de depender de un Excel externo. Es necesario convertir `docs/Tarifas_Federacion_2026_Estructurado.xlsx` en datos persistidos y relacionarlos con solicitudes de licencia por temporada para preparar una futura API operativa sobre un modelo estable.

## What Changes

- Crear un catalogo persistido de tarifas de licencias federativas a partir de la estructura del Excel 2026, usando `Temporada` como identificador de vigencia anual.
- Incorporar un modelo de solicitud de licencia federativa vinculado a un usuario, a una temporada y a una tarifa concreta del catalogo.
- Mantener el historico de solicitudes realizadas por el usuario entre distintos anios de vigencia sin sobrescribir solicitudes anteriores.
- Definir el ciclo de estado inicial de la solicitud con los valores `Pendiente`, `Confirmada` y `Cancelada`.
- Establecer reglas de unicidad para impedir multiples solicitudes de licencia para el mismo usuario y temporada.
- Preparar la persistencia EF Core y los contratos de repositorio necesarios para que en fases posteriores se expongan endpoints CQRS sobre este modelo.

## Capabilities

### New Capabilities
- `federative-license-catalog`: persistencia y consulta interna del catalogo de tarifas federativas derivado del Excel oficial por temporada.
- `federative-license-request-management`: gestion del modelo de solicitud de licencia federativa por usuario y temporada con estado de negocio.

### Modified Capabilities
Ninguna.

## Impact

- Domain: nuevas entidades/agregados y enum de estado para licencias federativas.
- Application: nuevos contratos de repositorio y casos de uso base para altas/cambios internos de solicitudes en fases posteriores.
- Infrastructure: configuraciones EF Core, `DbSet`, migracion SQL Server y mecanismo de carga inicial de tarifas 2026.
- Datos: el fichero `docs/Tarifas_Federacion_2026_Estructurado.xlsx` pasa a ser fuente de carga para una tabla catalogo persistida.
