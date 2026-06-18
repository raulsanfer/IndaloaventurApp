## Context

El backend ya persiste `Usuario.IsMember` y lo usa en administracion de usuarios, pero los tokens JWT solo incluyen subject, email y roles. Los handlers de autenticacion tampoco devuelven `IsMember` en `LoginResponse`, por lo que el frontend no puede confiar en la identidad autenticada para decidir comportamiento de UI sin hacer llamadas adicionales.

## Goals / Non-Goals

**Goals:**
- Emitir `IsMember` como claim JWT con nombre estable y valor booleano consistente.
- Devolver `IsMember` en `LoginResponse` para login clasico y social.
- Mantener el origen de verdad en `Usuario.IsMember` sin duplicar logica de membresia.
- Cubrir la regresion con tests que inspeccionen el token emitido y el payload HTTP.

**Non-Goals:**
- Crear nuevas policies de autorizacion basadas en `IsMember`.
- Cambiar el comportamiento de `fichas-socio/me` en este cambio.
- Introducir almacenamiento adicional de claims en ASP.NET Identity.

## Decisions

- Usar el claim literal `IsMember` para alinearlo con el handoff y con el contrato esperado por frontend. Se descarta usar URIs o nombres transformados porque hoy no existe una convencion previa para claims personalizados en el proyecto.
- Serializar el valor como `true` o `false` en minusculas usando `ToString().ToLowerInvariant()` para evitar ambiguedad entre consumidores y facilitar comparaciones directas.
- Extender los resultados de `IIdentityService` para devolver `IsMember` junto con `UserId`, `Email` y roles. Asi el handler no necesita una segunda consulta y el dato sigue saliendo de la validacion autenticada.
- Extender `IJwtTokenService.CreateToken(...)` para recibir `isMember` y emitir el claim en un unico punto centralizado.

## Risks / Trade-offs

- [Cambio de contrato de login] → Mitigar manteniendo compatibilidad aditiva: se anade un campo nuevo sin renombrar ni eliminar existentes.
- [Dependencia de nombre de claim estable] → Mitigar centralizando el nombre en una constante reutilizable.
- [Desalineacion entre login clasico y social] → Mitigar reutilizando el mismo contrato y añadiendo tests para ambos flujos.
