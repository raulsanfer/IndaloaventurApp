# Verificación de add-federative-license-types

Verificado el 16 de noviembre de 2026. Implementación limitada al backend y sus contratos; el frontend se sigue en su propio cambio, según `frontend-handoff.md`.

## Resultados

| Comprobación | Resultado |
| --- | --- |
| `dotnet test IndaloAventurApi.sln --no-restore -v minimal` | 279 correctas, 0 fallidas, 0 omitidas. |
| Dominio | 41 correctas. |
| Aplicación | 95 correctas. |
| Integración | 140 correctas; incluye 10 de `FederativeLicenseSqlServerTests`. |
| Arquitectura | 3 correctas. |
| `dotnet build IndaloAventurApi.sln --no-restore -v quiet` | 0 errores, 0 advertencias. |
| `dotnet ef migrations has-pending-model-changes ... --no-build` | Sin cambios del modelo pendientes. |
| Listado de migraciones con `--no-connect` | Incluye `20261116165233_AddFederativeLicenseTypes`. |
| Script idempotente de la migración | Generado y revisado; ejecución repetida probada en SQL Server. |
| `openspec validate add-federative-license-types --strict` | Cambio válido. |

Los argumentos de proyecto de EF son `--project src/IndaloAventurApi.Infrastructure --startup-project src/IndaloAventurApi.Api`.

Las pruebas SQL Server ejecutadas usan bases nuevas `indalo_license_tests_<guid>` sobre la instancia configurada; cada prueba elimina su base aislada al terminar. El entorno restringido falló al inicializar el soporte de cifrado, por lo que se ejecutaron con autorización fuera de él. La suite completa posterior pasó sin omisiones. No se aplicó la migración a la base de datos de la aplicación.

## Correspondencia de requisitos y pruebas

| Capacidad / escenarios | Evidencia |
| --- | --- |
| Tipos predeterminados, orden, autenticación y ausencia de mantenimiento | `LicenciasFederativasIntegrationTests.Tipos_ConsultaAutenticadaOrdenadaYSinMantenimiento`; prueba SQL de persistencia e inicialización desde cero. |
| Relación obligatoria, unicidad de tarifa por tipo y conservación del catálogo | `FederativeLicenseSqlServerTests.Persistencia_ImpideTiposDuplicadosTarifasDuplicadasYReferenciasInvalidas`; `FederativeLicensePersistenceTests`; pruebas de dominio de tarifas. |
| Migración de completas/medias, tarifas manuales de otras temporadas, importes personalizados e histórico | `FederativeLicenseSqlServerTests.Migracion_ConservaTodasLasTarifasYElHistoricoYAdmiteReversionCompatible` compara todos los datos funcionales y referencias antes/después, comprueba fechas históricas nulas y ausencia de tarifas diarias inventadas. |
| Despliegue repetido y reversión conservadora | `ScriptsIdempotentes_SeEjecutanDosVecesEnAmbosSentidos`; reversión compatible en la prueba de migración y `Reversion_IncompatibleAbortaSinModificarDatos`, con tres causas independientes de bloqueo. |
| Consulta y filtros de tarifas, precio y rechazo del booleano antiguo | `LicenciasFederativasHandlersTests.Tarifas_FiltraPorTemporadaYTipo`; casos HTTP de catálogo, filtros válidos/inválidos y colecciones vacías; flujo diario de varios días conserva el importe catalogado. |
| Cobertura diaria obligatoria, mismo día, varios días, pareja opcional, orden y año | `FederativeLicenseTests.Cobertura_ValidaParejaOrdenYTemporadaSegunTipo`; `Alta_CoberturaInvalidaDevuelve400SinPersistir`, `NoDiaria_AdmiteCoberturaOpcional` y flujo diario HTTP; restricciones SQL de pareja, orden y año. |
| Multiplicidad diaria y exclusividad completa/media, incluidas canceladas | `LicenciasFederativasHandlersTests.Alta_AplicaMultiplicidadPorTipo`; flujo HTTP con dos diarias, una completa y rechazo de una media adicional. |
| Exclusividad concurrente sin solicitudes previas y con una diaria previa | `AltasNoDiariasConcurrentes_RespetanExclusividadEnRangoVacioYOcupado`, ambos casos ejecutados en SQL Server. |
| Altas diarias simultáneas y mezcla diaria/no diaria | `AltasDiariasConcurrentes_ConvivenEntreSiYConNoDiariaYMaterializanFechas`, ambos casos ejecutados en SQL Server. |
| Tipo y fechas en alta, histórico propio, detalle y administración | Flujo HTTP `Diarias_ConvivenConCompletaYConservanCoberturaEnConsultasYCambioAdministrativo`; round-trip Dapper SQL Server en pruebas de concurrencia e histórico migrado. |
| Identidad, socio real, aislamiento y errores | Pruebas HTTP del módulo y `AuthorizationSecurityIntegrationTests`, incluida la matriz de rutas; pruebas de Application de no socio, tarifa inexistente y temporada incoherente. |
| Cambio administrativo limitado al estado | Flujo diario HTTP compara cobertura, tipo y creación y comprueba que otra solicitud de esa temporada sigue pendiente; se mantienen pruebas de rol insuficiente y propietario incorrecto. |
| Correo después del commit, sin envío ante un alta fallida y fallo de correo no bloqueante | `Alta_NotificaDespuesDelCommitYUnFalloDeCorreoNoRevierte`, `Alta_NoNotificaSiFallaLaPersistencia` y prueba HTTP previa de notificación. |

## Contrato y publicación

- `docs/openapi.json` se exportó desde la API real mediante un servidor de pruebas con SQLite aislado. Incluye la nueva consulta de tipos, el filtro por tipo y las fechas con formato `date`.
- `docs/licencias-federativas-tipos.md` documenta contratos, reglas, script revisable, despliegue coordinado y restricciones de reversión.
- `frontend-handoff.md` se contrastó con los DTOs y OpenAPI: nombres de propiedades, rutas y formatos coinciden. La implementación del frontend no forma parte de estas tareas del backend.
- Antes de publicar, completar el cambio del cliente, detener temporalmente las altas, migrar el esquema y publicar versiones compatibles de API y frontend. El catálogo real de precios diarios sigue pendiente de una fuente aprobada de importes.
