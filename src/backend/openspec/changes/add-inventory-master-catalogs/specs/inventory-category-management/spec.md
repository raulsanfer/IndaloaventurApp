## Purpose

Proporciona el catalogo jerarquico y seguro de categorias de inventario que el club usara para clasificar su material deportivo.

## ADDED Requirements

### Requirement: Catalogo de categorias de inventario con dos niveles como maximo
El sistema MUST mantener registros `InventarioCategoria` identificados por un entero autogenerado y con un `Nombre` obligatorio. Cada registro SHALL ser una categoria de primer nivel o una subcategoria vinculada a una unica categoria de primer nivel; el catalogo SHALL impedir cualquier tercer nivel.

#### Scenario: Administrador crea una categoria de primer nivel
- **WHEN** un usuario `Admin` crea una categoria con un nombre valido y sin categoria padre
- **THEN** el sistema SHALL persistir una categoria de primer nivel con identificador autogenerado

#### Scenario: Administrador crea una subcategoria
- **WHEN** un usuario `Admin` crea una categoria con un nombre valido y el identificador de una categoria de primer nivel existente como padre
- **THEN** el sistema SHALL persistir una subcategoria vinculada a dicha categoria padre

#### Scenario: Se rechaza un tercer nivel
- **WHEN** un usuario `Admin` intenta crear o mover una categoria bajo una subcategoria
- **THEN** el sistema SHALL rechazar la operacion y SHALL conservar la jerarquia existente

#### Scenario: Se rechaza una categoria padre inexistente
- **WHEN** un usuario `Admin` indica como padre un identificador que no existe
- **THEN** el sistema SHALL devolver un resultado de no encontrado y SHALL no persistir cambios

### Requirement: Nombres de categoria validos y no duplicados en su nivel
El sistema MUST exigir que el `Nombre` de una `InventarioCategoria` sea texto valido no vacio tras normalizacion, con una longitud maxima de 120 caracteres. El catalogo SHALL rechazar nombres duplicados, sin distinguir mayusculas, minusculas ni espacios extremos, entre categorias que comparten el mismo padre; una misma subcategoria SHALL poder existir bajo categorias padre diferentes.

#### Scenario: Nombre de categoria valido
- **WHEN** un administrador crea o actualiza una categoria con un nombre valido que contiene espacios extremos
- **THEN** el sistema SHALL almacenar el nombre normalizado sin los espacios extremos

#### Scenario: Nombre duplicado en el mismo nivel
- **WHEN** un administrador crea o renombra una categoria con un nombre ya usado bajo el mismo padre
- **THEN** el sistema SHALL rechazar la operacion con un error de conflicto y SHALL conservar el catalogo sin duplicados

#### Scenario: Nombre de categoria invalido
- **WHEN** un administrador envia un nombre vacio, solo compuesto por espacios, con caracteres de control no permitidos o que supera 120 caracteres
- **THEN** el sistema SHALL rechazar la solicitud con un error de validacion en espanol

### Requirement: Consulta autenticada de categorias de inventario
El sistema MUST permitir a cualquier usuario autenticado consultar el catalogo mediante `GET /api/inventario-categorias` y consultar una categoria existente mediante `GET /api/inventario-categorias/{id}`. La respuesta de lista SHALL representar de forma determinista la relacion categoria-subcategorias y exponera, como minimo, el identificador y el nombre de cada registro.

#### Scenario: Usuario autenticado consulta el arbol de categorias
- **WHEN** un usuario autenticado solicita `GET /api/inventario-categorias`
- **THEN** el sistema SHALL devolver las categorias de primer nivel con sus subcategorias, ordenadas alfabeticamente por nombre en cada nivel

#### Scenario: Usuario autenticado consulta una categoria existente
- **WHEN** un usuario autenticado solicita `GET /api/inventario-categorias/{id}` para una categoria existente
- **THEN** el sistema SHALL devolver su identificador, nombre y, cuando corresponda, su relacion jerarquica

#### Scenario: Consulta de categoria inexistente
- **WHEN** un usuario autenticado solicita una categoria que no existe
- **THEN** el sistema SHALL devolver un resultado de no encontrado

#### Scenario: Usuario anonimo no consulta categorias
- **WHEN** un cliente sin autenticar solicita cualquier endpoint de consulta de categorias de inventario
- **THEN** el sistema SHALL rechazar la solicitud por falta de autenticacion

### Requirement: Administracion de categorias de inventario
El sistema MUST permitir exclusivamente a usuarios autenticados con rol tecnico `Admin` crear, actualizar y eliminar categorias mediante `POST`, `PUT /api/inventario-categorias/{id}` y `DELETE /api/inventario-categorias/{id}`. La actualizacion SHALL permitir cambiar el nombre y la categoria padre respetando la jerarquia maxima y las reglas de unicidad.

#### Scenario: Administrador actualiza una categoria
- **WHEN** un usuario `Admin` actualiza el nombre o la categoria padre de una categoria existente con datos validos
- **THEN** el sistema SHALL persistir los cambios y SHALL mantener una jerarquia de dos niveles como maximo

#### Scenario: Actualizacion o eliminacion de categoria inexistente
- **WHEN** un usuario `Admin` actualiza o elimina una categoria que no existe
- **THEN** el sistema SHALL devolver un resultado de no encontrado y SHALL no crear registros de forma implicita

#### Scenario: No se elimina una categoria con subcategorias
- **WHEN** un usuario `Admin` intenta eliminar una categoria que contiene subcategorias
- **THEN** el sistema SHALL rechazar la operacion con un error de conflicto y SHALL no eliminar registros en cascada

#### Scenario: Usuario autenticado sin rol Admin no administra categorias
- **WHEN** un usuario autenticado sin el rol `Admin` intenta crear, actualizar o eliminar una categoria de inventario
- **THEN** el sistema SHALL rechazar la solicitud por autorizacion insuficiente y SHALL no modificar el catalogo

### Requirement: Errores de categoria compatibles con el contrato API
El sistema MUST devolver los errores de validacion, no encontrado y conflicto de categorias de inventario como ProblemDetails con mensajes expuestos en espanol.

#### Scenario: Error funcional de categorias
- **WHEN** una solicitud de categorias incumple una regla de validacion, integridad o jerarquia
- **THEN** el sistema SHALL devolver un error HTTP determinista conforme al contrato ProblemDetails de la API
