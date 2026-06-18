# Integracion Frontend: Recuperacion De Contrasena

## Objetivo

El backend expondra un flujo de recuperacion de contrasena basado en email y token seguro de ASP.NET Identity. El frontend debe implementar una pagina publica para que el usuario pueda definir una nueva contrasena desde el enlace recibido por correo.

## Flujo Esperado

1. El usuario informa su email en la pantalla "He olvidado mi contrasena".
2. El frontend llama al endpoint de recuperacion del backend.
3. El backend responde siempre de forma neutra y, si la cuenta existe, envia un correo con un enlace seguro al frontend.
4. El usuario abre el enlace del correo.
5. El frontend renderiza una pagina publica de reseteo con los campos de nueva contrasena y confirmacion.
6. El frontend envia email, token y nueva contrasena al endpoint final del backend.

## Endpoint 1: Solicitar Recuperacion

Metodo sugerido: `POST /api/auth/passrecovery`

Payload sugerido:

```json
{
  "email": "usuario@dominio.com"
}
```

Respuesta esperada:

- `200 OK` con mensaje neutro en espanol.
- El frontend debe mostrar siempre un mensaje tipo: "Si existe una cuenta asociada al email indicado, te hemos enviado instrucciones para recuperar tu contrasena."

## Enlace Del Correo

El backend generara una URL publica del frontend con este formato orientativo:

```text
https://frontend.example.com/reset-password?email=<email-url-encoded>&token=<token-url-encoded>
```

El frontend debe:

- Leer `email` y `token` desde la URL.
- No modificar ni decodificar de forma incompatible esos valores antes de enviarlos al backend.
- Mantener la pagina accesible sin autenticacion previa.

## Pantalla Publica De Reseteo

Campos recomendados:

- `Nueva contrasena`
- `Confirmar nueva contrasena`

Validaciones UX recomendadas:

- ambos campos obligatorios
- confirmacion debe coincidir
- mostrar ayuda visual sobre politica minima de contrasena si backend la expone en texto estatico

## Endpoint 2: Confirmar Nueva Contrasena

Metodo sugerido: `POST /api/auth/reset-password`

Payload sugerido:

```json
{
  "email": "usuario@dominio.com",
  "token": "<token-recibido-en-el-link>",
  "newPassword": "NuevaPass123",
  "confirmPassword": "NuevaPass123"
}
```

Comportamiento esperado:

- `200 OK` si el reseteo se completa correctamente.
- `400 BadRequest` o `409 Conflict` si el token es invalido/expirado o la contrasena no cumple la politica.
- El frontend debe mostrar los mensajes de error devueltos por backend sin traducirlos.

## Recomendaciones De UX

- Informar siempre una respuesta neutra tras solicitar la recuperacion.
- Si el token no es valido o ha expirado, ofrecer volver a iniciar el flujo desde "He olvidado mi contrasena".
- Tras un reseteo correcto, redirigir a login y mostrar un mensaje de confirmacion.

## Consideraciones De Seguridad

- No guardar el token en almacenamiento persistente del navegador salvo que sea estrictamente necesario.
- Evitar logs client-side con el token completo.
- No exigir sesion iniciada para esta pagina.
