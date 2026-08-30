## Context

La pagina `SignalDetailView` ya carga el detalle base de la signal, sus imagenes y la coleccion de comentarios mediante `ISignalService.GetSignalCommentsAsync`, y renderiza el bloque `Comentarios` dentro del propio flujo de lectura. El contrato del API ya expone `POST /api/signals/{id}/comments` con un `CreateSignalCommentRequest` que recibe `texto`, por lo que la brecha actual no es de backend disponible sino de integracion frontend, validacion y control de permisos.

La sesion frontend ya conserva roles, claim `IsMember` y `UserId` en `AuthSession`. Eso permite decidir en cliente si un usuario autenticado puede iniciar el alta sin introducir nuevas dependencias de identidad. El cambio toca varias fronteras del frontend: vista de detalle, modelo de formulario, recursos localizados, SCSS, contrato de `ISignalService`, cliente HTTP y pruebas de API/UI. Tambien debe convivir con cambios activos sobre el layout del detalle de signal, por lo que conviene mantener el alcance acotado al bloque `Comentarios`.

## Goals / Non-Goals

**Goals:**
- Permitir que el detalle de una signal muestre una accion `Anadir comentario` solo a usuarios autorizados.
- Reutilizar el endpoint existente `POST /api/signals/{id}/comments` desde la capa `ISignalService` sin acoplar la vista a HTTP.
- Abrir un modal/popup dentro del contexto de `SignalDetailView` con un campo de texto largo, validacion basica y estados de envio.
- Persistir el comentario asociado a la signal abierta y refrescar la lista de comentarios tras un alta correcta.
- Mantener mensajes, accesibilidad basica, estilos y localizacion alineados con los patrones ya usados en el proyecto.

**Non-Goals:**
- Editar, borrar, moderar o paginar comentarios existentes.
- Redisenar la pagina completa de detalle de signal o mover de nuevo la posicion del bloque `Comentarios`.
- Cambiar el contrato backend de comentarios mas alla del uso del endpoint ya disponible.
- Introducir una pantalla nueva separada para comentar o un sistema de permisos mas amplio para otras acciones sobre signals.

## Decisions

1. Centralizar la regla de permiso en la sesion frontend y reutilizarla desde la vista.
Rationale: la regla pedida es concreta y transversal (`Admin` o `Member` con `IsMember = true`). Encapsularla en `AuthSession` o en una utilidad equivalente evita repetir expresiones de roles/claims en Razor y deja el criterio listo para futuros usos relacionados con comments.
Alternatives considered:
- Resolver la condicion inline solo en `SignalDetailView`: descartado porque dispersa logica de autorizacion y hace mas fragiles los tests.

2. Extender `ISignalService` y `SignalApiClient` con una operacion dedicada de creacion de comentarios.
Rationale: el detalle ya consume comentarios desde una abstraccion de dominio. Mantener el alta en esa misma frontera preserva la separacion entre UI y transporte, permite mapear codigos HTTP a `ServiceResult` coherentes y facilita pruebas unitarias con dobles existentes.
Alternatives considered:
- Llamar al `HttpClient` directamente desde el componente: descartado porque rompe el patron actual del frontend y acopla la vista al contrato remoto.
- Reutilizar `UpdateSignalAsync` con payload mixto: descartado porque el comentario es un recurso distinto y ya tiene endpoint propio.

3. Implementar el alta como modal local al detalle siguiendo el patron visual de otros flujos popup del proyecto.
Rationale: anadir un comentario es una accion contextual y breve. Abrir un modal sobre `SignalDetailView` mantiene visible la signal a la que se asocia el comentario y evita navegacion adicional. El proyecto ya usa modales similares, por lo que la experiencia y el markup pueden mantenerse consistentes.
Alternatives considered:
- Navegar a una pantalla independiente: descartado porque anade friccion a una accion corta y rompe el contexto del detalle.
- Editor inline siempre visible: descartado porque aumenta el ruido visual del bloque `Comentarios`, especialmente en movil.

4. Refrescar solo la coleccion de comentarios tras un alta correcta, sin recargar todo el detalle.
Rationale: la creacion afecta unicamente al subrecurso de comentarios. Volver a consultar `GetSignalCommentsAsync` reduce trabajo innecesario, evita parpadeos en imagenes o tabs y mantiene el resto de la pagina estable. El modal debe cerrarse solo despues de que la operacion y el refresco hayan terminado correctamente.
Alternatives considered:
- Reejecutar `LoadAsync` completo: descartado porque recarga datos no afectados y hace mas costosa la interaccion.
- Insertar el nuevo comentario de forma optimista en memoria: descartado porque el backend es la fuente de verdad para fecha, orden y normalizacion final del texto.

5. Mantener validacion minima en frontend basada en texto requerido y delegar validaciones de negocio adicionales al backend.
Rationale: el contrato visible solo expone `texto`. El frontend debe bloquear comentarios vacios o con espacios para evitar round-trips triviales, pero no debe adivinar reglas no documentadas del backend. Los errores devueltos por el servicio se mostraran dentro del modal o del bloque de comentarios de forma comprensible.
Alternatives considered:
- Validacion rica con limites arbitrarios de longitud: descartado mientras el contrato o specs base no documenten ese requisito.

## Risks / Trade-offs

- [La propuesta convive con cambios activos sobre el layout del detalle de signal] -> Mitigacion: acotar el nuevo cambio al bloque `Comentarios`, sin reabrir decisiones de tabs, overview o etiquetas.
- [La regla `Admin` o `Member` con `IsMember = true` puede repetirse en otros puntos si no se centraliza] -> Mitigacion: encapsularla en la sesion o helper compartido desde el inicio.
- [El backend puede devolver errores de autorizacion o validacion con granularidad limitada] -> Mitigacion: mapear al menos casos de autenticacion invalida, acceso prohibido, signal inexistente y error generico de alta.
- [Refrescar la lista tras crear añade una segunda llamada al flujo de confirmacion] -> Mitigacion: restringir el refresco al subrecurso de comentarios y mantener feedback de envio hasta completar ambas operaciones.
