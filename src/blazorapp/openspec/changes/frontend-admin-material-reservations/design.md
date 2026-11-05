## Context

La API ya expone el recurso administrativo de reservas bajo `/api/inventario-reservas/admin`, con consulta de colección y detalle, y tres transiciones que devuelven `204 No Content`: retirada, cancelación y confirmación administrativa de devolución. El cambio activo `frontend-inventory-management` establece los patrones recientes de Configuración, autenticación Bearer, `ServiceResult` y cliente HTTP para administración de inventario.

El contrato devuelve una reserva con datos identificativos del socio, material, fechas, estado numérico, confirmaciones y disponibilidad. La guía `.vscode/reservas-material-frontend.md` determina que el estado de la API, no las fechas ni los booleanos, gobierna las acciones; también establece que la disponibilidad y la autorización finales pertenecen al servidor.

## Goals / Non-Goals

**Goals:**

- Incorporar una herramienta administrativa aislada en Configuración y protegida antes de solicitar o presentar información de socios.
- Ofrecer una experiencia de consulta lista/detalle y decisiones de estado clara en escritorio y móvil.
- Modelar el contrato administrativo completo con fechas de solo fecha y resultados de operación coherentes con las respuestas `204` y `ProblemDetails`.
- Mantener los patrones de la aplicación: Razor con code-behind parcial, contratos compartidos documentados, DI mediante cliente HTTP tipado, textos localizados y SCSS global con DaisyUI.

**Non-Goals:**

- No implementar las vistas, rutas, creación, edición, cancelación ni confirmación de devolución correspondientes al usuario Member, incluso si el contrato las expone.
- No cambiar la gestión de materiales, su campo de activo, sus cantidades ni el cálculo de disponibilidad; la pantalla solamente consume el resultado que devuelve el servidor.
- No añadir filtros, paginación, exportación, notificaciones por correo, auditoría ni reintentos automáticos, porque no están definidos en el contrato ni en las reglas de producto.

## Decisions

### Servicio administrativo independiente de los catálogos de inventario

Se añadirá un contrato compartido específico —`IInventoryReservationAdminService`— y su cliente HTTP tipado, en vez de ampliar `IInventoryService`. El recurso tiene identidad, datos personales y transiciones distintas de los catálogos y materiales; el límite explícito evita mezclar la futura experiencia Member con las operaciones exclusivas de Admin. El cliente reutilizará el patrón de sesión Bearer, `ServiceResult` y lectura segura de `ProblemDetails` ya utilizado por inventario, pero con códigos y mensajes de reserva.

Los modelos compartidos representarán la reserva, el socio, la disponibilidad, el estado `Solicitada`/`Retirada`/`Entregada`/`Cancelada` y las tres solicitudes de decisión. Las propiedades de fecha del contrato se tratarán como `DateOnly`; la fecha de solicitud UTC conservará su semántica temporal. Las DTO internas de transporte conservarán los nombres del JSON del contrato y normalizarán únicamente los textos cuya semántica permite hacerlo. Los contratos públicos y sus miembros recibirán documentación XML.

Alternativa descartada: añadir estas operaciones a `IInventoryService`. Reduciría archivos inicialmente, pero difuminaría la separación entre administración de reservas y administración del inventario, y complicaría la incorporación posterior de las operaciones del socio.

### Una ruta protegida y una vista lista/detalle en Configuración

Se añadirá la ruta `/configuracion/reservas-material` y su entrada localizada en `SettingsView`, ambas visibles solo para `Admin`. La página seguirá el guardia de sesión existente antes de instanciar la vista o invocar el servicio. Dentro de la herramienta, el componente cargará la colección y recuperará el detalle bajo demanda al seleccionar una reserva; esto evita que información personal completa quede persistida en cada fila y permite que las operaciones partan de un estado reciente.

En pantallas amplias se usará una composición lista/detalle; en pantallas estrechas, los mismos bloques se apilarán para evitar el desplazamiento horizontal. La lista y el detalle usarán tarjetas, badges, alertas, botones y formularios DaisyUI, con estilos de layout en la hoja SCSS global de Configuración. Los textos, etiquetas de estado, ayudas, validaciones y mensajes de error residirán en recursos compartidos; no habrá estilos inline ni CSS aislado para el componente.

