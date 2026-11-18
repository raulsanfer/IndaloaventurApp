# Feature: settings y admin

## Proposito

Agrupa pantallas administrativas para usuarios, fichas de socio, cargos, licencias federativas, categorias de signals, inventario y reservas de material.

## Componentes y paginas

- `IndaloaventurApp.Web.Client/Pages/SettingsPage.razor`: ruta `/configuracion`.
- `IndaloaventurApp.SharedUI/Components/Settings/SettingsView.razor`: hub de configuracion.
- `IndaloaventurApp.Web.Client/Pages/SettingsUsersPage.razor`: ruta `/configuracion/usuarios`.
- `IndaloaventurApp.SharedUI/Components/Settings/AdminUsersManagementView.razor`: listado y acciones de usuarios.
- `IndaloaventurApp.Web.Client/Pages/AdminUserPasswordChangePage.razor`: ruta administrativa `/configuracion/usuarios/{userId}/contrasena`.
- `IndaloaventurApp.SharedUI/Components/Settings/AdminUserPasswordChangeView.razor`: cambio de contraseña con validación de formato y feedback seguro.
- `IndaloaventurApp.Web.Client/Pages/SettingsCargoPage.razor`: ruta `/configuracion/cargos`.
- `IndaloaventurApp.SharedUI/Components/Settings/CargoManagementView.razor`: gestion de cargos.
- `IndaloaventurApp.Web.Client/Pages/SettingsSignalsPage.razor`: ruta `/configuracion/signals`.
- `IndaloaventurApp.SharedUI/Components/Settings/SignalSettingsHubView.razor`: hub admin de signals.
- `IndaloaventurApp.Web.Client/Pages/SettingsInventoryPage.razor`: ruta `/configuracion/inventario` para gestion de inventario.
- `IndaloaventurApp.SharedUI/Components/Settings/InventoryManagementView.razor`: categorias, estados y materiales de inventario.
- `IndaloaventurApp.Web.Client/Pages/SettingsMaterialReservationsPage.razor`: ruta `/configuracion/reservas-material` para la gestion administrativa de reservas.
- `IndaloaventurApp.SharedUI/Components/Settings/MaterialReservationsManagementView.razor`: listado, detalle y decisiones administrativas de reservas de material.

## Servicios y contratos

- `IAdminUserManagementService`
- `ICargoAdminService`
- `IAdminFederativeLicenseService`
- `ISignalService`
- `IInventoryReservationAdminService`
- `IInventoryService`
- `AdminUserManagementApiClient`
- `CargoAdminApiClient`
- `AdminFederativeLicenseApiClient`
- `InventoryReservationAdminApiClient`
- `ManagedUserItem`, `UpdateManagedUserRequest`, `AdminUserPasswordChangeRequest`, `CargoItem`, `CreateCargoRequest`, `UpdateCargoRequest`
- `MaterialReservation`, `MaterialReservationStatus`, `RegisterAdminMaterialReservationPickupRequest`, `CancelAdminMaterialReservationRequest`, `ConfirmAdminMaterialReservationReturnRequest`

## Estados

- la administracion de usuarios, fichas, cargos, licencias y signals se reserva a `Admin`;
- la gestion de inventario y las reservas administrativas se permite a `Admin` o a `InventoryManager` cuando la sesion tiene `IsMember = true`; las rutas personales `/me` mantienen su propiedad y condicion de socio;
- un administrador puede asignar o retirar `InventoryManager` desde la ficha administrativa de un socio mediante `PUT /api/users/{userId}`, preservando los demas roles; el listado queda limitado a localizar usuarios y navegar a sus acciones;
- despues de cambiar roles o condicion de socio, la persona afectada debe iniciar sesion de nuevo; si la persona administradora se modifica a si misma, la interfaz cierra su sesion;
- filtros parciales de usuarios por email y por nombre o apellidos, combinables y con limpieza explícita;
- activacion y desactivacion de usuarios;
- cambio de contraseña mediante `PUT /api/users/{userId}/password`, reservado a Admin y sin exponer secretos en la UI;
- creacion, edicion y borrado de cargos;
- gestion de categorias de signals;
- consulta de reservas de material con datos de socio solo en la ruta Admin;
- retirada y cancelacion para reservas solicitadas, y confirmacion administrativa de devolucion o cancelacion para reservas retiradas;
- recarga de listado y detalle despues de cada respuesta correcta, sin actualizar estados de forma optimista;
- errores de permisos, validacion o conectividad; un `401` limpia la sesion local y un `403` conserva la sesion y deshabilita la accion afectada.

