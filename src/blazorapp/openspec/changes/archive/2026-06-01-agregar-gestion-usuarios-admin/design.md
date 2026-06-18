## Context

El proyecto ya dispone de una página `Configuración` restringida visualmente por rol `Admin` y de una herramienta administrativa para `Cargos`. También existe una página de `Ficha de Socio` para autoservicio del propio socio, apoyada en `/api/fichas-socio/me`. El `endpoints.json` actual expone, además, endpoints administrativos relevantes:

- `GET /api/users` que devuelve `ManagedUserDto` con `userId`, `email`, `isMember` y `roles`.
- `GET/POST/PUT /api/fichas-socio/{userId}` para leer, crear y actualizar la ficha de un usuario concreto.

La funcionalidad pedida requiere unir ambos mundos: encontrar un usuario por email, distinguir si ya es socio y, en función de ello, redirigir a una edición administrativa o crear su ficha automáticamente antes de redirigir. El punto más ambiguo del contrato actual es que `GET /api/users` no documenta parámetros de búsqueda en `endpoints.json`, aunque el flujo sí necesita buscar por email.

## Goals / Non-Goals

**Goals:**
- Añadir la opción `Usuarios` al bloque `Administración` de `Configuración`.
- Permitir a un administrador buscar usuarios por email y ver el resultado en un listado claro.
- Habilitar el flujo `Editar` para usuarios ya socios y `Crear Ficha` para usuarios aún no socios.
- Crear una pantalla administrativa de ficha de socio para editar a terceros mediante `userId`.
- Mantener visual y estructuralmente la coherencia con el resto del área administrativa.

**Non-Goals:**
- Crear un sistema completo de paginación o gestión masiva de usuarios.
- Editar roles o el estado `IsMember` desde esta primera iteración salvo lo que derive implícitamente de crear la ficha si backend lo hace.
- Sustituir el autoservicio del socio por la pantalla administrativa.
- Modificar el backend o inventar parámetros de API no confirmados sin dejar la dependencia documentada.

## Decisions

1. Separar la utilidad en dos superficies: listado administrativo de usuarios y ficha administrativa del socio.
Rationale: el buscador y la edición detallada responden a tareas distintas; separar ambas vistas mantiene el flujo claro y escalable.
Alternatives considered:
- Editar todo inline desde el listado: descartado por complejidad y por poca ergonomía para un formulario largo.

2. Reutilizar el patrón visual de `Configuración` y `Cargos`.
Rationale: ya existe un lenguaje visual administrativo consistente con breadcrumb, fieldset, alerts y listados DaisyUI.

3. Modelar la ficha administrativa como una variante explícita de la ficha de socio.
Rationale: evita mezclar reglas del autoservicio (`/me`) con reglas de administración (`/{userId}`) y reduce riesgo de permisos o payloads incorrectos.

4. Crear la ficha de un no socio con el payload mínimo viable, al menos `email`.
Rationale: responde al flujo pedido y reduce fricción, dejando al administrador completar el resto de campos después de la redirección.
Alternatives considered:
- Obligar a rellenar un formulario previo de alta: descartado porque contradice el comportamiento automático deseado.

## Risks / Trade-offs

- [El contrato de `GET /api/users` no documenta el filtro por email] → Mitigación: dejarlo como dependencia explícita de implementación y confirmar si el backend soporta query string o si hará falta recuperar y filtrar en cliente.
- [Crear ficha con solo email puede chocar con validaciones backend del `POST /api/fichas-socio/{userId}`] → Mitigación: documentar la asunción y validar el mínimo requerido antes de implementar el submit real.
- [La pantalla administrativa de ficha puede solaparse visualmente con la ficha de autoservicio] → Mitigación: reutilizar patrones, pero mantener rutas, permisos y servicios diferenciados.

## Open Questions

- ¿Qué parámetro exacto usa `GET /api/users` para filtrar por email, o debemos asumir que devuelve colección completa y filtrar en frontend?
- ¿El `POST /api/fichas-socio/{userId}` admite realmente un payload mínimo con solo `email`, o backend exige más campos obligatorios en creación?
