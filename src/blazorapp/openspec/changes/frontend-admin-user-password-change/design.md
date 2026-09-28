## Context

La ficha administrativa obtiene tanto la ficha de socio como el usuario administrado mediante `IAdminUserManagementService`. El cliente ya concentra operaciones administrativas de usuario en esa abstracción y utiliza componentes SharedUI, recursos localizados y estilos SCSS globales. El backend está fuera de este proyecto, por lo que el contrato debe quedar documentado antes de implementarlo allí.

## Goals / Non-Goals

**Goals:**

- Añadir una ruta y componente reutilizable para el cambio administrativo de contraseña.
- Mantener la validación de forma y longitud en cliente, y repetirla obligatoriamente en servidor.
- Incorporar el contrato HTTP en el cliente y una especificación inequívoca para el backend.

**Non-Goals:**

- No crear algoritmos de hash, cifrado ni almacenamiento de contraseñas en el frontend.
- No sustituir el flujo público de recuperación de contraseña ni añadir confirmación de contraseña.
- No implementar el endpoint ni la persistencia del backend desde este proyecto.

## Decisions

- Se añadirá `PUT /api/users/{userId}/password` al cliente administrativo, con un cuerpo limitado a `newPassword`. Un endpoint dedicado conserva la separación de responsabilidades y evita que una actualización de perfil acepte secretos accidentalmente.
- La pantalla volverá a consultar la cuenta objetivo por `userId` para renderizar un nombre legible, en vez de propagarlo por URL o estado de navegador manipulable.
- El formulario usará DataAnnotations para requerir una cadena alfanumérica de 1 a 20 caracteres y `maxlength` en el input. La restricción de cliente mejora la experiencia; el contrato de backend exige la validación autoritativa porque el cliente no es una frontera de confianza.
- La abstracción existente de gestión de usuarios recibirá la operación para mantener todos los accesos administrativos en el mismo cliente HTTP. La alternativa, reutilizar `IAuthService`, se descarta porque este flujo no es una recuperación pública ni posee token de recuperación.
- El resultado mostrado será genérico y el valor se conservará únicamente el tiempo imprescindible para construir la petición. No se registrará ni se incluirá en navegación, mensajes o excepciones visibles.

## Risks / Trade-offs

- [La regla alfanumérica limita la variedad de contraseñas frente a recomendaciones actuales de usabilidad] → se aplica por requisito funcional, se documenta y el backend deberá complementar con los controles de identidad, limitación de tasa y rotación de sesiones.
- [El endpoint aún no existe en el backend] → el cliente maneja de forma segura respuestas no exitosas y `.vscode` deja el contrato completo para su implementación y pruebas de autorización.
- [Una sesión robada de Admin podría abusar de la operación] → autorización server-side, auditoría, rate limiting y, cuando el proveedor lo permita, reautenticación reciente y revocación de sesiones del objetivo.

## Migration Plan

1. Desplegar el endpoint protegido y sus pruebas de autorización/validación en el backend.
2. Desplegar el frontend que lo consume; hasta entonces, el flujo mostrará un error controlado ante una respuesta no disponible.
3. Si se requiere reversión, retirar el enlace y la ruta del frontend; las credenciales ya modificadas no deben revertirse automáticamente.
