## 1. Ruta, acceso y consumo de datos

- [x] 1.1 Crear la nueva página `Licencias Federativas` en `IndaloaventurApp.Web.Client` y activar desde `Mi Club` la navegación real hacia esa ruta
- [x] 1.2 Añadir una abstracción y cliente API específico para cargar `GET /api/licencias-federativas/me/solicitudes`
- [x] 1.3 Definir los modelos de UI necesarios para representar solicitudes, su `Estado` y las agrupaciones por `Temporada`

## 2. UI del listado y estados de la vista

- [x] 2.1 Añadir en `Mi Club` una nueva opción `Licencias Federativas` debajo de `Teléfonos de interés`, con aspecto homogéneo y visibilidad específica para miembros
- [x] 2.2 Implementar el componente compartido o vista de `Licencias Federativas` con cabecera, breadcrumb/título y botón `Solicitar` alineado a la derecha
- [x] 2.3 Renderizar el listado con patrón de tabla DaisyUI y grupos por `Temporada`, mostrando por cada licencia `Licencia`, `Categoria`, `Ambito/Territorio` y `Estado`
- [x] 2.4 Añadir estados de carga, vacío, error y acceso no operativo manteniendo el estilo visual existente de la app

## 3. Recursos, estilos y validación

- [x] 3.1 Incorporar literales localizados ES y ajustes SCSS globales necesarios para la nueva pantalla y su integración con `Mi Club`
- [x] 3.2 Verificar la visibilidad solo para usuarios `Member` con `IsMember = true` y el comportamiento controlado para `IsMember = false`
- [x] 3.3 Añadir o actualizar tests de componentes/servicios y ejecutar la validación del frontend correspondiente
