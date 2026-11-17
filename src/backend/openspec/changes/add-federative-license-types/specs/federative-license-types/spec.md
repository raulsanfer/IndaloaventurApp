## Purpose

Definir los tipos de licencia federativa disponibles para clasificar las tarifas y permitir su selección sin mantenimiento desde el frontend.

## ADDED Requirements

### Requirement: El sistema MUST disponer de tipos de licencia predeterminados
El sistema MUST persistir tipos de licencia con `Id` estable y `Nombre` obligatorio y único. La inicialización SHALL incluir exactamente los nombres `Completa`, `Media 01/07 - 31/12` y `Día`, sin duplicarlos al volver a desplegar.

#### Scenario: Inicialización de los tipos
- **WHEN** se inicializa una base de datos con el nuevo modelo
- **THEN** el catálogo SHALL contener los tipos `Completa`, `Media 01/07 - 31/12` y `Día` con identificadores distintos y estables

#### Scenario: Despliegue repetido
- **WHEN** se vuelve a ejecutar el despliegue sobre una base de datos ya actualizada
- **THEN** el sistema SHALL conservar los identificadores y SHALL no duplicar los tipos predeterminados

### Requirement: El sistema MUST permitir consultar los tipos a cualquier usuario autenticado
El sistema MUST exponer `GET /api/licencias-federativas/tipos` sin exigir un rol específico. La respuesta SHALL incluir `Id` y `Nombre`, con orden determinista: `Completa`, `Media 01/07 - 31/12`, `Día`, aunque algún tipo todavía no tenga tarifas.

#### Scenario: Consulta de tipos sin tarifas diarias
- **WHEN** un usuario autenticado consulta los tipos y todavía no hay tarifas de `Día`
- **THEN** el sistema SHALL devolver `200 OK` con los tres tipos en el orden definido

#### Scenario: Acceso anónimo al catálogo de tipos
- **WHEN** un cliente sin autenticación consulta los tipos
- **THEN** el sistema SHALL devolver `401 Unauthorized`

### Requirement: El catálogo de tipos MUST carecer de mantenimiento por el cliente
El sistema MUST gestionar los tipos mediante datos predeterminados del despliegue y SHALL no exponer operaciones de creación, modificación o eliminación de tipos para usuarios ni administradores.

#### Scenario: Intento de alta desde un cliente
- **WHEN** un cliente intenta crear un tipo mediante una petición de escritura al catálogo
- **THEN** el sistema SHALL no ofrecer una operación de alta y SHALL conservar el catálogo sin cambios
