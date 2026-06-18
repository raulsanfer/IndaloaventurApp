## 1. Configuracion e integracion base

- [x] 1.1 Definir la configuracion frontend necesaria para inicializar Google Identity Services con el `Client ID` de Google
- [x] 1.2 Registrar la carga del recurso JavaScript requerido para Google Identity Services y preparar una superficie minima de JS interop reutilizable
- [x] 1.3 Documentar en el cambio la dependencia de correspondencia entre el `Client ID` frontend y `SocialAuth:GoogleAudience` del backend

## 2. Capa de autenticacion frontend

- [x] 2.1 Ampliar `IAuthService` con una operacion especifica de login social basada en proveedor y token
- [x] 2.2 Implementar en `AuthApiClient` el consumo de `POST /api/auth/social-login` reutilizando el mapeo actual hacia `AuthSession`
- [x] 2.3 Definir el modelado compartido necesario para requests y errores del login social sin romper el login clasico

## 3. Flujo de LoginView

- [x] 3.1 Conectar el boton social de Google de `LoginView` con el inicio del flujo real de autenticacion
- [x] 3.2 Obtener el `id_token` de Google desde JS interop y enviarlo al nuevo metodo de login social
- [x] 3.3 Reutilizar `SessionService.SetSession(...)` y `OnLoginSucceeded` tras un login social exitoso
- [x] 3.4 Gestionar estados de carga, cancelacion y error del login social manteniendo operativo el formulario clasico

## 4. Proveedores y experiencia de usuario

- [x] 4.1 Mantener Facebook fuera de alcance funcional sin inducir al usuario a pensar que el proveedor esta soportado
- [x] 4.2 Añadir o ajustar claves de localizacion necesarias para mensajes de disponibilidad, error y estados del login social
- [x] 4.3 Ajustar estilos globales solo si son necesarios para reflejar estados de deshabilitado/carga de los botones sociales

## 5. Verificacion

- [ ] 5.1 Verificar manualmente que el boton de Google inicia el flujo social y que un `id_token` valido autentica correctamente contra el API
- [x] 5.2 Verificar que una respuesta exitosa crea sesion local y navega igual que el login clasico
- [x] 5.3 Verificar que cancelacion, fallo de proveedor y rechazo del API muestran error controlado sin bloquear el login por credenciales
- [x] 5.4 Ejecutar build y pruebas relevantes del frontend y registrar la evidencia antes de marcar tareas como completadas
