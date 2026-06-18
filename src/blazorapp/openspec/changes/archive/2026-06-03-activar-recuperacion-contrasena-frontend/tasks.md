## 1. Rutas y servicios de autenticación

- [x] 1.1 Crear la navegación pública desde login hacia la pantalla `He olvidado mi contraseña`
- [x] 1.2 Añadir en el cliente/auth service las operaciones necesarias para `POST /api/auth/passrecovery` y `POST /api/auth/reset-password`
- [x] 1.3 Definir los modelos de request/response y el manejo de errores necesarios para ambos pasos del flujo

## 2. UI pública de recuperación y reseteo

- [x] 2.1 Implementar la página pública de solicitud de recuperación con formulario de email, validación básica y mensaje neutro de confirmación
- [x] 2.2 Implementar la página pública de restablecimiento leyendo `email` y `token` desde la URL, con campos de nueva contraseña y confirmación
- [x] 2.3 Mostrar errores backend en la confirmación del reset y ofrecer una acción clara para reiniciar el flujo cuando el token sea inválido o haya expirado
- [x] 2.4 Redirigir al login tras un reset correcto mostrando una confirmación visible para el usuario

## 3. Estilo, recursos y validación

- [x] 3.1 Añadir literales localizados ES y estilos SCSS globales necesarios para mantener la coherencia visual del flujo
- [x] 3.2 Verificar que ambas páginas sean públicas, no persistan el token y respeten el contrato de query string definido por backend
- [x] 3.3 Añadir o actualizar tests de componentes/servicios y ejecutar la validación del frontend correspondiente
