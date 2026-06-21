## 1. Inventario y reglas de acceso

- [x] 1.1 Levantar y versionar la matriz de acceso de todos los endpoints HTTP, clasificando cada ruta como `Anonymous`, `Authenticated`, `Admin`, `OwnerOrAdmin` o `ClubMember`
- [x] 1.2 Revisar controladores y handlers para alinear cada endpoint con su clasificacion esperada, resolviendo de forma explicita donde aplica rol tecnico `Member` frente a claim `IsMember`
- [x] 1.3 Introducir politicas o helpers centralizados para reglas recurrentes de autorizacion donde hoy existan comprobaciones dispersas

## 2. Endurecimiento de autenticacion

- [x] 2.1 Configurar proteccion frente a intentos repetidos de login con password usando mecanismos nativos de ASP.NET Identity
- [x] 2.2 Hacer explicita y probada la validacion JWT de firma, emisor, audiencia, caducidad y estado activo del usuario en endpoints protegidos
- [x] 2.3 Verificar que los rechazos de autenticacion y autorizacion mantienen semantica HTTP correcta y mensajes user-facing en espanol

## 3. Pruebas de seguridad y autorizacion

- [x] 3.1 Anadir pruebas de integracion para rechazo anonimo en los grupos de endpoints protegidos del backend
- [x] 3.2 Anadir pruebas de integracion para rol insuficiente, acceso cruzado a recursos de terceros y diferencias entre `Member` e `IsMember` donde aplique
- [x] 3.3 Anadir o ajustar pruebas unitarias de autenticacion para lockout temporal y revocacion efectiva de usuarios desactivados

## 4. Verificacion

- [x] 4.1 Ejecutar `dotnet test IndaloAventurApi.sln` y confirmar que los escenarios de seguridad y autorizacion quedan cubiertos antes de marcar tareas como completadas
