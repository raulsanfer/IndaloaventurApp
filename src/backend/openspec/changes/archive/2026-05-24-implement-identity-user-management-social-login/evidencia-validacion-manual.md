# Evidencia de validacion manual

Fecha: 2026-05-24

## Escenarios validados (auth/autorizacion)

1. Registro y login de usuario miembro
- Registro exitoso con `POST /api/auth/register`.
- Login exitoso con `POST /api/auth/login`.
- Se recibe JWT valido en respuesta.

2. Acceso a endpoints protegidos sin token
- `GET /api/users` y `GET /api/agenda-telefonica` devuelven `401 Unauthorized`.

3. Restriccion por rol
- Usuario `Member` no puede acceder a `GET /api/users` ni `POST /api/users` (`403 Forbidden`).
- Usuario `Admin` puede crear/listar/editar usuarios gestionados.

4. Desactivacion y reactivacion de usuario
- Usuario desactivado no puede hacer login (`401 Unauthorized`).
- Tras reactivacion, vuelve a poder autenticarse correctamente.

5. Login social
- `POST /api/auth/social-login` con proveedor/token validos devuelve JWT local.

## Evidencia automatizada de soporte

Estos escenarios estan respaldados por la suite de integracion en `tests/IndaloAventurApi.IntegrationTests/ApiIntegrationTests.cs`.
