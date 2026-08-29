## 1. Permisos y contratos de comentarios

- [x] 1.1 Centralizar en la sesion frontend la regla que autoriza comentar una signal (`Admin` o `Member` con `IsMember = true`)
- [x] 1.2 Extender `ISignalService`, modelos asociados y `SignalApiClient` con la operacion de alta de comentario usando `POST /api/signals/{id}/comments`
- [x] 1.3 Mapear en la capa de servicio los errores relevantes del alta de comentario (validacion, autenticacion/autorizacion, signal inexistente y error generico)

## 2. Integracion del detalle y modal de alta

- [x] 2.1 Anadir en `SignalDetailView` la accion `Anadir comentario` visible solo dentro del bloque `Comentarios` para perfiles autorizados
- [x] 2.2 Implementar el popup modal con campo de texto largo, validacion de texto obligatorio, acciones `Confirmar`/`Cancelar` y estados de envio
- [x] 2.3 Enviar el comentario asociado a la signal abierta, cerrar el modal tras exito y refrescar la coleccion de comentarios sin recargar todo el detalle
- [x] 2.4 Incorporar recursos localizados ES y ajustes SCSS globales necesarios para el nuevo flujo sin introducir estilos inline

## 3. Verificacion

- [x] 3.1 Anadir o actualizar tests de `SignalApiClient` para cubrir el `POST` de comentarios y su manejo de respuestas de error
- [x] 3.2 Anadir o actualizar tests de `SignalDetailView` para cubrir visibilidad del CTA, apertura/cancelacion del modal, validacion y refresco tras alta correcta
- [x] 3.3 Ejecutar `dotnet test` en el frontend y revisar manualmente que solo `Admin` y `Member` con `IsMember = true` pueden publicar comentarios
