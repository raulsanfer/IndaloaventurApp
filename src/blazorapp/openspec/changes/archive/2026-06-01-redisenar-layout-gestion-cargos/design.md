## Context

`CargoManagementView` ya existe y dispone de lógica para listar, crear, editar y borrar cargos, pero la composición actual se apoya en dos cards paralelas y acciones de borrado inmediatas. El proyecto ya ha establecido en `SettingsView` un lenguaje visual con `breadcrumbs`, `fieldset` y utilidades DaisyUI/Tailwind, por lo que este rediseño debe acercar la gestión de cargos a ese patrón sin romper el desacoplamiento entre vista Razor, lógica partial y estilos SCSS globales.

Además, el usuario ha pedido priorizar una interacción de lista con formulario superior y aplazar la eliminación real para una versión posterior. Eso implica que el cambio es principalmente de experiencia de usuario e interacción, no de contrato API.

## Goals / Non-Goals

**Goals:**
- Reordenar `CargoManagementView` en un layout vertical con breadcrumb, cabecera reforzada y fieldset superior para creación/edición.
- Mantener un único formulario reutilizable cuyo estado alterne entre `Nuevo Cargo` y edición de cargo existente.
- Representar la colección como lista administrativa consistente con DaisyUI, mostrando acciones por elemento.
- Reutilizar la lógica actual de `GET`, `POST` y `PUT` minimizando cambios en servicios y modelos.
- Retirar la eliminación operativa de esta iteración sin perder la previsión visual de la acción.

**Non-Goals:**
- Rediseñar la página `Configuración` completa o alterar su navegación principal.
- Cambiar contratos backend, modelos API o validaciones de dominio de `Cargo`.
- Implementar confirmaciones, conflictos o llamadas `DELETE /api/cargos/{id}` en esta iteración.
- Introducir estilos inline, lógica de vista fuera de los patrones del proyecto o una librería adicional de componentes.

## Decisions

1. Reutilizar la ruta y la estructura funcional existentes de `CargoManagementView`, modificando solo su composición visual y el flujo de interacción.
Rationale: reduce riesgo, preserva el trabajo previo del CRUD base y concentra el cambio en UX.
Alternatives considered:
- Crear una nueva pantalla o componente desde cero: descartado porque duplicaría lógica ya disponible.

2. Adoptar un layout secuencial con `breadcrumb`, cabecera de página, `fieldset` superior y lista debajo.
Rationale: replica patrones ya visibles en `SettingsView`, mejora la claridad del flujo principal y se adapta mejor a móvil.
Alternatives considered:
- Mantener dos columnas o cards paralelas: descartado por dar más protagonismo a la estructura que a la tarea principal.

3. Modelar el formulario como un único editor con dos modos explícitos: `Nuevo Cargo` y edición.
Rationale: evita duplicidad de formularios, permite reaprovechar el submit actual y simplifica la recarga del listado tras `POST` o `PUT`.
Alternatives considered:
- Abrir edición inline en la propia lista: descartado porque complica estados y estilos para un beneficio bajo.
- Navegar a una pantalla separada de edición: descartado por añadir fricción a un catálogo pequeño.

4. Representar el borrado como acción visible pero no operativa, preferiblemente deshabilitada o marcada como próximamente.
Rationale: responde a la petición de reservar esta capacidad para una versión posterior sin esconder una acción prevista del CRUD.
Alternatives considered:
- Eliminar el botón `Borrar` de la UI: descartado porque contradice la intención del flujo deseado.
- Mantener el borrado actual activo: descartado por salirse del alcance pedido.

5. Mantener los estilos en SCSS global y los textos en recursos.
Rationale: alinea la solución con las reglas del proyecto y facilita consistencia visual entre componentes compartidos.

## Risks / Trade-offs

- [La nueva UI puede entrar en conflicto con estados de borrado ya implementados en la clase partial] → Mitigación: simplificar el estado de la vista y desacoplar temporalmente la rama de eliminación del renderizado principal.
- [La acción `Borrar` visible pero inactiva puede generar dudas al usuario] → Mitigación: definir un estado visual explícito de no disponibilidad o mensaje `próximamente`.
- [El patrón `list` de DaisyUI puede no encajar exactamente con las clases SCSS existentes] → Mitigación: usar DaisyUI como base semántica y ajustar la personalización desde parciales SCSS del proyecto.
- [El refresco de la lista tras guardar puede producir saltos de foco en móvil] → Mitigación: reutilizar el estado actual del formulario y devolver el foco al campo principal cuando sea viable en implementación.
