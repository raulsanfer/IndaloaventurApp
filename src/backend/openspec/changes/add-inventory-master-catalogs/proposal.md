## Why

El club necesita catalogos base y controlados para poder clasificar y gestionar el futuro material deportivo sin dispersar valores libres ni introducir jerarquias inconsistentes. Se incorporan ahora los maestros de categorias y estados como fundamento reutilizable del dominio de inventario.

## What Changes

- Incorporar el maestro `InventarioCategorias`, con categorias y subcategorias en una jerarquia de exactamente dos niveles como maximo.
- Incorporar el maestro plano `InventarioEstados`.
- Exponer operaciones de alta, consulta, modificacion y borrado para ambos maestros, con lectura para cualquier usuario autenticado y escritura solo para el rol tecnico `Admin`.
- Definir el flujo completo Domain, Application (CQRS), Infrastructure (EF Core/Dapper) y API, junto con sus contratos HTTP, autorizacion, tratamiento de errores y cobertura automatizada.
- Crear una migracion de EF Core que persista ambos maestros, sus restricciones de integridad e indices sin afectar datos existentes.

## Capabilities

### New Capabilities

- `inventory-category-management`: Catalogo jerarquico de categorias y subcategorias de inventario administrado por usuarios `Admin`.
- `inventory-status-management`: Catalogo plano de estados de inventario administrado por usuarios `Admin`.

### Modified Capabilities

Ninguna.

## Impact

- Domain: nuevos agregados `InventarioCategoria` e `InventarioEstado` bajo el espacio de nombres `Inventario` y sus contratos de persistencia.
- Application: comandos, consultas, validadores y DTOs CQRS para ambos maestros.
- Infrastructure: configuraciones EF Core, repositorios de escritura, proyecciones Dapper, registros de DI y una migracion SQL Server.
- API: controladores, contratos de solicitud/respuesta y matriz de autorizacion para `api/inventario-categorias` y `api/inventario-estados`.
- Tests: pruebas unitarias, de aplicacion e integracion de jerarquia, integridad, autorizacion y contratos HTTP.
