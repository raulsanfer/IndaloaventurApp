# Clientes HTTP

Los clientes API viven en `IndaloaventurApp.Web.Client/Infrastructure` y se registran como clientes HTTP tipados.

## Patron comun

- Reciben `HttpClient` por constructor.
- Cuando el endpoint requiere autenticacion, reciben tambien `ISessionService`.
- Construyen rutas relativas al API configurado.
- Serializan peticiones con JSON.
- Mapean respuestas a modelos de `SharedUI/Models`.
- Devuelven `ServiceResult<T>` para evitar lanzar excepciones hacia la UI por errores esperados.

## Areas

- `Infrastructure/Auth/AuthApiClient.cs`: autenticacion y recuperacion de password.
- `Infrastructure/Member/MemberProfileApiClient.cs`: perfil y ficha de socio.
- `Infrastructure/Member/AdminUserManagementApiClient.cs`: gestion admin de usuarios y fichas.
- `Infrastructure/Phonebook/PhonebookApiClient.cs`: agenda telefonica del club.
- `Infrastructure/Signals/SignalApiClient.cs`: listado, detalle, comentarios, categorias y creacion/edicion de signals.
- `Infrastructure/Licenses/FederativeLicenseApiClient.cs`: licencias federativas de socio.
- `Infrastructure/Licenses/AdminFederativeLicenseApiClient.cs`: gestion admin de licencias.
- `Infrastructure/Cargos/CargoAdminApiClient.cs`: cargos.
- `Infrastructure/Inventory/InventoryReservationAdminApiClient.cs`: consulta y transiciones administrativas de reservas de material.
- `Infrastructure/Inventory/MemberMaterialReservationApiClient.cs`: consulta, alta, cancelacion y confirmacion de devolucion de las reservas propias del socio; mapea la disponibilidad informativa del catalogo autenticado sin sustituir la validacion final de la API.
- `Infrastructure/WordPress/WordPressPostApiClient.cs`: noticias.
- `Infrastructure/FoodAlerts/FoodAlertAppApiClient.cs`: alertas alimentarias servidas por endpoints locales del host.

## Filtros del listado administrativo de usuarios

`IAdminUserManagementService.GetUsersAsync(email, nombre, cancellationToken)` permite filtros opcionales de email y nombre o apellidos. `AdminUserManagementApiClient` conserva la normalización del email, recorta `nombre`, omite valores vacíos y codifica cada valor con `Uri.EscapeDataString` en orden `email`, `nombre`. Sin criterios efectivos solicita `/api/users` sin query. La cancelación y la cabecera Bearer siguen el patrón del cliente existente.

El servidor aplica la coincidencia parcial de `nombre` sobre la ficha vinculada y combina ambos filtros con AND. El cliente no interpreta comodines ni filtra respuestas localmente; tampoco solicita fichas adicionales para resolver búsquedas. El DTO de respuesta se conserva. Los errores HTTP y de transporte se devuelven como `ServiceResult<T>` y la vista usa sus propios mensajes localizados.

## Manejo de errores

Usa codigos estables en `ServiceError.Code`, por ejemplo `auth.session_invalid`, `signals.timeout`, `licenses.admin_forbidden` o `reservations.conflict`. La UI debe decidir que mensaje mostrar a partir de esos resultados, normalmente mediante recursos localizados. Las operaciones de reserva propias no envian `userId`; la API resuelve la identidad desde el Bearer y mantiene la autoridad final sobre disponibilidad y transiciones.

## Testing

Los clientes se prueban con `StubHttpMessageHandler` en `IndaloaventurApp.Frontend.Tests/TestDoubles.cs`, verificando metodo HTTP, ruta, query string, payload y mapeo del resultado.
