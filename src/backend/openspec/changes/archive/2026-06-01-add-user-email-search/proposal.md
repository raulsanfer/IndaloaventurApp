## Why

Actualmente la gestion administrativa de usuarios solo permite recuperar el listado completo de cuentas, ordenado por email, sin ningun criterio de filtrado. A medida que crece el volumen de usuarios, localizar una cuenta concreta por correo se vuelve lento e innecesariamente manual.

## What Changes

- Anadir una opcion de busqueda de usuarios por email en el flujo de listado administrativo.
- Mantener el comportamiento actual de devolver todos los usuarios cuando no se informe filtro.
- Definir el comportamiento del filtro para que sea tolerante a mayusculas/minusculas y a espacios accidentales.
- Incorporar pruebas para el filtrado y para el caso sin coincidencias.

## Capabilities

### New Capabilities
Ninguna.

### Modified Capabilities
- `admin-user-management-api`: listado de usuarios administrativos con filtro opcional por email.

## Impact

- API: `GET /api/users` aceptara un criterio opcional de email.
- Application: la query de listado de usuarios transportara el filtro al servicio de identidad.
- Infrastructure: la consulta a Identity aplicara filtrado por email conservando orden ascendente.
- Tests: se ampliara cobertura para filtrado por coincidencia y lista vacia.
