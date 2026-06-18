## Context

La solución ya cuenta con login y home autenticada con navegación inferior, pero aún no dispone de una pantalla de cuenta de usuario que centralice información del miembro y acciones de perfil/sesión. El diseño objetivo está definido en `openspec/design/mi_cuenta` y exige coherencia con la arquitectura ya establecida: componentes reutilizables en SharedUI, estilos SCSS globales, localización por claves y servicios desacoplados con `IHttpClientFactory`.

La nueva pantalla debe consumir datos reales del miembro desde `GET /api/fichas-socio/me`, mostrar el bloque de Cargo solo cuando aplique, montar los enlaces y bloques visuales del diseño actualizado (métricas, actividad, soporte y programa de puntos), y habilitar cierre de sesión operativo.

## Goals / Non-Goals

**Goals:**
- Añadir una ruta/página "Mi cuenta" accesible desde el botón inferior "Mi cuenta" en el shell autenticado.
- Implementar el layout y componentes visuales según `openspec/design/mi_cuenta` en móvil y escritorio, incluyendo:
  - bloque de identidad (avatar + nombre),
  - badge de cargo condicional,
  - tarjetas de métricas,
  - listas de enlaces de actividad/soporte,
  - bloque de programa de puntos con CTA.
- Consumir `GET /api/fichas-socio/me` para poblar datos básicos del miembro en la página.
- Mostrar/ocultar el componente de Cargo en función de si el miembro tiene Cargo.
- Implementar cierre de sesión que limpie el estado local de sesión y redirija al login.

**Non-Goals:**
- Implementar edición completa de ficha de socio o flujo funcional de "Ficha Socio" en esta iteración.
- Introducir nuevos endpoints backend o cambiar contratos API existentes.
- Rediseñar el shell de navegación fuera de la adición del acceso "Mi cuenta".

## Decisions

1. Nueva composición de página `MiCuentaPage` sobre shell autenticado
- Decisión: componer la página en Web.Client y reutilizar componentes de SharedUI para secciones de perfil, enlaces y acciones.
- Rationale: mantiene desacoplo y reutilización en futuras plataformas.
- Alternativa: vista monolítica en un solo `.razor`. Rechazada por baja mantenibilidad.

2. Servicio dedicado para perfil de miembro (`IMemberProfileService`)
- Decisión: crear contrato de aplicación y cliente HTTP tipado que consuma `/api/fichas-socio/me`.
- Rationale: separa UI de transporte, facilita tests y respeta patrón vigente.
- Alternativa: llamadas HTTP directas desde componente. Rechazada por mezclar responsabilidades.

3. Componente `CargoBadge` con visibilidad condicional
- Decisión: representar Cargo como componente aislado y renderizarlo solo si el modelo devuelto informa cargo (`cargoId` o descripción equivalente no vacía).
- Rationale: encapsula regla de negocio visual y simplifica la página principal.
- Alternativa: condición inline repetida. Rechazada por duplicación.

4. Enlaces y CTA del diseño montados con estados explícitos
- Decisión: renderizar todos los enlaces previstos en diseño (`Mis Inscripciones`, `Mis Rutas Favoritas`, `Configuración`, `Ayuda`, `Cerrar Sesión`) y CTA `Ver Tienda`; "Ficha Socio" se muestra como "próximamente" sin navegación funcional por ahora.
- Rationale: permite validar UX completa sin adelantar funcionalidad no prioritaria.
- Alternativa: omitir enlaces no funcionales. Rechazada por divergencia con diseño.

5. Cierre de sesión como acción de sesión transversal
- Decisión: implementar `ISessionService.SignOut()` para limpiar token/estado y navegar a login.
- Rationale: centraliza seguridad de sesión y evita lógica duplicada.
- Alternativa: redirección simple sin limpieza. Rechazada por riesgo de sesión residual.

## Risks / Trade-offs

- [Inconsistencia del dato Cargo en API] → Mitigación: mapear de forma defensiva y tratar ausencia de cargo como "oculto".
- [Enlace "Ficha Socio" puede confundirse como funcional] → Mitigación: etiqueta visual "próximamente" y estado no interactivo claro.
- [Dependencia de token para `/me`] → Mitigación: si la petición falla por sesión inválida, forzar signout y vuelta a login.
- [Desalineación entre diseño de botonera (Mi Cuenta activo) y estado de ruta] → Mitigación: marcar estado activo por ruta y validar visualmente el ítem "Mi Cuenta".

## Migration Plan

- No requiere migración de datos.
- Despliegue incremental de frontend: añadir ruta, componentes, estilos y servicios.
- Rollback: revertir los nuevos componentes/rutas/registro DI asociados a "Mi cuenta".

## Open Questions

- ¿El enlace `Ver Tienda` debe abrir URL externa fija o quedarse como placeholder no operativo en esta iteración?
