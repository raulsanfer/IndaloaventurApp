## 1. Ajustes funcionales del paso de fotos

- [x] 1.1 Cambiar la validación del wizard para que `Foto 1` sea obligatoria y `Foto 2` opcional en el paso de fotos.
- [x] 1.2 Ajustar el mapeo del borrador al request de creación para enviar `foto2` solo cuando exista o como valor vacío/nulo compatible con el backend.
- [x] 1.3 Actualizar los literales de ayuda y validación del flujo para reflejar que solo la primera imagen es obligatoria.

## 2. Resumen final y acciones sobre previews

- [x] 2.1 Añadir en el paso de resumen una acción con icono de cruz para eliminar `Foto 2` sin perder el resto del borrador.
- [x] 2.2 Reorganizar el bloque de previews del resumen para mostrar `Foto 1` y `Foto 2` en dos columnas con tamaño reducido en ancho y alto.
- [x] 2.3 Mantener consistente el estado del wizard tras eliminar `Foto 2`, incluyendo refresco del resumen y posibilidad de guardar con una sola foto.

## 3. Cobertura y verificación

- [x] 3.1 Añadir o actualizar pruebas automatizadas del wizard para cubrir la obligatoriedad exclusiva de `Foto 1`, la eliminación de `Foto 2` y el guardado con una sola imagen.
- [x] 3.2 Ejecutar la suite de pruebas afectada del frontend.
- [ ] 3.3 Verificar manualmente el flujo completo: avanzar con solo `Foto 1`, eliminar `Foto 2` desde el resumen y guardar correctamente la señal.
