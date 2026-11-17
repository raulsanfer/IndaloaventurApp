# Proposal

## Why

El contrato actualizado de `.vscode/openapi.json` y las reglas de `.vscode/licencias-federativas-tipos.md` sustituyen la modalidad booleana por tipos de licencia y permiten solicitudes diarias con cobertura. El cliente actual envía `mediaTemporada`, que la API rechaza con 400, y no representa los nuevos tipos ni fechas; necesita adaptarse antes del despliegue coordinado.

## What Changes

- **BREAKING**: sustituir `MediaTemporada` por `tipoLicenciaId` y `tipoLicenciaNombre` en modelos, consultas de tarifas e histórico.
- Consultar el catálogo autenticado de tipos: `Completa`, `Media 01/07 - 31/12` y `Día`, sin mantenimiento desde la UI.
- Presentar `Temporada`, `Tipo de licencia`, `Nombre de licencia`, `Categoría` y `PrecioClub`, con selecciones dependientes y protección frente a respuestas asíncronas obsoletas.
- Exigir fechas de inicio y fin para `Día`; aceptar cobertura de un solo día y validar orden y año. Mostrar estado vacío cuando todavía no existan tarifas diarias, sin inventar precios ni multiplicarlos por días.
- Conservar todas las solicitudes por identificador, aunque compartan temporada; mostrar tipo y cobertura en socio y administración, admitiendo fechas nulas migradas.
- Ajustar el mensaje 409 al límite de una solicitud no diaria por temporada, incluso cancelada, sin bloquear nuevas solicitudes diarias.

## Capabilities

### New Capabilities

Ninguna; se amplían las capacidades existentes.

### Modified Capabilities

- `frontend-member-federative-license-request-flow`: catálogo de tipos, filtro de tarifas, cobertura diaria, confirmación e histórico con múltiples solicitudes por temporada.
- `frontend-admin-federative-license-management`: visualizar tipo y cobertura y preservar la identidad y los datos de cada solicitud al actualizar su estado.

## Impact

Modelos y abstracciones de `SharedUI/Models/Licenses` y `Abstractions/Licenses`, ambos clientes HTTP, vistas de socio y administración, recursos españoles, SCSS global si es necesario y pruebas en `IndaloaventurApp.Frontend.Tests/Features/Licenses` con sus dobles. Revisar `docs/features/licenses.md`, `docs/architecture/http-api-clients.md` y estrategia de pruebas; actualizar XML de contratos públicos y verificar DocFX durante la implementación. No se prevén dependencias nuevas.

## Non-goals

Modificar backend o migraciones, cargar importes diarios reales, mantener tipos o tarifas, añadir reglas de solapamiento, ampliar temporadas disponibles o editar tipo/cobertura desde el cambio administrativo de estado.

## Risks

El contrato es incompatible con el cliente anterior: publicar API y cliente compatibles antes de reactivar las altas. El catálogo diario se inicia vacío; las pruebas usarán tarifas sintéticas. Mantener permisos actuales y autoridad del servidor sobre identidad, precio, cobertura y exclusividad; no deducir permisos de los selectores ni registrar tokens o datos personales.
