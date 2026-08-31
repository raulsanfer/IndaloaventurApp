# Estilos

La estrategia de estilos esta centralizada en SCSS bajo `IndaloaventurApp.Web/IndaloaventurApp.Web/wwwroot/scss`.

## Estructura

- `style.scss`: punto de entrada global.
- `base/_variables.scss`: variables compartidas.
- `base/_global.scss`: reglas base.
- `components/_*.scss`: estilos por area o patron visual, por ejemplo `_login.scss`, `_signals.scss`, `_mi-club.scss`, `_food-alerts.scss`, `_configuracion.scss` y `_shell.scss`.

## Reglas

- No usar estilos inline para comportamiento visual estable.
- No crear estilos aislados por componente si el patron pertenece al sistema global.
- Mantener nombres orientados al dominio o al patron.
- Evitar cambios de estilo globales desde una feature sin revisar impacto.

## Relacion con componentes

Los componentes Razor deben emitir estructura semantica y clases. La presentacion vive en SCSS. Si una feature necesita un patron visual nuevo, se agrega al parcial SCSS correspondiente y se importa desde `style.scss` si todavia no esta enlazado.
