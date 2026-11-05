## Why

Tras disponer de los catalogos de categorias y estados, el club necesita registrar y mantener el material de montana que posee. Sin un inventario centralizado no es posible conocer con fiabilidad la cantidad, procedencia, condicion y clasificacion del equipamiento.

## What Changes

- Incorporar el agregado de material inventariable con su ciclo CRUD completo.
- Registrar descripcion, marca/modelo, cantidad, categoria, estado, fecha de adquisicion, procedencia de adquisicion o donacion, observaciones y numero de serie.
- Exigir categorias y estados existentes como referencias del material y preservar su integridad en base de datos.
- Exponer listado y detalle para cualquier usuario autenticado, y altas, modificaciones y borrados exclusivamente al rol tecnico `Admin`.
- Mantener el flujo arquitectonico vigente: dominio, CQRS con MediatR y FluentValidation, escritura EF Core, consultas Dapper, ProblemDetails en Espanol y pruebas automatizadas.
- Crear la migracion de SQL Server y actualizar la matriz de autorizacion de la API.

## Capabilities

### New Capabilities

- `inventory-item-management`: Gestion CRUD de material de inventario de montana clasificado por los catalogos de categorias y estados.

### Modified Capabilities

Ninguna.

## Impact

- Domain: nuevo agregado `InventarioMaterial` y sus reglas de invariantes bajo el modulo `Inventario`.
- Application: comandos, consultas, DTOs, validadores y abstracciones de persistencia y lectura para el material.
- Infrastructure: mapeo EF Core, repositorio de comandos, proyecciones Dapper, registros de DI y migracion de la tabla de material con sus claves foraneas.
- API: controlador y contratos HTTP bajo `api/inventario-materiales`, junto con la matriz de autorizacion.
- Tests: cobertura de dominio, aplicacion, infraestructura e integracion HTTP para CRUD, referencias de maestros, reglas de cantidad y autorizacion.
