## Why

La pantalla actual de `Teléfonos de interés` muestra la información de forma demasiado compacta y ocupa más altura de la necesaria en la cabecera, lo que dificulta escanear contactos rápidamente en móvil. Este ajuste mejora la navegación contextual y la legibilidad de cada registro sin cambiar la fuente de datos.

## What Changes

- Sustituir el enlace superior simple por un breadcrumb accionable que muestre `Volver a Mi Club` seguido de `Área del socio` como página actual.
- Eliminar la descripción situada bajo el título `Teléfonos de interés` para reducir ruido visual y ganar espacio útil.
- Rediseñar el listado de contactos para que cada registro se presente como una card diferenciada, con separación visual clara entre nombre, teléfonos y datos adicionales.
- Mantener la carga desde `GET /api/agenda-telefonica`, los estados de carga/error y el scroll vertical natural de la pantalla.

## Capabilities

### New Capabilities

None.

### Modified Capabilities

- `frontend-club-phonebook-page`: cambia la navegación visible de la cabecera y la presentación de cada contacto para mejorar jerarquía visual, escaneo rápido y separación entre registros.

## Impact

- Afecta a la UI de la pantalla `Teléfonos de interés` en `IndaloaventurApp.SharedUI`.
- Requiere actualizar recursos localizados si se añade el literal `Área del socio`.
- Requiere ajustes de estilos SCSS globales para breadcrumb y cards de contacto.
- No cambia contratos backend ni endpoints existentes.
