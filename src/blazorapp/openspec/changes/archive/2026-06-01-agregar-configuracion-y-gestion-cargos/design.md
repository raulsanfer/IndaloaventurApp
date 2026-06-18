## Context

La aplicación ya dispone de una pantalla `Mi cuenta` con la entrada visual `Configuración`, pero esa navegación todavía no lleva a una pantalla funcional. El cambio introduce una nueva página autenticada con un panel administrativo condicionado por rol `Admin`, además de una primera herramienta administrativa para gestionar el catálogo de `Cargos`, cuyo contrato backend ya existe en `openspec/endpoints.json`.

El proyecto separa la UI compartida en `IndaloaventurApp.SharedUI`, centraliza estilos en SCSS global y usa servicios desacoplados para acceder al API. La solución debe respetar ese patrón y evitar lógica de autorización repartida de forma inconsistente entre páginas y componentes.

## Goals / Non-Goals

**Goals:**
- Añadir una ruta `Configuración` enlazada desde `Mi cuenta`.
- Mostrar un bloque `Administración` solo cuando la sesión autenticada contenga el rol `Admin`.
- Exponer la opción `Cargos` dentro del panel administrativo con acceso solo `Admin`.
- Permitir gestionar el catálogo de cargos mediante listado, creación, edición y eliminación apoyándose en `/api/cargos`.
- Mantener la implementación desacoplada entre página, componentes visuales y cliente API.

**Non-Goals:**
- Asignar cargos a usuarios miembros o editar `cargoId` dentro de fichas de socio.
- Crear un sistema de permisos genérico más allá de la comprobación actual del rol `Admin`.
- Añadir más utilidades al panel `Administración` aparte de `Cargos` en esta iteración.
- Rediseñar de forma global la pantalla `Mi cuenta` o la navegación inferior.

## Decisions

1. Crear `Configuración` como pantalla autenticada separada y enlazarla desde `Mi cuenta`.
Rationale: mantiene una navegación clara, evita sobrecargar `Mi cuenta` y deja una superficie preparada para ampliar opciones de configuración o administración más adelante.
Alternatives considered:
- Resolver `Configuración` como modal desde `Mi cuenta`: descartado porque dificulta escalabilidad y navegación profunda.
- Incrustar el panel `Administración` dentro de `Mi cuenta`: descartado porque mezcla perfil personal con funciones administrativas.

2. Aplicar control de visibilidad del panel `Administración` y de la opción `Cargos` usando el rol `Admin` en la sesión del frontend.
Rationale: el requisito del usuario es de visibilidad y acceso exclusivo para `Admin`; centralizar esa comprobación en el modelo de sesión y en la página reduce duplicidad y evita parpadeos de UI.
Alternatives considered:
- Basarse en `IsMember`: descartado porque no representa privilegios administrativos.
- Mostrar la opción y dejar que falle en backend: descartado por mala experiencia y exposición innecesaria de navegación.

3. Modelar `Cargos` como gestión de catálogo independiente de las fichas de socio.
Rationale: los endpoints disponibles (`GET/POST /api/cargos`, `PUT/DELETE /api/cargos/{id}`) cubren CRUD del catálogo, no la asignación a usuarios; separar ambos problemas evita prometer un alcance que el contrato actual no soporta.
Alternatives considered:
- Incluir edición de cargos por miembro en esta propuesta: descartado porque requeriría otros endpoints y reglas de negocio no descritas.

4. Implementar la UI de `Cargos` con patrón de lista administrativa + formulario de alta/edición y acciones de borrado con feedback controlado.
Rationale: es una interacción simple, consistente con el stack Blazor y suficiente para operaciones CRUD sin introducir dependencias externas.
Alternatives considered:
- Tabla avanzada con librería de terceros: descartada por coste y falta de necesidad actual.
- Pantallas separadas por operación: descartado por añadir fricción para un catálogo pequeño.

5. Mantener textos en recursos ES y estilos en parciales SCSS globales.
Rationale: alinea la implementación con las reglas del proyecto y evita estilos inline o literales acoplados al componente.

## Risks / Trade-offs

- [La sesión actual podría no exponer el rol `Admin` de forma uniforme en todos los componentes] → Mitigación: validar el origen del rol en el modelo/autenticación actual y documentar la dependencia en implementación.
- [El borrado de un cargo puede devolver `409 Conflict` si el backend lo tiene en uso] → Mitigación: contemplar estado de error controlado y mensaje específico para que la UI no quede bloqueada.
- [Usuarios no admin podrían acceder manualmente a la URL de `Configuración`] → Mitigación: permitir la carga de la página autenticada pero ocultar completamente el bloque administrativo y sus accesos internos; si existe guard de ruta, reutilizarlo para páginas hijas admin.
- [La gestión de catálogo sin asignación a miembros puede percibirse incompleta] → Mitigación: dejar explicitado en propuesta y specs que el alcance se limita al catálogo de cargos.
