# Feature: club

## Proposito

Agrupa las pantallas de informacion del club accesibles desde `Mi Club`, incluyendo indice del area y telefonos de interes.

## Componentes y paginas

- `IndaloaventurApp.Web.Client/Pages/ClubPage.razor`: ruta `/mi-club`.
- `IndaloaventurApp.SharedUI/Components/Club/ClubIndexView.razor`: entrada funcional de Mi Club.
- `IndaloaventurApp.Web.Client/Pages/ClubPhonebookPage.razor`: ruta `/mi-club/telefonos-interes`.
- `IndaloaventurApp.SharedUI/Components/Club/ClubPhonebookView.razor`: agenda telefonica.

## Servicios y contratos

- `IPhonebookService`
- `PhonebookApiClient`
- `PhonebookContact`

La agenda se carga desde `/api/agenda-telefonica`.

## Estados

- estado de carga inicial;
- listado de contactos;
- contacto con o sin email;
- error de recuperacion de agenda;
- navegacion de vuelta a `Mi Club`.

## Tests relacionados

- `IndaloaventurApp.Frontend.Tests/Features/Navigation/ClubIndexViewTests.cs`

Ver tambien [SharedUI y componentes](../architecture/shared-ui.md).
