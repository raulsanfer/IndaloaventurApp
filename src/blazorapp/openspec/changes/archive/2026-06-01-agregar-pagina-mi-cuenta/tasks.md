## 1. Navegación y estructura de página

- [x] 1.1 Añadir la ruta/página `Mi Cuenta` en `IndaloaventurApp.Web.Client` y conectarla desde el botón inferior "Mi cuenta"
- [x] 1.2 Ajustar el shell autenticado para reflejar estado activo del botón "Mi cuenta" al navegar a esta pantalla (alineado con diseño de botonera inferior)

## 2. Componentes de Mi Cuenta en SharedUI

- [x] 2.1 Crear componentes Razor reutilizables para la pantalla "Mi cuenta" (cabecera de perfil, tarjetas de métricas, listados de accesos y acciones) con clase partial C# separada
- [x] 2.2 Crear componente específico de Cargo y su lógica de visibilidad condicional según datos de ficha de socio
- [x] 2.3 Montar todos los enlaces y CTA del diseño en la pantalla (`Mis Inscripciones`, `Mis Rutas Favoritas`, `Configuración`, `Ayuda`, `Cerrar Sesión`, `Ver Tienda`) incluyendo "Ficha Socio" como acceso no operativo en esta iteración

## 3. Servicios y consumo de datos de miembro

- [x] 3.1 Definir contrato de servicio de perfil de miembro desacoplado de UI
- [x] 3.2 Implementar cliente HTTP tipado con `IHttpClientFactory` para consumir `GET /api/fichas-socio/me`
- [x] 3.3 Integrar la carga de datos en la página "Mi cuenta" y mapear estado con/sin cargo

## 4. Cierre de sesión

- [x] 4.1 Definir servicio de sesión para signout y limpieza de estado local de autenticación
- [x] 4.2 Implementar acción "Cerrar sesión" operativa en "Mi cuenta" con redirección al login
- [x] 4.3 Validar que tras signout no se mantiene acceso efectivo al área autenticada con sesión previa

## 5. Estilos y localización

- [x] 5.1 Crear parciales SCSS de "Mi cuenta" y registrarlos en `style.scss` sin estilos inline en componentes
- [x] 5.2 Añadir claves de recursos ES para todos los textos de "Mi cuenta", enlaces y cierre de sesión
- [x] 5.3 Sustituir literales hardcodeados por localizer en componentes nuevos/ajustados

## 6. Verificación

- [x] 6.1 Verificar renderizado responsive de "Mi cuenta" (móvil y escritorio) alineado con `openspec/design/mi_cuenta`
- [x] 6.2 Verificar comportamiento del bloque de Cargo para usuario con cargo y usuario sin cargo
- [x] 6.3 Verificar presencia visual y estructura del bloque de puntos + CTA `Ver Tienda`
- [x] 6.4 Ejecutar build/tests del frontend y registrar evidencia de compilación sin regresiones

