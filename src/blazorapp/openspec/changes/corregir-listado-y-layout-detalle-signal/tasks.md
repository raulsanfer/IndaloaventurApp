## 1. Listado de signals

- [x] 1.1 Revisar el contrato frontend consumido por `SignalApiClient` y habilitar la resolución de la imagen principal real de cada signal cuando el payload la ofrezca.
- [x] 1.2 Adaptar el mapeo a `SignalCardItem` para poblar `ImageUrl` y mantener el placeholder solo como fallback legítimo.
- [x] 1.3 Verificar en `SignalHomeView` que la card renderiza imagen real o placeholder según disponibilidad sin romper el layout actual.

## 2. Reorganización del detalle

- [x] 2.1 Ajustar `SignalDetailView` para reducir el peso visual del resumen de cabecera basado en `GetHeaderSummary()`.
- [x] 2.2 Reconfigurar `signal-detail__overview-grid` para mantener al menos dos cards por fila en móvil, incluyendo `412px`, con spacing compacto y menos margen útil por `article`.
- [x] 2.3 Simplificar el sistema de tabs del detalle a `Datos de la signal` y `Mapa`.
- [x] 2.4 Mover el bloque de `Comentarios` fuera del sistema de tabs y renderizarlo debajo del bloque tabulado.
- [x] 2.5 Mover el bloque de `Etiquetas` al final de la página, por debajo de comentarios.

## 3. Estilos y responsive

- [x] 3.1 Actualizar `scss/components/_signals.scss` para reflejar la nueva jerarquía del detalle sin introducir estilos inline ni patrones fuera de la estrategia SCSS del proyecto.
- [x] 3.2 Ajustar el comportamiento responsive de tabs, overview cards, comentarios y etiquetas para móvil y escritorio, compactando márgenes o paddings excesivos de las overview cards.

## 4. Verificación

- [x] 4.1 Añadir o actualizar tests frontend para el listado de signals cubriendo imagen principal real y fallback de placeholder.
- [x] 4.2 Añadir o actualizar tests frontend para el detalle cubriendo la nueva estructura de tabs, comentarios fuera de tabs y etiquetas al final.
- [ ] 4.3 Verificar manualmente en móvil o viewport estrecho, incluyendo `412px`, que `signal-detail__overview-grid` mantiene dos bloques por fila y que las tabs visibles caben sin desbordar la navegación principal.
