## Purpose

Permite al club registrar, consultar y mantener de forma segura el material de montana que forma parte de su inventario.

## ADDED Requirements

### Requirement: Registrar material de inventario clasificado
El sistema MUST permitir a un usuario con rol tecnico `Admin` crear material de inventario con identificador entero autogenerado, `Descripcion`, `MarcaModelo`, `Cantidad`, `CategoriaId`, `EstadoId`, `FechaAdquisicion`, `AdquiridoDonado`, `Observaciones` y `NumeroSerie`. `Descripcion`, `Cantidad`, `CategoriaId`, `EstadoId` y `AdquiridoDonado` MUST ser obligatorios; `MarcaModelo`, `FechaAdquisicion`, `Observaciones` y `NumeroSerie` pueden estar ausentes cuando se desconozcan o no apliquen.

`Cantidad` MUST ser un entero positivo. La fecha de adquisicion, cuando se informe, MUST no ser futura. Los textos MUST normalizarse y rechazar valores vacios donde son obligatorios, caracteres de control no permitidos y valores que excedan los limites publicados por el contrato. Las observaciones MUST admitir texto libre seguro y opcional.

#### Scenario: Alta de material valida
- **WHEN** un administrador crea material con los datos obligatorios validos, una categoria existente y un estado existente
- **THEN** el sistema persiste el material, genera su identificador y conserva todos los campos informados

#### Scenario: Alta rechazada por cantidad o fecha no validas
- **WHEN** un administrador intenta crear material con cantidad cero o negativa, o con fecha de adquisicion futura
- **THEN** el sistema rechaza la solicitud con un error de validacion en Espanol y no crea el material

#### Scenario: Alta rechazada por categoria o estado inexistente
- **WHEN** un administrador crea material con un identificador de categoria o de estado que no existe
- **THEN** el sistema rechaza la solicitud indicando que la referencia de maestro no existe y no persiste el material

### Requirement: Gestionar numeros de serie de material individual
El sistema MUST permitir que `NumeroSerie` sea opcional. Cuando se informe, MUST identificar un unico registro de material sin distinguir mayusculas, minusculas ni espacios extremos, y el registro MUST tener `Cantidad` igual a uno.

#### Scenario: Alta de material individual con numero de serie
- **WHEN** un administrador crea material con cantidad uno y un numero de serie no usado
- **THEN** el sistema guarda el numero de serie normalizado para su comparacion y lo devuelve con el valor de presentacion informado

#### Scenario: Numero de serie duplicado o asociado a varias unidades
- **WHEN** un administrador intenta usar un numero de serie ya asignado o informa un numero de serie con cantidad superior a uno
- **THEN** el sistema rechaza la solicitud como conflicto o validacion, segun corresponda, sin alterar el inventario existente

### Requirement: Consultar material de inventario
El sistema MUST permitir a cualquier usuario autenticado obtener el listado completo de material y el detalle de un material por identificador. Cada resultado MUST incluir todos los datos del material y la identificacion legible de su categoria y estado asociados.

#### Scenario: Listado de material para usuario autenticado
- **WHEN** un usuario autenticado solicita el listado de material
- **THEN** el sistema devuelve los registros disponibles con su categoria y estado, sin requerir el rol `Admin`

#### Scenario: Consulta de detalle inexistente
- **WHEN** un usuario autenticado solicita el detalle de un identificador de material inexistente
- **THEN** el sistema devuelve una respuesta de no encontrado conforme al contrato de errores de la API

#### Scenario: Consulta anonima rechazada
- **WHEN** un cliente sin autenticacion solicita el listado o detalle de material
- **THEN** el sistema rechaza la solicitud por falta de autenticacion

### Requirement: Modificar y eliminar material de inventario
El sistema MUST permitir exclusivamente al rol tecnico `Admin` actualizar todos los campos de un material existente y eliminarlo. La modificacion MUST volver a validar cantidad, fecha, numero de serie, categoria y estado antes de persistir los cambios.

#### Scenario: Edicion de material valida
- **WHEN** un administrador actualiza un material existente con datos validos y referencias de maestros existentes
- **THEN** el sistema persiste los nuevos datos y el detalle posterior refleja la actualizacion

#### Scenario: Eliminacion de material valida
- **WHEN** un administrador solicita eliminar un material existente
- **THEN** el sistema elimina unicamente ese material y confirma que ya no puede consultarse

#### Scenario: CUD denegado para usuario no administrador
- **WHEN** un usuario autenticado que no tiene el rol `Admin` intenta crear, editar o eliminar material
- **THEN** el sistema rechaza la operacion por autorizacion insuficiente y no modifica datos

### Requirement: Preservar la integridad con los maestros de inventario
El sistema MUST conservar referencias validas hacia las categorias y estados de inventario. Mientras exista material asociado, el sistema MUST impedir eliminar la categoria o el estado referenciados y comunicar el conflicto sin exponer detalles internos de persistencia.

#### Scenario: Borrado de categoria referenciada
- **WHEN** un administrador intenta eliminar una categoria que esta asignada a uno o mas materiales
- **THEN** el sistema rechaza el borrado como conflicto y mantiene tanto la categoria como el material asociado

#### Scenario: Borrado de estado referenciado
- **WHEN** un administrador intenta eliminar un estado que esta asignado a uno o mas materiales
- **THEN** el sistema rechaza el borrado como conflicto y mantiene tanto el estado como el material asociado

### Requirement: Contrato HTTP y autorizacion del inventario de material
El sistema MUST exponer `GET /api/inventario-materiales` y `GET /api/inventario-materiales/{id}` para usuarios autenticados, y `POST /api/inventario-materiales`, `PUT /api/inventario-materiales/{id}` y `DELETE /api/inventario-materiales/{id}` exclusivamente para `Admin`. Las lecturas correctas MUST devolver `200`, la creacion `201` con la localizacion del detalle y el identificador creado, y actualizacion o borrado correctos `204`; los errores de validacion, inexistencia y conflicto MUST devolverse mediante ProblemDetails en Espanol con los codigos HTTP correspondientes.

#### Scenario: Creacion HTTP correcta
- **WHEN** un administrador envia una solicitud HTTP valida de creacion de material
- **THEN** la API devuelve `201 Created`, el identificador del material y una localizacion que permite consultar su detalle

#### Scenario: Conflicto HTTP por numero de serie o maestro dependiente
- **WHEN** una operacion HTTP incumple la unicidad de numero de serie o intenta borrar un maestro referenciado
- **THEN** la API devuelve `409 Conflict` mediante ProblemDetails en Espanol
