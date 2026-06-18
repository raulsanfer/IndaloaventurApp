## Context

El backend ya utiliza ASP.NET Identity como fuente autoritativa de usuarios y contrasenas, pero solo expone registro, login y login social. No existe flujo de recuperacion de contrasena, tampoco una abstraccion de correo saliente ni un contrato documentado para que el frontend renderice una pagina publica de reseteo.

Se trata de un cambio transversal porque afecta autenticacion, seguridad, configuracion, correo y coordinacion frontend-backend. ASP.NET Identity ya ofrece un flujo explicito y recomendado para recuperar contrasenas mediante `GeneratePasswordResetTokenAsync` y `ResetPasswordAsync`, por lo que la propuesta debe apoyarse en ese mecanismo en lugar de inventar criptografia propia.

## Goals / Non-Goals

**Goals:**
- Permitir que un usuario solicite la recuperacion de su contrasena informando su email.
- Enviar un correo con enlace seguro hacia una ruta publica del frontend preparada para definir una nueva contrasena.
- Permitir que el frontend confirme la nueva contrasena contra un endpoint backend usando email, token y contrasena nueva.
- Evitar enumeracion de usuarios y mantener mensajes user-facing en espanol.
- Dejar una guia clara en `docs/` para que frontend implemente la pagina publica y la llamada final al backend.

**Non-Goals:**
- Implementar la pagina frontend en este repositorio.
- Crear un sistema propio de tokens fuera de ASP.NET Identity.
- Anadir MFA, captcha o rate limiting avanzado en esta primera iteracion.
- Permitir recuperacion de contrasena por SMS u otros canales.

## Decisions

### 1. Reutilizar el flujo nativo de ASP.NET Identity
La generacion y validacion del token se basara en `GeneratePasswordResetTokenAsync` y `ResetPasswordAsync`.

Alternativas consideradas:
- Crear JWTs o tokens propios: descartado porque duplica capacidades de Identity y aumenta riesgo de implementacion insegura.
- Permitir cambio directo sin token: descartado por inseguro.

### 2. Endpoint de solicitud con respuesta neutra
El endpoint de inicio devolvera siempre una respuesta generica satisfactoria aunque el email no exista o el usuario este inactivo, para no revelar si una cuenta esta registrada.

Alternativas consideradas:
- Devolver `404` si no existe usuario: descartado por riesgo de enumeracion de usuarios.

### 3. Enlace al frontend con token URL-safe
El email incluira un enlace a una URL publica del frontend configurable desde backend, enviando `email` y `token` codificados de forma segura para query string o fragment, segun el contrato documentado.

Alternativas consideradas:
- Alojar la pagina de reseteo en backend: descartado porque el usuario ha pedido expresamente que la UI viva en frontend.

### 4. Confirmacion publica sin autenticacion previa
El endpoint final de reseteo sera anonimo y validara token, email y nueva contrasena. La seguridad residira en el token emitido por Identity, su expiracion y el cumplimiento de la politica de contrasenas.

Alternativas consideradas:
- Exigir JWT autenticado para resetear: descartado porque un usuario que olvido la contrasena precisamente no puede autenticarse.

### 5. Abstraccion de correo y plantilla dedicada
Se introducira una abstraccion de servicio de email para desacoplar el proveedor de envio y permitir pruebas. El correo utilizara una plantilla amable en espanol y un asunto claro.

Alternativas consideradas:
- Enviar correo directamente desde controlador o handler: descartado por acoplamiento y baja testabilidad.

### 6. Documentacion frontend en `docs/password-recovery-frontend.md`
Se creara una guia de integracion con la forma del link, campos esperados, validaciones UX recomendadas y payload del endpoint final.

Alternativas consideradas:
- Dejar la integracion solo implita en OpenSpec: descartado porque el usuario ha pedido un markdown especifico para frontend.

## Risks / Trade-offs

- [Proveedor de correo aun no definido] -> Mitigar con una abstraccion de envio y configuracion desacoplada del proveedor concreto.
- [Tokens en URL pueden quedar expuestos en logs o historiales] -> Mitigar codificando correctamente, minimizando datos adicionales y documentando buenas practicas en frontend.
- [Endpoint de recuperacion puede ser abusado] -> Mitigar con respuesta neutra y dejando espacio para rate limiting/captcha en una fase posterior.
- [Diferencias entre la URL esperada por frontend y la generada por backend] -> Mitigar documentando un contrato explicito y una configuracion unica de base URL del frontend.

## Migration Plan

- No requiere cambios de esquema.
- Incorporar configuracion para la URL publica del frontend y para el proveedor de correo.
- Desplegar backend junto con la pagina publica del frontend o, al menos, con una URL de destino ya acordada para no enviar enlaces rotos.
- Si fuera necesario rollback, bastara con retirar los endpoints y desactivar el envio de correos sin impacto sobre datos persistidos.

## Open Questions

- Confirmar proveedor real de correo saliente para produccion.
- Confirmar la ruta publica final del frontend, por ejemplo `/reset-password`.
- Confirmar si debe bloquearse la recuperacion para usuarios desactivados o si debe mantenerse la misma respuesta neutra sin enviar correo.
