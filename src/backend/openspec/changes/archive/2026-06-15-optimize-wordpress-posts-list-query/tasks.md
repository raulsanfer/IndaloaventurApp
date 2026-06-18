## 1. Configuracion y contrato

- [x] 1.1 Ampliar `WordPressOptions` y su validacion para soportar el numero por defecto de posts del listado desde `appsettings`.
- [x] 1.2 Ajustar el endpoint y/o caso de uso de listado para aplicar el `pageSize` configurado cuando el cliente no lo informe.
- [x] 1.3 Separar el contrato de listado del contrato de detalle para que el resumen de posts no exponga `Contenido` ni `Enlace`.

## 2. Consulta optimizada a WordPress

- [x] 2.1 Refactorizar `WordPressService` para que la consulta del listado solicite solo titulo, resumen, fecha e imagen destacada, corrigiendo la carga de `_embedded/wp:featuredmedia`.
- [x] 2.2 Mantener el flujo de detalle por `slug` con su consulta completa y revisar que no se vea afectado por la optimizacion del listado.

## 3. Verificacion

- [x] 3.1 Actualizar pruebas de integracion o servicio para validar el `pageSize` por defecto configurado, la URI generada hacia WordPress y el mapeo correcto de la imagen destacada en el listado.
- [x] 3.2 Actualizar pruebas del endpoint/listado para verificar el contrato resumido y ejecutar la bateria relevante antes de marcar las tareas como completadas.
