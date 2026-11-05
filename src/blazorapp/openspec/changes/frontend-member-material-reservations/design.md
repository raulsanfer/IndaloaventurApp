## Context

La administración de reservas ya comparte el modelo de reserva y consume el contrato de inventario, pero no existe un acceso personal desde `Mi Club`. El acceso visible de dicha sección usa la sesión actual y las rutas personales de reservas exigen en el servidor la política `ClubMember` (rol `Member` e `IsMember=true`).

El contrato de alta exige material, cantidad, retirada prevista, devolución estimada y observaciones opcionales. Aunque el contrato ofrece `PUT`, el alcance acordado excluye editar una solicitud ya creada para que cualquier cambio vuelva a pasar por la validación de disponibilidad de una nueva solicitud. Véanse `proposal.md` y la especificación de este cambio para el comportamiento observable.

## Goals / Non-Goals

**Goals:**

- Aislar el acceso del socio en un servicio de reservas personales, sin reutilizar operaciones administrativas ni exponer identificadores de otros usuarios.
- Mantener las pantallas navegables y seguras mediante rutas de listado, detalle y alta protegidas por la sesión.
- Dar una experiencia mobile-first coherente con `Mi Club`, DaisyUI y la organización SCSS global existente.
- Dejar que el servidor sea siempre la autoridad de autorización, propiedad, disponibilidad y transiciones de estado.

**Non-Goals:**

- No se implementa `PUT /api/inventario-reservas/me/{reservaId}` ni una acción de edición.
- No se crean rutas nuevas ni se modifica el cálculo de disponibilidad, la administración de reservas ni el inventario; la respuesta existente del catálogo se amplía con la disponibilidad calculada.
- No se reservará material localmente antes de la respuesta del servidor ni se presentará la disponibilidad como una garantía.
- No se añaden dependencias, almacenamiento de tokens ni estilos inline o aislados por componente.

## Decisions

### Rutas protegidas y acceso desde Mi Club

Se añadirá la tarjeta de `Reservas de material` tras `Teléfonos de interés` en `ClubIndexView`, condicionada a un predicado de sesión que compruebe simultáneamente `IsMember` y el rol `Member`. Se crearán páginas para el listado (`/mi-club/reservas-material`), el alta (`/mi-club/reservas-material/nueva`) y el detalle (`/mi-club/reservas-material/{reservaId:guid}`); cada una comprobará esa misma condición antes de inicializar componentes que consulten la API. Una sesión no autenticada seguirá el flujo existente de inicio, y una sesión autenticada sin condición de socio volverá a `Mi Club` sin peticiones de datos.

La comprobación de cliente evita revelar datos accidentalmente, pero no sustituye a la política `ClubMember`: las respuestas `401` y `403` de la API siguen manejándose en el servicio y las vistas. Se evitará incluir `userId` en rutas, cuerpos o estado de la interfaz.

Se prefiere usar rutas separadas frente a una única vista que cambie entre lista, detalle y formulario porque el historial del navegador, enlaces y redirecciones de seguridad son más predecibles. Las tres vistas reutilizarán el encabezado, breadcrumb, formato de estados y componentes de presentación compartidos; el formulario se mantendrá como componente reutilizable de alta, no como un editor de reservas existentes.

### Servicio personal y modelos de transporte acotados

Se creará `IMemberMaterialReservationService` junto con un cliente HTTP registrado mediante DI. Sus operaciones serán: listar y leer una reserva propia, cargar el catálogo para solicitud, crear, cancelar y confirmar devolución. Las reservas seguirán usando el modelo compartido `MaterialReservation`; el catálogo se mapeará a un modelo de lectura específico y mínimo para la solicitud (identificador, descripción, bandera de activo y cantidad disponible calculada por el servidor), en lugar de acoplar la experiencia Member a las operaciones administrativas de `IInventoryService`.

El cliente adjuntará exclusivamente el esquema y token de la sesión actual. Mapeará los DTO del contrato de forma explícita, validará las respuestas JSON y convertirá problemas HTTP en el `ServiceResult` existente. `401` se identificará como sesión inválida, `403` como acceso a socio denegado y `400`/`404`/`409` conservarán solo el detalle y errores de validación seguros proporcionados por la API. No se registrarán tokens, observaciones ni datos de reservas; los fallos de red, tiempo de espera o formato tendrán mensajes genéricos.

Se evita reutilizar el cliente administrativo: sus rutas y mensajes suponen permisos Admin y su uso haría más fácil una confusión de autorización. También se evita ampliar la interfaz administrativa de inventario para no mezclar sus capacidades de escritura con el selector Member.

