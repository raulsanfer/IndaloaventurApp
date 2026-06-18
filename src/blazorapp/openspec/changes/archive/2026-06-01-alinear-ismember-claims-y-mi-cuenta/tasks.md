## 1. Contrato y coordinación backend

- [x] 1.1 Redactar handoff backend con el origen de `AspNetUser.IsMember`, claim esperado y puntos de integración del token/login
- [x] 1.2 Alinear el nombre exacto del claim y el formato del valor (`true`/`false`)
- [x] 1.3 Actualizar la documentación del contrato para reflejar `IsMember` en autenticación y, si aplica, en DTOs relacionados

## 2. Sesión y autenticación en frontend

- [x] 2.1 Mantener `AuthSession.IsMember` como estado local derivado de la autenticación validada
- [x] 2.2 Ajustar el cliente/login para poblar `AuthSession.IsMember` desde la información autenticada acordada con backend
- [x] 2.3 Verificar que el cierre de sesión limpie también cualquier estado derivado de `IsMember`

## 3. Mi Cuenta para usuarios no socios

- [x] 3.1 Refactorizar `GetMyProfileAsync` para no tratar la ausencia de ficha como error cuando `IsMember = false`
- [x] 3.2 Adaptar `MyAccountView` para renderizar secciones no dependientes de `Profile` aunque no exista perfil de socio
- [x] 3.3 Mantener ocultos los bloques de socio (`MemberCargoBadge`, `Ficha Socio`, `Licencias Federativas`) cuando `IsMember = false`

## 4. Verificación

- [x] 4.1 Probar login de usuario con `IsMember = true` y verificar persistencia del valor en sesión
- [x] 4.2 Probar login de usuario con `IsMember = false` y verificar que `Mi Cuenta` no muestra error
- [x] 4.3 Ejecutar build/tests del frontend y registrar evidencia
- [x] 4.4 Validar con backend que el claim emitido y el valor consumido por frontend son consistentes
