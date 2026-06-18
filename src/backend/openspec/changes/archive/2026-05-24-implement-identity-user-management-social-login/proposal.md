## Why

La API ya dispone de autenticacion JWT, pero todavia no tiene una estrategia operativa para persistencia de Identity, migraciones evolutivas, gestion administrativa de usuarios ni federacion con proveedores sociales. Este cambio es necesario ahora para habilitar operaciones reales de alta/edicion de usuarios con control de acceso por roles y para alinear el backend con futuros cambios de esquema desde una base de datos real.

## What Changes

- Configurar ASP.NET Identity con EF Core y SQL Server usando la cadena `api_ContextConnection` de `appsettings.Development.json` para migraciones y actualizacion de esquema.
- Definir y aplicar migraciones a partir de ahora para cambios de modelo (incluyendo tablas de Identity y relaciones necesarias).
- Inicializar roles base y sembrar un usuario administrador por defecto en entorno de desarrollo con credenciales controladas por configuracion.
- Exponer endpoints de gestion de usuarios (alta, consulta y edicion) solo para administradores.
- Aplicar politicas de autorizacion para que usuarios no `Admin` no puedan acceder a endpoints de gestion.
- Extender el flujo de login para admitir autenticacion social mediante proveedores externos y emision de JWT propio tras validacion del proveedor.

## Capabilities

### New Capabilities
- `admin-user-management-api`: Gestion administrativa de usuarios (alta/edicion/listado) protegida por rol `Admin`.
- `social-login-federation`: Inicio de sesion mediante proveedores sociales externos con emision de JWT local.

### Modified Capabilities
- `jwt-identity-authentication`: Se amplian requisitos para bootstrap de roles/admin por defecto y politicas de autorizacion explicitas por endpoint.
- `repository-pattern-persistence`: Se amplian requisitos para estrategia de migraciones EF Core sobre SQL Server usando la conexion de desarrollo declarada.

## Impact

- Afecta `IndaloAventurApi.Api` (configuracion `Program.cs`, auth/authorization, endpoints y seed de inicio).
- Afecta `IndaloAventurApi.Infrastructure` (DbContext, mapeo Identity, migraciones, stores).
- Añade dependencias de proveedores OAuth/OIDC segun redes sociales soportadas.
- Introduce nuevas tablas/indices de Identity y datos semilla de roles/administrador.
- Cambia contrato de autenticacion al soportar login social ademas de credenciales locales.
