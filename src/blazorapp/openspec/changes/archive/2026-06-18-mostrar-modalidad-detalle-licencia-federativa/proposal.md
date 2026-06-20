## Why

El histórico de `Licencias Federativas` del socio muestra hoy únicamente `Temporada 2026`, aunque desde el flujo de solicitud ya existe la distinción funcional entre `Temporada Completa` y `Media Temporada`. Falta reflejar esa modalidad en el detalle para que el usuario pueda identificar correctamente qué variante solicitó.

## What Changes

- Ampliar el contrato frontend del histórico de solicitudes de licencias federativas para exponer si cada solicitud corresponde a `Temporada Completa` o `Media Temporada`.
- Modificar la presentación del detalle/listado de solicitudes del socio para que la temporada mostrada incluya también la modalidad solicitada.
- Añadir o ajustar literales y pruebas de componente/cliente para validar que la modalidad correcta se muestra en el histórico del usuario.

## Capabilities

### New Capabilities

### Modified Capabilities
- `frontend-member-federative-license-request-flow`: el histórico de solicitudes del socio pasa a informar la modalidad `Temporada Completa` o `Media Temporada` junto con la temporada de la licencia solicitada.

## Impact

- `IndaloaventurApp.SharedUI/Components/Club/FederativeLicensesView.*`
- `IndaloaventurApp.SharedUI/Models/Licenses/FederativeLicenseRequest.cs`
- `IndaloaventurApp.Web/IndaloaventurApp.Web.Client/Infrastructure/Licenses/FederativeLicenseApiClient.cs`
- Recursos localizados de `Licencias Federativas`
- Pruebas frontend del histórico y del cliente API de licencias federativas
