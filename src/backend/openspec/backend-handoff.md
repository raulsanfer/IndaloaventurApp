# Handoff backend: claim `IsMember` y contrato de autenticación

## Objetivo

Necesitamos que la condición de socio forme parte explícita de la identidad autenticada del usuario para poder:

- reutilizarla en backend como criterio de autorización/negocio,
- mantener `AuthSession.IsMember` en frontend de forma consistente,
- y evitar llamadas innecesarias a `fichas-socio/me` para usuarios no socios.

## Estado actual observado desde frontend

- El frontend ya consume `IsMember`.
- Hoy `IsMember` se persiste en `AuthSession`.
- En este subproyecto no existe lectura de claims/JWT para extraer ese valor.
- `MyAccount` usa `IsMember` para decidir visibilidad de bloques de socio.

## Cambio solicitado en backend

### 1. Emitir `IsMember` como claim

Al autenticar al usuario, backend debe incluir un claim explícito con el valor de `AspNetUser.IsMember`.

Requisitos:

- El claim MUST representar el valor real persistido en `AspNetUser.IsMember`.
- El claim MUST emitirse en cada autenticación/token nuevo.
- El formato del valor SHOULD ser booleano serializado de forma consistente (`true` / `false`).
- El nombre del claim SHOULD acordarse y mantenerse estable.

Sugerencia inicial:

- Nombre: `IsMember`
- Valor: `true` / `false`

Si el backend ya usa un naming convention distinto (`is_member`, URI, etc.), necesitamos documentarlo y consumir exactamente ese valor en frontend.

### 2. Mantener compatibilidad del login response

Mientras el frontend siga poblando `AuthSession` desde la validación/login, conviene que la respuesta de login también incluya `IsMember`.

Requisitos:

- `LoginResponse` SHOULD incluir `IsMember`.
- El valor de `LoginResponse.IsMember` MUST coincidir con el claim emitido en el token.

### 3. Actualizar contratos/documentación

Por favor, reflejar `IsMember` en:

- DTO/respuesta de login, si aplica.
- documentación OpenAPI/Swagger o contrato equivalente.
- cualquier test de autenticación que valide claims emitidos.

## Impacto esperado en frontend

Con este cambio, frontend hará lo siguiente:

- Mantener `AuthSession.IsMember`.
- Poblarlo con la información autenticada acordada con backend.
- Tratar `IsMember = false` como caso funcional válido en `MyAccount`.

## Comportamiento deseado en `MyAccount`

Cuando el usuario autenticado tenga `IsMember = false`:

- NO debe mostrarse error por no tener ficha de socio.
- NO deben renderizarse:
  - `MemberCargoBadge`
  - `Ficha Socio`
  - `Licencias Federativas`
- SÍ deben renderizarse las secciones no dependientes del perfil de socio, por ejemplo:
  - `AJUSTES Y SOPORTE`
  - `Cerrar sesión`

## Validaciones recomendadas

### Caso 1: usuario socio

- Login correcto
- Claim `IsMember = true`
- `LoginResponse.IsMember = true`
- `MyAccount` puede cargar ficha de socio

### Caso 2: usuario no socio

- Login correcto
- Claim `IsMember = false`
- `LoginResponse.IsMember = false`
- `MyAccount` no debe fallar aunque no exista ficha de socio

## Nota de diseño

`AuthSession.IsMember` en frontend no sustituye la autorización real del backend. La seguridad efectiva debe seguir validándose en servidor con claims, policies o reglas de negocio equivalentes.
