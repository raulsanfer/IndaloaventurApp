## Context

La app ya dispone de un índice `Mi Club` con la opción `Teléfonos de interés`, y la nueva necesidad de negocio encaja mejor en ese espacio que en `Mi Cuenta`, porque `Licencias Federativas` es una utilidad propia del área de miembros. Al mismo tiempo, `openspec/endpoints.json` ya documenta `GET /api/licencias-federativas/me/solicitudes`, que devuelve una colección de `SolicitudLicenciaFederativaDto` con los datos necesarios para construir el listado del socio: `temporada`, `licencia`, `categoria`, `territorio`, `estado`, precios y fecha de creación.

La implementación debe seguir los patrones ya usados en el proyecto: navegación desde `IndaloaventurApp.Web.Client`, componentes compartidos en `IndaloaventurApp.SharedUI`, lógica fuera del `.razor` cuando sea posible, recursos localizados y estilos centralizados en SCSS global. También debe respetar el lenguaje visual actual de `Mi Club`, `Teléfonos de interés` y `Ficha de Socio`, incorporando el patrón de tabla con filas ancladas por temporada pedido por negocio.

## Goals / Non-Goals

**Goals:**
- Ofrecer una página operativa para que un socio autenticado consulte sus solicitudes de licencias federativas.
- Incorporar en `Mi Club` una nueva opción debajo de `Teléfonos de interés`, con un aspecto homogéneo dentro del índice del área.
- Agrupar el listado por `Temporada` y mostrar, dentro de cada grupo, los datos de `Licencia`, `Categoria`, `Ambito/Territorio` y `Estado` con composición clara en móvil y escritorio.
- Mostrar un botón `Solicitar` alineado visualmente en la cabecera superior derecha para preparar la siguiente iteración funcional.
- Resolver correctamente estados de carga, vacío, error y acceso no permitido sin romper la composición de la app.

**Non-Goals:**
- Implementar el flujo de creación de una nueva solicitud mediante `POST /api/licencias-federativas/me/solicitudes`.
- Diseñar un detalle completo de licencia o una pantalla administrativa de revisión.
- Cambiar el contrato backend de `SolicitudLicenciaFederativaDto`.
- Rediseñar `Mi Club` más allá de añadir la nueva opción y mantener la coherencia visual del índice.

## Decisions

1. Crear una página dedicada de `Licencias Federativas` y acceder a ella desde `Mi Club`.
Rationale: `Mi Club` funciona como índice de utilidades del socio; ubicar ahí la entrada mejora la semántica de navegación y mantiene separadas las áreas de cuenta y servicios para miembros.
Alternatives considered:
- Mantener el acceso dentro de `Mi Cuenta`: descartado porque mezcla datos de perfil con una utilidad operativa específica del club.

2. Introducir un servicio específico de licencias federativas en lugar de extender `IMemberProfileService`.
Rationale: aunque ambos flujos son de autoservicio del socio, el contrato de licencias tiene estados, agrupaciones y evolución propia; aislarlo en una abstracción específica reduce acoplamiento con la ficha del socio.
Alternatives considered:
- Añadir métodos de licencias a `IMemberProfileService`: descartado por mezclar dominios distintos dentro de una misma interfaz.

3. Mapear `SolicitudLicenciaFederativaDto` a un modelo de vista propio agrupable por temporada.
Rationale: la UI necesita ordenar, agrupar y presentar textos compuestos sin depender directamente del DTO remoto; un modelo intermedio facilita saneado, ordenación descendente por temporada y futuras extensiones como detalle o filtros.
Alternatives considered:
- Bind directo al DTO del API: descartado porque deja la agrupación y normalización dispersas en la vista.

4. Renderizar la experiencia principal con tabla DaisyUI y filas de grupo por `Temporada`, pero el contenido interno de cada fila como lista compacta.
Rationale: satisface el requisito visual de tabla con pinned rows y, a la vez, permite que cada temporada muestre múltiples solicitudes con mejor legibilidad que una rejilla rígida en móvil.
Alternatives considered:
- Una tabla plana de una fila por solicitud: descartada porque diluye la agrupación por temporada pedida explícitamente.
- Cards por temporada: descartadas porque se alejan del patrón de referencia solicitado.

5. Añadir la opción `Licencias Federativas` en `Mi Club` justo debajo de `Teléfonos de interés` y con el mismo lenguaje visual base.
Rationale: el usuario ha pedido reutilizar el patrón de acceso ya reconocido dentro de `Mi Club`; colocarla debajo del acceso existente preserva el orden natural del índice y facilita el descubrimiento.
Alternatives considered:
- Crear un bloque visual distinto para licencias: descartado porque rompería la consistencia del índice y generaría jerarquías innecesarias.

6. Mostrar el botón `Solicitar` como CTA visible pero no operativo en esta iteración.
Rationale: negocio quiere consolidar ya la affordance visual y reservar la lógica de solicitud para el siguiente cambio; definirlo ahora evita retrabajo de layout y copy.
Alternatives considered:
- Ocultar el botón hasta que exista el flujo completo: descartado porque retrasa una decisión de diseño ya validada.
- Conectar provisionalmente a un formulario incompleto: descartado por generar expectativas erróneas y estados a medio implementar.

7. Aplicar doble control de acceso: visibilidad contextual en `Mi Club` y estado no operativo al entrar manualmente en la ruta si `IsMember != true` o falta sesión.
Rationale: sigue el patrón ya aplicado en `Ficha de Socio` y evita que una URL conocida exponga una pantalla incoherente a usuarios no autorizados.

## Risks / Trade-offs

- [El endpoint podría devolver múltiples solicitudes con valores incompletos de `licencia`, `categoria` o `territorio`] → Mitigación: mapear con valores seguros y priorizar render robusto sin null refs ni roturas visuales.
- [El patrón tabla + lista por temporada puede complicarse en pantallas estrechas] → Mitigación: usar tabla solo como esqueleto semántico y adaptar el contenido de cada grupo con bloques apilados y espaciado SCSS responsive.
- [El botón `Solicitar` visible pero no operativo puede generar expectativa de acción inmediata] → Mitigación: dejar contractual que en esta iteración actúa como placeholder controlado y acompañarlo con copy/estado coherente en implementación.
- [Una nueva abstracción de servicio añade algo de superficie técnica] → Mitigación: mantenerla pequeña, enfocada solo en lectura del listado y preparada para incorporar el `POST` en la siguiente iteración.
