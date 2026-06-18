## Why

La app no dispone hoy de una superficie dedicada para consultar alertas alimentarias segmentadas por perfil de riesgo. Incorporar esta funcionalidad ahora permite ofrecer una consulta clara por categorias, reutilizar un mismo flujo de listado y detalle, y conectar el frontend con el servicio externo ya definido para estas alertas.

## What Changes

- Crear una nueva pagina `Alertas Alimentarias` como punto de entrada de la funcionalidad.
- Mostrar en esa pagina tres categorias en formato lista con layout tipo card: `Generales`, `Complementos Alimenticios` y `Alergias`, cada una con titulo destacado y subtitulo descriptivo.
- Hacer que cada categoria navegue a una vista reutilizable de listado de alertas filtrada por el codigo de categoria correspondiente (`general`, `complementos`, `alergenos`).
- Consumir `http://redalertas.runasp.net/api/Alerts/all?code={code}` para recuperar las alertas de la categoria seleccionada.
- Mostrar en el listado el titulo de cada alerta y un extracto de su descripcion limitado aproximadamente a 100 caracteres.
- Incorporar una vista de detalle por `Id` para cada alerta, con carga de la informacion completa y visualizacion de la descripcion integra.
- Cubrir estados de carga, vacio y error tanto en el listado como en el detalle.

## Capabilities

### New Capabilities
- `frontend-food-alerts-pages`: alta de la nueva experiencia de alertas alimentarias con selector de categorias, listado reutilizable por categoria y vista de detalle.

### Modified Capabilities

## Impact

- `IndaloaventurApp.SharedUI`: nuevos componentes Razor, clases parciales, modelos y servicio frontend para alertas alimentarias.
- `IndaloaventurApp.Web`: nuevas rutas/paginas para entrada, listado por categoria y detalle por alerta.
- `IndaloaventurApp.SharedUI/Resources`: nuevos literales localizados en espanol para categorias, estados y navegacion.
- `IndaloaventurApp.Web/wwwroot/scss`: estilos SCSS para cards, listados y detalle, manteniendo la estrategia visual del proyecto.
- API externo `http://redalertas.runasp.net`: dependencia para recuperar alertas por categoria y resolver el detalle completo por `Id`.
