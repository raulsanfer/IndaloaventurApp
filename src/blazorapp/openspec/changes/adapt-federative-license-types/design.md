# Design

## Context

Ver la motivación en `proposal.md`. Fuentes revisadas: `.vscode/licencias-federativas-tipos.md`, `.vscode/openapi.json`, las dos especificaciones canónicas de licencias, clientes/modelos/vistas actuales y el handoff del backend en `../backend/openspec/changes/add-federative-license-types/frontend-handoff.md`. El enlace relativo al handoff del MD copiado en `.vscode` no existe en este frontend; se ha consultado el original del backend. Las reglas de negocio de fechas y exclusividad provienen del MD/handoff, pues OpenAPI representa fechas anulables sin expresar las condiciones por tipo.

El cliente de socio usa `bool MediaTemporada` en modelos, filtro y etiquetas. El administrativo aún no representa ni tipo ni cobertura. El histórico ya agrupa por temporada conservando una colección de solicitudes: se mantendrá esta presentación con identidad por `Id`. Ambos clientes usan `ServiceResult`, sesión existente y DTO privados; las pruebas usan bUnit y `StubHttpMessageHandler`.

## Goals / Non-Goals

**Goals:** adaptar ambos consumidores al contrato nuevo conservando arquitectura SharedUI/cliente HTTP, recursos españoles, SCSS global y clases parciales. Aislar el estado de cargas y enviar fechas sin conversiones horarias.

**Non-Goals:** introducir una capa API paralela, mantenimiento de catálogos, nuevas rutas, políticas de acceso nuevas o compatibilidad automática con la API booleana anterior.

## Decisions

### 1. Catálogo remoto y tipos explícitos

Añadir modelo de tipo (`Id`, `Nombre`) y operación de consulta en `IFederativeLicenseService`; sustituir booleano por identificador/nombre en tarifas y solicitudes, incluidas las administrativas y sus DTO. Consultar tipos al abrir el modal, con estados de carga/error y sin fallback booleano. El catálogo remoto evita duplicar nombres; los identificadores contractuales 1/2/3 definen selección inicial y condición diaria. No inferir comportamiento de tipos futuros: requerir una evolución explícita del contrato.

Cambiar `GetAvailableRatesAsync` para recibir temporada y tipo seleccionado; generar query con `tipoLicenciaId`, nunca `mediaTemporada`. Mantener `decimal` para importes y `PrecioIndependiente` separado. No añadir un método de detalle propio nuevo si no lo necesita la vista actual; si existe un consumidor, adaptar también su mapeo.

### 2. Cobertura con DateOnly anulable

Extender alta y solicitudes con `DateOnly? FechaInicio/FechaFin`, usando el soporte JSON de .NET 9 para `yyyy-MM-dd`. Esto evita desplazamientos por UTC respecto a `DateTime`. Para Día mostrar dos controles de fecha con etiquetas localizadas y validación de presencia, orden y año; para Completa/Media el formulario envía ambas nulas o las omite. La API admite pares opcionales válidos en esos tipos, pero no se añade captura opcional a la UI en este alcance.

La creación solo envía temporada, tarifa y fechas; usuario, clasificación e importes los determina la API. Nunca multiplicar PrecioClub por duración, ni suponer tarifas diarias. Resolver una única tarifa; si hay ambigüedad, bloquear la confirmación y tratarla como catálogo no resoluble.

### 3. Estado dependiente y solicitudes asíncronas

Orden visual: temporada, tipo, nombre de licencia, categoría, precio y fechas diarias. Conservar año actual y siguiente. Al cambiar temporada/tipo vaciar selecciones dependientes y precio antes de consultar; al cambiar nombre, vaciar categoría/precio. Limpiar cobertura al cambiar temporada o salir de Día. Usar un contador de versión de carga/modal, opcionalmente cancelación, para que únicamente la consulta vigente actualice catálogo, errores e indicador de carga. Invalidarlo al cerrar/reabrir. Deshabilitar confirmación durante cargas y envío para impedir doble alta accidental; no bloquear altas diarias posteriores.

