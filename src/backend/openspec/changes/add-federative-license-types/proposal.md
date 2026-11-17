# Proposal

## Why

El indicador `MediaTemporada` limita las tarifas federativas a dos modalidades y no permite representar licencias por días. Se necesita un catálogo de tipos de licencia y fechas de cobertura en las solicitudes, conservando las tarifas, sus importes y todo el histórico actual.

## What Changes

- Incorporar `TipoLicencia` con identificador y nombre, precargado con `Completa`, `Media 01/07 - 31/12` y `Día`. Se consultará desde el cliente, sin gestión de altas, ediciones o bajas en el frontend ni endpoints de mantenimiento.
- Relacionar obligatoriamente cada `TarifaLicenciaFederativa` con un `TipoLicencia` y sustituir la unicidad basada en `MediaTemporada` por temporada, tipo, licencia y categoría.
- Migrar todas las tarifas existentes en su sitio: `MediaTemporada = 0` → `Completa`; `MediaTemporada = 1` → `Media 01/07 - 31/12`, preservando identificadores, importes, territorio y solicitudes vinculadas.
- **BREAKING**: sustituir `MediaTemporada` por `TipoLicenciaId` y `TipoLicenciaNombre` en contratos y consultas; cambiar el filtro `mediaTemporada` por `tipoLicenciaId` y retirar la columna una vez completada la conversión.
- Incorporar `FechaInicio` y `FechaFin` anulables en `SolicitudLicenciaFederativa`; exigir ambas para `Día`, con inicio menor o igual al fin y cobertura dentro de la temporada. Las solicitudes históricas conservarán ambas fechas nulas.
- **BREAKING**: permitir varias solicitudes `Día` por usuario y temporada y su convivencia con una única solicitud de `Completa` o `Media`; sustituir la restricción global actual sin perder la protección frente a altas concurrentes de solicitudes no diarias.
- Definir el flujo de selección temporada → tipo de licencia → nombre de licencia → categoría → precio de club, con fechas obligatorias para `Día` y reflejo en el histórico propio y en administración.

## Capabilities

### New Capabilities

- `federative-license-types`: catálogo predeterminado de tipos, identidad estable y consulta autenticada sin mantenimiento.

### Modified Capabilities

- `federative-license-catalog`: relación obligatoria con tipo, nueva unicidad y migración conservadora de datos.
- `federative-license-rates-query-api`: filtro por tipo y representación que permita resolver las selecciones y el precio.
- `federative-license-request-management`: fechas de cobertura y multiplicidad según tipo con protección de concurrencia.
- `member-federative-license-api`: alta y consultas propias con tipo y fechas, admitiendo varias solicitudes diarias.
- `admin-federative-license-status-management`: consultas y cambios de estado que incluyan y conserven el tipo y la cobertura.

## Impact

- Domain/Application: modelo de tipos, reglas de creación, repositorios, validadores, DTOs y contratos de consulta.
- Infrastructure: configuraciones EF Core, datos iniciales, índices, migración SQL Server y proyecciones Dapper de solicitudes propias y administrativas.
- API: nueva lectura de tipos y actualización de contratos de tarifas y solicitudes, OpenAPI y matriz de autorización; se mantienen las políticas basadas en `IsMember` y `Admin`.
- Frontend Blazor: `FederativeLicenseApiClient`, modelos, servicio, formulario e histórico consumen actualmente el booleano. La adaptación necesaria se detalla en `frontend-handoff.md`; deberá planificarse y ejecutarse en su propio proyecto OpenSpec antes de publicar ambos componentes coordinadamente.
- Verificación: dominio, handlers, integración HTTP, persistencia, migración real sobre SQL Server y concurrencia; no bastan `EnsureCreated` o el proveedor InMemory para probar la conversión de datos.

## Scope / Non-Goals

Se planifica el cambio del backend y el contrato de integración del frontend. No se implementa código en esta propuesta. Quedan fuera el mantenimiento visual de tipos, la importación o invención de tarifas de `Día`, pagos y cálculos de precio por duración. El importe mostrado procede de `PrecioClub` de la tarifa seleccionada, sin multiplicación automática por días.

## Risks

- El contrato actual se utiliza en el frontend: publicar la API sin adaptar el cliente rompería el flujo de solicitud.
- Eliminar el índice único actual sin sustituir su protección permitiría duplicar solicitudes no diarias ante concurrencia.
- Un rollback al booleano no puede representar `Día` ni varias solicitudes por temporada; se bloqueará si existen datos incompatibles, sin borrarlos ni reclasificarlos.
