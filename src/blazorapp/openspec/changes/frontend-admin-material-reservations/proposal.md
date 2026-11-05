## Why

La gestión de inventario ya permite conocer el material disponible, pero el personal administrador no dispone de una interfaz para gestionar el ciclo de vida de las reservas que realizan los socios. Es necesario incorporar esta primera fase administrativa para registrar retiradas, confirmar devoluciones y resolver cancelaciones con la información y las garantías de acceso que expone la API.

## What Changes

- Añadir una nueva opción y vista de Configuración para que los usuarios con rol `Admin` consulten el listado y el detalle de las reservas de material.
- Integrar en el cliente las operaciones administrativas del contrato `/api/inventario-reservas/admin`: consulta, registro de retirada, cancelación y confirmación de devolución.
- Presentar socio, material, cantidades, fechas, estado, confirmaciones y observaciones, y habilitar exclusivamente las acciones permitidas por el estado de la reserva.
- Validar las decisiones administrativas en el cliente sin sustituir la autorización, validación de disponibilidad ni transiciones finales que impone el servidor; refrescar el estado confirmado tras cada operación.
- Aplicar los patrones visuales DaisyUI, localización y diseño responsive ya establecidos en Configuración, junto con pruebas de interfaz, cliente HTTP y flujos de error.

## Capabilities

### New Capabilities

- `frontend-admin-material-reservations`: Consulta y gestión segura, responsive y localizada de las reservas de material por usuarios administradores desde Configuración.

### Modified Capabilities

- Ninguna.

## Impact

- Componentes, página y navegación de Configuración del frontend Blazor, incluidos recursos localizados y estilos globales de Configuración.
- Nuevos contratos, servicio y cliente HTTP autenticado para los endpoints administrativos de reservas descritos en `.vscode/openapi.json`.
- Pruebas de componentes y de cliente para autorización, transición de estados, validación y errores de la API.
- La propuesta se coordina con el cambio activo `frontend-inventory-management`, pero no modifica su especificación ni implementa las vistas Member, la creación/edición de reservas por socios ni cambios sobre el material de inventario.
