## Context

La aplicación ya separa componentes reutilizables en `IndaloaventurApp.SharedUI`, rutas y clientes HTTP en `IndaloaventurApp.Web.Client`, y registra estos últimos como `HttpClient` tipados. Las herramientas de Settings resuelven la autorización de navegación con `ISessionService`, mientras que la API conserva la autorización efectiva. Los estilos se centralizan bajo `IndaloaventurApp.Web/wwwroot/scss` y los textos de interfaz viven en los recursos compartidos.

La API de inventario ya define recursos autenticados de materiales, categorías y estados; los cambios de los tres recursos exigen `Admin`, y el documento de integración aporta formas de JSON, validaciones y `ProblemDetails`. Véanse `proposal.md` y la especificación de esta propuesta para el comportamiento comprometido.

## Goals / Non-Goals

**Goals:**

- Añadir un área administrativa coherente que preserve la separación de página, UI compartida, contratos y transporte HTTP.
- Reutilizar un único servicio de inventario y utilidades comunes para normalización, validación, aplanado de categorías y tratamiento de errores, sin duplicar reglas entre pestañas.
- Hacer que los conflictos, referencias obsoletas y errores de validación conserven el contexto de edición y se comuniquen de manera segura.

**Non-Goals:**

- Cambiar los permisos o las reglas de negocio del backend, ni convertir las comprobaciones de rol del cliente en un mecanismo de autorización servidor.
- Añadir paginación, filtros remotos, exportación, reservas, préstamos o trazabilidad de movimientos de material.
- Introducir dependencias visuales nuevas, estilos aislados por componente, degradados o una taxonomía cerrada para `adquiridoDonado`.

## Decisions

1. Crear un único contrato `IInventoryService` en SharedUI, acompañado por modelos de lectura y solicitudes de escritura de materiales, categorías y estados; registrar un cliente `InventoryApiClient` tipado en Web.Client.

   El contrato agrupa recursos que comparten autorización, pantalla y referencias, evita tres clientes con código repetido de token, JSON y errores, y mantiene los componentes independientes del transporte. El cliente enviará peticiones relativas, `Authorization: Bearer` de la sesión y cuerpos camelCase; tratará `201` como identificador JSON escalar y `204` como respuestas sin cuerpo.

   Alternativas consideradas:
   - Reutilizar `ICargoAdminService`: descartada porque no modela jerarquías, referencias ni errores de inventario.
   - Crear tres clientes por recurso: descartada por multiplicar lógica idéntica de autenticación y `ProblemDetails` sin una frontera de dominio necesaria.

2. Añadir una ruta administrativa de Inventario protegida con el patrón de Settings existente y extender `SettingsView` con una única entrada visible para Admin. La ruta redirigirá sesiones no autenticadas a inicio y sesiones autenticadas no Admin a Configuración antes de inicializar la vista.

   Esta decisión responde al requisito de que toda la gestión se realice desde Settings solo por Admin, sin asumir que ocultar la UI sustituya la autorización de la API. El servidor sigue validando el rol para las escrituras y el cliente comunicará los `403` recibidos.

   Alternativa considerada:
   - Permitir que otros usuarios autenticados consulten el inventario porque la API acepta sus GET: descartada por contradecir el alcance explícito de producto para esta interfaz.

3. Implementar una vista raíz de inventario con estado compartido de pestaña y componentes especializados para materiales, categorías y estados. La vista de materiales cargará sus tres fuentes en paralelo y conservará los catálogos separados de los nombres recibidos en los materiales. Una utilidad única aplanará el árbol de categorías para el selector de material y limitará el selector de padre a categorías raíz.

   El estado compartido evita listas incoherentes entre pestañas y las vistas especializadas mantienen formularios y listas manejables. Tras modificar un maestro se recargará el catálogo afectado; tras modificar material se recargará el listado completo para respetar normalización y referencias del servidor.

   Alternativas consideradas:
   - Una pantalla por cada operación: descartada por añadir navegación y duplicación para catálogos pequeños.
   - Usar solo el estado optimista de la UI: descartada porque `201`/`204` no devuelven el recurso normalizado y las referencias pueden haber cambiado.

4. Centralizar la normalización y validación de los formularios en utilidades del dominio de inventario y mapear los nombres de propiedad de `ProblemDetails.errors` a `ValidationMessageStore`. Se ampliará de forma aditiva el resultado de error compartido, si es necesario, para transportar los mensajes de campo junto con un código estable y el detalle seguro de la API.

   Esto permite conservar el mismo comportamiento en los tres formularios y mostrar inmediatamente errores locales o de servidor sin repetir condicionales. Las reglas client-side son una ayuda de experiencia y no sustituyen las validaciones y conflictos del servidor.

   Alternativas consideradas:
   - Limitarse a un mensaje global: descartada porque el contrato proporciona errores de campo y dificultaría corregir formularios extensos.
   - Duplicar las validaciones en cada componente: descartada por riesgo de divergencia entre categorías, estados y materiales.

5. Usar confirmación accesible antes de borrar y mantener el registro y el editor sin cambios hasta recibir éxito. El cliente mapeará `400`, `403`, `404`, `409`, errores de red y tiempos de espera a resultados diferenciables; los detalles técnicos no se mostrarán al usuario.

   Esto evita pérdidas de datos y estados falsamente correctos ante condiciones de carrera o fallos. Los conflictos de número de serie, nombre, jerarquía y dependencias se validan definitivamente en la API.

## Risks / Trade-offs

- [El backend permite lecturas autenticadas a no Admin, pero la interfaz solicitada es solo administrativa] → La ruta y la entrada de Settings se restringen en cliente, mientras que las operaciones de escritura conservan la protección servidor y las pruebas cubren la ruta directa no autorizada.
- [Los catálogos pueden cambiar entre carga y guardado] → Se preserva el editor cuando la API responde `404` o `409` y se habilita la recarga explícita de referencias antes de reintentar.
- [La lista de materiales puede crecer porque la API no pagina] → Se evita solicitar parámetros de servidor inexistentes y se reserva cualquier búsqueda o filtro para cliente en una iteración futura si el volumen lo requiere.
- [Nombres de propiedades de `ProblemDetails.errors` pueden diferir del editor] → El cliente normaliza y prueba el mapeo desde el contrato API antes de entregar los errores al componente.

## Migration Plan

1. Desplegar el frontend con la nueva ruta, contratos y registro del cliente después de que los endpoints de inventario estén disponibles en el entorno objetivo.
2. Mantener las herramientas de Settings existentes sin cambios funcionales y añadir Inventario como una entrada adicional solo para Admin.
3. Si se produce una incidencia, retirar la entrada y la ruta de Inventario en una revisión posterior; no hay migraciones de datos ni cambios de compatibilidad en recursos existentes.
