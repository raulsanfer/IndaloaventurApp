## Why

La botonera inferior ya reserva una entrada para "Mi Club", pero hoy no existe una experiencia propia detrás de esa opción. Crear una página índice para "Mi Club" y arrancar con una primera utilidad de "Teléfonos de interés" permite dar contenido real a esa sección y acerca a los socios información útil y frecuente dentro de la app.

## What Changes

- Se crea una nueva página `Mi Club` accesible desde el botón inferior "Mi Club" del shell autenticado.
- La nueva página `Mi Club` se comporta como índice de opciones del área, empezando por una tarjeta o acceso a `Teléfonos de interés`.
- Se crea una nueva página de `Teléfonos de interés` accesible desde `Mi Club`.
- La página de `Teléfonos de interés` consume `GET /api/agenda-telefonica` y muestra el listado completo de contactos con desplazamiento vertical cuando el volumen de datos lo requiera.
- Se añade una capa de servicio frontend desacoplada para la agenda telefónica, alineada con la arquitectura actual.
- Se actualizan navegación, recursos localizados y estilos SCSS globales para la nueva sección.
- Se documenta la discrepancia actual del contrato backend: `FichaContactoDto` no expone `email`, por lo que ese dato requerirá confirmación o ampliación de API si debe mostrarse en esta iteración.

## Capabilities

### New Capabilities
- `frontend-mi-club-page`: Define la navegación y la página índice de `Mi Club` como punto de entrada del área desde la botonera inferior.
- `frontend-club-phonebook-page`: Define la pantalla de `Teléfonos de interés`, su carga desde `agenda-telefonica` y la presentación scrollable de contactos.

### Modified Capabilities
- Ninguna.

## Impact

- Afecta a `IndaloaventurApp.SharedUI` con nuevos componentes reutilizables para el índice `Mi Club` y la lista de contactos.
- Afecta a `IndaloaventurApp.Web.Client` con nuevas rutas/páginas y la navegación desde la botonera inferior.
- Afecta a la capa de servicios frontend con un nuevo contrato/cliente para `GET /api/agenda-telefonica`.
- Afecta a los recursos localizados ES y al SCSS global modular para textos, tarjetas de opción y listado vertical.
- Puede afectar al contrato backend de `agenda-telefonica` si se confirma que el email es obligatorio en la UI final.
