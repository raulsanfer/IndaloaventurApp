# Tasks

Solo marcar tareas cuando su verificación pase. Los comandos de pruebas usan `IndaloaventurApp.Frontend.Tests/IndaloaventurApp.Frontend.Tests.csproj`; para comprobación focalizada añadir `--filter FullyQualifiedName~<clase>`.

## 1. Contrato y cliente del socio

- [x] 1.1 Releer `security.md`, añadir modelo de tipo, reemplazar booleanos por identificador/nombre y extender alta/solicitudes con DateOnly anulable; actualizar interfaz, XML público y dobles. Verificar compilación y pruebas de consumidores actualizadas al nuevo contrato.
- [x] 1.2 Implementar GET de tipos autenticado y tarifas con temporada/tipoLicenciaId. Ampliar `FederativeLicenseApiClientTests` para catálogo ordenado, Bearer, filtros y ausencia de mediaTemporada; verificar pruebas focalizadas.
- [x] 1.3 Adaptar DTO/mapeo de listado y alta, JSON de fechas yyyy-MM-dd y nulos, errores 400/409 y sesión/red/JSON. Verificar con pruebas HTTP que no se envían usuario/precio ni se pierden tipo/fechas y que 409 no se presenta como prohibición de toda la temporada.
- [x] 1.4 Revisar impacto en `docs/architecture` y `docs/features`, actualizar `http-api-clients.md` y contratos de licencias según corresponda, y verificar con `docfx docs/docfx.json`.

## 2. Formulario y cobertura diaria

- [x] 2.1 Sustituir Modalidad por tipos remotos y ordenar temporada/tipo/nombre/categoría/precio; mantener año actual/siguiente y deduplicación. Actualizar recursos españoles y SCSS global si procede. Verificar orden, selección inicial y opciones en `FederativeLicensesViewTests`.
- [x] 2.2 Limpiar dependencias, precio y cobertura conforme al diseño e invalidar cargas al cambiar selección o cerrar/reabrir modal. Verificar con pruebas de tareas controladas que respuestas antiguas no sustituyen catálogo, error ni loading vigentes.
- [x] 2.3 Añadir fechas diarias y bloquear confirmación durante carga/envío, sin tarifa única o con fechas ausentes, invertidas o fuera del año. Probar fechas iguales, límites del año, intervalo multidiario con PrecioClub constante, salida de Día y ausencia de tarifas Día; verificar pruebas de vista.
- [x] 2.4 Ajustar envío, cancelación, errores y refresco tras alta; probar 400 de cobertura, 409 no diario y posibilidad de nuevas altas diarias, incluso coincidentes. Verificar pruebas de vista y actualizar `docs/features/licenses.md` con el flujo; ejecutar DocFX.

## 3. Histórico y administración

- [x] 3.1 Reemplazar etiquetas booleanas por nombre de tipo y mostrar cobertura informada; conservar agrupación visual con identidad por Id y registros con fechas nulas. Probar en la vista dos solicitudes Día y una Completa/Media de la misma temporada, incluso con fechas coincidentes, sin pérdidas tras refrescar.
- [x] 3.2 Extender modelo/DTO/mapeo administrativo con tipo y fechas en consulta y respuesta PUT; ampliar `AdminFederativeLicenseApiClientTests` para nulos, fechas y cuerpo de solo estado dirigido por UserId/Id. Verificar pruebas focalizadas.
- [x] 3.3 Mostrar tipo/cobertura en administración manteniendo edición de solo estado. Verificar en `AdminFederativeLicenseManagementViewTests` que cambiar una solicitud no altera otra de la misma temporada ni tipo, tarifa, propietario, creación o cobertura. Actualizar documentación de gestión y XML público y ejecutar DocFX.

## 4. Seguridad e integración

- [x] 4.1 Verificar regresiones A01/A07 con pruebas de Member real, Member IsMember=false, sin sesión/caducada y Admin con IsMember true/false; comprobar aislamiento del endpoint propio y errores 401/403 sin ampliar permisos ni asumir autorización del servidor probada por frontend.
- [x] 4.2 Verificar A05/A06/A08/A09/A10 con pruebas de nombres/errores como texto codificado, validación negativa, precio de tarifa sin cálculo cliente y errores seguros sin tokens o datos personales en logs; revisar checklist de `security.md` y registrar evidencia.
- [x] 4.3 Ejecutar `dotnet test IndaloaventurApp.Frontend.Tests/IndaloaventurApp.Frontend.Tests.csproj`, `dotnet build IndaloaventurApp.sln`, `docfx docs/docfx.json` y `openspec validate adapt-federative-license-types --strict`. Revisar documentación de estrategia de pruebas y actualizarla si cambia; no versionar salida generada DocFX. Registrar resultados y cualquier escenario sin evidencia.

## Workflow follow-up

- Coordinar pausa de altas, migración backend y publicación compatible de API/cliente, incluida actualización PWA; reactivar tras comprobación del flujo en entorno de validación. No cargar tarifas diarias reales como parte de este cambio.
- Mantener revisión del rollback conservador del backend; no ejecutar Down automáticamente sobre datos incompatibles.
- Archivar después de implementación y verificación completas.
