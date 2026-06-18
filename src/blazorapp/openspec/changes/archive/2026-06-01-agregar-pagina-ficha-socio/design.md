## Context

El proyecto ya tiene una pantalla `Mi Cuenta`, una sesión autenticada que expone `IsMember` y un servicio `MemberProfileApiClient` que consume `GET /api/fichas-socio/me` para mostrar una versión resumida del perfil. El `endpoints.json` actualizado describe además un contrato completo de `FichaSocioDto` y `UpdateFichaSocioRequest` para lectura y actualización de la propia ficha del socio, incluyendo datos personales, contacto y consentimientos.

La nueva funcionalidad debe apoyarse en ese contrato sin exponer campos sensibles o de control de acceso como `IsMember` o roles. También debe respetar las reglas del proyecto: componentes Razor compartidos en `SharedUI`, lógica en clases partial separadas, recursos localizados y estilos globales SCSS sin CSS inline.

## Goals / Non-Goals

**Goals:**
- Ofrecer una página propia de ficha editable solo para sesiones con `IsMember = true`.
- Mostrar la ficha en un `fieldset` DaisyUI con una maquetación moderna, usable y responsive.
- Permitir editar los campos del contrato de socio aptos para autoservicio y guardar mediante `PUT /api/fichas-socio/me`.
- Añadir validación por campo, tanto de formato/valor como de endurecimiento básico del input enviado al backend.
- Mantener consistencia visual y de navegación con `Mi Cuenta` y `Configuración`.

**Non-Goals:**
- Permitir que el propio usuario edite `IsMember`, roles o cualquier atributo de autorización.
- Crear un panel administrativo para editar la ficha de otros usuarios.
- Rediseñar por completo `Mi Cuenta` más allá de enlazar la nueva pantalla si se decide en implementación.
- Cambiar el contrato backend de `FichaSocioDto` o `UpdateFichaSocioRequest`.

## Decisions

1. Construir una página dedicada de ficha de socio y no un modal dentro de `Mi Cuenta`.
Rationale: la ficha contiene bastantes campos y necesita validaciones, feedback y espacio responsive; una página dedicada escala mejor y permite breadcrumb claro.
Alternatives considered:
- Resolver la edición inline en `Mi Cuenta`: descartado por saturar una pantalla que hoy actúa más como hub.

2. Reutilizar y ampliar la capa de perfil del socio en lugar de crear una integración aislada.
Rationale: ya existe un punto de entrada para `/api/fichas-socio/me`; extenderlo evita duplicidad y mantiene el contrato agrupado en la capa `Member`.

3. Crear un modelo de edición explícito que solo incluya campos permitidos.
Rationale: evita fugas accidentales de `IsMember`, roles u otros campos no editables, y centraliza validaciones y normalización.
Alternatives considered:
- Bind directo contra el DTO del API: descartado por mezclar contrato remoto con reglas de UI/seguridad.

4. Aplicar validación en dos capas: reglas de formulario y saneado de salida.
Rationale: el usuario necesita feedback inmediato, pero además debemos recortar espacios, validar formatos y no reenviar valores peligrosos o inconsistentes.

5. Mantener el lenguaje visual de la app usando DaisyUI + SCSS global.
Rationale: permite un formulario moderno y responsive sin romper la identidad ya establecida en `Mi Cuenta`, `Configuración` y `Cargos`.

## Risks / Trade-offs

- [El DTO de ficha incluye más campos de los que hoy usa el frontend] → Mitigación: introducir un modelo de edición dedicado y mapear solo los campos soportados explícitamente.
- [La visibilidad basada en `IsMember` podría desalinearse con acceso manual a la URL] → Mitigación: ocultar el acceso desde la UI y además hacer que la página/servicio resuelvan un estado no operativo cuando `IsMember` sea `false`.
- [Las validaciones de cliente pueden no coincidir exactamente con backend] → Mitigación: implementar reglas conservadoras, reflejar errores de backend y evitar asumir validaciones de dominio no documentadas.
- [El formulario extenso puede hacerse pesado en móvil] → Mitigación: agrupar campos, usar layout responsive de una o dos columnas según ancho y priorizar controles claros tipo input, checkbox y date.