### Estados, orden y coherencia remota

El listado ordenará localmente por `RequestedAtUtc` descendente como salvaguarda, aunque la API normalmente lo entregue así. La fecha de solicitud se convertirá desde UTC a la zona de presentación de la aplicación; las fechas `DateOnly` se presentarán sin conversión horaria. El estado se mapeará desde el enum recibido, nunca a partir de fechas o confirmaciones.

El detalle leerá de nuevo la reserva elegida antes de habilitar acciones. Solo renderizará cancelar para `Solicitada` y confirmar devolución para `Retirada`; ambos flujos pedirán confirmación explícita, deshabilitarán el control mientras esté en curso y, tras `204`, recargarán detalle y listado. No existirá ningún `PUT` de socio. En caso de cambio requerido, el detalle explicará que debe cancelarse la solicitud y generarse una nueva.

El alta cargará `GET /api/inventario-materiales` y filtrará materiales inactivos. El catálogo incluirá la disponibilidad calculada en el servidor a partir de reservas activas y el formulario la mostrará cuando se seleccione un material, etiquetada como información de la consulta y no como garantía. Un modelo de formulario con validación declarativa y cruzada limitará material y cantidad a enteros positivos, exigirá ambas fechas y rechazará devolución anterior a recogida; normalizará observaciones permitiendo saltos de línea, sin caracteres de control no permitidos y con máximo de 2.000 caracteres. Después de `201`, la interfaz consultará de nuevo el listado antes de navegar o confirmar el resultado.

No habrá actualizaciones optimistas ni reintentos automáticos de mutaciones. Un `400` conservará la entrada y los errores de campo; un `404` eliminará selección y volverá al listado; un `409` conservará el formulario o decisión, mostrará el detalle seguro y actualizará catálogo/listado/detalle según proceda antes de reintentar; y un fallo transitorio mantendrá un estado recuperable sin afirmar éxito.

### Componentes, estilos y accesibilidad

Se crearán componentes SharedUI con parciales `.razor.cs` separados para listado, detalle y solicitud, respaldados por un formulario reutilizable de alta y modelos de entrada explícitos. Se incorporarán textos en los recursos compartidos españoles, incluidos nombres de estados, mensajes de acceso y errores, etiquetas de fecha, confirmaciones y texto accesible del botón `+`.

El listado usará `card`, `badge`, `btn` y demás clases DaisyUI coherentes con la aplicación. Las clases semánticas propias se definirán en los parciales SCSS globales: pila vertical de tarjetas, bloques de metadatos, barra de acciones y botón circular flotante a la derecha. Las reglas responsive priorizarán una columna, controles de ancho completo y espaciado seguro para navegación móvil; a partir del punto de ruptura existente limitarán el ancho de lectura sin impedir una lista vertical. Carga, vacío, error y botones inactivos expondrán texto o atributos accesibles, no solo color o iconos.

## Risks / Trade-offs

- [El usuario puede intentar crear varias solicitudes en paralelo] → La interfaz no afirma disponibilidad ni hace escrituras optimistas; el `409` se conserva y obliga a refrescar el contexto antes de reintentar.
- [La sesión puede cambiar entre la guarda de ruta y la petición] → Cada solicitud lleva la sesión actual y las respuestas `401`/`403` invalidan o bloquean la acción; el servidor aplica la autorización final.
- [La disponibilidad o el catálogo pueden cambiar mientras el formulario está abierto] → La cifra mostrada es una fotografía informativa; el servidor valida el material y las unidades al crear, y un conflicto recarga el catálogo conservando los demás valores posibles del formulario.
- [No editar reduce comodidad cuando el socio comete un error] → La cancelación confirmada y el acceso directo a nueva solicitud mantienen el flujo sencillo y fuerzan una nueva verificación de disponibilidad.
- [El endpoint de catálogo contiene más campos que el selector necesita] → El cliente los reduce a un modelo de lectura mínimo y no los muestra ni retiene en el estado de la vista.

## Migration Plan

1. Incorporar modelos, servicio HTTP y pruebas de mapeo/errores sin cambiar el contrato del backend.
2. Añadir las rutas guardadas, navegación de `Mi Club`, componentes, recursos y SCSS.
3. Ejecutar la batería de pruebas de frontend, las pruebas de autorización y el build de documentación si se actualizan documentos o contratos públicos.
4. Desplegar junto al backend que ya expone las rutas personales; si se requiere reversión, retirar las rutas y la entrada de navegación del frontend no altera reservas ya persistidas.
