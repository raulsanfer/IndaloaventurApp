## Context

La API administra cuentas con ASP.NET Core Identity y emite JWT de vida limitada. Actualmente valida el estado activo de la cuenta, pero no vincula los tokens al `SecurityStamp`; tampoco tiene refresh tokens. Véanse `proposal.md` y los deltas de especificación para el contrato funcional.

## Goals / Non-Goals

**Goals:**

- Añadir una operación administrativa de cambio de contraseña con autorización, reautenticación reciente, validación de cuerpo, limitación, auditoría e invalidación efectiva de JWT.
- Mantener las respuestas a front genéricas y libres de secretos.

**Non-Goals:**

- No se añade un almacén de refresh tokens, MFA ni un flujo de recuperación para el usuario final.
- No se relaja ni sustituye la política de contraseñas configurada en Identity.

## Decisions

- Se añadirá un comando de aplicación que valida `newPassword` con FluentValidation y delega el cambio en `UserManager` mediante el flujo de restablecimiento de Identity con un token generado y consumido internamente. Así se reutilizan hash, validadores, sello de seguridad y persistencia de Identity; no se crean hashes ni tokens propios. Como alternativa se descartará eliminar y volver a añadir la contraseña porque dejaría dos escrituras y una ventana de credencial incompleta.
- El resultado de la operación distinguirá usuario inexistente, entrada/política rechazada y éxito, pero el controlador solo expondrá `404` o `400` genéricos. Los detalles de Identity no cruzarán la frontera HTTP.
- Los JWT incluirán un claim interno de sello de seguridad. La validación de JWT comparará el claim con el sello actual resuelto desde Identity, además de comprobar que la cuenta sigue activa. Así un cambio correcto invalida JWT ya emitidos sin introducir refresh tokens. Como alternativa, esperar a que caduquen los JWT no cumple la revocación pedida.
- La emisión de JWT incluirá `auth_time`; una política dedicada de administrador exigirá un valor no superior a cinco minutos para el endpoint de cambio. Una sesión administrativa antigua deberá iniciar sesión de nuevo. Como alternativa, aceptar cualquier JWT Admin no reduce el riesgo de token administrativo robado.
- Se configurará un limitador de ventana fija conservador por par `(actorId, targetUserId)`, con una respuesta `429` sin detalles. Su rechazo y los resultados que alcancen el flujo de aplicación se registrarán estructuradamente usando `TraceIdentifier`; el mensaje y los campos excluyen explícitamente secretos.
- El DTO del endpoint deshabilitará miembros JSON no mapeados. El manejo de estado de modelo devolverá un problema genérico para evitar revelar la estructura o un valor de contraseña.

## Risks / Trade-offs

- [Cada validación de JWT consulta Identity] → El coste es una consulta adicional por solicitud protegida; se prefiere a un periodo de revocación diferida para una API interna de bajo volumen.
- [El límite es local al proceso] → Se documenta como límite de una instancia; al escalar horizontalmente deberá sustituirse por un almacén distribuido.
- [La política de Identity puede ser más restrictiva que 1–20 alfanumérico] → El frontend recibe `400` genérico y la política de Identity sigue siendo la autoridad final.
- [Reautenticación basada en `auth_time` no es MFA] → Reduce la reutilización de JWT antiguos; una futura exigencia de MFA requerirá un factor de confirmación adicional.

## Migration Plan

1. Desplegar la emisión y validación del claim de sello simultáneamente: los tokens existentes sin claim quedarán invalidados y los usuarios deberán iniciar sesión de nuevo.
2. Desplegar el endpoint y el limitador junto con monitorización de los eventos de auditoría.
3. Si fuese necesario revertir, retirar el endpoint y la validación del claim; las contraseñas cambiadas permanecen válidas y no requieren migración de datos.
