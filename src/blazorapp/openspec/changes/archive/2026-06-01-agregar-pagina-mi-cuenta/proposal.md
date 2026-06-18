## Why

La app ya dispone de login y home autenticada, pero falta una pantalla de "Mi cuenta" para que el socio vea su información personal y acceda a acciones de perfil. Implementarla ahora cierra el flujo base de navegación autenticada y habilita una gestión mínima de sesión con cierre de sesión operativo.

## What Changes

- Se crea una nueva página "Mi cuenta" accesible desde el botón inferior correspondiente en el shell autenticado.
- Se implementa el layout y los componentes de la página según la referencia en `openspec/design/mi_cuenta`, incluyendo avatar/nombre de miembro, tarjetas de métricas, bloques de actividad/soporte y módulo de puntos con CTA.
- Se añade un componente de "Cargo" con visibilidad condicional: solo se muestra cuando el usuario tiene cargo informado en su ficha.
- Se define carga de datos de miembro para "Mi cuenta" consumiendo `GET /api/fichas-socio/me`.
- Se montan todos los enlaces visuales del diseño en la página "Mi cuenta" (`Mis Inscripciones`, `Mis Rutas Favoritas`, `Configuración`, `Ayuda`, `Cerrar Sesión`, `Ver Tienda`) y se deja `Ficha Socio` como acceso no operativo para una iteración posterior.
- Se implementa un enlace/botón de "Cerrar sesión" operativo para invalidar sesión local y redirigir al login.
- Se ajusta la navegación inferior para mostrar el estado activo en "Mi Cuenta" cuando el usuario está en esa página.
- Se mantienen reglas de arquitectura: SharedUI reusable, servicios por interfaz, `IHttpClientFactory`, localización por claves y estilos SCSS globales.

## Capabilities

### New Capabilities
- `frontend-mi-cuenta-page`: Define la pantalla "Mi cuenta" (layout, navegación, enlaces, y bloque de cargo condicional) basada en el diseño de referencia.
- `frontend-session-signout`: Define el comportamiento de cierre de sesión en el área autenticada y retorno al login.

### Modified Capabilities
- Ninguna.

## Impact

- Afecta a `IndaloaventurApp.SharedUI` (nuevos componentes Razor de "Mi cuenta" y bloques reutilizables de perfil/cargo/enlaces).
- Afecta a `IndaloaventurApp.Web` y `IndaloaventurApp.Web.Client` (ruta `mi-cuenta`, navegación desde menú inferior y flujo de signout).
- Afecta a la capa de servicios frontend (nuevo contrato/implementación para obtener ficha de socio desde `/api/fichas-socio/me`).
- Afecta al sistema de recursos localizados ES (nuevas claves de textos de "Mi cuenta" y acciones de sesión).
- Afecta al SCSS global modular (`style.scss` + parciales de componentes de "Mi cuenta").
