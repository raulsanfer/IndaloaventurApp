# Verificación de implementación

Fecha: 2026-11-16. Alcance: frontend Blazor; pruebas HTTP con dobles y componentes bUnit.

## Evidencia por comportamiento

| Comportamiento | Evidencia |
| --- | --- |
| Catálogo de tipos, Bearer, orden y nombres del servidor | `FederativeLicenseContractTests.Types_UseAuthenticatedCatalog_AndPreserveServerOrderAndNames` |
| Tarifas con temporada/tipo, sin mediaTemporada y con PrecioClub | `FederativeLicenseApiClientTests.GetAvailableRatesAsync_UsesSeasonAndModalityFilters` comprueba query exacta y mapeo |
| Alta con solo temporada, tarifa y fechas yyyy-MM-dd/nulas | `Creation_SendsOnlyAllowedFields_AndMaterializesDateOnlyCoverage` |
| Temporadas actuales, orden de campos, deduplicación y limpieza de dependencias | Pruebas existentes de vista adaptadas y `FederativeLicenseCoverageTests.Changes_ClearDependenciesAndCoverage_AndDeduplicateOptions` |
| Fechas obligatorias, iguales, invertidas, fuera de año y precio constante | `DailyCoverage_ValidatesDates_AndKeepsCatalogPrice` con siete casos |
| Catálogo vacío/ambiguo y error de tipos | `EmptyOrAmbiguousDailyRates_CannotBeConfirmed`, `FailedOrEmptyTypeCatalog_DoesNotInventChoices` |
| Respuestas antiguas, estados loading y cierre/reapertura | `OldRateResponses_CannotOverwriteNewSelection`, `OldRateResponse_CannotClearLoadingOfNewRequest_OrAffectReopenedModal`, `ClosedModal_IgnoresOldTypeResponse_AfterReopen` |
| Bloqueo durante envío y recuperación tras error | `Submit_DisablesConfirmationUntilCompletion_AndCannotBeOpenedAgain`, `FailedDailyRequest_LeavesFormUsable_WithoutRenderingServerDetails` |
| Histórico con dos Día coincidentes y una no diaria cancelada | `DailyCreation_RefreshesAllRequestsOfSeason_AndAllowsAnotherDailyRequest` |
| Mapeo de registros múltiples y cobertura migrada nula | `List_PreservesMultipleRequestsPerSeason_AndMigratedNullDates`, `AdminOperations_PreserveTypeAndCoverage_AndOnlySendState` |
| PUT solo estado por usuario/solicitud, preservando datos y otras filas | `AdminOperations_PreserveTypeAndCoverage_AndOnlySendState`, `AdminFederativeLicenseManagementViewTests.AdminFederativeLicenseManagementView_UpdatesStatusAndReloadsList` |
| Sesión ausente, Member inactivo, rol incorrecto, Admin con/sin IsMember, 401/403 | `MemberOperations_DenyMissingSession`, `MemberOperations_DenyWrongRolesAndInactiveMembers`, `Queries_HandleExpiredAndForbiddenSessions`, `AdminOperations_HandleExpiredAndForbiddenSessions` y casos administrativos anteriores |
| Errores 400/409, JSON/red/timeout seguros y salida codificada | `Creation_FailsSafely_ForHttpErrors`, `Failures_ReturnSafeErrors`, `UntrustedCatalogAndHistoryNames_AreRenderedAsText` |

## Comandos

- `dotnet test IndaloaventurApp.Frontend.Tests/IndaloaventurApp.Frontend.Tests.csproj --verbosity quiet`: **267 correctas, 0 fallidas, 0 omitidas**.
- `dotnet build IndaloaventurApp.sln --verbosity quiet`: correcto, 0 errores.
- `.vscode/docfx-tools/docfx.exe docs/docfx.json`: correcto, 0 avisos y 0 errores. DocFX 2.81.0 instalado localmente para esta verificación; salida generada excluida de los cambios versionables.
- `openspec validate adapt-federative-license-types --strict`: válido (se ejecuta mediante el CLI descargado en la caché local).
- `git diff --check -- .`: correcto.

La compilación y las pruebas mantienen el aviso NU1902 ya presente para AngleSharp 1.4.0, dependencia de pruebas: GHSA-pgww-w46g-26qg. No se han cambiado dependencias de la aplicación.

## Revisión security.md

- A01/A07: se conservan guardas Member+IsMember y Admin; las peticiones propias no incluyen usuario y las administrativas se dirigen a la identidad concreta. No se amplían permisos.
- A05/A06/A08: nombres renderizados como texto Razor, fechas validadas antes de enviar, tarifa única, precio remoto fijo, sin restricciones nuevas de solapamiento ni bloqueo global por temporada. La API conserva la autoridad de integridad y autorización.
- A09/A10: errores de UI localizados por código, sin mostrar mensajes internos de excepciones o ProblemDetails; no se añaden logs, secretos ni persistencia de datos personales. Las consultas obsoletas no modifican el estado vigente.
- A02/A03/A04: sin cambios de hosting, autenticación, criptografía ni paquetes del proyecto. DocFX es una herramienta local de verificación.

## Límites y seguimiento operativo

No se han ejecutado migraciones, pruebas de autorización por objeto ni concurrencia del backend: no quedan demostradas por estos dobles HTTP. La publicación coordinada API/cliente, actualización efectiva PWA y prueba manual contra un backend actualizado siguen siendo pasos de despliegue. Las tarifas Día de las pruebas son sintéticas; no se han cargado importes reales. No se ha realizado un despliegue ni archivado el cambio.