Alternativa descartada: una ruta de detalle independiente. Es innecesaria para las pocas operaciones del contrato, introduce otra superficie de protección y obliga a gestionar la reserva seleccionada entre navegaciones sin aportar un requisito de URL compartible.

### Formularios de decisión acotados por el estado del detalle confirmado

El componente derivará las acciones disponibles exclusivamente de `estado` del detalle recién recibido:

| Estado API | Decisiones mostradas | Datos que se envían |
| --- | --- | --- |
| `Solicitada` | Registrar retirada; cancelar | `fechaRetirada`; `observacionAdministracion` opcional |
| `Retirada` | Confirmar devolución administrativa; cancelar | `fechaDevolucionEfectiva`; `observacionAdministracion` obligatoria |
| `Entregada` / `Cancelada` | Ninguna | No se envía ninguna transición |

Cada decisión se abrirá en un formulario o diálogo de confirmación explícita, deshabilitará su envío mientras está en curso y no modificará ni eliminará el estado mostrado de forma optimista. La fecha de devolución será obligatoria y se validará contra la fecha de retirada efectiva disponible; no se impondrán otras restricciones de fecha ni límites de observación que no estén en el contrato. La cancelación en `Retirada` exigirá texto no vacío, mientras que en `Solicitada` enviará la observación tal y como corresponda a la entrada opcional.

Tras un `204`, se recargarán lista y detalle. Esto cubre especialmente el caso en que la confirmación administrativa llega antes que la del socio: la pantalla conserva `Retirada` o muestra `Entregada` solo si lo confirma el servidor. Las solicitudes que cambian estado no se reintentarán automáticamente ante fallos de red.

Alternativa descartada: deducir una transición con fechas, confirmaciones o disponibilidad local. Es inconsistente con el contrato concurrente y podría presentar una reserva como finalizada cuando aún falta una confirmación.

### Tratamiento de errores con límites de divulgación y recarga controlada

El cliente conservará los datos de `ProblemDetails` permitidos (`detail` y errores de campo) para `400`, `404` y `409`, pero expondrá mensajes genéricos para JSON inválido, red, timeout y respuestas `5xx`. La capa de vista asociará los errores de campo a los controles y preservará las entradas en `400` y `409`.

Un `401` delegará en el flujo de sesión; un `403` bloqueará la acción y dejará de ofrecer reintento. En un `404`, la selección se limpiará inmediatamente para no retener el nombre o email del socio; después se refrescará la colección. En un `409`, se conservará el formulario y su detalle seguro, se recargarán lista y detalle si aún existen, y solo entonces se permitirá que el administrador decida si vuelve a enviar la operación. Las salidas de Razor se mantendrán codificadas por defecto y no se interpretará contenido de observaciones como marcado.

## Risks / Trade-offs

- [La transición puede cambiar entre la consulta y la decisión] → No se realizarán actualizaciones optimistas; `409` obliga a recargar antes de reintentar y el detalle posterior a `204` es la fuente de verdad.
- [La vista muestra correo electrónico y nombre de socios] → La ruta y la entrada se guardan por rol antes de cualquier petición, los datos se limpian ante `404`, y la API continúa siendo la autorización final.
- [Una operación puede haberse aplicado aunque se pierda la respuesta] → No habrá reintento automático de escrituras; el administrador recargará el detalle/listado para verificar el estado antes de volver a actuar.
- [El cambio de inventario aún está activo] → Se reutilizarán solamente sus convenciones ya presentes; la implementación no modificará sus requisitos ni dependerá de que sus pantallas se carguen para utilizar reservas.
- [El endpoint de listado no ofrece filtros ni paginación] → La primera fase cargará la colección completa y no inventará parámetros; se podrá extender posteriormente si el contrato los incorpora.

## Migration Plan

1. Incorporar modelos, contrato, cliente HTTP tipado y registro DI junto con sus pruebas de contrato HTTP.
2. Añadir la entrada de Configuración, la ruta protegida y la vista administrativa con pruebas de autorización y presentación.
3. Activar las tres decisiones condicionadas por estado y verificar la recarga posterior a cada respuesta correcta y los errores de concurrencia.
4. Publicar con los endpoints ya desplegados. Si fuera necesario revertir, retirar la entrada y la ruta del frontend; no hay migraciones de datos ni cambios de API que revertir.