## Búsqueda administrativa de usuarios

`/configuracion/usuarios` carga inicialmente todos los usuarios mediante `GET /api/users` sin filtros. El formulario permite introducir parte del email, nombre o apellidos y enviar con `Buscar` o Enter. No es necesario conocer una dirección de correo completa.

El campo `Nombre o apellidos` se envía como `nombre`: el API busca texto literal contenido en `FichaSocio.Nombre` O `FichaSocio.Apellidos`, sin distinguir mayúsculas. No concatena ambos campos ni garantiza equivalencia entre caracteres con y sin acentos. Un usuario sin ficha vinculada no coincide por nombre, aunque su email contenga ese texto; sigue apareciendo en el listado completo y en búsquedas solo por email. Una ficha coincidente puede encontrarse aunque `IsMember` sea falso.

Si se completan email y nombre, se envían ambos en una sola petición y el API exige que se cumplan los dos criterios (AND). Por ejemplo, `email=club.es&nombre=ont` localiza correos que contienen `club.es` cuya ficha contiene `ont` en nombre o apellidos. Los valores se recortan por los extremos, se codifican como valores independientes y se omiten si quedan vacíos. El cliente conserva el resultado del servidor sin filtrar ni consultar cada ficha.

`Limpiar filtros` vacía ambos campos y vuelve a consultar el listado completo. Funciona también después de cero coincidencias o un error. Durante la petición, buscar y limpiar están deshabilitados; cambiar un campo todavía no enviado no altera el estado de los resultados de la petición en curso. Los errores muestran feedback localizado, sin mensajes internos, y permiten reintentar. Los filtros no se persisten en la URL ni en almacenamiento local y no se añaden registros con esos datos personales.

El API desplegado debe admitir el parámetro `nombre`; una versión anterior podría ignorarlo. La autorización administrativa continúa aplicándose en el servidor. Las pruebas frontend verifican el envío, la presentación y la recuperación; la coincidencia real y AND se comprueban en las pruebas HTTP de integración del backend y en el entorno objetivo antes del despliegue.

## Tests relacionados

- `IndaloaventurApp.Frontend.Tests/Features/Admin/AdminUsersManagementViewTests.cs`
- `IndaloaventurApp.Frontend.Tests/Features/Admin/AdminUserManagementApiClientTests.cs`
- `IndaloaventurApp.Frontend.Tests/Features/Admin/AdminUserPasswordChangeViewTests.cs`
- `IndaloaventurApp.Frontend.Tests/Features/Admin/CargoManagementViewTests.cs`
- `IndaloaventurApp.Frontend.Tests/Features/Admin/CargoAdminApiClientTests.cs`
- `IndaloaventurApp.Frontend.Tests/Features/Settings/SettingsViewTests.cs`
- `IndaloaventurApp.Frontend.Tests/Features/Inventory/SettingsMaterialReservationsPageTests.cs`
- `IndaloaventurApp.Frontend.Tests/Features/Inventory/MaterialReservationsManagementViewTests.cs`
- `IndaloaventurApp.Frontend.Tests/Features/Inventory/InventoryReservationAdminApiClientTests.cs`

Ver tambien [Autenticacion y sesion](../architecture/authentication-session.md).
