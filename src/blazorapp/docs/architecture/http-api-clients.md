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
- `Infrastructure/WordPress/WordPressPostApiClient.cs`: noticias.
- `Infrastructure/FoodAlerts/FoodAlertAppApiClient.cs`: alertas alimentarias servidas por endpoints locales del host.

## Manejo de errores

Usa codigos estables en `ServiceError.Code`, por ejemplo `auth.session_invalid`, `signals.timeout` o `licenses.admin_forbidden`. La UI debe decidir que mensaje mostrar a partir de esos resultados, normalmente mediante recursos localizados.

## Testing

Los clientes se prueban con `StubHttpMessageHandler` en `IndaloaventurApp.Frontend.Tests/TestDoubles.cs`, verificando metodo HTTP, ruta, query string, payload y mapeo del resultado.
