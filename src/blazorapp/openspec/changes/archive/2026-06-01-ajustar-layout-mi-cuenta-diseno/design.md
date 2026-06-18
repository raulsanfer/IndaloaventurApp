## Context

Tras implementar la página `Mi Cuenta`, el resultado visual no replica fielmente la referencia final de `openspec/design/mi_cuenta/screen.png`. El diseño objetivo actual es más compacto y específico: bloque de perfil superior, tarjeta de cargo, dos grupos de enlaces (`MIS DATOS` y `AJUSTES Y SOPORTE`) y navegación inferior con `Mi Cuenta` activa.
Además, el API incorpora una nueva propiedad de usuario `IsMember` (bit) que debe gobernar la visibilidad de bloques específicos de miembro en esta pantalla.

## Goals / Non-Goals

**Goals:**
- Igualar la estructura visual de `MyAccountView` al `screen.png` de referencia.
- Mantener visibilidad condicional de Cargo (mostrar solo si existe cargo).
- Mostrar `Ficha Socio` y `Licencias Federativas` como enlaces del bloque `MIS DATOS`.
- Aplicar regla de visibilidad por `IsMember`:
  - `IsMember = true`: se muestran `MemberCargoBadge`, `Ficha Socio` y `Licencias Federativas`.
  - `IsMember = false`: esos tres elementos se ocultan.
- Mantener `Cerrar Sesión` operativo en `AJUSTES Y SOPORTE`.
- Confirmar estado activo del botón `Mi Cuenta` en la botonera inferior.

**Non-Goals:**
- Introducir nuevos módulos no visibles en el diseño de referencia.
- Cambiar contratos de servicios de autenticación o perfil de miembro.
- Implementar funcionalidad completa de `Ficha Socio` en esta corrección visual.

## Decisions

1. Reajuste de layout por secciones exactas del screen
- Decisión: reestructurar `MyAccountView` para reflejar únicamente bloques presentes en el diseño final.
- Rationale: evita desviaciones de UX y facilita validación visual.

2. Conservación de lógica de Cargo condicional
- Decisión: mantener `MemberCargoBadge` como componente aislado y condicional.
- Rationale: ya cumple la regla funcional y encaja con la nueva maqueta.

3. Visibilidad por bandera de usuario `IsMember`
- Decisión: aplicar `IsMember` como condición principal para renderizar elementos de miembro (`MemberCargoBadge`, `Ficha Socio`, `Licencias Federativas`).
- Rationale: evita mostrar acciones/estado de socio a usuarios no socios y alinea UI con semántica de dominio.
- Alternativa: condicionar solo por presencia de `Cargo`. Rechazada porque `IsMember` expresa mejor la intención de negocio.

4. Estado activo en navegación inferior
- Decisión: asegurar activación visual por ruta (`/mi-cuenta`) en botonera de 3 elementos (`Home`, `Mi Club`, `Mi Cuenta`).
- Rationale: coincide con el diseño y mejora orientación del usuario.

5. No incluir bloques extra en esta corrección
- Decisión: retirar/omitir bloques no presentes en la captura de referencia (puntos, tarjetas extra).
- Rationale: mantener fidelidad estricta con diseño aprobado.

## Risks / Trade-offs

- [Pérdida de contenido previamente implementado] → Mitigación: mover bloques fuera de la vista actual sin eliminar capacidades base de datos/servicios.
- [Desalineación entre CSS compilado y SCSS fuente] → Mitigación: actualizar ambos (`scss` y `app.css`) en el mismo cambio.
- [Ambigüedad en enlaces no funcionales] → Mitigación: mantener etiquetado visual y comportamiento no disruptivo donde aplique.
- [`IsMember` no disponible en el modelo frontend] → Mitigación: extender DTO/modelo de perfil para mapear explícitamente la bandera desde API.

## Migration Plan

- No requiere migración de datos.
- Deploy de frontend sin cambios de API.
- Rollback simple revirtiendo componentes/estilos de este change.

## Open Questions

- ¿`Licencias Federativas` debe quedar como placeholder visual o enlazar a ruta interna vacía (`/mi-cuenta/licencias`) en esta iteración?
