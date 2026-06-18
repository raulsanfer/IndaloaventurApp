## 1. Ajustes del paso de fotos

- [x] 1.1 Reemplazar el disparador único por acciones explícitas de cámara y galería/archivo para `Foto 1` y `Foto 2` en `SignalCreateView`
- [x] 1.2 Actualizar los literales y el marcado del paso de fotos para explicar las nuevas opciones sin romper la experiencia en escritorio

## 2. Normalización de imágenes en cliente

- [x] 2.1 Implementar un helper de procesamiento de imágenes que redimensione y/o recomprima la imagen seleccionada antes de crear `SignalPhotoDraft`
- [x] 2.2 Cambiar la validación actual para comprobar el límite de 2 MB sobre la imagen procesada y mostrar error comprensible cuando no pueda adaptarse
- [x] 2.3 Mantener la preview y los datos finales del wizard usando la imagen optimizada que se enviará al backend

## 3. Integración y robustez

- [x] 3.1 Conectar el procesamiento de imágenes con los `InputFile` y el JS existente sin alterar el contrato `CreateSignalRequest`
- [x] 3.2 Añadir o actualizar pruebas de la lógica de selección/procesamiento de imágenes si hay cobertura automatizada disponible para esta capa

## 4. Verificación

- [ ] 4.1 Verificar manualmente en móvil que cada slot permite abrir cámara y galería, seleccionar una foto existente y continuar el wizard
- [ ] 4.2 Verificar manualmente en escritorio que sigue siendo posible elegir archivos locales y que una imagen inicial superior a 2 MB puede quedar aceptada tras el procesamiento cuando sea adaptable
