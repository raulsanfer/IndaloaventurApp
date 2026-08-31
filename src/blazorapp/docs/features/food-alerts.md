# Feature: alertas alimentarias

## Proposito

Muestra alertas alimentarias por categorias y detalle, usando endpoints locales del host que encapsulan el servicio externo.

## Componentes y paginas

- `IndaloaventurApp.Web.Client/Pages/FoodAlertsPage.razor`: ruta `/alertas-alimentarias`.
- `IndaloaventurApp.SharedUI/Components/FoodAlerts/FoodAlertsHomeView.razor`: home de alertas.
- `IndaloaventurApp.Web.Client/Pages/FoodAlertCategoryPage.razor`: ruta `/alertas-alimentarias/categoria/{CategoryCode}`.
- `IndaloaventurApp.SharedUI/Components/FoodAlerts/FoodAlertCategoryView.razor`: listado por categoria.
- `IndaloaventurApp.Web.Client/Pages/FoodAlertDetailPage.razor`: ruta `/alertas-alimentarias/alerta/{AlertId}`.
- `IndaloaventurApp.SharedUI/Components/FoodAlerts/FoodAlertDetailView.razor`: detalle.

## Servicios y contratos

- `IFoodAlertService`
- `FoodAlertAppApiClient`
- `ExternalFoodAlertService`
- `FoodAlertEndpointRouteBuilderExtensions`
- `FoodAlertCatalog`, `FoodAlertCategoryItem`, `FoodAlertListItem`, `FoodAlertDetailItem`
- `FoodAlertTextFormatter`

## Estados

- categorias fijas del catalogo;
- carga de alertas por categoria;
- detalle por identificador;
- error de servicio externo;
- payload o formato no esperado.

## Tests relacionados

- `IndaloaventurApp.Frontend.Tests/Features/FoodAlerts/FoodAlertViewsTests.cs`
- `IndaloaventurApp.Frontend.Tests/Features/FoodAlerts/FoodAlertAppApiClientTests.cs`
- `IndaloaventurApp.Frontend.Tests/Features/FoodAlerts/ExternalFoodAlertServiceTests.cs`

Ver tambien [Inyeccion de dependencias](../architecture/dependency-injection.md).
