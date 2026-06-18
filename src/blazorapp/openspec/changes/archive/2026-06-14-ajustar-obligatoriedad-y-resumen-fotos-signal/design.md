## Context

El wizard de creación de `Signal` ya existe en el frontend y actualmente trabaja con un borrador que conserva tipología, datos principales y hasta dos imágenes antes del guardado. Tras los cambios recientes de selección de imágenes, el flujo ya contempla cámara, galería/archivo y normalización previa al límite de tamaño, pero sigue tratando `Foto 1` y `Foto 2` como si ambas fueran necesarias para completar el alta.

El ajuste pedido afecta a dos zonas muy concretas del flujo:

- la validación del paso de fotos y del guardado final, para que solo `Foto 1` sea obligatoria
- el paso de resumen, para poder eliminar `Foto 2` y compactar visualmente las previews en móvil

La restricción principal es mantener el contrato actual de creación de señales sin rediseñar el wizard ni introducir nuevas rutas o estados complejos.

## Goals / Non-Goals

**Goals:**
- Hacer obligatoria únicamente `Foto 1` durante el flujo de creación.
- Permitir que `Foto 2` permanezca vacía sin bloquear el avance ni el guardado.
- Añadir en el resumen una acción explícita para eliminar `Foto 2`.
- Reducir el tamaño visual de ambas previews del resumen para mostrarlas a dos columnas en móvil.
- Mantener la lógica actual de captura/selección y de normalización de imágenes.

**Non-Goals:**
- No cambiar el número máximo de imágenes soportadas por el wizard.
- No añadir edición avanzada de imágenes, recorte ni reordenación.
- No convertir `Foto 1` en opcional ni mover su eliminación al resumen.
- No rediseñar el contrato backend más allá de enviar `foto2` vacía o nula cuando falte.

## Decisions

### 1. Mantener un único modelo de borrador con `Photo2` opcional

El borrador de creación seguirá representando las dos imágenes en el mismo modelo actual, pero la validación funcional tratará `Photo1` como requerida y `Photo2` como opcional.

Rationale:
- Minimiza impacto sobre el wizard ya implementado.
- Evita bifurcar el modelo o introducir ramas específicas para señales con una o dos fotos.

Alternatives considered:
- Crear un modelo diferente para señales con una sola imagen.
  Rechazado porque complica innecesariamente el flujo y el mapeo final al request.

### 2. Ajustar la validación en el paso de fotos y en el guardado final

La regla de validación se moverá a un criterio único: el usuario puede avanzar al resumen y guardar si `Foto 1` está informada. `Foto 2` solo se incluirá si existe.

Rationale:
- Evita inconsistencias entre lo que permite el paso 3 y lo que finalmente acepta el paso 4.
- Refleja exactamente el comportamiento pedido por negocio.

Alternatives considered:
- Permitir avanzar con una sola foto pero seguir exigiendo dos al guardar.
  Rechazado porque genera una UX contradictoria y un error tardío evitable.

### 3. Implementar la eliminación de `Foto 2` solo desde el resumen

El resumen mostrará un control de eliminación con icono de cruz asociado a `Foto 2`, que limpiará la imagen del borrador y refrescará la preview antes del guardado.

Rationale:
- Responde al requisito explícito del usuario.
- Mantiene el paso de fotos enfocado en capturar o seleccionar imágenes, dejando la decisión final de descarte para la revisión previa.

Alternatives considered:
- Permitir eliminar ambas fotos desde el resumen.
  Rechazado en esta iteración porque el requisito solo pide actuar sobre `Foto 2` y `Foto 1` seguirá siendo obligatoria.
- Enviar al usuario de vuelta al paso 3 para borrar `Foto 2`.
  Rechazado porque introduce fricción innecesaria en una corrección simple.

### 4. Compactar las previews del resumen con un layout a dos columnas

Las imágenes del resumen se renderizarán con un layout responsivo de dos columnas en móvil, limitando tanto ancho como alto para que cada preview ocupe aproximadamente la mitad del ancho disponible.

Rationale:
- Mejora la lectura del resumen y evita que las imágenes desplacen excesivamente el resto de la información.
- Responde al ajuste visual pedido sin alterar el contenido del flujo.

Alternatives considered:
- Mantener previews grandes apiladas verticalmente.
  Rechazado porque ocupa demasiado espacio en el último paso.
- Recortar agresivamente las imágenes para forzar un alto fijo muy pequeño.
  Rechazado porque puede dificultar la revisión visual de la foto.

## Risks / Trade-offs

- [El backend puede seguir esperando `foto2` con valor no vacío] → Mitigación: validar explícitamente en integración que `foto2` nula o vacía no rompe el alta; si falla, coordinar el ajuste backend antes de aplicar en producción.
- [La eliminación desde el resumen puede dejar desalineada la UI si no se refresca el borrador] → Mitigación: centralizar la acción sobre el mismo estado del wizard y cubrirla con pruebas de vista.
- [Reducir demasiado el alto de las previews puede hacerlas poco legibles] → Mitigación: usar límites moderados y preservar `object-fit` consistente en las imágenes del resumen.
