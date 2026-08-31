# Feature: signals

## Proposito

Permite consultar signals, filtrar por categorias, ver detalle, cargar imagenes, comentar cuando el usuario esta autorizado, crear nuevas signals y editar signals propias.

## Componentes y paginas

- `IndaloaventurApp.Web.Client/Pages/SignalHomePage.razor`: ruta `/signals`.
- `IndaloaventurApp.SharedUI/Components/Signals/SignalHomeView.razor`: listado y filtros.
- `IndaloaventurApp.Web.Client/Pages/SignalDetailPage.razor`: ruta `/signals/{SignalId:guid}`.
- `IndaloaventurApp.SharedUI/Components/Signals/SignalDetailView.razor`: detalle, imagenes, tabs y comentarios.
- `IndaloaventurApp.Web.Client/Pages/SignalCreatePage.razor`: ruta `/signals/nueva`.
- `IndaloaventurApp.SharedUI/Components/Signals/SignalCreateView.razor`: creacion multistep.
- `IndaloaventurApp.SharedUI/Components/Settings/SignalCategoriesManagementView.razor`: gestion admin de categorias.

## Servicios y contratos

- `ISignalService`
- `SignalApiClient`
- `SignalHomeData`, `SignalListQuery`, `SignalCardItem`, `SignalDetailItem`, `SignalImagesItem`
- `SignalCreateDraft`, `SignalCreateRequest`, `UpdateSignalRequest`
- `SignalCommentItem`, `CreateSignalCommentRequest`
- `SignalCategoryItem`, `CreateSignalCategoryRequest`, `UpdateSignalCategoryRequest`

## Estados

- home con categorias y listado;
- detalle con tabs, tags, imagenes y comentarios;
- sesion no autorizada para comentar;
- validacion de comentario;
- payload invalido, timeout o recurso no encontrado;
- gestion admin de categorias.

## Tests relacionados

- `IndaloaventurApp.Frontend.Tests/Features/Signals/SignalApiClientTests.cs`
- `IndaloaventurApp.Frontend.Tests/Features/Signals/SignalViewsTests.cs`
- `IndaloaventurApp.Frontend.Tests/Features/Signals/SignalCategoriesManagementViewTests.cs`

Ver tambien [Clientes HTTP](../architecture/http-api-clients.md).
