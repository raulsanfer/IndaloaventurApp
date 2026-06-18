## 1. Base de navegación y contratos

- [x] 1.1 Crear la nueva página `SignalHome` en `IndaloaventurApp.Web.Client` y enlazarla desde la entrada `Señales` del dashboard Home
- [x] 1.2 Añadir `Signals` al `BottomNav` autenticado con icono de advertencia, visibilidad para todos los roles y orden fijo entre `Home` y `Mi Cuenta`, preferiblemente antes de `Mi Club`
- [x] 1.3 Definir la abstracción frontend para signals y categories, junto con sus modelos DTO y view-model de listado
- [x] 1.4 Implementar el cliente HTTP para `GET /api/signals` y `GET /api/signal-types`, dejando documentada la estrategia para imágenes si siguen en endpoint separado

## 2. Vista de listado SignalHome

- [x] 2.1 Crear el componente compartido `SignalHomeView` con breadcrumb, título y estructura general de pantalla
- [x] 2.2 Implementar el buscador y el filtrado por categorías en formato píldora
- [x] 2.3 Implementar la fila de categorías con scroll horizontal y estado visual de selección
- [x] 2.4 Renderizar el listado de señales como cards alineadas con el diseño base, apoyadas en DaisyUI y tolerantes a campos opcionales ausentes

## 3. Recursos y estilos

- [x] 3.1 Añadir los literales localizados necesarios para `SignalHome`, breadcrumb, búsqueda, filtros y estados
- [x] 3.2 Montar la base visual de Signals con DaisyUI y crear los estilos SCSS complementarios del módulo, registrándolos en la hoja global
- [ ] 3.3 Verificar que la composición funciona correctamente en móvil y escritorio sin introducir estilos inline

## 4. Validación funcional

- [ ] 4.1 Verificar manualmente la navegación desde Home hasta `SignalHome` y el breadcrumb de retorno
- [ ] 4.2 Verificar manualmente búsqueda, selección de categoría y scroll horizontal de chips cuando existan muchas categorías
- [ ] 4.3 Verificar manualmente los estados de carga, vacío y error, además del render estable de cards con datos opcionales ausentes
