## 1. Credenciales y revocación de sesiones

- [x] 1.1 Vincular los JWT al sello de seguridad actual de Identity y comprobarlo al autorizar solicitudes protegidas; verificar con pruebas de autenticación y compilación.
- [x] 1.2 Añadir la operación de Identity y el comando de aplicación para restablecer una contraseña administrativamente sin exponer errores del proveedor; verificar con pruebas unitarias del manejador.

## 2. API segura de administración

- [x] 2.1 Exponer `PUT /api/users/{userId}/password` con DTO de cuerpo estricto, validación de contraseña y respuestas HTTP genéricas; verificar con pruebas de integración de éxito, 400 y 404.
- [x] 2.2 Exigir autorización Admin con reautenticación reciente, limitar por actor y objetivo, y registrar auditoría estructurada sin secretos; verificar con pruebas de integración de 401, 403, 429 y logs.

## 3. Verificación integral

- [x] 3.1 Añadir o actualizar pruebas de integración para el cambio propio y de terceros, la conservación de contraseña ante errores y la invalidación del JWT previo; verificar ejecutando `dotnet test` para la solución.
- [x] 3.2 Validar el cambio OpenSpec y ejecutar los proyectos de pruebas afectados; verificar con `openspec validate admin-user-password-change --strict` y los comandos de prueba sin fallos.
