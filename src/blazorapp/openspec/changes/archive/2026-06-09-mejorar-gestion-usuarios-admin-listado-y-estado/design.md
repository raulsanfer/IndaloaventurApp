## Context

La pantalla `Configuración -> Usuarios` ya está restringida a perfiles `Admin` y actualmente consume `GET /api/users?email=...` a través de `AdminUserManagementApiClient`. La experiencia parte de un estado vacío hasta que el administrador introduce un email, y el listado resultante solo permite crear ficha para no socios o navegar a la edición de la ficha del socio.

En paralelo, el backend ya soporta dos piezas que el frontend no aprovecha todavía: `GET /api/users` sin filtro para devolver el listado completo, y los endpoints `POST /api/users/{userId}/deactivate` y `POST /api/users/{userId}/reactivate` para el ciclo de vida lógico de la cuenta. El problema restante es que el contrato `ManagedUserDto` documentado para frontend no expone el estado activo/deactivado, así que la UI no puede decidir ni comunicar con seguridad qué acción corresponde.

## Goals / Non-Goals

**Goals:**
- Mostrar el listado completo de usuarios al abrir la página administrativa.
- Mantener la búsqueda por email como filtro opcional sobre el mismo endpoint de usuarios.
- Permitir editar un usuario desde su fila y seguir ofreciendo `Crear ficha` cuando aún no sea socio.
- Exponer el estado activo de la cuenta en el contrato consumido por frontend para renderizar badges y acciones coherentes.
- Permitir desactivar y reactivar usuarios desde la ficha administrativa con feedback controlado.

**Non-Goals:**
- Añadir paginación, ordenación avanzada o filtros múltiples.
- Editar roles o privilegios administrativos.
- Rediseñar la arquitectura general de `Configuración` o de la ficha administrativa.
- Resolver desactivación/reactivación mediante heurísticas derivadas de errores de login o llamadas extra no explícitas.

## Decisions

1. Usar `GET /api/users` sin query al entrar en la página y reutilizar el mismo endpoint con `?email=` cuando el administrador filtre.
Rationale: aprovecha un contrato ya soportado por backend y convierte la búsqueda en refinamiento, no en requisito de entrada.
Alternatives considered:
- Mantener el estado inicial vacío y añadir un botón “cargar todos”: descartado porque mantiene fricción innecesaria.

2. Extender el contrato consumido de `ManagedUserDto` con un booleano explícito de estado activo, preferentemente `isActive`.
Rationale: la UI necesita decidir entre `Desactivar` y `Reactivar` sin inferencias. Un campo explícito evita ambigüedad y permite reutilizarlo tanto en listado como en ficha administrativa.
Alternatives considered:
- Consultar un endpoint adicional por usuario: descartado por sobrecoste y complejidad de coordinación.
- Inferir el estado a partir de claims o roles: descartado porque no representa el ciclo de vida lógico de la cuenta.

3. Mantener la edición administrativa y el ciclo de vida de cuenta en la ficha administrativa del usuario, no inline en el listado.
Rationale: la vista de edición ya existe, es el contexto natural para acciones sensibles y evita sobrecargar el listado con operaciones de alto impacto.
Alternatives considered:
- Añadir botones de desactivar/reactivar directamente en cada fila del listado: descartado en esta iteración por riesgo de errores operativos y menor contexto.

4. Refrescar el estado administrativo tras desactivar o reactivar sin sacar al admin de la ficha.
Rationale: el admin necesita confirmación inmediata y continuidad para seguir revisando o editando el mismo registro.
Alternatives considered:
- Redirigir de vuelta al listado tras cada acción: descartado porque rompe el flujo de trabajo y obliga a relocalizar al usuario.

## Risks / Trade-offs

- [El contrato actual documentado para frontend no incluye `isActive`] -> Mitigación: dejar explícito en la propuesta que el modelo de respuesta debe enriquecerse antes de implementar la UI final.
- [Un listado completo puede crecer con el tiempo] -> Mitigación: limitar esta iteración a la experiencia actual y dejar paginación/ordenación fuera del alcance si el volumen lo exige después.
- [Desactivar un usuario es una acción sensible] -> Mitigación: ubicarla en la ficha administrativa, mostrar estado actual y exigir feedback claro tras ejecutar la operación.
- [Puede existir desfase entre el estado mostrado en listado y la ficha si el usuario cambia mientras se navega] -> Mitigación: recargar la ficha desde backend al entrar y tras cada acción de ciclo de vida.
