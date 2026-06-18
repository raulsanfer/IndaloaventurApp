## 1. Navegación y páginas

- [x] 1.1 Crear la nueva ruta/página `Mi Club` en `IndaloaventurApp.Web.Client` y conectar a ella el botón inferior "Mi Club"
- [x] 1.2 Ajustar el shell autenticado para reflejar el estado activo del botón "Mi Club" al navegar a esta pantalla
- [x] 1.3 Crear la nueva ruta/página `Teléfonos de interés` y conectarla desde la página índice `Mi Club`

## 2. Componentes SharedUI

- [x] 2.1 Crear los componentes Razor reutilizables de la página índice `Mi Club` con clases partial C# separadas
- [x] 2.2 Crear el componente/listado de `Teléfonos de interés` para renderizar fichas de contacto y estados de carga/error
- [x] 2.3 Diseñar la estructura visual de las fichas para soportar nombre, teléfonos y datos opcionales como `email` sin rehacer el layout

## 3. Servicios y datos

- [x] 3.1 Definir el contrato frontend desacoplado para la agenda telefónica
- [x] 3.2 Implementar un cliente HTTP tipado con `IHttpClientFactory` para consumir `GET /api/agenda-telefonica`
- [x] 3.3 Validar el contrato actual de `FichaContactoDto` y resolver el tratamiento de `email` según disponibilidad real del backend
- [x] 3.4 Integrar la carga de contactos en la pantalla `Teléfonos de interés`

## 4. Estilos y localización

- [x] 4.1 Añadir las claves de recursos ES para textos de `Mi Club`, `Teléfonos de interés` y sus estados de carga/error
- [x] 4.2 Crear parciales SCSS para la página índice y la agenda telefónica y registrarlos en el `style.scss` global
- [ ] 4.3 Verificar que la agenda mantiene scroll vertical usable en móvil y escritorio sin estilos inline

## 5. Verificación

- [ ] 5.1 Verificar manualmente la navegación desde la botonera inferior a `Mi Club` y desde `Mi Club` a `Teléfonos de interés`
- [ ] 5.2 Verificar manualmente el render de contactos y el scroll vertical con una lista larga
- [ ] 5.3 Verificar el comportamiento visual con y sin dato `email` según el contrato disponible
- [x] 5.4 Ejecutar `dotnet build` y las pruebas automatizadas aplicables antes de marcar tareas como completadas
