# Integración con el frontend Blazor

Este documento define la adaptación necesaria en `../blazorapp`. No modifica sus especificaciones canónicas ni sustituye una propuesta en ese proyecto. La publicación del cambio de contrato del backend depende de completar y verificar esta adaptación.

## Contrato que consumirá el cliente

- `GET /api/licencias-federativas/tipos`: devuelve `id` y `nombre` para los tres tipos predeterminados, en orden completa, media y día.
- `GET /api/licencias-federativas/tarifas?temporada=2026&tipoLicenciaId=3`: filtra por temporada y tipo. Cada tarifa devuelve `tipoLicenciaId` y `tipoLicenciaNombre` en lugar de `mediaTemporada` y conserva sus identificadores, nombre de licencia, categoría, territorio e importes.
- `POST /api/licencias-federativas/me/solicitudes`: recibe `temporada`, `tarifaLicenciaFederativaId`, `fechaInicio` y `fechaFin`. Las fechas son `yyyy-MM-dd`, obligatorias en `Día` y anulables en completa/media. El cliente no envía usuario, clasificación alternativa ni precio.
- El alta, los listados propios/administrativos, el detalle propio y la respuesta al cambio administrativo de estado devuelven `tipoLicenciaId`, `tipoLicenciaNombre`, `fechaInicio` y `fechaFin`.
- Una temporada puede contener varias solicitudes diarias y una solicitud no diaria. Un `409` ya no significa que toda solicitud de esa temporada esté prohibida: limita la duplicación de completa/media.
- El filtro `mediaTemporada` deja de estar soportado y se rechaza con `400`. No se debe publicar el cliente actual contra esta API sin adaptación.

Los identificadores predeterminados son `1 = Completa`, `2 = Media 01/07 - 31/12` y `3 = Día`. Los nombres se presentan desde el catálogo, sin reconstruirlos a partir de un booleano. Una futura ampliación de tipos requerirá definir su comportamiento de cobertura en un cambio posterior.

## Comportamiento del formulario

1. Presentar los campos en orden `Temporada`, `Tipo de licencia`, `Nombre de licencia`, `Categoría` y precio. Mantener el criterio existente de temporadas disponibles; no ampliar temporadas como parte de esta adaptación.
2. Cargar el catálogo de tipos sin ofrecer controles para crearlos, editarlos o eliminarlos. Se puede mantener `Completa` como selección inicial, sin alterar el orden visual solicitado.
3. Una vez elegidas temporada y tipo, consultar las tarifas de esa combinación. Deduplicar las opciones de licencia y las categorías disponibles para la licencia elegida.
4. Resolver un único identificador de tarifa con la combinación completa y mostrar su `PrecioClub`. Conservar `PrecioIndependiente` como dato separado. No multiplicar por el número de días.
5. Para `Día`, mostrar y exigir `Fecha de inicio` y `Fecha de fin`, admitiendo un solo día con fechas iguales. Validar orden y pertenencia a la temporada; la API repetirá estas comprobaciones.
6. Al cambiar temporada o tipo, borrar licencia, categoría, tarifa resuelta y precio y descartar respuestas anteriores de cargas asíncronas. Al cambiar licencia, borrar categoría y precio. Al salir de `Día`, limpiar sus fechas; al cambiar temporada, limpiar la cobertura diaria para evitar arrastrar fechas de otro año.
7. Impedir confirmar mientras falte una combinación válida, se carguen tarifas o las fechas requeridas no sean válidas. Si no hay tarifas `Día`, mostrar que no existen tarifas disponibles, sin inventar opciones ni precios y manteniendo bloqueada la confirmación.
8. Después del alta, refrescar el historial sin agrupar ni descartar solicitudes por tener la misma temporada. Mantener el aislamiento por usuario y la autorización basada en socio real.

## Histórico y administración

Mostrar el nombre del tipo que devuelve la API. Si la solicitud es diaria, mostrar su intervalo de cobertura; para solicitudes antiguas con fechas nulas no fabricar fechas ni dejar de mostrar la solicitud. Las listas deberán usar el identificador de solicitud como identidad de fila, pues la temporada deja de ser única por usuario.

El cambio de estado administrativo debe dirigirse a la solicitud concreta por identificador y conservar su tipo y cobertura; la interfaz no ofrecerá edición de estos datos mediante ese flujo.

## Áreas actuales afectadas

- `IndaloaventurApp.SharedUI/Models/Licenses/FederativeLicenseRate.cs` y `FederativeLicenseRequest.cs`.
- `IndaloaventurApp.SharedUI/Abstractions/Licenses/IFederativeLicenseService.cs`.
- `IndaloaventurApp.Web/IndaloaventurApp.Web.Client/Infrastructure/Licenses/FederativeLicenseApiClient.cs`.
- `IndaloaventurApp.SharedUI/Components/Club/FederativeLicensesView.razor` y su código asociado; revisar cualquier consumidor administrativo del mismo contrato.
- `IndaloaventurApp.Frontend.Tests/TestDoubles.cs` y pruebas de cliente y vista bajo `Features/Licenses`.
- Especificación `frontend-member-federative-license-request-flow`, que actualmente define el combo booleano de modalidad, su filtro, el payload de alta y las etiquetas del histórico.

## Aceptación y pruebas de la adaptación

- El cliente envía `tipoLicenciaId` y materializa tipo/fechas, incluidos valores nulos de solicitudes migradas.
- Los selectores siguen el orden solicitado, deduplican valores y limpian dependencias y precio al cambiar una selección superior.
- `Día` exige ambas fechas, permite inicio igual a fin y bloquea intervalos invertidos o fuera de temporada.
- No se confirma una selección sin tarifa ni se cambia el precio según la duración de la cobertura.
- El historial muestra dos solicitudes diarias de una temporada y una no diaria sin sustituir unas por otras.
- Un cambio de estado afecta solo al identificador seleccionado y conserva las fechas.
- El flujo mantiene permisos y aislamiento y trata los errores de consulta y de alta con la interfaz existente.

Crear el cambio OpenSpec del frontend y ejecutar sus pruebas de cliente y componentes antes del despliegue coordinado. La comprobación completa puede usar tarifas diarias de prueba; la carga de importes diarios reales queda fuera de esta propuesta.
