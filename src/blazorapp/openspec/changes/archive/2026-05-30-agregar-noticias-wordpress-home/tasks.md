## 1. Capa de datos WordPress

- [x] 1.1 Definir los modelos frontend y el contrato de servicio para listado y detalle de posts de WordPress.
- [x] 1.2 Implementar un cliente HTTP tipado que consuma `GET /api/wordpress/posts?page=1&pageSize=10` y `GET /api/wordpress/posts/{slug}`.
- [x] 1.3 Registrar el servicio en DI del cliente/host siguiendo el patrón existente de clientes API.

## 2. Noticias en Home

- [x] 2.1 Crear un componente reutilizable para mostrar la sección de noticias en Home con carga inicial, estado vacío y estado de error.
- [x] 2.2 Renderizar las 10 noticias en un carrusel horizontal con scroll táctil, mostrando imagen destacada y título en cada card.
- [x] 2.3 Integrar la nueva sección siempre al final de `HomeDashboard` sin alterar el resto del layout actual.

## 3. Pantalla de detalle

- [x] 3.1 Crear una nueva ruta/página de detalle de noticia basada en `slug`.
- [x] 3.2 Implementar la vista de detalle consumiendo el endpoint por `slug`, mostrando título, imagen destacada, fecha y contenido completo del post.
- [x] 3.3 Añadir estados de carga y error comprensibles para la pantalla de detalle.

## 4. Estilos y recursos

- [x] 4.1 Añadir los textos localizados necesarios para cabecera de noticias, estados de carga, vacío y error.
- [x] 4.2 Incorporar estilos SCSS/CSS para el carrusel horizontal, las cards de noticia y la página de detalle con buen comportamiento móvil.

## 5. Verificación

- [ ] 5.1 Verificar manualmente que Home muestra las noticias al final y permite scroll horizontal con touch o puntero.
- [ ] 5.2 Verificar manualmente que pulsar una noticia abre su detalle y carga el contenido correcto por `slug`.
- [x] 5.3 Ejecutar `dotnet build` y las pruebas automatizadas aplicables, dejando las tareas marcadas solo si pasan.
