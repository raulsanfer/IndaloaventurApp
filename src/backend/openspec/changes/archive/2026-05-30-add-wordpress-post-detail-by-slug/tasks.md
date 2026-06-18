## 1. Contratos de aplicación

- [x] 1.1 Extender `IWordPressService` con una operación para obtener un post por `slug`.
- [x] 1.2 Añadir query y handler de aplicación para el detalle de post reutilizando `WordPressPostDto`.

## 2. Integración y API

- [x] 2.1 Implementar la consulta de WordPress por `slug`, incluyendo manejo controlado de post no encontrado.
- [x] 2.2 Exponer un endpoint `GET /api/wordpress/posts/{slug}` autenticado y documentar sus respuestas.

## 3. Verificación

- [x] 3.1 Añadir o ajustar pruebas del servicio WordPress para éxito, `slug` inexistente y error remoto.
- [x] 3.2 Añadir o ajustar pruebas de integración del endpoint de detalle por `slug`.
- [x] 3.3 Ejecutar `dotnet test` y corregir regresiones.
