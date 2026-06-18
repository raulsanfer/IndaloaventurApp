## Context

La aplicacion separa paginas en `IndaloaventurApp.Web.Client`, vistas reutilizables en `IndaloaventurApp.SharedUI` y acceso a datos mediante servicios tipados. Esa estructura encaja con la nueva funcionalidad, pero el proveedor de alertas alimentarias introduce varias restricciones tecnicas que no aparecen en el resto de dominios actuales:

- El origen de datos es externo al `ApiSettings:BaseUrl` principal del proyecto.
- El `swagger.json` disponible el 2026-06-13 documenta `GET /api/Alerts/all`, `GET /api/Alerts/last` y `GET /api/Alerts/allRange`, y el usuario aclara que el detalle se incorporara con la forma `GET /api/Alerts/{id}`.
- La respuesta observada en listado incluye `fecha`, `titulo`, `url` y `descripcion`, y devuelve `descripcion` como HTML.

Ademas, la app se renderiza con `InteractiveAuto`, por lo que una integracion que funcione solo en servidor no basta: el flujo debe seguir funcionando cuando el componente pase a WebAssembly. Como el proveedor responde por `http` y no devolvio cabeceras CORS en la comprobacion manual, una llamada directa desde el navegador seria fragil o inviable.

## Goals / Non-Goals

**Goals:**
- Crear una pagina `Alertas Alimentarias` con tres categorias fijas y el layout pedido en cards blancas con esquinas redondeadas.
- Reutilizar una misma vista de listado para `general`, `complementos` y `alergenos`.
- Permitir navegacion a detalle por alerta reutilizando el `Id` nativo que expondra `GET /api/Alerts/{id}`.
- Mostrar en listado solo titulo y extracto de descripcion, y en detalle la descripcion completa normalizada.
- Encapsular la dependencia externa de forma compatible con `InteractiveAuto`, evitando acoplar la UI al host `redalertas.runasp.net`.
- Mantener localizacion y estilos dentro de los patrones ya exigidos por el proyecto.

**Non-Goals:**
- No anadir notificaciones push, subscripciones ni almacenamiento offline de alertas.
- No incorporar filtros adicionales por fecha, texto o estado en esta primera iteracion.
- No anadir en este cambio un acceso nuevo en Home, footer u otras superficies no solicitadas.
- No ampliar esta primera iteracion a filtros o subflujos no cubiertos por `all` y `/{id}`.

## Decisions

### 1. Introducir una capa BFF/proxy propia para alertas alimentarias

La app web expondra un endpoint propio, por ejemplo bajo `/api/food-alerts`, que sera el contrato consumido por el navegador. Ese endpoint llamara al proveedor `redalertas.runasp.net`, adaptara la respuesta y la devolvera con un DTO interno estable.

Rationale:
- Evita problemas de CORS y mixed content en WebAssembly.
- Permite centralizar timeout, manejo de errores, normalizacion de texto y generacion de identificadores.
- Mantiene la UI desacoplada del formato exacto del proveedor externo.

Alternatives considered:
- Llamar al proveedor externo directamente desde `IndaloaventurApp.Web.Client`.
  Rechazado porque el proveedor solo se observo por `http` y sin CORS, por lo que la ejecucion en navegador no es fiable.
- Resolver la funcionalidad solo en renderizado server-side.
  Rechazado porque la app usa `InteractiveAuto` y la experiencia debe sobrevivir al paso a cliente.

### 2. Registrar implementaciones diferenciadas del servicio segun host de ejecucion

Se definira una abstraccion compartida, por ejemplo `IFoodAlertService`, en `IndaloaventurApp.SharedUI`. En `IndaloaventurApp.Web.Client` se registrara un cliente HTTP contra la propia app (`/api/food-alerts`). En `IndaloaventurApp.Web` se registrara una implementacion equivalente que pueda consultar el proveedor externo durante prerender o SSR.

Rationale:
- Mantiene una unica API de dominio para componentes y paginas.
- Permite que prerender y WebAssembly devuelvan el mismo modelo sin introducir condicionales en la vista.

Alternatives considered:
- Forzar un unico cliente HTTP con la misma `BaseAddress` que el resto de dominios.
  Rechazado porque `ApiSettings:BaseUrl` apunta al backend principal y no al host de la propia app ni al proveedor externo.

### 3. Fijar un catalogo local de categorias con mapping explicito a codes remotos

Las tres categorias se modelaran localmente con copia fija y codigo remoto:

- `Generales` -> `general`
- `Complementos Alimenticios` -> `complementos`
- `Alergias` -> `alergenos`

La pagina inicial usara este catalogo local para mostrar titulo y subtitulo, y la vista de listado reutilizable recibira el codigo como parametro de ruta.

Rationale:
- El negocio ya define exactamente tres categorias y su copy, por lo que consultarlas en remoto no aporta valor.
- Garantiza consistencia visual y de navegacion aunque el proveedor no exponga un endpoint de categorias.

