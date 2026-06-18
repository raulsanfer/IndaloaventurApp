## Context

`ClubPhonebookView` ya dispone de una estructura propia con SCSS global, pero el resultado actual sigue viéndose demasiado lineal: nombre y bloques de datos aparecen casi como texto apilado, sin el peso visual de una ficha realmente independiente. Además, la captura actual confirma que el usuario espera un resultado mucho más evidente: cada contacto debe leerse como una card autónoma, con borde limpio y con los datos de teléfono/email presentados debajo del nombre con mayor intención visual.

El objetivo no es migrar la pantalla completa ni reescribir el sistema visual del proyecto, sino medir el encaje práctico de las clases `card` de DaisyUI sobre los items del listado.

## Goals / Non-Goals

**Goals:**
- Aplicar DaisyUI únicamente al patrón visual de las fichas de contacto del listado.
- Conseguir que cada contacto se perciba claramente como una card independiente con borde sencillo y separación clara.
- Mantener el nombre del contacto con su jerarquía actual y colocar debajo teléfonos y email con un diseño más elaborado.
- Mantener intacta la lógica de datos, localización y navegación ya existente.
- Evaluar convivencia entre clases DaisyUI/Tailwind y estilos SCSS actuales sin propagar el cambio al resto de vistas.

**Non-Goals:**
- No migrar toda la página `Teléfonos de interés` a DaisyUI.
- No refactorizar shell, breadcrumb, servicios ni modelos de contacto.
- No convertir DaisyUI en estándar general del proyecto en esta iteración.
- No sustituir todavía todos los estilos SCSS del módulo `Mi Club`.

## Decisions

1. Limitar DaisyUI al contenedor `card` y a la composición interna de cada item del listado.
Rationale: permite que el cambio sea muy visible en los registros de contacto sin invadir toda la pantalla ni romper el resto del layout.
Alternatives considered:
- Migrar toda la vista a DaisyUI: descartado por exceso de alcance para un piloto.

2. Mantener el nombre del contacto como encabezado principal de la card y mover debajo la información de teléfono/email en bloques visuales más cuidados.
Rationale: responde exactamente al resultado esperado por el usuario sin cambiar la jerarquía semántica del nombre.
Alternatives considered:
- Reorganizar toda la ficha con cabeceras de sección dominantes: descartado porque en la captura actual las secciones pesan más que la propia card y diluyen el efecto visual buscado.

3. Mantener breadcrumb, estados de carga/error y grid exterior bajo control del layout existente.
Rationale: así el experimento mide la utilidad de DaisyUI en cards concretas, no una reescritura completa de la vista.

4. Permitir coexistencia temporal entre clases DaisyUI y SCSS propio.
Rationale: el piloto debe validar esa convivencia, ya que el proyecto hoy no está construido enteramente sobre Tailwind.
Alternatives considered:
- Eliminar SCSS del componente durante el piloto: descartado porque hace más difícil comparar y aumenta el coste de rollback.

5. Evitar una apariencia “plana” o demasiado textual dentro de cada ficha.
Rationale: el objetivo del piloto no es solo cambiar clases, sino conseguir una percepción real de card independiente.

## Risks / Trade-offs

- [DaisyUI puede introducir un look demasiado distinto al resto de la app] → Mitigar limitando el piloto a las cards y ajustando tokens/colores de apoyo si es necesario.
- [La convivencia con SCSS puede duplicar responsabilidades de estilo] → Mitigar definiendo claramente qué parte controla DaisyUI y qué parte sigue en SCSS.
- [El proyecto puede no tener todavía Tailwind/DaisyUI operativo de verdad] → Mitigar incluyendo como primer paso técnico la validación o activación mínima del toolchain.
- [La ficha puede seguir viéndose como texto apilado aunque se añada una clase `card`] → Mitigar definiendo explícitamente borde simple, separación entre registros y bloques internos más trabajados para teléfono/email.

## Migration Plan

- Confirmar o habilitar soporte real de Tailwind + DaisyUI en el frontend.
- Refactorizar solo el marcado de cada ficha de contacto para usar el patrón `card` del piloto.
- Ajustar el SCSS residual para eliminar conflictos y preservar la maquetación general del listado.
- Validar visualmente y decidir si DaisyUI se amplía, se mantiene como uso puntual o se descarta.

## Open Questions

- ¿Quieres que dirección y observaciones sigan visibles en la misma card como contenido secundario, o prefieres que este primer piloto se centre en nombre + teléfono + email?
