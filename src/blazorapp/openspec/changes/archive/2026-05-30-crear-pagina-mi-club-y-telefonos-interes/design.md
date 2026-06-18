## Context

La app ya dispone de shell autenticado, Home y Mi Cuenta, y la botonera inferior muestra una entrada "Mi Club" que actualmente apunta a un ancla en Home (`/home#club`) en lugar de a una página dedicada. El cambio introduce un pequeño flujo nuevo en dos niveles:

- una página índice `Mi Club`
- una primera pantalla de detalle `Teléfonos de interés`

El endpoint relevante ya existe en `openspec/endpoints.json` como `GET /api/agenda-telefonica` y devuelve una colección de `FichaContactoDto`. El contrato actual expone `nombre`, `telefono1`, `telefono2` y `observaciones`; no expone `email`, aunque el objetivo funcional deseado menciona ese dato. La implementación debe respetar además las reglas del proyecto: componentes reutilizables en `SharedUI`, lógica C# en clases partial, literales localizados y estilos en SCSS global sin CSS inline.

## Goals / Non-Goals

**Goals:**
- Sustituir el acceso actual de la botonera inferior para que `Mi Club` navegue a una página propia.
- Mostrar en `Mi Club` un índice simple de opciones con una primera entrada `Teléfonos de interés`.
- Crear una pantalla de agenda telefónica que cargue contactos desde `GET /api/agenda-telefonica`.
- Permitir lectura cómoda de listados largos mediante scroll vertical.
- Diseñar la UI de forma que pueda ampliarse con más opciones de `Mi Club` sin rehacer la estructura base.

**Non-Goals:**
- Añadir edición, búsqueda o filtros sobre la agenda telefónica en esta iteración.
- Cambiar la estructura general del shell autenticado más allá de la navegación de `Mi Club`.
- Resolver de forma definitiva el soporte de `email` si el backend no lo expone todavía.
- Introducir dependencias JS externas para listas, navegación o scroll.

## Decisions

1. Página `Mi Club` como índice desacoplado y extensible
- Decision: crear una ruta dedicada de `Mi Club` con un listado/tarjetas de opciones en vez de llevar al usuario directamente a la agenda.
- Rationale: el requerimiento describe `Mi Club` como un índice y eso deja preparada la sección para futuras utilidades.
- Alternatives considered:
  - Navegar directamente a `Teléfonos de interés`: descartado porque limita la evolución de la sección y no cumple la idea de índice.

2. Nueva pantalla específica para `Teléfonos de interés`
- Decision: separar el índice `Mi Club` y la lista de teléfonos en dos páginas distintas.
- Rationale: mejora claridad de navegación, evita sobrecargar la pantalla índice y facilita futuras rutas adicionales.
- Alternatives considered:
  - Renderizar la agenda dentro de `Mi Club`: descartado por mezclar navegación y contenido de detalle.

3. Servicio frontend tipado para `agenda-telefonica`
- Decision: introducir un contrato desacoplado de UI y un cliente HTTP dedicado para `GET /api/agenda-telefonica`.
- Rationale: sigue el patrón ya usado en otras áreas (`Mi Cuenta`, noticias WordPress) y reduce acoplamiento entre UI y transporte.
- Alternatives considered:
  - Consumir HTTP directamente desde el componente: descartado por mezclar presentación y acceso a datos.

4. Lista vertical basada en scroll nativo
- Decision: usar un contenedor vertical estándar con tarjetas de contacto y scroll nativo.
- Rationale: es suficiente para un directorio telefónico, mantiene buena experiencia en móvil y escritorio y evita complejidad innecesaria.
- Alternatives considered:
  - Virtualización o paginación inicial: descartadas para esta primera iteración por no estar justificadas todavía.

5. Modelo de contacto preparado para datos opcionales
- Decision: diseñar la tarjeta de contacto para mostrar `nombre`, teléfonos y el resto de datos disponibles, dejando `email` como campo opcional dependiente del contrato final.
- Rationale: el contrato actual no incluye `email`, así que la propuesta debe reflejar el estado real del backend sin bloquear el avance del frontend.
- Alternatives considered:
  - Inventar un campo `email` en frontend sin soporte de API: descartado por inconsistencia y riesgo funcional.
  - Forzar cambio backend dentro de este mismo alcance sin confirmación: descartado hasta validar si ese dato debe incorporarse al endpoint.

## Risks / Trade-offs

- [El contrato actual de `agenda-telefonica` no incluye `email`] → Mitigación: dejar el campo como dependencia abierta y diseñar la UI para aceptar ampliación del DTO sin rediseño.
- [La agenda puede crecer y producir tarjetas largas o repetitivas] → Mitigación: usar una lista vertical limpia, espaciado consistente y tarjetas compactas.
- [La sección `Mi Club` puede quedarse demasiado vacía con una sola opción] → Mitigación: presentar la pantalla claramente como índice y dejar estructura preparada para añadir nuevas utilidades.
- [Cambiar la ruta del botón inferior puede afectar expectativas de navegación actuales] → Mitigación: mantener el estado activo de la botonera y un naming consistente con `Mi Cuenta` y `Home`.

## Migration Plan

- Crear la nueva ruta/página `Mi Club` y redirigir a ella el botón inferior actual.
- Añadir la nueva ruta/página `Teléfonos de interés`.
- Implementar el servicio frontend para `agenda-telefonica` y conectarlo a la nueva vista.
- Incorporar recursos localizados y parciales SCSS globales.
- Rollback simple: restaurar el enlace anterior de la botonera a `/home#club` y retirar las nuevas páginas/componentes.

## Open Questions

- ¿Debe ampliarse el backend de `agenda-telefonica` para incluir `email`, o en esta iteración la pantalla debe limitarse a los campos actualmente disponibles en `FichaContactoDto`?
