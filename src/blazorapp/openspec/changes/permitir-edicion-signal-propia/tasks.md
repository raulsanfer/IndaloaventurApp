## 1. Contexto de ownership

- [x] 1.1 Extender la sesion frontend para exponer el identificador del usuario autenticado resolviendolo desde el JWT o el mecanismo equivalente vigente.
- [x] 1.2 Ampliar el modelo y mapeo del detalle de signal para conservar la autoria necesaria para decidir si la signal es propia.

## 2. Flujo de edicion propia

- [x] 2.1 Anadir en el detalle de signal la accion `Editar` visible solo para el propietario autenticado.
- [x] 2.2 Implementar el flujo de edicion limitada con los campos `Titulo`, `Descripcion` y `Estado`, incluyendo precarga y cancelacion.
- [x] 2.3 Incorporar en `ISignalService` y `SignalApiClient` la operacion de guardado compatible con edicion limitada de signals propias.
- [x] 2.4 Refrescar el detalle tras un guardado correcto y manejar errores de validacion, servicio o autorizacion sin perder el borrador.

## 3. Verificacion

- [x] 3.1 Anadir o actualizar tests frontend para cubrir visibilidad del boton `Editar` en signals propias y su ocultacion en signals ajenas.
- [x] 3.2 Anadir o actualizar tests frontend para cubrir alcance de campos editables, guardado correcto y denegacion de edicion ajena.
- [ ] 3.3 Verificar manualmente que un usuario solo puede editar sus propias signals desde el detalle y que solo cambia `Titulo`, `Descripcion` y `Estado`.
