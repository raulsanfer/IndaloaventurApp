# Feature: club

## Proposito

Agrupa las pantallas de informacion y gestiones personales del club accesibles desde `Mi Club`, incluyendo indice del area, telefonos de interes y reservas de material.

## Componentes y paginas

- `IndaloaventurApp.Web.Client/Pages/ClubPage.razor`: ruta `/mi-club`.
- `IndaloaventurApp.SharedUI/Components/Club/ClubIndexView.razor`: entrada funcional de Mi Club.
- `IndaloaventurApp.Web.Client/Pages/ClubPhonebookPage.razor`: ruta `/mi-club/telefonos-interes`.
- `IndaloaventurApp.SharedUI/Components/Club/ClubPhonebookView.razor`: agenda telefonica.
- `IndaloaventurApp.Web.Client/Pages/ClubMaterialReservationsPage.razor`: ruta personal `/mi-club/reservas-material`.
- `IndaloaventurApp.Web.Client/Pages/ClubMaterialReservationCreatePage.razor`: ruta `/mi-club/reservas-material/nueva`.
- `IndaloaventurApp.Web.Client/Pages/ClubMaterialReservationDetailPage.razor`: ruta `/mi-club/reservas-material/{reservaId}`.
- `IndaloaventurApp.SharedUI/Components/Club/MemberMaterialReservationsListView.razor`, `MemberMaterialReservationCreateView.razor` y `MemberMaterialReservationDetailView.razor`: listado, alta y detalle de reservas propias.

## Servicios y contratos

- `IPhonebookService`
- `PhonebookApiClient`
- `PhonebookContact`
- `IMemberMaterialReservationService`
- `MemberMaterialReservationApiClient`
- `MaterialReservation`, `ReservableInventoryMaterial` y `CreateMemberMaterialReservationRequest`

La agenda se carga desde `/api/agenda-telefonica`.
Las reservas personales se consultan y modifican exclusivamente mediante `/api/inventario-reservas/me`; el selector de alta carga `/api/inventario-materiales`, presenta solo materiales activos y muestra la disponibilidad informativa calculada por el servidor para el material elegido. Esa cifra puede cambiar y la API la vuelve a validar al crear la solicitud.

## Estados

- estado de carga inicial;
- listado de contactos;
- contacto con o sin email;
- error de recuperacion de agenda;
- navegacion de vuelta a `Mi Club`.
- acceso a reservas solo para sesiones con rol `Member` e `IsMember=true`, comprobado tambiÃ©n por la API;
- tarjetas verticales ordenadas por fecha de solicitud descendente, con material, unidades, solicitud, recogida prevista y estado;
- alta con cantidad positiva, retirada prevista y devolucion estimada no anterior;
- cancelacion solo en `Solicitada`, confirmacion de devolucion solo en `Retirada` y detalle final en solo lectura;
- recarga remota tras crear, cancelar o confirmar, sin actualizaciones optimistas.

## Tests relacionados

- `IndaloaventurApp.Frontend.Tests/Features/Navigation/ClubIndexViewTests.cs`
- `IndaloaventurApp.Frontend.Tests/Features/Inventory/ClubMaterialReservationsPageTests.cs`
- `IndaloaventurApp.Frontend.Tests/Features/Inventory/MemberMaterialReservationsListViewTests.cs`
- `IndaloaventurApp.Frontend.Tests/Features/Inventory/MemberMaterialReservationDetailViewTests.cs`
- `IndaloaventurApp.Frontend.Tests/Features/Inventory/MemberMaterialReservationCreateViewTests.cs`
- `IndaloaventurApp.Frontend.Tests/Features/Inventory/MemberMaterialReservationApiClientTests.cs`

Ver tambien [SharedUI y componentes](../architecture/shared-ui.md).
