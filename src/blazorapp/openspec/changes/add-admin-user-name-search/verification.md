# Verificación de add-admin-user-name-search

Fecha: 2026-11-18.

## Resultados

- Pruebas focalizadas: **61 correctas**, sin fallos ni omisiones, ejecutadas con `dotnet test IndaloaventurApp.Frontend.Tests/IndaloaventurApp.Frontend.Tests.csproj --no-restore --filter "FullyQualifiedName~AdminUserManagementApiClientTests|FullyQualifiedName~AdminUsersManagementViewTests" --verbosity minimal`.
- Suite frontend completa: **309 correctas**, sin fallos ni omisiones, con `dotnet test IndaloaventurApp.Frontend.Tests/IndaloaventurApp.Frontend.Tests.csproj --no-build --no-restore --verbosity minimal` después de compilar las pruebas focalizadas.
- API local: **25 pruebas HTTP correctas**, sin fallos ni omisiones, con `dotnet vstest ../backend/tests/IndaloAventurApi.IntegrationTests/bin/Debug/net9.0/IndaloAventurApi.IntegrationTests.dll --TestCaseFilter:FullyQualifiedName~ManagedUserNameSearchTests --ResultsDirectory:.vscode/test-results/admin-user-name-search /Logger:console`. Se utiliza el ensamblado existente del backend y su servidor de pruebas con SQLite temporal; no se modifica ni recompila el backend.
- OpenSpec: `validate add-admin-user-name-search --strict` correcto, ejecutando con Node la CLI existente en `../backend/.vscode/npm-cache/_npx/abab5bd700860149/node_modules/@fission-ai/openspec/bin/openspec.js`.
- DocFX: `dotnet .vscode/docfx-tools/.store/docfx/2.81.0/docfx/2.81.0/tools/net8.0/any/docfx.dll docs/docfx.json` correcto, **0 advertencias y 0 errores**. Las salidas generadas permanecen ignoradas.
- SCSS compila con Sass local. Las cuatro reglas afectadas del CSS servido coinciden con la compilación del parcial `_configuracion.scss`; se evita incorporar diferencias ajenas de la regeneración global.
- Ambos recursos españoles son XML válido sin claves duplicadas. `openspec/endpoints.json` es JSON válido y su única diferencia estructural es el parámetro opcional `nombre` de `GET /api/users`.
- `git diff --check -- .` correcto.

## Cobertura por requisito

Los nombres siguientes se refieren a pruebas de `AdminUsersManagementViewTests` y `AdminUserManagementApiClientTests`, salvo las pruebas `List_*` del backend.

| Requisito / escenarios | Evidencia |
| --- | --- |
| Carga completa inicial, ausencia de usuarios y filtros de espacios | `AdminUsersManagementView_LoadsInitialUsersAndShowsMemberAndNonMemberActions`, `Search_SubmitsEffectiveFiltersAndDisplaysOnlyServerResults`, `GetUsersAsync_SendsOnlyEffectiveFiltersWithAuthorization`, `ClearFilters_UsesUnfilteredEmptyStateAndRecoversFromReloadFailure` |
| Email existente, fragmentos y estado vacío filtrado | `AdminUsersManagementView_FiltersUsersByEmail`, `Search_SubmitsEffectiveFiltersAndDisplaysOnlyServerResults`, `Search_HasAccessibleFieldsAndNativeSubmitWithoutFullEmailValidation`, `ClearFilters_EmptiesBothFieldsAndReloadsAllUsers` |
| Nombre parcial, ambos apellidos, mayúsculas y espacios | `Search_SubmitsEffectiveFiltersAndDisplaysOnlyServerResults`, `GetUsersAsync_SendsOnlyEffectiveFiltersWithAuthorization`; coincidencia real mediante `List_WithName_ShouldMatchAnyPositionInNameOrEitherSurname` |
| Nombre vacío, ausencia de ficha y condición de no socio | `GetUsersAsync_SendsOnlyEffectiveFiltersWithAuthorization`, `Search_ShowsMatchingNonMemberWithoutFilteringByMembershipOrEmail`; `List_WithoutEffectiveName_ShouldIncludeUsersWithoutFicha`, `List_WithName_ShouldExcludeUsersWithoutFichaEvenWhenEmailMatches` |
| Combinación AND y respuesta única, incluyendo cero coincidencias | `Search_SubmitsEffectiveFiltersAndDisplaysOnlyServerResults`, `ClearFilters_EmptiesBothFieldsAndReloadsAllUsers`; `List_WithBothFilters_ShouldRequireBothAndPreserveContractAndOrdering` |
| Envío por formulario y semántica nativa de Enter | `Search_HasAccessibleFieldsAndNativeSubmitWithoutFullEmailValidation`, pruebas que envían el formulario con `Submit`; no se ha ejecutado una sesión de navegador real |
| Limpieza tras resultados, cero coincidencias, error y fallo de recarga | `ClearFilters_EmptiesBothFieldsAndReloadsAllUsers`, `ClearFilters_UsesUnfilteredEmptyStateAndRecoversFromReloadFailure` |
| Unicode, caracteres reservados y ausencia de parámetros inyectados | `GetUsersAsync_EncodesNameAsOneLiteralQueryValue`; literalidad en consulta real mediante `List_WithName_ShouldTreatSqlWildcardsAsLiteralText` |
| Autorización y errores seguros | `GetUsersAsync_SendsOnlyEffectiveFiltersWithAuthorization`, `GetUsersAsync_WithNameMapsHttpFailuresWithoutExposingResponse`, `Search_UsesSafeErrorFeedbackAndAllowsRetry`, pruebas de `SettingsViewTests`; `List_WithName_ShouldRemainAdminOnly` verifica 401 y 403 en el API |
| Red, timeout, JSON inválido y cancelación | `GetUsersAsync_WithNameMapsTransportFailures`, `GetUsersAsync_WithNameRejectsInvalidPayload`, `GetUsersAsync_WithNameHonorsCancellation`, `Search_UsesSafeErrorFeedbackAndAllowsRetry` |
| Bloqueo de peticiones simultáneas e instantánea de criterios enviados | `Search_PreventsOverlappingRequestsAndKeepsSubmittedFilterState` |

## Revisión de seguridad y límites

Se han revisado `security.md` y las reglas A01, A04/A09, A05 y A10. Se conserva la política Admin del API y no se modifica la autorización de la ruta. Los filtros se codifican individualmente y no se añaden registros ni persistencia de los valores. La vista muestra recursos localizados y no los mensajes de respuesta del servidor. Las acciones de usuario y las operaciones administrativas ajenas se conservan y están incluidas en la suite completa.

La compilación comunica una advertencia NU1902 ya existente para la dependencia transitiva `AngleSharp` 1.4.0 (GHSA-pgww-w46g-26qg, gravedad moderada). No se cambian dependencias en este trabajo.

La coincidencia se ha probado en el API local de integración, no en un entorno desplegado ni en SQL Server durante esta implementación frontend. Antes de desplegar, confirmar que el API destino admite `nombre`; la implementación y las pruebas de SQL Server previas se documentan en el cambio archivado homónimo del backend. No se promete equivalencia adicional entre acentos ni búsqueda por nombre y apellidos concatenados.
