## Context

`AdminUsersManagementView` carga inicialmente todos los usuarios y después llama a `IAdminUserManagementService.GetUsersAsync` con un email opcional. El cliente normaliza ese email y usa `Uri.EscapeDataString` para construir la query. La vista solo considera el email al calcular `IsFilterApplied` y no dispone de una acción explícita para limpiar filtros.

El contrato actual del backend acepta `email` y `nombre`, recorta espacios, ignora mayúsculas y combina los criterios mediante AND. `nombre` busca texto literal contenido en `FichaSocio.Nombre` O `FichaSocio.Apellidos`, sin exigir `IsMember = true`; los usuarios sin ficha no coinciden con ese criterio. El listado y el DTO de respuesta se conservan. La copia `openspec/endpoints.json` del frontend todavía solo enumera `email` en esta operación.

La ruta actual comprueba sesión y rol Admin. El API aplica su política administrativa en servidor. Otros cambios activos de gestión de usuarios no deben mezclarse con este delta.

## Goals / Non-Goals

**Goals:**

- Buscar mediante un único campo de nombre o apellidos, solo o combinado con email, usando el filtrado del API.
- Poder vaciar ambos criterios y recargar todos los usuarios en una sola acción.
- Preservar la carga inicial, las acciones de usuario, el contrato de respuesta y los estados seguros existentes.
- Definir pruebas que distingan el contrato HTTP, el estado del formulario y la semántica que corresponde verificar en el backend.

**Non-Goals:**

- Cambiar autorización, ficha administrativa, permisos de inventario, ciclo de vida de cuentas o restablecimiento de contraseñas.
- Implementar SQL, filtrado local, un endpoint nuevo, campos de nombre en el DTO, persistencia de filtros o consultas adicionales por usuario.
- Resolver búsqueda difusa, acentos o coincidencias entre nombre y apellidos concatenados.

## Decisions

### Ampliar el servicio compartido y delegar la búsqueda al API

Extender `GetUsersAsync` para admitir `string? nombre = null` antes de `CancellationToken cancellationToken = default`, conservando `email` como primer argumento opcional. Actualizar los llamadores que pasen el token por posición y el doble `RecordingAdminUserManagementService`. Las llamadas sin argumentos, solo con email y con `cancellationToken` nombrado mantienen su comportamiento. Actualizar los comentarios XML para explicar ambos filtros, AND y cancelación.

El cliente conserva la normalización actual del email. Para `nombre`, recorta espacios exteriores y convierte cadenas vacías o de espacios en ausencia de filtro; no divide palabras, elimina acentos ni añade comodines. Construye la query solo con parámetros efectivos, codificados individualmente mediante `Uri.EscapeDataString` o una utilidad segura existente, en orden determinista `email`, `nombre`. Sin criterios efectivos, solicita exactamente `/api/users`, sin query vacía. Conserva el token de cancelación, la cabecera de autorización y el mapeo de resultados.

La alternativa de cargar todos los usuarios y filtrar en memoria requeriría datos que no contiene el DTO y podría discrepar del servidor. Las consultas de fichas por usuario también multiplicarían las solicitudes. Se descartan ambas.

### Un formulario con dos criterios independientes y envío explícito

La vista mantiene `SearchEmail` y `SearchNombre`, presenta etiquetas localizadas `Email` y `Nombre o apellidos` y envía ambos valores con el botón de búsqueda o Enter. El campo de nombre es texto libre; no exige email ni coincidencia exacta. El campo email debe permitir los fragmentos admitidos por el API, usando un tipo de entrada que no bloquee el envío por validación HTML de dirección completa; se conserva la normalización existente.

`IsFilterApplied` representa los criterios efectivos de la última petición, con OR entre la presencia de email y nombre, y no los cambios todavía sin enviar del formulario. Así, los estados de carga y vacío funcionan al buscar solo por nombre, solo por email o por ambos. Un resultado vacío es un resultado válido y muestra el mensaje filtrado cuando hay al menos un criterio aplicado.

