## Why

Los socios reales no disponen todavía de una forma autónoma de solicitar, consultar y cerrar la devolución de material de montaña. Tras habilitar la gestión administrativa, hace falta completar el flujo desde `Mi Club` sin permitir que una modificación de una solicitud ya creada desincronice la disponibilidad que decide la API.

## What Changes

- Incorporar la entrada `Reservas de material` en `Mi Club`, inmediatamente después de `Teléfonos de interés`, solo para sesiones con rol `Member` e `IsMember=true`.
- Crear rutas y vistas protegidas para que un socio consulte sus reservas, abra su detalle y cree una nueva solicitud.
- Presentar el listado como tarjetas verticales, en orden descendente de fecha de solicitud, con únicamente material y unidades, fecha de solicitud, fecha prevista de recogida y estado; incluir un botón flotante `+` para iniciar una solicitud.
- Crear un formulario reutilizable para el alta que permita elegir un material activo, indicar cantidad, fecha prevista de recogida, fecha estimada de devolución y observaciones opcionales. Las solicitudes existentes no se editarán: el socio deberá cancelarlas y crear otra cuando necesite cambiar datos que puedan afectar a la disponibilidad.
- Mostrar, al elegir un material, las unidades disponibles calculadas por el servidor como una fotografía informativa; la API seguirá validando la disponibilidad al crear la reserva.
- Permitir en el detalle cancelar exclusivamente solicitudes `Solicitada` y confirmar de forma explícita la devolución exclusivamente en reservas `Retirada`; las reservas finales quedarán en solo lectura.
- Consumir exclusivamente las rutas personales de reservas y el catálogo autenticado de inventario, respetando que la API es la autoridad de permisos, titularidad, disponibilidad y cambios de estado.
- Mantener el lenguaje visual responsive de la aplicación mediante DaisyUI, recursos localizados y SCSS global, sin estilos inline ni dependencias nuevas.

## Capabilities

### New Capabilities

- `frontend-member-material-reservations`: Navegación, listado, detalle, alta y acciones de reserva de material para socios reales.

### Modified Capabilities

- Ninguna.

## Impact

- Afecta a `IndaloaventurApp.SharedUI` (modelos, contrato de servicio, componentes de `Mi Club`, recursos y SCSS) y a `IndaloaventurApp.Web.Client` (páginas, guardas de sesión, cliente HTTP y registro de DI).
- Consume `GET`, `POST`, `DELETE` y confirmación de devolución de `/api/inventario-reservas/me`, además de `GET /api/inventario-materiales` para el selector de materiales activos.
- Amplía la respuesta autenticada del catálogo de inventario con la disponibilidad calculada, sin introducir rutas, paquetes ni almacenamiento de credenciales adicionales.