### 4. Histórico y administración

Usar `tipoLicenciaNombre` recibido en cada registro y conservar `Id` como clave de fila, incluso con mismo tipo, temporada y fechas. Mantener agrupación visual por temporada y orden estable con desempate por identificador. Mostrar cobertura informada y mantener registros migrados sin fechas, sin sintetizar intervalos desde el tipo.

Extender mapeo administrativo y presentación; PUT sigue enviando únicamente estado y usa `UserId`/`Id` del registro seleccionado. Comprobar en pruebas que tipo, fechas, tarifa y creación sobreviven al refresco y que otro registro de la misma temporada no se modifica.

### 5. Errores y seguridad

Aplicar `security.md`: A01/A07 para permisos y sesión; A05 para entrada y salida; A06/A08 para integridad; A09/A10 para errores seguros. Mantener Member + IsMember=true para el flujo propio y Admin independiente de IsMember para administración. El catálogo de tipos requiere autenticación en API; la UI de socio conserva su restricción actual. El servidor sigue siendo la autoridad de autorización por objeto, cobertura, precio y exclusividad.

Preservar `ServiceResult` y recursos localizados para errores de catálogo, validación 400, sesión 401/403, red/JSON y conflicto 409. El 409 debe explicar la exclusividad conjunta Completa/Media, contando canceladas, sin deshabilitar Día ni toda la temporada. No introducir reglas de solapamiento. Renderizar nombres y errores como texto codificado, sin HTML sin sanitizar; no registrar tokens ni datos personales. Probar permisos y peticiones propias sin userId; los tests frontend no demuestran por sí solos autorización del servidor.

## Risks / Trade-offs

- Contrato incompatible → entrega coordinada y prueba contra API actualizada; no conservar fallback silencioso a `mediaTemporada`.
- No hay tarifas Día reales → probar con fixtures sintéticas; producción muestra vacío hasta carga aprobada en backend.
- Respuestas desordenadas o cierre durante carga → invalidación por versión y pruebas con tareas controladas.
- Identidad por temporada → usar Id en filas/actualizaciones y probar tres registros de la misma temporada.
- Fuentes temporales en `.vscode` → dejar el contrato funcional y criterios necesarios en esta propuesta y actualizar documentación conceptual durante implementación.

## Migration Plan

1. Implementar y ejecutar pruebas HTTP/componentes con fixtures del nuevo contrato, pruebas completas, build y DocFX.
2. Coordinar con el responsable del backend la pausa de escrituras y su migración conservadora; este cambio no ejecuta migraciones ni modifica backend.
3. Publicar API y cliente compatibles antes de reactivar altas. Verificar también la actualización del cliente PWA mediante su mecanismo existente para evitar clientes antiguos activos.
4. Comprobar tipos, solicitud no diaria, histórico migrado y gestión administrativa; comprobar Día con tarifas de prueba en entorno de validación, sin crear importes reales.
5. Ante incidencia, suspender altas y restablecer una pareja compatible de API/cliente. No revertir automáticamente la base: el Down del backend aborta si hay fechas o multiplicidad incompatibles y requiere un procedimiento de conservación específico.

## Verification Strategy

`FederativeLicenseApiClientTests` demuestra ruta/query sin booleano, Bearer, fechas JSON/nulos, DTO y errores; `FederativeLicensesViewTests` demuestra orden, deduplicación, dependencias, carreras, fechas, precio fijo, vacío diario e histórico múltiple. Las pruebas administrativas demuestran tipo/fechas y PUT por identidad conservando datos. Ampliar regresiones de permisos existentes y casos de sesión caducada, 400/409 y contenido textual no confiable. Ejecutar `dotnet test IndaloaventurApp.Frontend.Tests/IndaloaventurApp.Frontend.Tests.csproj`, `dotnet build IndaloaventurApp.sln` y `docfx docs/docfx.json`; no considerar comprobaciones omitidas como evidencia aprobada.
