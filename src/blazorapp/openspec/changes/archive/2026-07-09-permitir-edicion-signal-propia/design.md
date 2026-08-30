## Context

La pantalla de detalle de `Signals` ya recupera y renderiza una signal concreta, pero hoy es completamente de solo lectura. El payload de detalle ya contiene `UserIdAlta` en el API, aunque el frontend actual lo descarta al mapear `SignalDetailItem`, y la sesión frontend no expone todavía el identificador del usuario autenticado aunque ya parsea el JWT para resolver roles e `IsMember`.

Además, `SignalApiClient` no ofrece una operación de actualización de signals y el contrato `PUT /api/signals/{id}` existente en el API actual exige más campos de los que el usuario debe poder editar en este cambio. Eso convierte la edición limitada de signals propias en un cambio transversal entre sesión, detalle, servicio frontend y dependencia de autorización/contrato en el API adyacente.

## Goals / Non-Goals

**Goals:**
- Mostrar `Editar` en el detalle solo cuando la signal pertenezca al usuario autenticado.
- Permitir modificar exclusivamente `Título`, `Descripción` y `Estado` de una signal propia.
- Mantener sin cambios el resto de campos de la signal fuera del alcance de edición.
- Reflejar éxito, error o denegación de autorización sin romper la navegación actual de detalle.

**Non-Goals:**
- Permitir edición de categoría, fotos, tags, ubicación o comentarios.
- Añadir puntos de entrada globales adicionales para editar signals fuera del detalle.
- Habilitar que un usuario edite signals de otros usuarios o que un admin use este mismo flujo para terceros.
- Rediseñar el layout base del detalle más allá del CTA y la experiencia mínima necesaria para editar.

## Decisions

1. Exponer el identificador del usuario autenticado en `AuthSession` a partir del JWT ya recibido en login.
Rationale: el frontend ya extrae claims del token para roles e `IsMember`. Reutilizar ese patrón para `NameIdentifier` evita depender de un endpoint adicional solo para comparar ownership.
Alternatives considered:
- Añadir un endpoint `whoami`: descartado por sobrecoste de integración para una necesidad que el JWT ya puede cubrir.
- Mostrar el botón `Editar` a todos y confiar solo en el backend: descartado porque degrada la UX y no cumple la expectativa de ocultar la acción en signals ajenas.

2. Conservar el `UserIdAlta` de la signal en el modelo de detalle frontend y derivar un estado `CanEditOwnSignal` en la vista de detalle.
Rationale: el control de visibilidad del CTA debe resolverse en el propio flujo de detalle, que es la única superficie desde la que el usuario puede iniciar esta edición.
Alternatives considered:
- Pedir un flag `isOwner` específico al backend: descartado porque duplica información que ya puede derivarse con sesión + autor.
- Resolver ownership solo al guardar: descartado porque no evita enseñar una acción inválida.

3. Modelar la edición como un flujo dedicado de edición limitada invocado desde el detalle, sin ampliar el alcance de campos editables.
Rationale: separar el modo lectura del modo edición reduce ambigüedad y hace más fácil validar que el usuario solo toca `Título`, `Descripción` y `Estado`.
Alternatives considered:
- Convertir el detalle entero en inline-editable: descartado porque mezcla demasiados bloques de solo lectura con edición y aumenta el riesgo de regresión visual.
- Reutilizar el flujo de creación completo: descartado porque introduce pasos y campos no editables en este cambio.

4. Exigir que el contrato de actualización preserve en backend los campos no editables por este flujo.
Rationale: el usuario no debe verse obligado a reenviar fotos, tipología, coordenadas o tags solo para corregir texto o estado. La operación efectiva debe ser compatible con una edición limitada de dominio.
Alternatives considered:
- Hacer que el frontend recupere todos los campos no editables y reenvíe un `PUT` completo: descartado por acoplamiento innecesario, peor UX y mayor riesgo sobre fotos o datos que no se pretendían tocar.

## Risks / Trade-offs

- [El JWT puede no incluir `NameIdentifier` o usar otra claim] -> Mitigación: dejar explícita la resolución de claim como parte del diseño y contemplar error/ocultación segura si no se puede determinar ownership.
- [El API actual de update exige demasiados campos para una edición limitada] -> Mitigación: tratar la compatibilidad del contrato como parte del cambio y no como detalle implícito del frontend.
- [Ocultar el botón en frontend no sustituye la seguridad real] -> Mitigación: exigir también denegación efectiva al intentar guardar una signal ajena.
- [Añadir modo de edición dentro del detalle puede tensar la complejidad del componente] -> Mitigación: aislar estado/modelo de edición y limitar claramente el alcance del formulario.

## Migration Plan

1. Extender sesión frontend para conocer `UserId` del usuario autenticado.
2. Ampliar el detalle de signal para conservar ownership y mostrar `Editar` solo al propietario.
3. Incorporar la operación de actualización limitada y la experiencia de formulario.
4. Verificar con tests de componente/servicio y validación manual que un propietario puede editar solo esos tres campos y que un no propietario no recibe la acción.

## Open Questions

- Queda por concretar si el API adyacente adaptará `PUT /api/signals/{id}` para edición limitada o si expondrá un contrato específico para esta operación.
- Si en el futuro se quiere permitir edición de fotos, tags o ubicación, convendrá diseñar un flujo distinto para no sobrecargar esta primera iteración.
