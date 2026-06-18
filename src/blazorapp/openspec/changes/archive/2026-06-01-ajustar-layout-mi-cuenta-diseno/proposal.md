## Why

El layout actual de `MyAccountView` no coincide con el `screen.png` objetivo de `openspec/design/mi_cuenta`, generando diferencias visuales y estructurales en una pantalla clave de usuario autenticado. Ajustarlo ahora asegura consistencia de UX con el diseño aprobado antes de continuar nuevas iteraciones funcionales.

## What Changes

- Se corrige la composición de `MyAccountView` para alinearla exactamente con el `screen.png` proporcionado.
- Se define la jerarquía visual esperada: avatar + nombre, badge de membresía, tarjeta de Cargo, sección `MIS DATOS` y sección `AJUSTES Y SOPORTE`.
- Se ajusta la lista de accesos para mostrar `Ficha Socio` y `Licencias Federativas` dentro de `MIS DATOS`.
- Se incorpora la nueva propiedad de usuario `IsMember` para controlar visibilidad de componentes en `Mi Cuenta`.
- Se define que `MemberCargoBadge`, `Ficha Socio` y `Licencias Federativas` MUST ocultarse cuando `IsMember = false`.
- Se elimina del layout de `Mi Cuenta` cualquier bloque no presente en el diseño actual (por ejemplo, programa de puntos o métricas no representadas en la captura objetivo).
- Se mantiene `Cerrar Sesión` operativo y visible dentro de `AJUSTES Y SOPORTE`.
- Se valida el estado activo de `Mi Cuenta` en la botonera inferior.

## Capabilities

### New Capabilities
- `frontend-mi-cuenta-layout-alignment`: Alinea la estructura y el estilo de la página `Mi Cuenta` con el diseño de referencia actual (`screen.png`).

### Modified Capabilities
- Ninguna.

## Impact

- Afecta a `IndaloaventurApp.SharedUI` (`MyAccountView`, subcomponentes y literales asociados).
- Afecta a `IndaloaventurApp.Web` en estilos globales SCSS/CSS de la pantalla `Mi Cuenta` y botonera inferior.
- Afecta al mapeo de datos de usuario/perfil para exponer `IsMember` en la capa frontend.
- No introduce nuevos endpoints; usa la nueva propiedad disponible en el API.
