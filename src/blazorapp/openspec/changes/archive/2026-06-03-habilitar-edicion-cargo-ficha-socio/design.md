## Context

La aplicación ya dispone de un catálogo administrativo de cargos consumible desde `GET /api/cargos`, y las fichas de socio (`MemberSelfProfileView` y `AdminMemberProfileView`) ya leen y guardan `CargoId` dentro del modelo `MemberSelfProfile`. Sin embargo, la UI actual muestra ese dato como un número editable, lo que rompe la experiencia de usuario y no respeta diferencias de permisos entre perfiles `Admin` y `Member`.

El cambio afecta a dos flujos relacionados pero distintos: la autoedición de ficha por un socio autenticado y la edición administrativa de fichas ajenas. Ambos comparten modelo, request de actualización y parte del mapeo de datos, por lo que conviene resolver el problema con un enfoque común y desacoplado dentro de `SharedUI`.

## Goals / Non-Goals

**Goals:**
- Mostrar el cargo con una etiqueta legible en lugar del identificador numérico.
- Permitir que un usuario autenticado con rol `Admin` seleccione un cargo válido desde el catálogo disponible.
- Mantener el campo visible pero no editable para usuarios `Member`, mostrando el nombre del cargo actual.
- Reutilizar la misma estrategia entre `MemberSelfProfileView` y `AdminMemberProfileView` para evitar divergencias funcionales.

**Non-Goals:**
- Rediseñar visualmente la ficha de socio más allá del cambio funcional del control de cargo.
- Cambiar el contrato de actualización de ficha si el backend ya acepta `CargoId`.
- Introducir CRUD de cargos dentro de la ficha; el catálogo sigue gestionándose en la pantalla administrativa de cargos.

## Decisions

### 1. Resolver el catálogo de cargos en la capa SharedUI mediante un contrato reutilizable
Se añadirá o reutilizará un servicio desacoplado accesible desde `SharedUI` para cargar el catálogo de cargos y exponer pares `Id/Description`.

Rationale: los componentes de ficha viven en `SharedUI` y no deben conocer detalles de implementación del cliente web. Usar un contrato compartido mantiene la arquitectura actual y permite reutilizar la misma lógica en futuros hosts.

Alternatives considered:
- Consumir directamente `CargoAdminApiClient` desde cada componente: descartado porque acopla la vista a una implementación concreta.
- Resolver el nombre del cargo solo con el `CargoLabel` ya devuelto por la ficha: insuficiente porque `Admin` necesita opciones editables desde catálogo.

### 2. Separar la representación editable y la de solo lectura según rol autenticado
El valor de cargo se renderizará como selector (`select`) solo para `Admin`. Para `Member`, el campo se mostrará como lectura usando el nombre del cargo y sin permitir cambios.

Rationale: el requisito funcional diferencia claramente entre capacidad de edición y visibilidad. Reutilizar el mismo espacio visual evita saltos en el formulario y hace explícito qué campo existe aunque no sea editable.

Alternatives considered:
- Ocultar el campo completamente a `Member`: descartado porque el usuario debe seguir viendo su cargo.
- Mantener un input deshabilitado con el ID: descartado porque no aporta contexto útil.

### 3. Mantener `CargoId` como valor persistido y usar descripción solo para presentación
La actualización de la ficha seguirá enviando `CargoId` en `UpdateMemberSelfProfileRequest`. La descripción del cargo se usará exclusivamente para renderizar la UI.

Rationale: evita cambios innecesarios en contratos, APIs y mapeadores ya operativos, concentrando el cambio en la experiencia y la selección.

Alternatives considered:
- Cambiar request/modelos para persistir también el nombre: descartado por duplicar estado y arriesgar inconsistencias.

### 4. Aplicar el mismo comportamiento en la ficha administrativa
Aunque la petición original nace en `MemberSelfProfileView`, el diseño incluye `AdminMemberProfileView` para que la edición administrativa de fichas no vuelva a exponer el `CargoId` numérico.

Rationale: ambos componentes editan el mismo recurso y deben compartir reglas de negocio y expectativas de uso.

## Risks / Trade-offs

- [El catálogo de cargos puede fallar al cargar] → Mitigación: mostrar estado controlado, mantener el valor actual legible si existe y evitar dejar un selector inconsistente.
- [El backend de ficha puede devolver `CargoId` sin `CargoLabel`] → Mitigación: resolver el nombre mediante el catálogo y usar fallback solo si no se puede encontrar coincidencia.
- [La distinción de roles puede no estar disponible hoy en el componente] → Mitigación: introducir una abstracción mínima de contexto/autorización reutilizable ya usada por el resto de vistas si existe, o documentar esa dependencia en implementación.
- [El usuario `Member` podría asumir que puede editar el cargo al verlo en formulario] → Mitigación: usar un control explícitamente de solo lectura y mantener consistencia visual con el resto de campos editables.
