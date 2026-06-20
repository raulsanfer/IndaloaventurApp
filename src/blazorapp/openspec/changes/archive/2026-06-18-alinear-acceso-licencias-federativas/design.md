## Context

La capacidad actual de `Licencias Federativas` está definida alrededor del flujo del socio y mezcla en frontend varias decisiones de autorización: visibilidad de entrada, acceso operativo a la vista y alcance de los datos cargados. La nueva necesidad introduce un segundo modo de uso, administrativo, donde `Admin` debe poder inspeccionar y editar solicitudes de cualquier usuario, mientras que el modo socio debe seguir restringido a solicitudes propias.

Además, las reglas combinan dos señales distintas (`Roles` e `IsMember`), por lo que el cambio es de seguridad funcional y afecta a componentes, clientes HTTP y pruebas de regresión.

## Goals / Non-Goals

**Goals:**
- Centralizar las reglas de acceso a `Licencias Federativas` para evitar condiciones divergentes entre navegación, carga y edición.
- Mantener el flujo de socio acotado a solicitudes propias cuando el usuario sea `Member` con `IsMember = true`.
- Definir un modo administrativo explícito que permita listar y editar solicitudes de cualquier usuario.
- Dejar preparado el contrato frontend para distinguir claramente operaciones “sobre mí” y operaciones “sobre otro usuario”.

**Non-Goals:**
- Rediseñar la experiencia visual completa de `Mi Club`.
- Cambiar la semántica general de otros módulos que usan `IsMember` o `Admin`.
- Definir cambios de negocio ajenos a licencias federativas, como cargos, ficha de socio o gestión global de usuarios.

## Decisions

### 1. Separar permiso de acceso y alcance de datos
Se definirá una política de frontend con dos niveles:
- `CanAccessFederativeLicenses`: decide si el usuario puede entrar en la funcionalidad.
- `FederativeLicenseScope`: decide si opera sobre `Self` o `AnyUser`.

Rationale:
- El bug reciente demuestra que una sola condición booleana no cubre correctamente casos `Member`/`Admin`.
- Separar acceso de alcance evita que un `Admin` vea la opción pero siga cargando endpoints “me”.

Alternativas consideradas:
- Mantener condiciones inline por componente. Se descarta porque ya ha generado reglas inconsistentes.
- Usar solo roles. Se descarta porque el requisito sigue distinguiendo `Member` con `IsMember = false`.

### 2. Mantener el flujo de socio en endpoints y vistas “self”
El modo `Member + IsMember = true` seguirá usando vistas y contratos centrados en el usuario autenticado (`mis solicitudes`, `crear para mí`).

Rationale:
- Conserva el comportamiento ya especificado y reduce riesgo de fuga de datos entre socios.
- Hace explícito que la creación y visualización para socios no debe aceptar un `userId` arbitrario.

Alternativas consideradas:
- Reutilizar el mismo endpoint/listado genérico para socio y admin. Se descarta porque diluye el aislamiento de alcance y complica validaciones.

### 3. Introducir un flujo administrativo separado para consulta y edición de terceros
La capacidad `Admin` se modelará como un flujo diferenciado, previsiblemente con:
- una vista/listado o acceso contextual desde administración,
- selección del usuario objetivo,
- carga de solicitudes del usuario objetivo,
- edición de estados u otros campos permitidos por el backend.

Rationale:
- `Admin` no solo necesita ver la misma pantalla que un socio; necesita operar sobre usuarios arbitrarios.
- Un flujo diferenciado evita añadir controles de administración complejos dentro de la pantalla de autoservicio del socio.

Alternativas consideradas:
- Extender directamente la pantalla actual de `Mi Club` para todos los casos. Se descarta porque mezcla autoservicio con backoffice.

### 4. Asegurar la autorización en cliente y pruebas, no solo en la navegación
Las comprobaciones deberán existir en:
- visibilidad de la opción o entrada,
- carga operativa de la vista,
- cliente API antes de invocar endpoints incompatibles con el rol,
- pruebas de componentes y de cliente.

Rationale:
- Evita falsos positivos donde la navegación permita entrar pero el cliente quede bloqueado o cargue datos incorrectos.

## Risks / Trade-offs

- [El backend actual puede no exponer aún endpoints administrativos] → Mitigación: dejar el cambio especado y diseñar el frontend para soportar contratos `self` y `by-user`, confirmando en implementación qué endpoints existen o deben incorporarse.
- [Duplicación de UI entre modo socio y modo admin] → Mitigación: reutilizar componentes presentacionales y separar solo contenedores, permisos y acciones.
- [Regresiones en visibilidad para `Admin` y `Member`] → Mitigación: cubrir con pruebas específicas las combinaciones `Member false`, `Member true` y `Admin` con y sin `IsMember`.

## Migration Plan

1. Alinear la spec del flujo de socio con las reglas de autoservicio.
2. Añadir la spec nueva del flujo administrativo.
3. Implementar la política compartida de acceso/alcance.
4. Adaptar vistas y clientes HTTP según el alcance.
5. Ejecutar pruebas de componente y cliente antes de promover el cambio.

## Open Questions

- Qué campos exactos puede editar un `Admin` en una solicitud existente además del estado.
- Dónde debe vivir la entrada principal del flujo administrativo: en `Mi Club`, en administración de usuarios o en una pantalla administrativa dedicada.
- Si el backend ya dispone de endpoints administrativos por `userId` o si habrá que añadirlos en una fase posterior.
