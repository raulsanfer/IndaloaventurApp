## Context

`UsersController.List` recibe actualmente `email` y envía `ListManagedUsersQuery` a su handler. Este normaliza los espacios extremos y delega en `IIdentityService.ListUsersAsync`. La implementación filtra por contenido en `Usuario.NormalizedEmail`, ordena por email y devuelve `ManagedUserDto(UserId, Email, IsMember, Roles)`.

`Usuario` no almacena nombre ni apellidos. Esos campos están en `FichaSocio`, accesible mediante `ApplicationDbContext.FichasSocio`, con índice único en `UserId`. Una cuenta puede existir sin ficha. El host de integración utiliza SQLite y la persistencia de producción utiliza SQL Server.

## Goals / Non-Goals

**Goals:**
- Buscar un fragmento literal en cualquier posición del nombre o de los apellidos de la ficha vinculada.
- Ofrecer comparación sin distinción de mayúsculas/minúsculas y normalización de espacios extremos.
- Permitir usar `email`, `nombre` o ambos, conservando la compatibilidad del endpoint.
- Mantener la ejecución del filtro en base de datos y cubrir el comportamiento mediante pruebas.

**Non-Goals:**
- Duplicar datos personales en Identity o modificar el ciclo de vida de las fichas.
- Condicionar la búsqueda a `IsMember` o al rol técnico `Member`.
- Cambiar la respuesta, implementar la interfaz de administración o añadir otro endpoint.
- Resolver errores tipográficos, dividir el texto en palabras, concatenar los campos o definir equivalencia entre tildes.

## Decisions

### 1. Extender el endpoint con `nombre` opcional

El contrato será `GET /api/users?email=<fragmento>&nombre=<fragmento>`. Ambos parámetros serán opcionales e independientes. El nombre público `nombre` sigue la terminología de los datos personales existentes; en Application se usará una propiedad `Nombre` opcional, manteniendo `Email` como primer argumento de la query para minimizar cambios en sus consumidores.

Se descarta reutilizar `email` como búsqueda general porque modificaría el significado del filtro existente. Se descarta otro endpoint porque el resultado sigue siendo el listado administrativo filtrado.

### 2. Buscar en los campos de la ficha con OR y combinar filtros con AND

Con `nombre` activo, la condición será: existe una ficha cuyo `UserId` coincide con `Usuario.Id` y cuyo `Nombre` contiene el texto o cuyos `Apellidos` contienen el texto. Si además hay `email`, también deberá coincidir con el email de Identity, conservando su semántica actual.

El filtro se trata como una única subcadena literal. Por ejemplo, `arc` coincide con `García Montes`, `ont` coincide con `Montes` y `ari` coincide con `Ana Marina`. Una entrada como `Marina García` no implica buscar a través de los límites entre `Nombre` y `Apellidos`.

Las cuentas sin ficha solo quedan excluidas cuando hay un filtro `nombre` efectivo. No se añadirá una condición por `IsMember`: la búsqueda usa los datos de la ficha cuando existe.

### 3. Normalizar el filtro antes de consultar

El handler recortará espacios extremos de ambos filtros y convertirá nulos, cadenas vacías y cadenas con solo espacios en ausencia de filtro. Infrastructure aplicará una comparación sin distinción de mayúsculas mediante normalización de los campos y del término a la misma caja, con operaciones que EF Core traduzca a SQL Server y SQLite. Los espacios internos permanecerán literales.

La consulta será parametrizada. Se utilizará una operación de contenido literal traducible por EF Core, evitando que `%`, `_` o `[` amplíen los resultados como comodines de SQL. La equivalencia de tildes queda sujeta a la comparación del proveedor; no forma parte del nuevo contrato.

### 4. Reutilizar la infraestructura de Identity y la ficha existente

Se extenderá `IIdentityService.ListUsersAsync` para aceptar `nombre` junto con `email`, y se inyectará `ApplicationDbContext` en `IdentityService` para construir una subconsulta correlacionada sobre `FichasSocio` con `Any`/`EXISTS`. Se aplicarán los filtros antes de `OrderBy` y `ToListAsync`, preservando el token de cancelación.

La subconsulta evita duplicar usuarios y evita cargar todas las fichas en memoria. Se conserva la obtención de roles y la proyección a `ManagedUserDto` existentes. Los cambios de firma requieren actualizar los dobles de identidad en las pruebas de usuarios, autenticación y fichas de socio.

### 5. Mantener el contrato y la política de acceso

El listado seguirá restringido por `AuthorizationPolicies.Admin`, ordenado por email ascendente, con los mismos campos de respuesta. Una búsqueda sin coincidencias devolverá `200 OK` con `[]`. La documentación XML del controlador describirá ambos filtros, la búsqueda parcial y su combinación.

## Risks / Trade-offs

- **Cobertura limitada a cuentas con ficha:** es consecuencia del modelo actual. La búsqueda por email conserva acceso a las demás cuentas; no se crean fichas automáticamente.
- **Coste de la búsqueda por contenido:** la normalización y las coincidencias interiores pueden requerir recorrer texto. Se mantiene el filtrado en base de datos usando la relación indexada por `UserId`. Paginación e índices especializados quedan fuera de este cambio.
- **Diferencias entre proveedores:** las pruebas HTTP con SQLite cubren el contrato, pero no prueban por sí solas la traducción o comparación en SQL Server. El host SQLite de pruebas registra `upper` con `ToUpperInvariant` para convertir también letras acentuadas, ya que SQLite solo convierte ASCII por defecto. Una prueba adicional en SQL Server comprueba el filtro real, incluyendo nombres con tildes, antes de dar por verificado ese punto.
- **Cambio transversal de firma:** compilar la solución y actualizar todos los dobles de `IIdentityService` evita dejar proyectos de pruebas incompatibles.

## Migration Plan

1. Extender el contrato de consulta, el handler y el servicio de identidad.
2. Añadir el filtro sobre `FichasSocio` y actualizar la documentación del endpoint.
3. Completar las pruebas descritas en `tasks.md` y ejecutar la suite.
4. Desplegar el backend sin migraciones. Los clientes pueden empezar a enviar `nombre` tras el despliegue.

Rollback: revertir los cambios de código del filtro `nombre`; no existen cambios de esquema o de datos que revertir.

## Open Questions

Ninguna bloqueante. Esta propuesta utiliza `FichaSocio` como fuente de nombres y apellidos, de acuerdo con el modelo actual. Una futura búsqueda de nombres de cuentas sin ficha requeriría definir dónde se guardan esos datos.
