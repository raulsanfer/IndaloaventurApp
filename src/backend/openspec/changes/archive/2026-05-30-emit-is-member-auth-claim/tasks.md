## 1. Contrato de autenticacion

- [x] 1.1 Extender los contratos de identidad y JWT para transportar `IsMember` desde `Usuario` hasta la emision del token.
- [x] 1.2 Actualizar los handlers y `LoginResponse` para devolver `IsMember` en login clasico y social.

## 2. Emision y verificacion

- [x] 2.1 Emitir el claim `IsMember` en el JWT con formato booleano consistente y documentarlo mediante el contrato API actual.
- [x] 2.2 Agregar o ajustar pruebas de aplicacion e integracion que validen `LoginResponse.IsMember` y el claim emitido.

## 3. Verificacion final

- [x] 3.1 Ejecutar `dotnet test` y corregir regresiones antes de marcar las tareas completadas.
