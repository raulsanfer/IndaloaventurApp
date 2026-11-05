## Context

Vease `proposal.md` para la motivacion y `specs/inventory-item-management/spec.md` para el contrato funcional. El modulo `Inventario` ya dispone de categorias jerarquicas y estados planos, persistidos como `InventarioCategorias` e `InventarioEstados`, y sus endpoints siguen la separacion vigente: comandos con EF Core, proyecciones de lectura con Dapper, MediatR y validacion antes de los handlers. La API dispone de politicas `Authenticated` y `Admin`, matriz de autorizacion y un contrato de errores ProblemDetails en Espanol.

Tambien existe un cambio activo para endurecer las entradas de texto. Esta implementacion debe consumir su politica comun cuando este disponible; no debe introducir validadores o saneadores ad hoc por propiedad.

## Goals / Non-Goals

**Goals:**

- Modelar el material inventariable como un agregado coherente, con invariantes que no dependan unicamente del controlador.
- Mantener las categorias y estados como maestros reutilizados, con integridad referencial y mensajes funcionales previsibles.
- Ofrecer un CRUD HTTP uniforme con los maestros recien implantados y con autorizacion por operacion.
- Evitar duplicar rutas de acceso a datos, validacion textual o reglas de normalizacion de numeros de serie.

**Non-Goals:**

- No se incorporan prestamos, reservas, asignaciones a socios, ubicaciones, proveedores, movimientos de stock, fotografias ni auditoria historica.
- No se anaden filtros, paginacion, exportacion, importacion masiva ni alertas de mantenimiento o reposicion.
- No se crean categorias ni estados iniciales, ni se cambia su jerarquia funcional.
- No se descompone un lote de varias unidades en activos individuales; un material con numero de serie representa necesariamente una unica unidad.

## Decisions

### Agregado, campos y reglas de negocio

Se anadira el agregado `InventarioMaterial` en `Domain/Inventario`, con identificador entero y las propiedades `Descripcion`, `MarcaModelo`, `Cantidad`, `InventarioCategoriaId`, `InventarioEstadoId`, `FechaAdquisicion`, `AdquiridoDonado`, `Observaciones` y `NumeroSerie`. La tabla y la coleccion de persistencia usaran el plural `InventarioMateriales`, siguiendo la convencion de los maestros existentes.

`Descripcion`, `Cantidad`, categoria, estado y `AdquiridoDonado` seran obligatorios. Marca/modelo, fecha de adquisicion, observaciones y numero de serie seran opcionales para poder inventariar tanto material sin informacion historica como material no serializado. La cantidad sera positiva y la fecha, si se proporciona, no sera posterior a la fecha actual de la operacion. Las longitudes se fijaran una sola vez en la politica comun de texto y se mapearan al esquema: descripcion hasta 250, marca/modelo hasta 200, procedencia hasta 250, observaciones hasta 2.000 y numero de serie hasta 120 caracteres.

El numero de serie, cuando exista, se recortara y mantendra tambien como clave normalizada no expuesta. Sera unico mediante indice filtrado y requerira `Cantidad = 1`, evitando que un identificador fisico unico represente un lote. Se descarta permitir una coleccion de numeros de serie en este cambio porque exigiria un agregado y contratos distintos para movimientos unitarios, fuera del alcance del CRUD solicitado.

Las reglas de texto se aplicaran en dos capas: validadores FluentValidation para respuestas `400` claras y el agregado o value objects reutilizables para conservar las invariantes en cualquier punto de entrada. Se extendera la politica transversal de texto del cambio `harden-text-input-validation`; si ese cambio aun no esta integrado al iniciar la implementacion, se coordinara su incorporacion antes de crear una politica paralela. Se descarta repetir `Trim`, limites o rechazo de controles en cada handler, validator y entidad.

### Referencias a categorias y estados

`CategoriaId` y `EstadoId` seran claves foraneas obligatorias con borrado restrictivo hacia `InventarioCategorias` e `InventarioEstados`. Los handlers de alta y modificacion comprobaran que ambas referencias existen antes de crear o actualizar el agregado. La base de datos seguira siendo la ultima barrera frente a carreras o modificaciones externas.

La categoria puede ser de primer nivel o subcategoria: el maestro ya decide si dispone de jerarquia y el material conserva la referencia concreta elegida. Las proyecciones de lectura incluiran el identificador y nombre de categoria y estado, asi como el padre de categoria cuando exista, para que el cliente pueda presentar el contexto completo sin consultas adicionales.

