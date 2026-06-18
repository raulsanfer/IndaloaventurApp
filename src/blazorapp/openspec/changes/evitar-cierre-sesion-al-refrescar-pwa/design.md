## Context

La autenticaci�n del frontend se resuelve hoy con `AuthApiClient` y un `SessionService` `scoped` que solo conserva `AuthSession` en memoria. Las p�ginas protegidas redirigen en `OnInitialized` cuando `SessionService.IsAuthenticated` es `false`, por lo que cualquier recarga completa del navegador provoca navegaci�n inmediata al login aunque el `accessToken` siga vigente.

La app se renderiza con `InteractiveAuto`, as� que en la primera fase de prerender no existe acceso a `localStorage` ni `sessionStorage`. Adem�s, el backend actual devuelve `accessToken`, `tokenType`, `expiresInSeconds` e informaci�n derivable de claims, pero no expone `refresh token`. El dise�o debe, por tanto, restaurar una sesi�n ya emitida sin prometer renovaci�n silenciosa cuando esa credencial haya expirado.

## Goals / Non-Goals

**Goals:**
- Mantener la sesi�n del usuario tras `F5`, recarga del navegador o gesto de refresco en la PWA mientras el token siga siendo v�lido.
- Restaurar la sesi�n antes de que una p�gina protegida decida si debe redirigir al login.
- Aprovechar `RememberMe` para distinguir entre persistencia solo durante la sesi�n del navegador y persistencia m�s duradera.
- Eliminar autom�ticamente la sesi�n persistida al cerrar sesi�n o detectar expiraci�n.

**Non-Goals:**
- Implementar refresh token, renovaci�n silenciosa o extensi�n autom�tica de la vida del `accessToken`.
- Cambiar el backend de autenticaci�n o el contrato actual de login.
- Reescribir la navegaci�n de la app para usar `AuthorizeRouteView` o ASP.NET Core Identity.
- Garantizar continuidad cuando el navegador elimine almacenamiento local por pol�ticas externas o limpieza manual.

## Decisions

1. Persistir una instant�nea de sesi�n en almacenamiento de navegador y no solo el `AuthSession` en memoria.
Rationale: el servicio actual pierde todo el contexto en una recarga completa. Una instant�nea serializada con `accessToken`, `tokenType`, `isMember`, `roles` y una expiraci�n absoluta permite rehidratar la sesi�n sin llamada extra al backend.
Alternatives considered:
- Reconstruir la sesi�n llamando a un endpoint de perfil al arrancar: descartado porque seguir�a requiriendo un token que hoy se pierde en la recarga.
- Persistir solo el JWT y recalcular siempre el resto: descartado porque `isMember` y roles ya se resuelven en el cliente y es m�s robusto persistir el estado normalizado.

2. Usar `sessionStorage` por defecto y `localStorage` cuando `RememberMe` est� activado.
Rationale: ambas opciones sobreviven a una recarga completa, pero `sessionStorage` limita la persistencia al ciclo de la pesta�a/navegador y `localStorage` encaja con la sem�ntica esperada de `Recordarme` sin ampliar demasiado el alcance.
Alternatives considered:
- Usar siempre `localStorage`: descartado por ampliar la persistencia incluso cuando el usuario no la ha pedido.
- Usar siempre `sessionStorage`: descartado porque dejar�a `RememberMe` sin efecto funcional.

3. Introducir una fase expl�cita de inicializaci�n/rehidrataci�n de sesi�n antes de evaluar las guardas de navegaci�n.
Rationale: con `InteractiveAuto`, la restauraci�n depende de JS interop y no puede ocurrir durante el prerender. Las p�ginas protegidas necesitan distinguir entre "todav�a no he intentado restaurar" y "ya s� que no hay sesi�n v�lida" para evitar redirecciones prematuras.
Alternatives considered:
- Mantener las redirecciones en `OnInitialized` y restaurar despu�s: descartado porque seguir�a expulsando al usuario antes de leer el almacenamiento del navegador.
- Duplicar la restauraci�n en cada p�gina protegida: descartado por repetici�n y riesgo de comportamientos inconsistentes.

4. Validar la expiraci�n en cliente y limpiar el almacenamiento antes de exponer la sesi�n restaurada.
Rationale: `expiresInSeconds` es relativo al momento del login, as� que el cliente debe persistir una expiraci�n absoluta (`expiresAtUtc`) y rechazar instant�neas caducadas para no navegar con una sesi�n que el backend ya no aceptar�.
Alternatives considered:
- Confiar en que el backend devolver� `401` en la primera llamada: descartado porque empeora la UX y retrasa la limpieza del estado inv�lido.
- Leer siempre `exp` desde el JWT: descartado como �nica fuente porque el contrato actual ya expone `expiresInSeconds` y puede haber tokens de desarrollo no JWT.

## Risks / Trade-offs

- [El prerender inicial no puede acceder al almacenamiento del navegador] -> Mitigaci�n: modelar la sesi�n con un estado de inicializaci�n pendiente y aplazar la redirecci�n hasta que la app est� interactiva.
- [Persistir tokens en almacenamiento web aumenta superficie ante XSS] -> Mitigaci�n: mantener el alcance limitado, no almacenar m�s datos de los necesarios y conservar las defensas existentes del frontend; este riesgo ya existe en cualquier estrategia de token en navegador.
- [La restauraci�n solo funciona mientras el token siga vivo] -> Mitigaci�n: dejar expl�cito en la UX y en la spec que, al expirar, el usuario deber� autenticarse de nuevo hasta que exista `refresh token`.
- [Habrá varias p�ginas protegidas con patrones de guardia similares] -> Mitigaci�n: centralizar la inicializaci�n de sesi�n o encapsular la guardia para evitar divergencias entre rutas.

## Migration Plan

1. Introducir la abstracci�n de almacenamiento y la instant�nea persistida de sesi�n sin cambiar todav�a la navegaci�n.
2. Adaptar `SessionService` y el flujo de login/logout para escribir, restaurar y limpiar la sesi�n persistida.
3. Mover la comprobaci�n de acceso protegido a un punto que espere la inicializaci�n de sesi�n antes de redirigir.
4. Verificar manualmente y con tests la recarga en navegador de escritorio, recarga en m�vil/PWA y expiraci�n de sesi�n.

## Open Questions

- Si el backend va a incorporar `refresh token` m�s adelante, convendr� extender esta capacidad en vez de reemplazarla.
- Queda por decidir si el login social debe reutilizar exactamente la selecci�n actual de `RememberMe` o asumir persistencia de sesi�n normal cuando el usuario no interact�a con ese checkbox.