Las etiquetas y mensajes se incorporan a los recursos compartidos españoles existentes. Se usan las convenciones de Settings y SCSS global, sin estilos inline ni archivos de estilos por componente.

Se descarta la búsqueda automática al escribir para mantener el envío explícito existente y evitar peticiones adicionales y respuestas concurrentes innecesarias.

### Limpiar los dos campos y recargar el listado desde el servidor

El botón localizado `Limpiar filtros` usa `type="button"`, vacía ambos campos, elimina el estado de filtro y los mensajes de la consulta anterior, y ejecuta una nueva carga sin parámetros. Está disponible después de resultados, ausencia de coincidencias o errores. Si falla esa recarga, mantiene los campos vacíos, muestra el error seguro y permite volver a buscar o limpiar.

Las acciones de buscar y limpiar se deshabilitan durante la consulta y el manejador evita iniciar otra si ya hay una en curso. La búsqueda usa una instantánea de los valores al enviar, para que editar un campo durante la espera no cambie el significado de la respuesta. Se conserva el comportamiento actual de vaciar resultados al iniciar la carga; una respuesta anterior no se reutiliza como listado completo después de limpiar.

Se descarta limpiar solo el formulario sin recargar: dejaría resultados filtrados bajo campos vacíos.

### Mantener las fronteras de seguridad y tratar los valores como datos

Aplicar `security.md`: A01 conserva la ruta Admin y la autorización del API; la query nunca concede permisos. A05 requiere codificar cada valor una sola vez, manteniendo espacios internos, Unicode y caracteres como `&`, `+`, `?`, `#`, `%`, `_` o `[` como datos y sin interpretar HTML. Las coincidencias literales de SQL corresponden al backend.

A04/A09 exige no añadir registros, almacenamiento persistente o mensajes que expongan nombres, correos, tokens o respuestas internas. A10 conserva los resultados controlados para `401`, `403`, fallos de red, timeout y respuesta inválida. El componente muestra feedback localizado y recuperable, sin propagar mensajes internos del API. No se amplía el alcance a rediseñar la gestión global de sesión.

## Risks / Trade-offs

- [Contrato disponible en el árbol de trabajo pero no necesariamente en el entorno desplegado] → Confirmar soporte de `nombre` mediante una comprobación del API objetivo antes del despliegue del frontend.
- [La copia de endpoints está desactualizada] → Añadir el parámetro opcional `nombre` a esa operación sin regenerar ni modificar contratos ajenos.
- [Sin ficha no hay coincidencia por nombre] → Explicar la semántica en documentación y conservar el filtrado exclusivo por email y la carga completa.
- [Los tests con servicio simulado no prueban la búsqueda real en base de datos] → Verificar en el API objetivo los fragmentos de nombre/apellidos, AND y exclusión sin ficha; las pruebas frontend solo prueban petición y presentación.
- [Cambiar la firma afecta a llamadores y dobles compartidos] → Actualizarlos juntos y ejecutar toda la suite frontend para detectar regresiones de ficha y otras operaciones administrativas.

## Migration Plan

1. Confirmar que el API destino implementa el contrato existente con `nombre` y AND con email.
2. Integrar cliente, formulario, recursos, pruebas, comentarios XML, copia de endpoints y documentación en el mismo cambio. No se requiere migración de datos.
3. Ejecutar pruebas del cliente y de la vista, suite frontend completa, `openspec validate add-admin-user-name-search --strict` y `docfx docs/docfx.json`.
4. Comprobar manualmente en el API objetivo búsquedas por fragmentos de nombre y apellidos, combinación y limpieza. Si no está disponible, registrar esa verificación como pendiente.
5. Para revertir, desplegar el frontend anterior; el backend conserva su contrato compatible por email.

## Open Questions

No hay decisiones funcionales pendientes. La disponibilidad del contrato en el entorno de integración debe comprobarse durante la implementación y el despliegue.
