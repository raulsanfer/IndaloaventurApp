## Context

La app ya dispone de una pantalla de login pública con autenticación por usuario/contraseña y login social, pero el enlace `He olvidado mi contraseña` todavía no tiene operativa. El backend ha definido un flujo clásico de recuperación con dos endpoints: uno para solicitar el correo y otro para confirmar el nuevo password usando `email` y `token` recibidos en la URL.

La implementación debe respetar los patrones actuales del proyecto: rutas en `IndaloaventurApp.Web.Client`, componentes compartidos en `IndaloaventurApp.SharedUI`, recursos localizados en español, lógica separada del `.razor` cuando sea razonable y estilos centralizados en SCSS. Además, por seguridad, el token no debe persistirse en almacenamiento del navegador ni registrarse en logs del cliente.

## Goals / Non-Goals

**Goals:**
- Habilitar un flujo público completo para solicitar recuperación de contraseña desde login.
- Habilitar una página pública de restablecimiento que consuma `email` y `token` desde la query string.
- Reutilizar el sistema de autenticación del frontend para encapsular ambas llamadas HTTP sin mezclar lógica de UI con detalle de transporte.
- Mantener mensajes de UX coherentes: respuesta neutra en la solicitud, errores backend visibles en el reset y confirmación tras éxito con retorno a login.
- Preservar el estilo visual actual de la experiencia de autenticación.

**Non-Goals:**
- Cambiar el contrato de backend o el formato del enlace emitido por correo.
- Persistir borradores del formulario o el token entre sesiones.
- Implementar autenticación automática tras restablecer la contraseña.
- Rediseñar toda la pantalla de login más allá de integrar el nuevo acceso y páginas relacionadas.

## Decisions

1. Crear dos rutas públicas diferenciadas: una para solicitar la recuperación y otra para restablecer la contraseña.
Rationale: separa claramente el inicio del flujo y la confirmación final, simplifica validaciones y encaja con el enlace que el backend ya enviará al usuario.
Alternatives considered:
- Resolver ambos pasos en una sola página condicional: descartado porque mezcla estados distintos y complica la entrada directa desde el correo.

2. Extender el servicio de autenticación del frontend con operaciones específicas de password recovery.
Rationale: los endpoints pertenecen al dominio de autenticación y deben quedar accesibles desde una abstracción ya usada por login, manteniendo consistencia de testing y DI.
Alternatives considered:
- Crear un servicio independiente solo para recuperación: descartado porque fragmenta un dominio pequeño que ya vive en `Auth`.

3. Tratar `email` y `token` como datos efímeros de navegación leídos desde query string.
Rationale: permite cumplir el contrato del backend y evita persistencia innecesaria de credenciales sensibles.
Alternatives considered:
- Guardar el token temporalmente en `localStorage` o sesión: descartado por aumentar superficie de exposición sin necesidad funcional.

4. Mostrar siempre una respuesta neutra tras `passrecovery`, independientemente del resultado funcional de existencia de cuenta.
Rationale: el propio backend y el markdown de negocio fijan este patrón para no filtrar si un email existe o no en el sistema.
Alternatives considered:
- Diferenciar errores por cuenta inexistente: descartado por romper el requisito de seguridad del flujo.

5. Mostrar errores del reset devueltos por backend sin traducir y ofrecer una vía visible para reiniciar el flujo.
Rationale: el backend ya será la fuente de verdad para token inválido, expirado o política de contraseña; exponer ese mensaje reduce ambigüedad y facilita soporte.
Alternatives considered:
- Mapear todos los errores a mensajes genéricos del frontend: descartado porque perdería precisión operativa.

6. Tras un reset exitoso, redirigir al login con un estado de confirmación visible en la propia experiencia de autenticación.
Rationale: cierra el flujo con la siguiente acción natural del usuario, sin introducir una pantalla intermedia innecesaria.
Alternatives considered:
- Quedarse en la página de reset mostrando éxito: descartado porque deja al usuario en una pantalla ya consumida y sin siguiente paso claro.

## Risks / Trade-offs

- [El backend podría devolver mensajes distintos según el fallo del reset] → Mitigación: diseñar la UI para renderizar texto backend sin asumir catálogo cerrado.
- [Los tokens de Identity suelen incluir caracteres sensibles a encoding] → Mitigación: leer y reenviar los parámetros tal como llegan desde la URL, evitando transformaciones manuales innecesarias.
- [La navegación pública puede quedar inconsistente con el shell actual de la app] → Mitigación: reutilizar la composición visual de autenticación ya existente y limitar los cambios al área pública.
- [La redirección con mensaje de éxito requiere transportar estado entre páginas] → Mitigación: usar query string o estado de navegación simple y efímero, sin almacenamiento persistente.
- [Puede faltar copy exacto para ayudas de política de contraseña] → Mitigación: dejar ese texto como opcional y depender del mensaje backend cuando la validación falle.
