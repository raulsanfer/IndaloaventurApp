## Context

Vease `proposal.md` para la motivacion. La solucion mantiene las cuatro capas del proyecto y los contratos vigentes: los comandos persisten agregados mediante EF Core, las consultas devuelven proyecciones mediante Dapper, MediatR separa comandos de consultas y FluentValidation se ejecuta antes de los handlers. La API ya cuenta con las politicas `Authenticated` y `Admin`, matriz de autorizacion y un contrato de errores ProblemDetails en espanol.

Los maestros existentes no cubren una jerarquia ni garantizan de forma uniforme las restricciones necesarias para que los catalogos puedan ser referenciados por el futuro material inventariable. Esta propuesta no modificara los cambios OpenSpec activos `harden-text-input-validation` ni `handle-missing-ficha-socio-in-my-account`; al implementar se reutilizara cualquier componente comun que haya quedado disponible, sin duplicar reglas de texto.

## Goals / Non-Goals

**Goals:**

- Definir dos maestros coherentes bajo el modulo `Inventario`: categorias jerarquicas de dos niveles y estados planos.
- Hacer que la integridad de la jerarquia y la unicidad sean seguras tanto en la aplicacion como en SQL Server.
- Mantener una unica fuente de reglas para nombres de maestros y una unica ruta de persistencia por tipo de operacion.
- Ofrecer contratos HTTP previsibles, autorizacion por accion y cobertura automatizada de los resultados funcionales y de seguridad.

**Non-Goals:**

- No se gestionan articulos, existencias, prestamos, ubicaciones, movimientos, proveedores ni imagenes de material.
- No se definen semillas iniciales de categorias ni estados: el catalogo comienza vacio y se administra desde la API.
- No se incorporan borrados logicos, auditoria historica, paginacion ni internacionalizacion.
- No se realizan migraciones de datos de otros catalogos existentes ni se sustituyen `SignalType` o `Cargo`.

## Decisions

### Nombres, limites de modulo y contratos de dominio

Se creara el espacio de nombres `IndaloAventurApi.Domain.Inventario`, con los agregados `InventarioCategoria` e `InventarioEstado`. Los nombres en singular respetan la convencion de tipos de C#; los nombres funcionales solicitados se mantienen en plural para las colecciones, tablas, DTOs de lista, rutas y modulos: `InventarioCategorias` e `InventarioEstados`.

`InventarioCategoria` tendra `Id`, `Nombre` y una relacion estructural opcional `CategoriaPadreId`; no se anaden otros datos de negocio. `InventarioEstado` tendra `Id` y `Nombre`. La relacion padre no es un dato adicional de catalogacion sino el mecanismo indispensable para expresar la subcategoria solicitada.

La normalizacion y validacion comun de nombres se concentrara en un unico componente de dominio del modulo Inventario (value object o politica equivalente). Este componente eliminara espacios extremos, rechazara vacios, caracteres de control no permitidos y longitudes superiores a 120, y producira una clave normalizada para la unicidad. Los validadores de comandos validaran el contrato de entrada y el dominio conservara la misma invariante para que nunca pueda persistirse un agregado invalido por otra ruta.

Se descarta copiar las mismas comprobaciones en ambos agregados o confiar exclusivamente en los validadores HTTP: duplicaria reglas y permitiria estados invalidos desde otros consumidores de Application.

### Jerarquia de categorias y operaciones de cambio

Una categoria con `CategoriaPadreId = null` es de primer nivel. Una subcategoria debe apuntar a una categoria existente de primer nivel. Los handlers de crear y actualizar cargaran el padre propuesto y rechazaran el autoreferenciado, un padre inexistente y un padre que ya sea subcategoria. Al mover una categoria de primer nivel que tenga hijos bajo otra categoria, el cambio tambien se rechazara porque convertiría sus hijos en un tercer nivel.

Se permite actualizar `Nombre` y `CategoriaPadreId`: una subcategoria puede moverse a otra categoria de primer nivel o pasar a primer nivel si se mantienen las invariantes. Esto evita tener que crear y borrar registros para reorganizar el catalogo. Un borrado exige que la categoria sea una hoja; no habra borrados en cascada.

Se descarta una tabla separada para subcategorias porque ambos niveles comparten exactamente identidad, nombre, autorizacion y ciclo CUD; una autorrelacion reduce superficie y mantiene una sola fuente de reglas.

### Persistencia, unicidad y migracion

`ApplicationDbContext` expondra `DbSet<InventarioCategoria>` e `DbSet<InventarioEstado>`. Las configuraciones EF Core se ubicaran en `Infrastructure/Persistence/Inventario` y mapearan las tablas `InventarioCategorias` e `InventarioEstados` con claves enteras generadas por la base de datos.

La tabla de categorias contendra la FK nullable `CategoriaPadreId` contra si misma con `DeleteBehavior.Restrict`, columna `Nombre` requerida de 120 caracteres y un indice unico compuesto por padre y clave normalizada del nombre. La tabla de estados tendra un indice unico sobre su clave normalizada. La clave normalizada sera un detalle de persistencia no expuesto por el contrato API y asegurara la unicidad aun si la colacion de la base cambia; la combinacion de validacion previa e indice unico cubre tanto mensajes claros como carreras concurrentes.

