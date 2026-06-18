## Context

Actualmente el frontend usa `AuthSession.IsMember` como una bandera de sesión local para controlar visibilidad en `Mi Cuenta`, pero no existe en este subproyecto una lectura del valor desde claims/JWT ni un contrato documentado que garantice su origen. A la vez, `GetMyProfileAsync` sigue intentando obtener la ficha de socio incluso cuando el usuario autenticado no es socio, lo que convierte un caso de negocio válido en un error de experiencia.

## Goals / Non-Goals

**Goals:**
- Formalizar `IsMember` como parte de la identidad autenticada emitida por backend.
- Permitir que el frontend siga trabajando con `AuthSession.IsMember`, pero alimentado desde la autenticación real del usuario.
- Evitar que `MyAccount` falle o muestre error para usuarios autenticados con `IsMember = false`.
- Mantener visibles en `Mi Cuenta` las secciones no dependientes de la ficha de socio.
- Dejar un documento de handoff claro para backend.

**Non-Goals:**
- Implementar autorización completa basada en policies dentro de este frontend.
- Rediseñar visualmente `MyAccount` más allá del ajuste funcional del estado no socio.
- Introducir nuevas pantallas o nuevos permisos funcionales en esta iteración.

## Decisions

1. `IsMember` se modela como claim de identidad
- Decisión: backend emitirá un claim explícito de `IsMember` derivado de `AspNetUser.IsMember`.
- Rationale: permite que la señal forme parte de la identidad autenticada y pueda reutilizarse tanto en backend como en frontend.

2. El frontend mantiene `AuthSession.IsMember`
- Decisión: no se elimina `AuthSession.IsMember`; se mantiene como proyección local de la identidad autenticada.
- Rationale: simplifica la UI y evita dispersar lógica de claims dentro de componentes.

3. `MyAccount` debe ser tolerante con usuarios no socios
- Decisión: cuando `IsMember = false`, el flujo no debe intentar recuperar ficha de socio como requisito de render.
- Rationale: un usuario autenticado no socio es un caso válido, no una condición de error.

4. Render parcial sin `Profile`
- Decisión: la vista mostrará secciones comunes (`AJUSTES Y SOPORTE`, cierre de sesión y cualquier bloque no dependiente de `Profile`) aunque el perfil de socio no exista.
- Rationale: desacopla la experiencia básica de cuenta del concepto “ficha de socio”.

5. Contrato explícito para backend
- Decisión: documentar tanto el claim esperado como la recomendación de mantener `IsMember` en la respuesta de login mientras el frontend siga usando `AuthSession`.
- Rationale: reduce ambigüedad entre origen en claims y conveniencia del payload de autenticación.

## Risks / Trade-offs

- [Claim emitido pero no documentado] -> Mitigación: actualizar el contrato de autenticación y el handoff backend con nombre, tipo y semántica exactos.
- [Frontend con sesión en memoria pero sin parsing de claims] -> Mitigación: seguir rellenando `AuthSession.IsMember` desde la validación/login hasta introducir una estrategia de principal más rica.
- [Componentes que asumen `Profile` siempre presente] -> Mitigación: separar render dependiente de `Profile` del render general de `Mi Cuenta`.
- [Doble fuente temporal de verdad: claim y payload login] -> Mitigación: declarar que el payload de login es una proyección del mismo origen de identidad y debe permanecer consistente con el claim.

## Migration Plan

- Backend incorpora `IsMember` al proceso de generación de claims/token y, si aplica, al DTO de login.
- Frontend adapta la carga de sesión para confiar en el valor autenticado de `IsMember`.
- Frontend modifica la lógica de `MyAccount` para que la ausencia de ficha de socio no genere error cuando `IsMember = false`.
- Se actualizan pruebas y contratos documentados.

## Open Questions

- ¿El backend devolverá `IsMember` tanto en JWT claim como en `LoginResponse` durante la transición, o se quiere migrar a parseo explícito del token en frontend?
- ¿El nombre canónico del claim será `is_member`, `IsMember` o un URI/namespace propio?
