## Context

`Usuario.IsMember` se inicializa a `false` en los flujos de alta y hoy solo cambia mediante la gestion administrativa de usuarios. En paralelo, `CreateFichaSocio` crea la ficha vinculada sin tocar identidad, aunque `ApplicationDbContext` aloja tanto `AspNetUsers` como `FichasSocio`.

El modelo actual no define un estado explicito de activacion en `FichaSocio`, por lo que la creacion administrativa de la ficha es la mejor aproximacion disponible al alta activa de socio. La inconsistencia actual afecta consultas de usuarios, claims JWT y reglas funcionales que dependen de `IsMember`.

## Goals / Non-Goals

**Goals:**
- Hacer que la creacion satisfactoria de una `FichaSocio` marque al usuario vinculado con `IsMember = true`.
- Mantener consistencia entre identidad y ficha en el mismo flujo de alta de socio.
- Cubrir el flujo con pruebas de aplicacion y, si procede, de integracion.

**Non-Goals:**
- Redisenar el rol tecnico `Member` o su relacion con `IsMember`.
- Introducir un ciclo de vida nuevo para `FichaSocio` con estados activo/inactivo.
- Corregir automaticamente usuarios o fichas historicas ya existentes fuera del flujo de nueva creacion.

## Decisions

### 1. La creacion de `FichaSocio` representara el alta activa de socio

La ausencia de un flag de estado en `FichaSocio` obliga a tomar una decision de negocio concreta. Se considera que una ficha creada por administracion equivale a un alta activa y, por tanto, el sistema SHALL fijar `IsMember = true` al completar la operacion.

Alternativas consideradas:
- Mantener la activacion manual posterior desde gestion de usuarios. Rechazada porque conserva la inconsistencia que motiva el cambio.
- Anadir un estado `Activa` a `FichaSocio`. Rechazada en esta propuesta por ampliar innecesariamente el alcance.

### 2. La sincronizacion de `IsMember` se hara en el mismo flujo de aplicacion que crea la ficha

`CreateFichaSocioCommandHandler` debe orquestar tanto la creacion de la ficha como la actualizacion del usuario vinculado. La implementacion debe apoyarse en una abstraccion de identidad enfocada al cambio de membresia, evitando reutilizar `UpdateUserAsync`, que exige email y roles y mezcla responsabilidades ajenas a este caso de uso.

Alternativas consideradas:
- Reutilizar `UpdateUserAsync`. Rechazada porque obliga a reconstruir datos no relacionados y aumenta el riesgo de sobreescrituras accidentales.
- Actualizar `IsMember` desde el controlador. Rechazada porque rompe la coherencia del caso de uso y desplaza logica de negocio fuera de aplicacion.

### 3. La persistencia debe ser consistente entre `FichaSocio` y `Usuario`

Dado que `FichaSocio` y `Usuario` comparten `ApplicationDbContext`, la implementacion debe intentar confirmar ambos cambios dentro de la misma unidad de persistencia. Si la sincronizacion de identidad no puede completarse, el alta de ficha no debe considerarse satisfactoria.

Alternativas consideradas:
- Aceptar consistencia eventual. Rechazada porque deja abierto exactamente el estado incoherente que se intenta eliminar.

## Risks / Trade-offs

- [Existen fichas historicas con `IsMember = false`] -> Mitigacion: dejarlo fuera del alcance de este cambio y documentar que solo corrige nuevas altas.
- [La abstraccion de identidad requiere ampliacion] -> Mitigacion: introducir una operacion especifica y pequena, con pruebas centradas en el caso de uso.
- [El concepto de "ficha activa" puede cambiar en el futuro] -> Mitigacion: documentar en specs y design que la creacion se toma como alta activa mientras no exista un estado explicito.
