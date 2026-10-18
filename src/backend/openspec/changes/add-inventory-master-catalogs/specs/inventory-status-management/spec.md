## Purpose

Proporciona el catalogo plano y controlado de estados de inventario que se aplicara posteriormente al material deportivo del club.

## ADDED Requirements

### Requirement: Catalogo plano de estados de inventario
El sistema MUST mantener registros `InventarioEstado` identificados por un entero autogenerado y con un `Nombre` obligatorio. Cada estado SHALL ser independiente de los demas estados y el catalogo SHALL no modelar relaciones jerarquicas entre ellos.

#### Scenario: Administrador crea un estado
- **WHEN** un usuario `Admin` crea un estado con un nombre valido
- **THEN** el sistema SHALL persistir el estado con identificador autogenerado

#### Scenario: Estado sin jerarquia
- **WHEN** un usuario consulta el catalogo de estados
- **THEN** el sistema SHALL devolver cada estado como un registro plano sin padre, hijos ni nivel jerarquico

### Requirement: Nombres de estado validos y unicos
El sistema MUST exigir que el `Nombre` de un `InventarioEstado` sea texto valido no vacio tras normalizacion, con una longitud maxima de 120 caracteres. El catalogo SHALL rechazar nombres duplicados sin distinguir mayusculas, minusculas ni espacios extremos.

#### Scenario: Nombre de estado valido
- **WHEN** un administrador crea o actualiza un estado con un nombre valido que contiene espacios extremos
- **THEN** el sistema SHALL almacenar el nombre normalizado sin los espacios extremos

#### Scenario: Nombre de estado duplicado
- **WHEN** un administrador crea o renombra un estado con un nombre ya registrado
- **THEN** el sistema SHALL rechazar la operacion con un error de conflicto y SHALL conservar el catalogo sin duplicados

#### Scenario: Nombre de estado invalido
- **WHEN** un administrador envia un nombre vacio, solo compuesto por espacios, con caracteres de control no permitidos o que supera 120 caracteres
- **THEN** el sistema SHALL rechazar la solicitud con un error de validacion en espanol

### Requirement: Consulta autenticada de estados de inventario
El sistema MUST permitir a cualquier usuario autenticado consultar el catalogo mediante `GET /api/inventario-estados` y consultar un estado existente mediante `GET /api/inventario-estados/{id}`. La lista SHALL devolver los identificadores y nombres de todos los estados ordenados alfabeticamente por nombre.

#### Scenario: Usuario autenticado lista estados
- **WHEN** un usuario autenticado solicita `GET /api/inventario-estados`
- **THEN** el sistema SHALL devolver todos los estados del catalogo ordenados alfabeticamente por nombre

#### Scenario: Usuario autenticado consulta un estado existente
- **WHEN** un usuario autenticado solicita `GET /api/inventario-estados/{id}` para un estado existente
- **THEN** el sistema SHALL devolver su identificador y nombre

#### Scenario: Consulta de estado inexistente
- **WHEN** un usuario autenticado solicita un estado que no existe
- **THEN** el sistema SHALL devolver un resultado de no encontrado

#### Scenario: Usuario anonimo no consulta estados
- **WHEN** un cliente sin autenticar solicita cualquier endpoint de consulta de estados de inventario
- **THEN** el sistema SHALL rechazar la solicitud por falta de autenticacion

### Requirement: Administracion de estados de inventario
El sistema MUST permitir exclusivamente a usuarios autenticados con rol tecnico `Admin` crear, actualizar y eliminar estados mediante `POST`, `PUT /api/inventario-estados/{id}` y `DELETE /api/inventario-estados/{id}`.

#### Scenario: Administrador actualiza un estado
- **WHEN** un usuario `Admin` actualiza el nombre de un estado existente con datos validos
- **THEN** el sistema SHALL persistir el nuevo nombre sin cambiar su identificador

#### Scenario: Actualizacion o eliminacion de estado inexistente
- **WHEN** un usuario `Admin` actualiza o elimina un estado que no existe
- **THEN** el sistema SHALL devolver un resultado de no encontrado y SHALL no crear registros de forma implicita

#### Scenario: Usuario autenticado sin rol Admin no administra estados
- **WHEN** un usuario autenticado sin el rol `Admin` intenta crear, actualizar o eliminar un estado de inventario
- **THEN** el sistema SHALL rechazar la solicitud por autorizacion insuficiente y SHALL no modificar el catalogo

### Requirement: Errores de estado compatibles con el contrato API
El sistema MUST devolver los errores de validacion, no encontrado y conflicto de estados de inventario como ProblemDetails con mensajes expuestos en espanol.

#### Scenario: Error funcional de estados
- **WHEN** una solicitud de estados incumple una regla de validacion o unicidad
- **THEN** el sistema SHALL devolver un error HTTP determinista conforme al contrato ProblemDetails de la API
