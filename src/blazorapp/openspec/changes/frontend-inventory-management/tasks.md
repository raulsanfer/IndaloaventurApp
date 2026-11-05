## 1. Contratos y acceso al API de inventario

- [ ] 1.1 Crear los modelos públicos, solicitudes y `IInventoryService` de categorías, estados y materiales con comentarios XML, incluyendo el soporte aditivo necesario para trasladar errores de campo; verificar que `dotnet build` completa sin errores ni advertencias nuevas de documentación pública.
- [ ] 1.2 Implementar y registrar el cliente HTTP tipado de inventario con autenticación Bearer, DTOs camelCase y las operaciones de listado y CRUD de los tres recursos; verificar con pruebas que cada método usa ruta, verbo, cuerpo y tratamiento de `201`/identificador y `204` correctos.
- [ ] 1.3 Interpretar `ProblemDetails` y distinguir `400`, `401`, `403`, `404`, `409`, red, timeout y carga inválida sin exponer detalles internos; verificar con pruebas de cliente que se conserva `detail` seguro y los errores por propiedad cuando existen.

## 2. Navegación administrativa de Inventario

- [ ] 2.1 Añadir la entrada `Inventario` al hub de Settings, los recursos localizados y la ruta de página bajo Configuración; verificar mediante pruebas de Settings que la entrada solo se renderiza para sesiones Admin.
- [ ] 2.2 Proteger la ruta de Inventario usando el patrón existente para sesiones no autenticadas y no Admin, antes de inicializar datos; verificar que una sesión no Admin redirige a Configuración y que el componente no solicita el API.
- [ ] 2.3 Implementar la vista raíz con pestañas accesibles `Inventario`, `Categorías` y `Estados`, activa inicialmente en Inventario; verificar en pruebas de componente los atributos de selección, el cambio de panel y la ausencia de textos decorativos añadidos.

## 3. Gestión de catálogos maestros

- [ ] 3.1 Implementar la pestaña Categorías con árbol de raíces y subcategorías, alta, edición, selector de padre limitado a raíces y confirmación previa de borrado; verificar en pruebas la creación raíz/subcategoría, la conservación de una raíz con hijos y el refresco tras éxito.
- [ ] 3.2 Aplicar en Categorías la normalización y validación de nombre, jerarquía y caracteres de control, además de conservar editor y mostrar `detail` ante `400`, `404` o `409`; verificar pruebas negativas para entrada inválida, nombre duplicado y borrado bloqueado por dependencias.
- [ ] 3.3 Implementar la pestaña Estados con listado, alta, edición, confirmación de borrado y recarga del catálogo; verificar en pruebas operaciones correctas y que un `409` por estado en uso conserva el registro y muestra el mensaje de la API.

## 4. Gestión de materiales

- [ ] 4.1 Implementar la carga paralela de materiales, categorías y estados, conservando separados los nombres de la tabla y los catálogos de los selectores; verificar en pruebas que no se añaden filtros ni paginación remota y que el selector muestra tanto raíces como rutas raíz > subcategoría.
- [ ] 4.2 Implementar el editor de material para alta y edición completa, enviando `null` para opcionales vacíos y confirmando el borrado; verificar en pruebas los cuerpos `POST`/`PUT`, el refresco posterior a `201`/`204` y la eliminación solo tras éxito.
- [ ] 4.3 Aplicar validación local centralizada de texto, cantidad, referencias, fecha UTC, límites y regla de número de serie, y enlazar los errores de `ProblemDetails` a los campos; verificar pruebas para cantidad no positiva, fecha futura, obligatorios vacíos, serie con cantidad distinta de uno, `404` de referencia obsoleta y `409` sin pérdida del editor.

## 5. Estilos, documentación y validación final

- [x] 5.1 Añadir las claves de recursos y los estilos SCSS globales necesarios, siguiendo los patrones visuales existentes sin estilos inline ni degradados; verificar visualmente las tres pestañas y los estados de carga, vacío, éxito, error y reintento en pantalla estrecha y ancha.
- [ ] 5.2 Revisar si el cambio afecta a `docs/architecture` o `docs/features`, actualizar la documentación conceptual y los comentarios XML si procede, y verificar con `docfx docs/docfx.json`; comprobar que no se incluyen en el cambio los archivos generados de `docs/_site/` ni `docs/api/*.yml`.
- [ ] 5.3 Ejecutar `dotnet test` para la solución y `openspec validate frontend-inventory-management --strict`; verificar que las pruebas cubren acceso no Admin, autenticación del cliente, CRUD, validaciones y errores de API sin regresiones.
