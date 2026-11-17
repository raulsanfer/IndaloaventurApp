# Tasks

Marcar cada tarea únicamente después de que pase su verificación; la validación de la propuesta no demuestra la implementación.

## 1. Modelo de dominio y contratos

- [x] 1.1 Crear `TipoLicencia` con nombre obligatorio e identidades estables para completa, media y día; añadir la relación obligatoria en `TarifaLicenciaFederativa` y retirar su booleano. Verificar con pruebas de dominio el catálogo, el tipo obligatorio y la conservación de reglas de importes y temporada.
- [x] 1.2 Añadir `DateOnly? FechaInicio/FechaFin` a `SolicitudLicenciaFederativa` y validar cobertura según el tipo real de la tarifa: obligatoria para día, pareja opcional para los demás, orden y año de temporada. Verificar con pruebas de dominio fechas nulas, una fecha ausente, mismo día, varios días, intervalo invertido y año incorrecto, además de estados y coherencia de temporada.
- [x] 1.3 Actualizar DTOs, comandos, validadores y contratos de repositorio para tipo, fechas y múltiples solicitudes por usuario/temporada; retirar la presuposición de un resultado único en `GetByUserIdAndTemporadaAsync`. Verificar compilación y pruebas de validación y arquitectura, manteniendo dependencias de Infrastructure fuera de Application.

## 2. Persistencia y conversión de datos

- [x] 2.1 Configurar `TiposLicencias`, datos iniciales, FK restrictiva de tarifas, unicidad por temporada/tipo/licencia/categoría, columnas `date NULL` y restricciones de cobertura; cambiar el índice de solicitudes a no único. Verificar con pruebas de persistencia nombres únicos, FK inválida, duplicados tarifarios y coexistencia de tipos y solicitudes diarias.
- [x] 2.2 Generar la migración y snapshot siguiendo `design.md`: crear tipos, asignar el tipo de todas las tarifas desde el booleano, verificar asignación y retirar la columna al final. Mantener IDs y no regenerar filas ni actualizar importes; verificar el script SQL, los datos iniciales y la lista de migraciones con `dotnet ef migrations list --project src/IndaloAventurApi.Infrastructure --startup-project src/IndaloAventurApi.Api`.
- [x] 2.3 Añadir y ejecutar una prueba SQL Server aislada que migre desde el esquema previo, con ambas variantes, tarifas adicionales de otras temporadas, precios personalizados, independientes nulos y solicitudes existentes. Verificar igualdad antes/después de IDs, datos funcionales y referencias, conversión correcta, cobertura histórica nula y ausencia de tarifas diarias inventadas; probar también instalación desde cero y despliegue repetido.
- [x] 2.4 Implementar la validación previa del `Down` y documentar el procedimiento de despliegue/reversión. Verificar en SQL Server el rollback compatible y que se aborta sin cambios con tarifas diarias, fechas informadas o multiplicidad por temporada.

## 3. Alta de solicitudes y concurrencia

- [x] 3.1 Implementar la operación de escritura transaccional del repositorio con bloqueo de rango por usuario/temporada y validación conjunta de solicitudes no diarias; mantener la clasificación en la tarifa. Verificar en SQL Server dos altas no diarias simultáneas sobre un rango vacío y uno existente: como máximo una se acepta y la otra produce conflicto funcional.
- [x] 3.2 Adaptar el handler de alta para permitir varias solicitudes diarias y su coexistencia con una no diaria, contando canceladas para el límite no diario; cargar el tipo real y validar fechas antes de persistir. Verificar con pruebas de Application ambos órdenes de convivencia, varias diarias, conflicto completa/media, solicitud cancelada, tarifa inexistente y temporada incoherente.
- [x] 3.3 Mantener identidad del token, `IsMember` y correo posterior al commit, con respuesta que incluya tipo y fechas. Verificar con pruebas de Application el rechazo de no socios, ausencia de correo cuando falla el alta y conservación de la solicitud si falla el correo después del commit.
- [x] 3.4 Ejecutar pruebas SQL Server de dos altas diarias concurrentes con fechas distintas y carreras entre una diaria y una no diaria. Verificar que ambas solicitudes válidas se conservan, que las fechas retornan sin desplazamientos y que los reintentos o bloqueos no generan duplicación parcial.

## 4. Consultas, API y contrato publicado

- [x] 4.1 Incorporar la consulta autenticada de tipos sin mantenimiento y actualizar el filtro de tarifas a `tipoLicenciaId`, con orden estable y carga del nombre. Verificar con pruebas de handlers e integración filtros independientes/combinados, catálogo vacío, los tres tipos sin tarifas diarias, anónimo y cualquier rol autenticado.
- [x] 4.2 Rechazar explícitamente `mediaTemporada`, validar el nuevo identificador y actualizar los DTOs, contratos HTTP, OpenAPI y matriz de autorización. Verificar con pruebas HTTP el `400` del filtro antiguo, filtros mal formados y no positivos, los campos nuevos y las políticas existentes.
- [x] 4.3 Actualizar las proyecciones Dapper propias y administrativas y el mapeo de respuesta al cambiar estado, con join al catálogo, tipo, cobertura y orden determinista sin colapsar solicitudes. Verificar con pruebas HTTP y round-trip SQL Server de `DateOnly?` el histórico antiguo, varios registros por temporada, detalle propio, aislamiento entre usuarios y filtros administrativos.
- [x] 4.4 Completar pruebas de contrato del alta diaria y no diaria: fechas nulas, pareja incompleta, mismo día, intervalo invertido y fuera de temporada, `201/400/403/404/409` según la operación. Verificar ProblemDetails en español, identidad del token y ausencia de persistencia ante errores de fechas.
- [x] 4.5 Adaptar el cambio administrativo de estado para devolver tipo y cobertura conservados. Verificar con integración que afecta solo al identificador solicitado, no altera los demás datos, conserva otras solicitudes de esa temporada y mantiene rechazos por rol insuficiente y usuario ajeno.

## 5. Integración con el frontend y cierre

- [x] 5.1 Entregar y contrastar `frontend-handoff.md` con el contrato real y los casos de aceptación. Verificar que identifica los consumidores del booleano, el orden de selección, limpieza de dependencias, precio de club, fechas, historial múltiple y despliegue coordinado; la implementación del otro proyecto se seguirá en su propio cambio OpenSpec.
- [x] 5.2 Ejecutar `dotnet test IndaloAventurApi.sln` y `dotnet build IndaloAventurApi.sln --no-restore`, corrigiendo regresiones. Verificar resultados completos y registrar explícitamente cualquier prueba SQL Server omitida; no dar por probadas migración y concurrencia hasta ejecutarlas en SQL Server aislado.
- [x] 5.3 Ejecutar `openspec validate add-federative-license-types --strict` y comprobar que requisitos y escenarios están cubiertos por las pruebas incorporadas. Verificar que todas las tareas marcadas cuentan con evidencia y que el procedimiento de publicación exige un frontend compatible antes de reactivar las altas.

## Workflow follow-up

- Crear y completar el cambio OpenSpec del frontend a partir de `frontend-handoff.md`, con pruebas de cliente y componentes en su proyecto.
- Publicar API y frontend de forma coordinada según `design.md`; cargar tarifas diarias reales únicamente cuando exista una fuente aprobada de importes.
- Archivar este cambio cuando finalicen la implementación y su verificación.
