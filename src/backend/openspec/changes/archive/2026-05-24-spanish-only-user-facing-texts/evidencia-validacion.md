# Evidencia de aplicacion

## Inventario de literales user-facing (1.1)

Se revisaron controladores, handlers, servicios de identidad, validadores y mapeo de excepciones con `rg`.

Archivos con mensajes user-facing detectados y ajustados:
- `src/IndaloAventurApi.Api/Common/ProblemDetailsExceptionHandler.cs`
- `src/IndaloAventurApi.Application/Features/Auth/Login/LoginQueryHandler.cs`
- `src/IndaloAventurApi.Application/Features/Auth/SocialLogin/SocialLoginCommandHandler.cs`
- `src/IndaloAventurApi.Infrastructure/Security/GoogleSocialTokenValidator.cs`
- `src/IndaloAventurApi.Infrastructure/Security/IdentityService.cs`

## Clasificacion por categoria (1.2)

- Autenticacion/autorizacion:
  - `Credenciales invalidas.`
  - `El token social no es valido.`
  - `La cuenta de usuario esta desactivada.`
  - `No autorizado` (ProblemDetails title)
- Validacion:
  - `Error de validacion`
  - `Una o mas validaciones han fallado.`
- Conflicto de negocio:
  - `Conflicto de regla de negocio`
- Error tecnico:
  - `Error del servidor`
  - `Error inesperado.`
  - `No encontrado`

## Convencion temporal definida (1.3)

Mientras no exista i18n/localizacion:
- Todos los mensajes user-facing nuevos DEBEN escribirse en espanol.
- Los codigos HTTP y la estructura de ProblemDetails se mantienen sin cambios.
- Los mensajes tecnicos internos (logs, comentarios internos, nombres de test) pueden permanecer en ingles si no llegan al cliente.

## Verificacion automatizada (3.1, 3.2)

- Se actualizaron pruebas para aserciones de mensajes en espanol.
- Se ejecuto `dotnet test` para validar ausencia de regresiones.

## Verificacion final de literales en ingles (3.3)

Se revisaron literales de error user-facing y no quedaron mensajes en ingles dentro del flujo de autenticacion/autorizacion/ProblemDetails.
No se detectaron excepciones justificadas pendientes para mensajes expuestos al cliente.
