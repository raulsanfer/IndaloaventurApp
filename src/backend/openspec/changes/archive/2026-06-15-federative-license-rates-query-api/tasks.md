## 1. Contratos de lectura

- [x] 1.1 Definir la query y el DTO de lectura para listar tarifas federativas con filtro opcional por `Temporada`.
- [x] 1.2 Preparar la abstraccion de acceso a datos necesaria para recuperar el catalogo con orden determinista.

## 2. Endpoint autenticado

- [x] 2.1 Exponer el endpoint GET autenticado de tarifas federativas en `LicenciasFederativasController` sin restriccion por rol.
- [x] 2.2 Implementar el handler de consulta reutilizando la persistencia existente y devolviendo los campos funcionales requeridos.

## 3. Verificacion

- [x] 3.1 Crear pruebas automatizadas para acceso autenticado de cualquier rol, rechazo anonimo y filtrado por temporada.
- [x] 3.2 Ejecutar `dotnet test` y marcar las tareas solo cuando la verificacion sea satisfactoria.
