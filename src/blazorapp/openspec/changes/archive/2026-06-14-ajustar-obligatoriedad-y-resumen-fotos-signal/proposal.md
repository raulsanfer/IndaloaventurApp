## Why

El wizard actual obliga a completar las dos fotos, lo que añade fricción en un flujo que muchas veces puede resolverse con una única imagen válida. Además, el resumen final no permite descartar `Foto 2` una vez revisada la señal y las previews ocupan más espacio del deseado en móvil.

## What Changes

- Permitir completar y guardar una nueva `Signal` teniendo `Foto 1` informada y `Foto 2` vacía.
- Mantener `Foto 1` como obligatoria en el paso de fotos y en la validación final del wizard.
- Añadir en el paso de resumen una acción visual con icono de cruz para eliminar `Foto 2` antes de guardar.
- Reducir el tamaño de las previews de `Foto 1` y `Foto 2` en el resumen para que se muestren a dos columnas, ocupando aproximadamente el 50 % del ancho disponible cada una.
- Mantener sin cambios el contrato general del alta, enviando `foto2` vacía o nula cuando el usuario decida no incluirla.

## Capabilities

### New Capabilities
- `frontend-signal-create-flow`: comportamiento funcional del wizard de creación de señales, incluyendo obligatoriedad de fotos, resumen final y acciones sobre previews.

### Modified Capabilities

## Impact

- Afecta al wizard de creación de señales en `IndaloaventurApp.SharedUI`, especialmente los pasos de fotos y resumen.
- Afecta a validaciones del borrador de creación, literales localizados y estilos SCSS del flujo `Signal`.
- Requiere verificar que el backend acepte `foto2` nula o vacía en el contrato actual de creación; si no fuera así, habrá que adaptar también esa validación.
