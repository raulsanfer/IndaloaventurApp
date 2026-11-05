## Purpose

Define la gestión administrativa del inventario de material de montaña desde Configuración, con catálogos de categorías y estados que mantienen referencias válidas para los materiales.

## ADDED Requirements

### Requirement: Inventario MUST ser una herramienta exclusiva de Administración
El sistema MUST mostrar `Inventario` dentro de Configuración únicamente a una sesión autenticada con el rol `Admin`. La ruta de inventario MUST impedir la carga de datos y operaciones para una sesión no autenticada o sin ese rol, mediante el mismo patrón de redirección de las herramientas administrativas existentes. El cliente MUST enviar el token Bearer de la sesión en toda petición de inventario y MUST tratar la API como la autoridad final de autorización.

#### Scenario: Admin abre Configuración
- **WHEN** una persona con sesión autenticada y rol `Admin` accede a Configuración
- **THEN** el sistema MUST mostrar la entrada `Inventario` junto a las demás herramientas administrativas

#### Scenario: Usuario no administrador abre Configuración
- **WHEN** una persona autenticada sin el rol `Admin` accede a Configuración
- **THEN** el sistema MUST no mostrar la entrada `Inventario`

#### Scenario: Usuario no administrador accede directamente a Inventario
- **WHEN** una persona sin el rol `Admin` navega directamente a la ruta de Inventario
- **THEN** el sistema MUST redirigirla a Configuración sin cargar datos de inventario ni ofrecer operaciones de gestión

### Requirement: La página de Inventario MUST organizar la gestión en tres pestañas
La página administrativa MUST mostrar las pestañas `Inventario`, `Categorías` y `Estados`, seleccionando `Inventario` inicialmente. Cada pestaña MUST exponer solo las operaciones de su recurso y mantener una semántica accesible de pestañas. La interfaz MUST reutilizar los patrones visuales existentes, con títulos funcionales, textos localizados, estilos SCSS globales y sin introducir degradados ni subtítulos decorativos.

#### Scenario: Admin abre Inventario
- **WHEN** un administrador navega a la herramienta Inventario
- **THEN** el sistema MUST mostrar las tres pestañas `Inventario`, `Categorías` y `Estados`
- **AND** MUST mostrar la pestaña `Inventario` como la pestaña activa inicial

#### Scenario: Admin cambia de pestaña
- **WHEN** un administrador selecciona Categorías o Estados
- **THEN** el sistema MUST mostrar la gestión correspondiente sin abandonar la herramienta Inventario
- **AND** MUST mantener identificable la pestaña activa para tecnologías de asistencia

### Requirement: Categorías MUST gestionarse como un catálogo jerárquico de dos niveles
El sistema MUST cargar y mostrar el árbol completo devuelto por `GET /api/inventario-categorias`. Un administrador MUST poder crear, editar y solicitar el borrado de categorías mediante los endpoints de categorías. El formulario MUST permitir una categoría raíz o una subcategoría, pero MUST ofrecer como padre únicamente categorías raíz y MUST impedir localmente una jerarquía inválida antes de enviarla. Antes del borrado, el sistema MUST solicitar confirmación.

#### Scenario: Admin crea una categoría raíz y una subcategoría
- **WHEN** un administrador guarda una categoría sin padre y después guarda otra seleccionando la primera como padre
- **THEN** el sistema MUST enviar `categoriaPadreId: null` para la raíz y el identificador de la raíz para la subcategoría
- **AND** MUST refrescar el árbol de categorías tras cada operación correcta

#### Scenario: Admin edita una categoría raíz con subcategorías
- **WHEN** un administrador edita una categoría raíz que contiene subcategorías
- **THEN** el sistema MUST permitir modificar su nombre
- **AND** MUST mantenerla como categoría raíz en lugar de permitir asignarle un padre

#### Scenario: Borrado de categoría rechazado por dependencias
- **WHEN** el administrador confirma borrar una categoría y la API devuelve `409 Conflict` porque tiene subcategorías o materiales asignados
- **THEN** el sistema MUST conservar la categoría en pantalla
- **AND** MUST mostrar el `detail` proporcionado por la API sin considerar completado el borrado

### Requirement: Estados MUST poder gestionarse como catálogo plano
El sistema MUST cargar los estados desde `GET /api/inventario-estados` y MUST permitir que un administrador cree, edite y elimine estados con los endpoints correspondientes. Antes del borrado, el sistema MUST solicitar confirmación y, tras una respuesta correcta, MUST refrescar el catálogo.

#### Scenario: Admin crea y edita un estado
- **WHEN** un administrador guarda un estado válido nuevo o modifica un estado existente
- **THEN** el sistema MUST usar `POST /api/inventario-estados` o `PUT /api/inventario-estados/{id}` según corresponda
- **AND** MUST mostrar el catálogo actualizado después de la operación correcta