Alternatives considered:
- Descubrir categorias desde el proveedor.
  Rechazado porque el contrato observado no publica ese catalogo y el alcance ya esta cerrado a tres opciones concretas.

### 4. Usar rutas explicitas y no ambiguas para selector, listado y detalle

La navegacion se estructurara con tres superficies:

- `/alertas-alimentarias`
- `/alertas-alimentarias/categoria/{categoryCode}`
- `/alertas-alimentarias/alerta/{alertId}`

La vista de listado sera comun a las tres categorias y la de detalle resolvera la alerta a partir de `alertId`, manteniendo la categoria como contexto de navegacion cuando el usuario llegue desde el listado, por ejemplo mediante query string o estado de navegacion.

Rationale:
- Mantiene URLs legibles y alineadas con la estructura mental del flujo.
- Evita colisiones entre rutas de categoria y rutas de detalle dentro del router de Blazor.
- Hace posible rehidratar el detalle por URL directa sin depender de estado en memoria del listado.

Alternatives considered:
- Pasar el objeto completo entre paginas mediante estado temporal.
  Rechazado porque rompe el acceso directo por URL y complica la recarga del detalle.

### 5. Tratar el `Id` remoto como contrato de navegacion del detalle

El listado usara el `Id` nativo que el proveedor devolvera para construir el enlace al detalle, y la capa proxy expondra ese mismo identificador al frontend sin transformarlo.

Rationale:
- Simplifica el contrato del frontend y evita generar claves derivadas no oficiales.
- Hace que el detalle use el mismo identificador que el backend remoto, lo que reduce ambiguedad funcional y tecnica.

Alternatives considered:
- Generar un identificador estable derivado en el frontend o en el proxy.
  Rechazado tras la aclaracion del usuario porque complica innecesariamente la navegacion cuando el backend ya tiene previsto exponer un `Id` real.

### 6. Normalizar `descripcion` HTML a texto plano para listado y detalle

La descripcion del proveedor se convertira a texto plano seguro antes de llegar a la UI. El listado truncara esa version normalizada a unos 100 caracteres y el detalle mostrara la misma descripcion completa, sin ejecutar HTML remoto dentro de la app.

Rationale:
- El usuario solo ha pedido visualizar el contenido, no preservar maquetacion HTML externa.
- Evita introducir riesgo XSS o una dependencia nueva de sanitizacion HTML para esta fase.
- Simplifica truncado, pruebas y consistencia entre listado y detalle.

Alternatives considered:
- Renderizar HTML remoto con `MarkupString`.
  Rechazado porque el contenido proviene de un origen externo y no controlado.
- Anadir un sanitizador HTML de terceros.
  Rechazado para esta fase porque aumenta complejidad sin ser necesario para cumplir el objetivo funcional.

## Risks / Trade-offs

- [El endpoint `GET /api/Alerts/{id}` aun no esta publicado] -> Mitigacion: implementar el cliente y el proxy contra el contrato previsto y dejar la validacion final integrada para cuando el backend lo exponga.
- [La respuesta real del detalle puede diferir ligeramente del listado] -> Mitigacion: aislar DTOs de listado y detalle y mapear ambos al mismo modelo de dominio.
- [La descripcion HTML puede incluir estructuras complejas] -> Mitigacion: normalizar a texto plano y probar casos con etiquetas, enlaces y entidades HTML.
- [El host externo puede fallar o responder lento] -> Mitigacion: encapsular timeout y errores en la capa proxy y mostrar estados comprensibles en listado y detalle.
- [Implementar BFF y doble registro de servicio aumenta el numero de piezas] -> Mitigacion: mantener DTOs y mapeadores compartidos y limitar el contrato publico a un solo servicio de dominio.

## Migration Plan

1. Anadir configuracion propia del proveedor externo en `appsettings`.
2. Crear el contrato compartido de `FoodAlerts` y el mapeador de datos normalizados.
3. Implementar el proxy/BFF en `IndaloaventurApp.Web` y su cliente de navegador en `IndaloaventurApp.Web.Client`, incluyendo `all` por categoria y `/{id}` para detalle.
4. Registrar las implementaciones del servicio en servidor y cliente.
5. Crear las paginas Razor y vistas compartidas para selector, listado y detalle.
6. Incorporar recursos localizados, SCSS del modulo y pruebas automatizadas del servicio/mapeo y de las vistas.
7. Validar navegacion, estados y recarga directa del detalle.

## Open Questions

- Confirmar la forma exacta del payload de `GET /api/Alerts/{id}` cuando el backend lo publique, especialmente si reutiliza `fecha`, `titulo`, `url` y `descripcion` o anade campos nuevos.
- Confirmar si en una fase posterior se quiere un acceso directo desde `HomeDashboard` a `/alertas-alimentarias`.
