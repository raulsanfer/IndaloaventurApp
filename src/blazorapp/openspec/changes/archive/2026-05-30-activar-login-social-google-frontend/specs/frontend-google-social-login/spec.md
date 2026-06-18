## ADDED Requirements

### Requirement: LoginView MUST permitir autenticacion social con Google
El sistema MUST ofrecer desde `LoginView` un flujo operativo de autenticacion con Google vinculado al boton social de Google existente.

#### Scenario: Inicio de login social desde el boton de Google
- **WHEN** el usuario pulsa el boton social de Google en `LoginView`
- **THEN** el sistema MUST iniciar el flujo de autenticacion social con Google
- **AND** el sistema MUST no alterar el comportamiento del formulario de login clasico

### Requirement: El frontend MUST obtener un id_token de Google compatible con el API
El sistema MUST obtener un `id_token` de Google emitido para el `Client ID` configurado en frontend y usarlo como entrada del login social con el API.

#### Scenario: Token de Google obtenido correctamente
- **WHEN** Google completa correctamente la autenticacion del usuario
- **THEN** el frontend MUST recibir un `id_token`
- **AND** el frontend MUST tratar ese `id_token` como el valor de `token` para el login social contra el API

### Requirement: El frontend MUST consumir /api/auth/social-login para Google
El sistema MUST enviar el resultado de autenticacion de Google al endpoint `/api/auth/social-login` usando el contrato esperado por backend.

#### Scenario: Llamada correcta al endpoint social
- **WHEN** el frontend dispone de un `id_token` de Google valido
- **THEN** el sistema MUST invocar `/api/auth/social-login`
- **AND** el payload MUST incluir `provider = "google"`
- **AND** el payload MUST incluir `token = <id_token>`

### Requirement: El login social exitoso MUST crear una sesion local equivalente al login clasico
El sistema MUST convertir una respuesta exitosa del login social en una sesion autenticada local reutilizando el pipeline actual de sesion del frontend.

#### Scenario: Sesion creada tras login social exitoso
- **WHEN** `/api/auth/social-login` devuelve una respuesta de autenticacion valida
- **THEN** el frontend MUST construir `AuthSession` con el token recibido
- **AND** el frontend MUST persistir la sesion mediante `ISessionService`
- **AND** el frontend MUST ejecutar el mismo flujo de continuacion que el login clasico tras autenticar

### Requirement: El login social MUST informar estados de error y disponibilidad sin romper el login clasico
El sistema MUST manejar errores de proveedor, cancelacion o rechazo del API sin dejar la pantalla de login en un estado inconsistente.

#### Scenario: Error al obtener o validar el token social
- **WHEN** Google no devuelve un token valido o el API rechaza el login social
- **THEN** el frontend MUST mostrar un estado de error comprensible para el usuario
- **AND** el frontend MUST permitir volver a intentar el login social o usar el login clasico

#### Scenario: Cancelacion del flujo social
- **WHEN** el usuario cancela o cierra el flujo de Google antes de completarlo
- **THEN** el frontend MUST finalizar el intento sin crear sesion
- **AND** el frontend MUST mantener operativa la pantalla de login

### Requirement: El frontend MUST dejar Facebook fuera de alcance funcional en esta iteracion
El sistema MUST evitar presentar Facebook como un proveedor autenticable real mientras no exista soporte equivalente de frontend y backend.

#### Scenario: Render del proveedor no soportado
- **WHEN** `LoginView` muestra las acciones sociales
- **THEN** el sistema MUST mantener Google como unico proveedor social operativo
- **AND** el sistema MUST no iniciar un flujo real de autenticacion con Facebook
