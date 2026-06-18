## Why

Actualmente no existe una agenda telefonica centralizada en la API para gestionar fichas de contacto reutilizables por todo tipo de usuario. Este cambio se necesita ahora para cubrir un caso de uso operativo basico del club: administracion segura por rol `Admin` y consulta transversal para cualquier usuario autenticado.

## What Changes

- Incorporar un nuevo agregado de dominio `FichaContacto` con identificador `Guid`, `FechaAlta`, `Nombre`, `Telefono1`, `Telefono2` y `Observaciones`.
- Modelar los campos criticos con `ValueObject` y validaciones de dominio para proteger invariantes (por ejemplo nombre y telefonos).
- Implementar operaciones CRUD completas de Agenda Telefonica:
  - Crear ficha (solo `Admin`).
  - Consultar listado y detalle (cualquier usuario autenticado, sin restriccion de rol).
  - Editar ficha (solo `Admin`).
  - Eliminar ficha (solo `Admin`).
- Exponer endpoints HTTP siguiendo reglas de estilo del proyecto (contratos claros, validacion y respuestas consistentes).
- Añadir persistencia y mapeos necesarios para SQL Server dentro de la arquitectura actual.

## Capabilities

### New Capabilities
- `phonebook-contact-management`: Gestion de fichas de Agenda Telefonica con CRUD completo, restricciones por rol y validaciones de dominio.

### Modified Capabilities
- `jwt-identity-authentication`: Se añaden reglas explicitas de autorizacion por endpoint para permitir consulta a cualquier rol autenticado y restringir mutaciones al rol `Admin`.
- `repository-pattern-persistence`: Se amplian requisitos para persistir el nuevo agregado `FichaContacto` y soportar consultas/actualizaciones/eliminaciones desde la capa de repositorio.

## Impact

- Afecta `IndaloAventurApi.Domain` (nueva entidad, value objects y reglas de validacion).
- Afecta `IndaloAventurApi.Application` (comandos/consultas, handlers, DTOs y validaciones de entrada).
- Afecta `IndaloAventurApi.Infrastructure` (configuracion EF Core, repositorio y migracion).
- Afecta `IndaloAventurApi.Api` (nuevos endpoints y politicas de autorizacion).
- Afecta pruebas unitarias e integracion para cubrir invariantes de dominio, autorizacion y comportamiento CRUD.