## 1. Estructura base de UI

- [x] 1.1 Crear en `IndaloaventurApp.SharedUI` los componentes Razor del layout de login (contenedor, cabecera, formulario y pie) con sus clases partial C# separadas
- [x] 1.2 Crear el app shell autenticado reutilizable (cabecera de app + contenedor de contenido + menú inferior/botonera)
- [x] 1.3 Integrar la composición de login y home en `IndaloaventurApp.Web` con transición al shell autenticado tras validación de usuario

## 2. Sistema de estilos SCSS

- [x] 2.1 Crear parciales SCSS del dominio login siguiendo la guía de `openspec/design/login`
- [x] 2.2 Crear parciales SCSS del dominio home/shell (cabecera y navegación inferior) siguiendo la guía de `openspec/design/home`
- [x] 2.3 Registrar los parciales en `style.scss` y validar que no existan estilos inline ni estilos en componentes Razor

## 3. Localización y contenido

- [x] 3.1 Definir claves cortas de recursos para todos los literales visibles de login, home, cabecera y menú inferior en español
- [x] 3.2 Inyectar localizer en componentes y sustituir textos hardcodeados por claves de recurso

## 4. Fundaciones de servicios y HttpClientFactory

- [x] 4.1 Definir interfaces de servicios de aplicación para el dominio de acceso en una capa desacoplada de UI
- [x] 4.2 Implementar servicio HTTP tipado con `IHttpClientFactory` y registro DI centralizado (base URL, timeout y configuración común)
- [x] 4.3 Asegurar que los componentes consumen solo interfaces de servicio y no clases concretas HTTP

## 5. Código limpio y calidad

- [x] 5.1 Aplicar convenciones de código limpio en componentes/servicios (SRP, nombres claros, separación de responsabilidades)
- [x] 5.2 Definir manejo uniforme de errores para servicios base y documentar el patrón de resultados

## 6. Verificación funcional y técnica

- [x] 6.1 Validar renderizado responsive de login y home autenticada en móvil y escritorio
- [x] 6.2 Validar comportamiento visual de cabecera y menú inferior persistente en navegación de home
- [x] 6.3 Ejecutar pruebas/build del proyecto y registrar evidencia de que la nueva base no rompe la compilación