Los borrados de maestros que esten referenciados produciran conflicto. Se ajustara el manejo de errores de los repositorios/handlers de maestros para traducir la restriccion de clave foranea a `409 Conflict` en Espanol, sin depender de mensajes del proveedor SQL. Se descarta el borrado en cascada porque eliminaria materiales al administrar un catalogo.

### Application, persistencia y consultas

El modulo Application se organizara bajo `Features/Inventario/InventarioMateriales`, con comandos `Create`, `Update` y `Delete`, consultas `GetAll` y `GetById`, sus validadores y DTOs. Un contrato `IInventarioMaterialRepository` encapsulara la carga, deteccion de numero de serie repetido, alta, borrado y confirmacion de comandos por EF Core. Para comprobar las referencias se reutilizaran los repositorios de categorias y estados ya existentes, sin duplicar acceso a sus tablas.

La implementacion EF Core del repositorio sera la unica ruta de escritura y se registrara en la inyeccion de dependencias. `ApplicationDbContext` expondra el `DbSet`; una configuracion dedicada mapeara campos obligatorios, longitudes, claves foraneas restrictivas, indices de consulta para categoria y estado y el indice unico filtrado de numero de serie normalizado. La migracion y el snapshot se generaran con EF Core y se revisaran antes de incluirse.

Las lecturas no materializaran agregados ni reutilizaran el repositorio de escritura. Un servicio de consulta Dapper parametrizado proyectara la lista y el detalle, uniendose a categorias, categorias padre y estados. La lista tendra una orden estable por descripcion y despues identificador. Se descarta usar EF Core para las proyecciones porque contradice el patron CQRS establecido.

### Superficie HTTP, resultados y autorizacion

Se creara `InventarioMaterialesController` bajo `Api/Features/Inventario`, con ruta base `api/inventario-materiales`. El controlador exigira `Authenticated` por defecto y `Admin` exclusivamente en `POST`, `PUT` y `DELETE`; estas reglas se anadiran a `ApiAuthorizationMatrix`. La autorizacion usa el rol tecnico `Admin`, no el rol `Member` ni el indicador `IsMember`.

Los contratos de creacion y edicion expondran los campos de dominio sin permitir que el cliente imponga el identificador. Las consultas devolveran DTOs de lectura con los nombres de los maestros. El patron de respuesta seguira a los maestros: lecturas `200`, alta `201 Created` con `Location` a detalle e identificador, actualizacion y borrado `204`, y `400`, `404` o `409` mediante ProblemDetails en Espanol. Se descarta incorporar endpoints especificos de modificacion de cantidad porque no se ha definido todavia un modelo de movimientos que los audite.

### Flujo de informacion

```text
HTTP + JWT
  -> InventarioMaterialesController
  -> request API -> comando o consulta MediatR
  -> FluentValidation -> handler Application
  -> comando: InventarioMaterial + repositorios EF Core -> SQL Server
  -> consulta: Dapper parametrizado -> DTO de lectura con maestros
  -> ActionResult o ProblemDetails en Espanol
```

## Risks / Trade-offs

- [El cambio de endurecimiento textual puede no estar integrado al comenzar] -> Coordinar la reutilizacion de su abstraccion comun antes de codificar validacion de material; no crear una alternativa incompatible.
- [Numero de serie con cantidad mayor que uno es ambiguo] -> Modelar un serial como unidad individual y rechazar esa combinacion; los lotes se registran sin serial hasta que exista gestion de activos individuales.
- [Una categoria o estado puede eliminarse mientras se crea material] -> Validacion previa para errores claros y claves foraneas restrictivas para garantizar integridad ante concurrencia.
- [La violacion de indices o claves foraneas revela detalles de SQL] -> Traducir excepciones de persistencia a conflictos funcionales y verificarlo mediante integracion.
- [El listado completo puede crecer] -> Usar una unica consulta Dapper parametrizada y establecer orden determinista; introducir paginacion solo cuando exista necesidad funcional.

## Migration Plan

1. Incorporar agregado, contratos, validadores, handlers, consultas, mapeo, repositorio, controlador, registros y pruebas.
2. Generar la migracion EF Core y actualizar el snapshot; revisar que crea unicamente `InventarioMateriales`, indices y claves foraneas nuevas, sin tocar datos de maestros.
3. Ejecutar pruebas y aplicar la migracion mediante el proceso normal de despliegue antes de activar los consumidores HTTP.
4. Si se requiere revertir antes de introducir material, retirar los consumidores y ejecutar la migracion `Down`, que elimina primero las restricciones e indices y despues la tabla de material. Con material en produccion, una reversion debe planificarse como migracion de datos y no como borrado automatico.
