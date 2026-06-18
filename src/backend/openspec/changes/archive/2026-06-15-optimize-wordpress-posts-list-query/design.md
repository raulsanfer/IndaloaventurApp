## Context

La integración actual con WordPress usa un único DTO para listado y detalle. Eso obliga al listado a exponer campos como `Contenido` y `Enlace` aunque la vista resumida no los necesite. Además, el servicio de listado solicita un conjunto de campos que no incluye `_embedded`, pero el mapeo de `ImagenDestacadaUrl` depende precisamente de `wp:featuredmedia`, por lo que el listado termina sin imagen destacada.

El endpoint `GET /api/wordpress/posts` también fija un `pageSize` por defecto en el controlador, mientras que la configuración WordPress ya está centralizada en opciones tipadas y es el lugar natural para parametrizar ese comportamiento.

## Goals / Non-Goals

**Goals:**
- Configurar en `appsettings` el número de posts por defecto del listado WordPress.
- Corregir la consulta remota del listado para recuperar la imagen destacada.
- Reducir el payload solicitado y devuelto por el listado a los campos estrictamente necesarios: título, resumen, fecha e imagen destacada.
- Mantener el detalle por `slug` como consulta independiente para contenido completo.

**Non-Goals:**
- Cambiar la semántica del endpoint de detalle de WordPress.
- Añadir persistencia local, cache o sincronización de posts.
- Rediseñar la autenticación o la configuración base del cliente WordPress.

## Decisions

### 1. El listado y el detalle usarán contratos distintos

El listado de posts y el detalle por `slug` sirven necesidades diferentes. El listado debe exponer un modelo resumido y el detalle debe conservar el modelo completo. La propuesta debe permitir DTOs separados, o al menos contratos de salida distintos por endpoint, para evitar que el listado arrastre `Contenido` y `Enlace`.

Alternativas consideradas:
- Mantener un único DTO y devolver campos vacíos en el listado. Rechazada porque conserva un contrato sobredimensionado y ambiguo.
- Mantener un único DTO y seguir consultando campos no usados. Rechazada porque contradice el objetivo de optimización.

### 2. El tamaño por defecto del listado se leerá desde `WordPressOptions`

Se añadirá un parámetro tipado en configuración para el número de posts por defecto. El controlador o el caso de uso aplicará ese valor solo cuando el cliente no informe explícitamente `pageSize`.

Alternativas consideradas:
- Dejar el valor hardcoded en el controlador. Rechazada porque dispersa configuración operativa fuera de `appsettings`.
- Forzar siempre el valor de configuración ignorando `pageSize` del cliente. Rechazada porque elimina flexibilidad ya soportada por el contrato.

### 3. La consulta del listado pedirá solo campos mínimos más la información necesaria para la imagen destacada

La llamada a WordPress para listados debe incluir únicamente los campos imprescindibles para el resumen y la mecánica necesaria para mapear la imagen destacada. Eso implica revisar el uso de `_fields` y `_embed` para no solicitar `content`, `link` u otros datos irrelevantes en el listado.

Alternativas consideradas:
- Quitar `_fields` y usar `_embed=true` completo. Rechazada porque amplía en exceso el payload remoto.
- Mantener `_fields` sin `_embedded`. Rechazada porque reproduce el bug actual de la imagen destacada ausente.

## Risks / Trade-offs

- [Cambio de contrato del listado rompe consumidores actuales] -> Mitigación: documentar el breaking change en specs y validar clientes dependientes antes de desplegar.
- [WordPress puede requerir un formato exacto de `_fields` para devolver `_embedded`] -> Mitigación: cubrir la URI generada y el mapeo con pruebas de integración.
- [El `pageSize` configurado puede ser inválido o excesivo] -> Mitigación: validar el nuevo parámetro en la carga de opciones y mantener límites en el validador/query si ya existen.
