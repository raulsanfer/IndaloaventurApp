## 1. Identity Persistence Setup

- [x] 1.1 Configurar `DbContext` y servicios de ASP.NET Identity sobre SQL Server usando `api_ContextConnection`.
- [x] 1.2 Crear y versionar la migracion EF Core para esquema de Identity (y ajustes de modelo asociados).
- [x] 1.3 Aplicar migraciones en entorno de desarrollo y verificar creacion correcta de tablas/indices.

## 2. Roles And Admin Bootstrap

- [x] 2.1 Implementar seeding idempotente de roles base requeridos por negocio (`Admin` y rol no-admin definido).
- [x] 2.2 Implementar seeding del usuario administrador por defecto con configuracion por entorno.
- [x] 2.3 Añadir pruebas de integracion para validar que bootstrap no duplica datos y respeta politicas de password.

## 3. Admin User Management API

- [x] 3.1 Diseñar comandos/queries y DTOs para alta, listado y edicion de usuarios gestionados.
- [x] 3.2 Exponer endpoints de gestion de usuarios y aplicar `Authorize` con restriccion de rol `Admin`.
- [x] 3.3 Añadir pruebas de autorizacion (admin permitido, no-admin prohibido, anonimo no autorizado).

## 4. Social Login Federation

- [x] 4.1 Integrar al menos un proveedor social configurable y la validacion de token externo.
- [x] 4.2 Implementar enlace/provisionamiento de usuario local tras autenticacion social exitosa.
- [x] 4.3 Emitir JWT local unificado para login social y agregar pruebas de exito/fallo.

## 5. Validation And Documentation

- [x] 5.1 Ejecutar la suite de pruebas automatizadas (`dotnet test`) y corregir regresiones.
- [x] 5.2 Validar manualmente escenarios clave de auth/autorizacion y registrar evidencia.
- [x] 5.3 Actualizar documentacion tecnica minima (configuracion social, seed admin, flujo de migraciones).
