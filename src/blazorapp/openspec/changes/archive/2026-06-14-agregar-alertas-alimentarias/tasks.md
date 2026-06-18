## 1. Contrato y adaptador de alertas alimentarias

- [x] 1.1 Crear la abstraccion compartida de `FoodAlerts` con modelos de categoria, listado y detalle.
- [x] 1.2 Implementar el adaptador del proveedor externo para mapear `fecha`, `titulo`, `url`, `descripcion` e `id` al contrato interno.
- [x] 1.3 Normalizar la descripcion HTML del proveedor a texto plano seguro y reutilizable tanto en listado como en detalle.
- [x] 1.4 Anadir pruebas automatizadas del mapeo, truncado e identificador nativo en `IndaloaventurApp.Frontend.Tests`.

## 2. Proxy y registro de servicios

- [x] 2.1 Anadir configuracion del proveedor de alertas alimentarias en la app web.
- [x] 2.2 Exponer un endpoint propio en `IndaloaventurApp.Web` para listado por categoria y detalle por `id`.
- [x] 2.3 Registrar las implementaciones de `IFoodAlertService` para servidor y cliente, garantizando compatibilidad con `InteractiveAuto`.
- [x] 2.4 Anadir pruebas automatizadas del cliente/proxy para errores, respuesta vacia y resolucion de detalle.

## 3. Paginas y vistas de UI

- [x] 3.1 Crear la pagina `Alertas Alimentarias` con las tres cards de categoria y sus subtitulos.
- [x] 3.2 Crear la pagina reutilizable de listado por categoria y la pagina de detalle por alerta en `IndaloaventurApp.Web.Client`.
- [x] 3.3 Implementar las vistas compartidas Razor y sus clases parciales separadas en `IndaloaventurApp.SharedUI`.
- [x] 3.4 Mostrar en el listado el titulo y un extracto aproximado de 100 caracteres, y en el detalle la descripcion completa.
- [x] 3.5 Anadir los literales localizados necesarios para titulos, categorias y estados.

## 4. Estilos y validacion

- [x] 4.1 Crear los estilos SCSS del modulo para las cards blancas, esquinas redondeadas, listados y detalle, sin estilos inline ni CSS embebido en componentes.
- [x] 4.2 Anadir pruebas bUnit de las vistas de selector, listado y detalle, incluyendo estados de carga, vacio y error.
- [ ] 4.3 Ejecutar la suite de pruebas del frontend y verificar manualmente la navegacion selector -> listado -> detalle y la recarga directa del detalle.
