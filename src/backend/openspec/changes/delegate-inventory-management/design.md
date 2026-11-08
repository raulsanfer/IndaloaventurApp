## Context

Véase `proposal.md` y `specs/inventory-management-delegation/spec.md`. La identidad se apoya en ASP.NET Identity: la gestión de usuarios ya acepta varios roles, las respuestas de login emiten los roles en JWT y toda petición protegida valida el *security stamp* actual. Las escrituras de categorías, estados y materiales, y las rutas administrativas de reservas, están hoy limitadas a la política `Admin`.

Las rutas administrativas de reservas tienen una segunda comprobación de aplicación: pasan un booleano `IsAdmin` a comandos y consultas que usan `ReservaMaterialAdminAccess`. Por tanto, cambiar solo los atributos HTTP no permitiría operar al delegado. Los cambios OpenSpec de inventario siguen activos, de modo que esta implementación debe coordinarse con ellos y actualizar su autorización al integrar el código.

## Goals / Non-Goals

**Goals:**

- Reutilizar el modelo de roles, el perfil de usuario y la emisión de JWT ya existentes para delegar exclusivamente la gestión de inventario.
- Mantener una única regla de acceso equivalente en la política HTTP y en los guardas de Application de reservas.
- Hacer efectivos de inmediato los cambios de rol o de condición de socio invalidando sesiones ya emitidas.
- Mantener trazable la autorización en la matriz de la API y protegida por pruebas negativas.

**Non-Goals:**

- No crear una columna de permiso, un claim de autorización nuevo ni una pantalla o endpoint específico para delegar inventario.
- No cambiar el significado de `Member` ni de `IsMember`, ni exigir que los administradores sean socios.
- No conceder al delegado gestión de usuarios, fichas, licencias, señales u otros módulos administrativos.
- No incorporar firma digital; las confirmaciones administrativas actuales de devolución son el alcance de reservas de este cambio.

## Decisions

### Usar el rol `InventoryManager`, no una casilla de perfil ni un claim nuevo

Se añadirá `InventoryManager` al catálogo de roles de Identity y se aprovisionará al arrancar junto con `Admin` y `Member`. La lista `Roles` que ya se recibe y devuelve desde la gestión de usuarios será la única interfaz de asignación: un administrador podrá marcarlo en el cliente que consuma esa lista, sin cambios de contrato ni migración de base de datos.

Se descarta una nueva propiedad booleana de usuario porque exigiría migración, propagación a DTOs, contratos, JWT y validaciones, al tiempo que modelaría otro permiso discreto con la misma semántica de un rol. También se descarta un claim de Identity independiente: requeriría administración y emisión propias y duplicaría la fuente de autorización ya consolidada en roles.

### Política única de gestión de inventario

Se incorporará una política `InventoryManagement` cuya expresión sea `Admin OR (InventoryManager AND IsMember=true)`. El `Admin` queda como alternativa completa para no alterar el acceso existente del administrador semilla, que puede no ser socio. La política se implementará como una aserción centralizada sobre los claims del principal, pues combinar `RequireRole` y `RequireClaim` exigiría erróneamente ambas condiciones también a los administradores.

Los atributos de autorización de las escrituras de categorías, estados y materiales, y de cada ruta `/api/inventario-reservas/admin`, pasarán a esa política. Las lecturas de catálogos y materiales seguirán siendo `Authenticated`; las rutas `/me` seguirán usando `ClubMember` y la identidad del token. La matriz ganará una clasificación explícita de gestión de inventario y registrará la política compuesta para cada ruta afectada.

### Defensa en profundidad para reservas administrativas

Se reemplazará el contexto booleano y el guarda denominados `IsAdmin`/`RequireAdmin` de comandos, consultas y handlers de reservas por un contexto de capacidad `CanManageInventory` y un guarda con el mismo significado. El controlador lo derivará de una extensión de `ClaimsPrincipal` que replique exactamente la regla de la política. Así los handlers siguen siendo seguros ante invocaciones que no pasan por el controlador y los delegados autorizados no se bloquean tras superar la política HTTP.

Se descarta eliminar este guarda de Application y confiar solo en `[Authorize]`: las pruebas y otros futuros adaptadores pueden invocar los handlers sin atravesar la capa HTTP.

### Revocación por security stamp

Cuando la actualización administrada cambie la colección de roles o el valor `IsMember`, y cuando el flujo de ficha de socio cambie `IsMember`, Identity actualizará el *security stamp* después de completar con éxito la mutación. La validación de JWT existente detectará el valor anterior y rechazará el token; el usuario deberá iniciar sesión de nuevo para recibir los claims actualizados.

No se consultará la base de datos en cada autorización para recalcular los roles o `IsMember`, porque ya existe una validación de *security stamp* por petición que proporciona revocación inmediata sin añadir una segunda ruta de acceso ni cambiar el contrato de JWT.

## Risks / Trade-offs

- [La actualización de roles y de perfil no es una transacción de dominio única] → Actualizar el *security stamp* solamente tras el resultado correcto y conservar los errores de Identity; las pruebas cubrirán concesión y retirada completas.
- [Los cambios activos de inventario aún documentan acceso exclusivo de `Admin`] → Coordinar su integración, sustituir sus atributos y guardas y reflejar la política transversal antes de archivarlos.
- [Un cliente puede conservar temporalmente un token ya emitido] → Rechazarlo por *security stamp* y requerir una nueva autenticación, en vez de confiar en su caducidad de una hora.
- [La expresión de política se duplica entre API y Application] → Centralizar el cálculo del principal en una extensión y pasar solo la capacidad resultante a Application; mantener pruebas para ambos límites.

## Migration Plan

1. Desplegar el código que aprovisiona el rol y registra la política; no requiere migración de esquema porque Identity ya persiste roles.
2. Aplicar los cambios de controladores, guardas de reservas, matriz y revocación de *security stamp* junto con la integración de los módulos de inventario activos.
3. Ejecutar las pruebas unitarias e integración de autorización y verificar que un administrador mantiene acceso y que un delegado socio puede completar cada operación administrativa.
4. Para revertir, retirar la política de los controladores y volver a `Admin`; las filas existentes de `InventoryManager` en Identity pueden permanecer inertes o eliminarse mediante una operación administrativa planificada, sin pérdida de datos de inventario.
