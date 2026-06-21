# api-authorization-security-audit Specification

## Purpose
TBD - created by archiving change audit-authentication-authorization-security. Update Purpose after archive.
## Requirements
### Requirement: Every HTTP endpoint MUST have an explicit access classification
El sistema MUST mantener una matriz versionada de autorizacion para toda la superficie HTTP publicada por la API. Cada endpoint SHALL quedar clasificado de forma explicita al menos como `Anonymous`, `Authenticated`, `Admin`, `OwnerOrAdmin` o `ClubMember`, indicando ademas si la regla depende de rol tecnico, claim funcional o propiedad del recurso.

#### Scenario: Nuevo endpoint incluido en la matriz
- **WHEN** se anade o modifica un endpoint HTTP del backend
- **THEN** la matriz de autorizacion SHALL reflejar su ruta, metodo y clasificacion de acceso antes de considerar el cambio como completo

### Requirement: Protected endpoints MUST preserve negative authorization coverage
El sistema MUST disponer de pruebas automatizadas de regresion para cada endpoint protegido. Como minimo, esas pruebas SHALL verificar rechazo anonimo y, cuando aplique, rechazo por rol insuficiente o por acceso a recursos de otro usuario.

#### Scenario: Rechazo anonimo de endpoint protegido
- **WHEN** un cliente sin token llama a un endpoint clasificado como distinto de `Anonymous`
- **THEN** la suite automatizada SHALL verificar una respuesta de no autorizado o equivalente segun el contrato HTTP

#### Scenario: Rechazo de acceso cruzado o privilegio insuficiente
- **WHEN** un usuario autenticado llama a un endpoint protegido sin cumplir la condicion requerida de rol, propiedad o claim funcional
- **THEN** la suite automatizada SHALL verificar que el sistema no expone datos ajenos ni ejecuta la operacion solicitada

### Requirement: Self-service data endpoints MUST enforce owner isolation explicitly
El sistema MUST derivar la identidad efectiva de autoservicio desde el token autenticado o, alternativamente, SHALL exigir una regla explicita `OwnerOrAdmin` cuando la ruta permita informar un identificador de usuario destino.

#### Scenario: Usuario intenta acceder a un recurso personal ajeno
- **WHEN** un usuario autenticado sin privilegios administrativos solicita o modifica un recurso ligado a otro `UserId`
- **THEN** el sistema SHALL denegar el acceso sin exponer informacion del titular legitimo

#### Scenario: Administrador opera sobre recurso ajeno permitido
- **WHEN** un usuario autenticado con privilegios administrativos llama a un endpoint clasificado como `OwnerOrAdmin`
- **THEN** el sistema SHALL permitir la operacion siempre que el resto de validaciones funcionales se cumplan

### Requirement: Club-sensitive endpoints MUST distinguish technical role from club membership
El sistema MUST declarar de forma explicita si un endpoint protegido depende del rol tecnico `Member` o del claim funcional `IsMember`. Ningun endpoint que requiera condicion real de socio SHALL inferirla unicamente del rol tecnico `Member`.

#### Scenario: Endpoint dependiente de socio real
- **WHEN** un usuario autenticado con rol tecnico `Member` pero claim `IsMember = false` llama a un endpoint clasificado como `ClubMember`
- **THEN** el sistema SHALL denegar el acceso o la operacion por no cumplir la condicion funcional de socio

#### Scenario: Endpoint accesible para cualquier autenticado
- **WHEN** un endpoint este clasificado solo como `Authenticated`
- **THEN** el sistema SHALL no exigir roles adicionales ni `IsMember = true` para responder correctamente

