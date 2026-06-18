## Context

La app ya dispone de un área privada de `Configuración` con herramientas administrativas como `Usuarios` y `Cargos`, accesibles solo para administradores y construidas con el patrón `ruta privada + vista SharedUI + cliente API`. En paralelo, el dominio de `Signals` ya consume `GET /api/signal-types` para poblar categorías visibles en el frontend público, pero no existe todavía una superficie administrativa para mantener esos tipos desde la propia aplicación.

El usuario quiere que esta gestión cuelgue de `Mi Cuenta -> Configuración -> Signals -> Categorías`, dejando además preparado un nodo intermedio `Signals` que sirva como contenedor futuro de otras funciones administrativas del dominio. El `OpenAPI` del repositorio ya describe `GET/POST /api/signal-types` y `PUT/DELETE /api/signal-types/{id}`, por lo que el frontend puede modelar el CRUD sin necesidad de inventar contratos nuevos.

## Goals / Non-Goals

**Goals:**
- Añadir una entrada administrativa `Signals` visible solo para `Admin` dentro de `Configuración`.
- Añadir una subruta `Categorías` dentro de esa zona para listar, crear, editar y eliminar `SignalTypes`.
- Permitir editar los dos campos funcionales de la categoría: `Nombre` e `Icono`.
- Mantener la coherencia visual, de rutas y de control de acceso con `Usuarios` y `Cargos`.
- Gestionar de forma controlada el intento de borrar categorías que no puedan eliminarse por tener señales asignadas.

**Non-Goals:**
- Implementar todavía una gestión administrativa completa de señales individuales.
- Añadir selector visual de iconos, búsqueda avanzada, paginación o reordenación de categorías.
- Deducir localmente si una categoría tiene señales asignadas sin apoyo del backend; esa regla se tratará como restricción operativa del endpoint de borrado.

## Decisions

### 1. Introducir un hub administrativo `Signals` antes de `Categorías`
La navegación se dividirá en una pantalla contenedora `Signals` bajo `Configuración` y una segunda pantalla `Categorías`. Esto respeta la jerarquía pedida y evita acoplar la futura administración de señales a una única página de categorías.

Alternativas consideradas:
- Enlazar `Configuración` directamente con `Categorías`: más corto hoy, pero rompe la estructura funcional pedida y deja peor preparada la expansión futura.

### 2. Reutilizar el patrón de `CargoManagementView`
La gestión de categorías seguirá el patrón ya usado en `Cargos`: breadcrumb, hero superior, formulario de alta/edición, listado inferior, mensajes de estado y rutas privadas protegidas por rol `Admin`. La razón es mantener consistencia visual y minimizar decisiones nuevas de layout.

Alternativas consideradas:
- Crear una experiencia modal o tabulada: posible, pero introduce una interacción nueva sin necesidad clara y se aleja del patrón administrativo existente.

### 3. Extender el cliente de `signals` con operaciones administrativas de `signal-types`
Se ampliará la abstracción/frontend de `Signals` para soportar crear, actualizar y eliminar categorías, en lugar de crear un cliente aislado y desconectado del dominio. La razón es que `signal-types` ya pertenece al mismo agregado funcional consumido por el listado y el wizard de creación.

Alternativas consideradas:
- Crear un servicio administrativo completamente separado de `ISignalService`: más explícito, pero añade duplicidad de modelos y de normalización de `SignalTypeDto` para un recurso ya gestionado por el cliente actual.

### 4. Delegar la restricción de borrado en la respuesta del backend
La regla “solo se puede eliminar una categoría sin señales asignadas” se modelará en frontend mediante la acción de borrado y el manejo controlado del error cuando el backend no permita la operación. Esto evita prometer una validación previa que hoy no está representada por el contrato del listado.

Alternativas consideradas:
- Bloquear el botón de borrar en base a una cuenta local de señales: requeriría datos adicionales que la API actual no expone en `signal-types`.

## Risks / Trade-offs

- [La API de borrado no distingue explícitamente “tiene señales asignadas” de otros fallos] → Mitigar con mensajes de error administrativos claros y, si hiciera falta en implementación, mapear códigos HTTP o `ProblemDetails` más específicos.
- [Extender `ISignalService` mezcla consumo público y administración] → Mitigar agrupando métodos y modelos por región funcional y manteniendo contratos explícitos.
- [Añadir un nodo intermedio `Signals` genera una pantalla extra] → Mitigar haciéndola ligera y claramente orientada a futuras herramientas del dominio.
- [Entrada textual de icono puede inducir errores de formato] → Mitigar normalizando el valor y mostrando feedback/previsualización básica si el equipo lo considera útil durante implementación.

## Migration Plan

No requiere migración de datos. El despliegue consiste en publicar nuevas rutas privadas, recursos y operaciones de cliente para `signal-types`. Si hubiera que revertir, bastaría con retirar la navegación `Signals` en `Configuración` y las vistas administrativas asociadas, sin afectar al consumo existente de categorías en `SignalHome` o en el wizard de creación.

## Open Questions

- Conviene confirmar durante implementación qué código o `ProblemDetails` devuelve el backend cuando una categoría no se puede borrar por tener señales asignadas, para afinar el mensaje mostrado.
- Falta decidir si la pantalla de categorías debe mostrar una vista previa del icono introducido o limitarse al campo textual y al render en listado.