#### Scenario: Estado en uso no se puede eliminar
- **WHEN** el administrador confirma el borrado de un estado que está asignado a material y la API devuelve `409 Conflict`
- **THEN** el sistema MUST mantener el estado visible
- **AND** MUST mostrar el `detail` de la respuesta como causa del conflicto

### Requirement: Materiales MUST listar y gestionar el inventario completo
La pestaña Inventario MUST cargar en paralelo los materiales, categorías y estados, sin enviar paginación ni filtros a la API. MUST mostrar para cada material, como mínimo, su descripción, cantidad, categoría y estado con los nombres legibles devueltos por el servicio. Un administrador MUST poder crear, editar y eliminar un material; el formulario MUST enviar el objeto editable completo y MUST usar categorías raíz y subcategorías como opciones de categoría, mostrando las subcategorías con su ruta de categoría raíz. Antes del borrado, el sistema MUST solicitar confirmación.

#### Scenario: Carga inicial de inventario
- **WHEN** un administrador abre la pestaña Inventario
- **THEN** el sistema MUST solicitar en paralelo `GET /api/inventario-materiales`, `GET /api/inventario-categorias` y `GET /api/inventario-estados`
- **AND** MUST usar los nombres incluidos en los materiales para la tabla y los catálogos para los selectores del formulario

#### Scenario: Admin crea material con maestros existentes
- **WHEN** un administrador completa un material válido con una categoría y un estado disponibles
- **THEN** el sistema MUST enviar todos los campos editables a `POST /api/inventario-materiales`
- **AND** tras `201 Created` MUST refrescar el listado de materiales sin actualizarlo de forma optimista

#### Scenario: Admin edita material y elimina valores opcionales
- **WHEN** un administrador borra el contenido de un campo opcional y guarda la edición
- **THEN** el sistema MUST enviar `null` para ese campo en `PUT /api/inventario-materiales/{id}`
- **AND** MUST refrescar el listado de materiales tras `204 No Content`

#### Scenario: Admin elimina un material
- **WHEN** un administrador confirma borrar un material y la API devuelve `204 No Content`
- **THEN** el sistema MUST refrescar el listado y MUST dejar de mostrar el material eliminado

### Requirement: Formularios de inventario MUST validar y normalizar los datos antes de solicitar cambios
Los formularios MUST recortar los textos y MUST rechazar caracteres de control no permitidos. Las categorías y los estados MUST exigir un nombre de entre 1 y 120 caracteres. Los materiales MUST exigir descripción de 1 a 250 caracteres, cantidad entera positiva, categoría y estado existentes, fecha de adquisición no posterior al día UTC actual y texto adquirido/donado de 1 a 250 caracteres. Los campos opcionales MUST respetar sus límites contractuales y convertirse en `null` cuando estén vacíos; las observaciones MUST admitir solamente saltos de línea como carácter de control. Si se indica número de serie, el formulario MUST exigir cantidad exactamente igual a uno.

#### Scenario: Material con datos locales inválidos
- **WHEN** un administrador intenta guardar un material con cantidad no positiva, fecha futura, texto obligatorio vacío o número de serie con cantidad distinta de uno
- **THEN** el sistema MUST bloquear la solicitud
- **AND** MUST mostrar el error junto al campo que requiere corrección

#### Scenario: Nombre de maestro duplicado detectado por el servidor
- **WHEN** un administrador guarda una categoría o estado y la API devuelve `409 Conflict` por un nombre duplicado
- **THEN** el sistema MUST conservar los valores introducidos
- **AND** MUST mostrar el `detail` de la respuesta sin afirmar que el catálogo se haya modificado

### Requirement: La gestión MUST presentar errores de API de forma segura y recuperable
El cliente MUST interpretar `ProblemDetails` de las operaciones de inventario. Para `400`, MUST conservar el formulario y asociar los errores de campo recibidos a sus controles; para `403`, MUST detener la operación sin reintentarla; para `404`, MUST conservar los valores introducidos y ofrecer la recarga de catálogos o listado; para `409`, MUST conservar los valores y mostrar `detail`; y para errores de red, tiempo de espera o `5xx`, MUST mostrar un fallo genérico recuperable con reintento. El sistema MUST no exponer detalles internos ni marcar como guardada una operación sin respuesta correcta.

#### Scenario: API devuelve errores de validación
- **WHEN** una operación de creación o edición recibe `400 Bad Request` con `errors`
- **THEN** el sistema MUST conservar el formulario
- **AND** MUST mostrar los mensajes de cada propiedad junto a su campo correspondiente

#### Scenario: Referencia eliminada mientras se edita un material
- **WHEN** al guardar un material la API devuelve `404 Not Found` porque ya no existe una categoría o estado seleccionado
- **THEN** el sistema MUST conservar los valores editados
- **AND** MUST permitir recargar las referencias antes de un nuevo intento
