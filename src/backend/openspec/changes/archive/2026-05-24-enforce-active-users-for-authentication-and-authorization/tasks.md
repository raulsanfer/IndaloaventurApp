## 1. Endurecer autenticacion por estado activo

- [x] 1.1 Revisar y ajustar login por credenciales para bloquear emision de JWT cuando `IsActive = false`
- [x] 1.2 Revisar y ajustar social login para aplicar la misma regla de usuario activo

## 2. Endurecer autorizacion en endpoints protegidos

- [x] 2.1 Incorporar validacion de usuario activo durante autorizacion de requests autenticadas
- [x] 2.2 Garantizar que usuarios desactivados con token previo no puedan acceder a endpoints protegidos

## 3. Pruebas y verificacion

- [x] 3.1 Actualizar pruebas unitarias de handlers/servicios de autenticacion con casos de usuario inactivo
- [x] 3.2 Actualizar pruebas de integracion para verificar bloqueo en autenticacion y autorizacion de usuarios inactivos
- [x] 3.3 Ejecutar `dotnet test IndaloAventurApi.sln` y dejar la suite en verde antes de marcar completado
