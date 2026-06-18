## 1. Modelo de dominio y persistencia

- [x] 1.1 Anadir la propiedad booleana `IsMember` en la entidad `Usuario` y ajustar constructores/defaults para nuevos usuarios.
- [x] 1.2 Crear migracion EF Core para persistir `IsMember` como columna no nula con valor por defecto `false`.
- [x] 1.3 Verificar que el mapeo de Identity/EF Core refleja correctamente el nuevo campo en lectura y escritura.

## 2. Flujos de aplicacion y API

- [x] 2.1 Actualizar comandos de administracion de usuario para permitir edicion de `IsMember`.
- [x] 2.2 Actualizar queries/DTOs de usuario para incluir `IsMember` en las respuestas.
- [x] 2.3 Revisar validaciones/autorizaciones para asegurar que solo administradores puedan modificar `IsMember`.

## 3. Verificacion

- [x] 3.1 Anadir o ajustar pruebas de aplicacion/integracion para escenarios de actualizacion y consulta de `IsMember`.
- [x] 3.2 Ejecutar la suite de pruebas del backend y corregir regresiones.