Los repositorios de comando seran contratos separados (`IInventarioCategoriaRepository` e `IInventarioEstadoRepository`) que solo cargan agregados requeridos por comandos, agregan, eliminan y confirman cambios por EF Core. Las consultas no reutilizaran estos repositorios: servicios de consulta especificos ejecutaran SQL Dapper parametrizado sobre `SqlQueryConnectionFactory` y proyectaran DTOs sin tracking. La consulta de categorias obtendra una proyeccion plana ordenada y construira el arbol en Application, evitando una consulta por nodo; la consulta de detalle devolvera padre y datos de hijos cuando aplique.

La migracion se generara mediante EF Core con nombre descriptivo, se revisaran sus operaciones y el `ApplicationDbContextModelSnapshot`, y se incluira en el cambio. No se ejecutara `database update` contra entornos compartidos como parte de la implementacion. La reversión elimina primero las restricciones e indices y despues las tablas nuevas, por lo que no afecta esquemas ya existentes.

Se descarta aplicar la unicidad solo mediante una consulta previa, porque no protege ante dos escrituras simultaneas. Tambien se descarta que Dapper escriba estos datos, porque contradice la separacion de persistencia ya especificada para comandos.

### Application, API y autorizacion

El modulo Application se organizara bajo `Features/Inventario/InventarioCategorias` e `Features/Inventario/InventarioEstados`, con comandos `Create`, `Update` y `Delete`, consultas `GetAll` y `GetById`, DTOs de lectura y validadores FluentValidation. Los handlers convertirán ausencias en resultados de no encontrado y reglas de jerarquia o duplicidad en conflictos deterministas. Las excepciones de restriccion unica de SQL Server se traduciran al mismo conflicto funcional para no exponer detalles de base de datos.

Los controladores viviran en `Api/Features/Inventario`, con rutas `api/inventario-categorias` y `api/inventario-estados`. Tendran `[Authorize(Policy = AuthorizationPolicies.Authenticated)]` a nivel de controlador y la politica `Admin` exclusivamente en `POST`, `PUT` y `DELETE`. Usaran contratos de entrada compactos (`Nombre` y, para categorias, `CategoriaPadreId` nullable), devolveran `200` para lecturas, `201` con `Location` hacia el detalle e identificador creado para altas, `204` para actualizaciones y borrados correctos, y `404`, `409` o `400` mediante ProblemDetails cuando corresponda.

Las rutas se añadiran a `ApiAuthorizationMatrix`: los dos `GET` quedan clasificados como `Authenticated`; las tres operaciones CUD de cada maestro como `Admin`, que depende del rol tecnico y no de `IsMember`. Se descarta usar el rol `Member` o el claim `IsMember`, pues los maestros no son una operacion propia de socio real.

### Flujo de informacion

```text
HTTP request + JWT
  -> InventarioCategoriasController / InventarioEstadosController
  -> request API -> comando o consulta MediatR
  -> FluentValidation -> handler Application
  -> comando: agregado Domain -> repositorio EF Core -> SQL Server
  -> consulta: servicio Dapper parametrizado -> DTO de lectura
  -> ActionResult o ProblemDetails en espanol
```

En los comandos de categoria, el handler consulta el padre y los hijos necesarios antes de modificar el agregado. En eliminacion verifica primero que no existan hijos y despues ejecuta el borrado EF Core. El indice y FK de SQL Server son la defensa final de integridad. En las lecturas, Dapper obtiene solo columnas publicas y nunca materializa agregados; el DTO arbol devuelve categorias de primer nivel y sus subcategorias en orden alfabetico.

## Risks / Trade-offs

- [La implementacion de una clave normalizada puede divergir de la normalizacion visible] -> El mismo componente de dominio producira `Nombre` y su clave, y las pruebas cubriran espacios y diferencias de mayusculas.
- [Una escritura concurrente puede superar la comprobacion previa de duplicidad] -> El indice unico es obligatorio y las violaciones se mapearan a `409 Conflict` sin filtrar errores SQL.
- [Un padre borrado o una categoria con hijos puede comprometer la jerarquia] -> FK con borrado restrictivo, comprobacion de hijos y ausencia de cascadas.
- [La lista de categorias puede crecer] -> Una unica consulta plana y composicion lineal del arbol; no se ejecutaran consultas por cada categoria.
- [Los dos cambios OpenSpec activos pueden tocar reglas de texto] -> Antes de implementar se revisaran sus cambios aplicados; se reutilizara la abstraccion comun resultante o se encapsulara la regla de Inventario en un solo componente temporal sin duplicarla entre maestros.

## Migration Plan

1. Implementar agregados, contratos, configuraciones, repositorios, consultas, handlers, validadores, controladores y registros de DI junto con las pruebas.
2. Generar y revisar la migracion EF Core y su snapshot; ejecutar la suite de pruebas y una comprobacion de migracion contra una base SQL Server aislada cuando la infraestructura de pruebas este disponible.
3. Desplegar el binario y aplicar la migracion mediante el proceso ordinario de despliegue antes de habilitar consumidores del API.
4. Si se requiere reversión antes de que existan dependientes de inventario, retirar los consumidores y aplicar la migracion `Down`, que solo elimina las dos tablas nuevas. Una vez haya material inventariable dependiente, la reversión se tratara como una migracion de datos planificada y no como una eliminacion automatica.
