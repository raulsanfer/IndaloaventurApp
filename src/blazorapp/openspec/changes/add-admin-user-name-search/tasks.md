## 1. Contrato y cliente administrativo

- [x] 1.1 Revisar `security.md` y confirmar el contrato `email`/`nombre` en el API objetivo o registrar la dependencia de despliegue pendiente; verificar nombre O apellidos, AND con email, espacios, ausencia de ficha y conservación del DTO. Sin un API accesible, no dar por probada la búsqueda real mediante mocks.
- [x] 1.2 Ampliar `IAdminUserManagementService.GetUsersAsync` y `AdminUserManagementApiClient` con `nombre` opcional; actualizar comentarios XML, llamadores con cancelación y `RecordingAdminUserManagementService`. Verificar que siguen funcionando las llamadas sin filtros, solo con email y las operaciones que reutilizan el listado.
- [x] 1.3 Construir una query determinista con valores recortados, omisión de filtros vacíos y codificación individual; conservar normalización de email, autorización, cancelación y mapeo. Añadir pruebas HTTP de nombre solo, email solo, ambos, espacios, Unicode y caracteres reservados, incluido `Ana&email=otro@club.es` sin inyección de parámetros.
- [x] 1.4 Sincronizar solo la operación `GET /api/users` de `openspec/endpoints.json` con el parámetro opcional `nombre`; verificar que el JSON es válido y coincide con el contrato comprobado del backend.

## 2. Formulario de búsqueda y limpieza

- [x] 2.1 Añadir `Nombre o apellidos` al formulario con estado y manejador en la clase parcial, y enviar ambos filtros mediante botón o Enter. Permitir los fragmentos de email admitidos por el API. Verificar con bUnit los valores enviados, la carga inicial sin filtros y los casos de nombre, email y combinación.
- [x] 2.2 Añadir `Limpiar filtros` como botón independiente del envío, vaciar ambos campos, retirar feedback previo y recargar sin parámetros. Verificar con bUnit la limpieza tras uno o ambos filtros, cero resultados, error previo y fallo de recarga, incluidos los estados vacíos general y filtrado.
- [x] 2.3 Calcular filtro aplicado con los criterios efectivos enviados y evitar consultas simultáneas al buscar o limpiar; verificar con una respuesta diferida que no se lanza una segunda petición y que los resultados corresponden a los valores enviados.
- [x] 2.4 Incorporar etiquetas y mensajes en los recursos españoles existentes y adaptar SCSS global si procede, conservando estilos y acciones de la lista. Verificar etiquetas asociadas, uso con teclado, ausencia de estilos inline y las regresiones de navegación y creación de ficha.

## 3. Seguridad y cobertura de aceptación

- [x] 3.1 Verificar OWASP A01 y A05 con pruebas de acceso administrativo existentes, envío autorizado con nombre y manejo controlado de `401`/`403`; comprobar que los valores reservados se mantienen como datos y no alteran otras claves de query.
- [x] 3.2 Añadir pruebas de error de red, timeout y payload inválido con el nuevo filtro y recuperación del formulario; revisar A04/A09 y A10 para asegurar que no se registran o persisten nombres/correos y que el feedback no expone respuestas internas.
- [x] 3.3 Comprobar contra el API objetivo los escenarios de fragmentos internos de nombre y de ambos apellidos, mayúsculas, AND, exclusión sin ficha y limpieza. Asociar cada escenario a evidencia del API; mantener pendiente lo que solo se haya cubierto mediante dobles frontend.

## 4. Documentación y validación final

- [x] 4.1 Review whether the change affects `docs/architecture` or `docs/features`, update documentation if needed, and verify with `docfx docs/docfx.json`. Documentar en `docs/features/settings-admin.md` ambos filtros, AND, coincidencia parcial sobre ficha y limpieza; revisar arquitectura del cliente y los comentarios XML del contrato. No incluir salida DocFX generada.
- [x] 4.2 Ejecutar las pruebas focalizadas con `dotnet test IndaloaventurApp.Frontend.Tests/IndaloaventurApp.Frontend.Tests.csproj --filter "FullyQualifiedName~AdminUserManagementApiClientTests|FullyQualifiedName~AdminUsersManagementViewTests"`, después toda la suite con `dotnet test IndaloaventurApp.Frontend.Tests/IndaloaventurApp.Frontend.Tests.csproj`, y corregir regresiones atribuibles al cambio.
- [x] 4.3 Ejecutar `openspec validate add-admin-user-name-search --strict`, revisar que los requisitos y escenarios tienen cobertura y marcar tareas completas solo tras superar sus verificaciones. Si una herramienta o integración no está disponible, dejar constancia de la verificación pendiente.

## Evidencia de verificaci?n

Resultados, comandos, cobertura y l?mites documentados en [verification.md](verification.md): 61 pruebas focalizadas, 309 de frontend y 25 HTTP del API local correctas; validaci?n estricta de OpenSpec y DocFX superadas. La compatibilidad del API desplegado debe confirmarse antes del despliegue.
