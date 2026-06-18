## Context

`MyAccountView` ya usa el perfil cargado desde la sesión y `GET /api/fichas-socio/me` para decidir qué bloques mostrar. Actualmente los usuarios `IsMember = true` ven accesos propios del socio, mientras que el resto no dispone de una llamada a la acción operativa para convertirse en socio. El proceso real de alta existe fuera de la app en `https://indaloaventura.com/hazte-socio/`, por lo que la intervención frontend consiste en exponerlo con la visibilidad correcta y una navegación externa segura.

La solución debe respetar el lenguaje visual ya asentado en `Mi Cuenta`, mantener componentes compartidos en `SharedUI`, textos localizados y estilos SCSS globales. También debe comportarse bien en contexto PWA, donde abrir una URL externa debe sacar al usuario a un navegador o pestaña nueva en lugar de secuestrar la navegación interna.

## Goals / Non-Goals

**Goals:**
- Mostrar una opción `Hazte socio` en `Mi Cuenta` solo para usuarios autenticados con rol `Member` y `IsMember = false`.
- Abrir la URL externa de alta en una pestaña nueva o navegador externo según el contexto del dispositivo.
- Mantener la nueva acción visualmente consistente con el resto de enlaces de `Mi Cuenta`.
- Cubrir la lógica de visibilidad con tests de componente.

**Non-Goals:**
- Crear un flujo de alta de socios dentro de la propia app.
- Añadir nuevas APIs, proxies backend o cambios en el contrato autenticado actual.
- Mostrar la opción a usuarios que ya son socios o reemplazar otros accesos existentes de `Mi Cuenta`.

## Decisions

1. Resolver la acción como enlace externo directo en `MyAccountView`.
Rationale: el alta ya existe en la web pública y no necesita intermediación del backend ni una página interna nueva.
Alternatives considered:
- Crear una página interna intermedia: descartado por añadir fricción y no aportar valor funcional.

2. Basar la visibilidad en la combinación de sesión/perfil que ya usa `Mi Cuenta`.
Rationale: reutiliza la información existente y evita introducir nuevas fuentes de verdad para decidir quién ve el CTA.

3. Abrir la URL con semántica de nueva pestaña (`target="_blank"` + `rel` seguro).
Rationale: es la forma web estándar que mejor se traduce a pestaña o navegador externo en contexto PWA.

## Risks / Trade-offs

- [La combinación exacta rol `Member` + `IsMember = false` puede no estar presente en todos los escenarios de sesión] → Mitigación: centrar la condición en los datos disponibles hoy y validar con tests los casos principales.
- [Abrir una URL externa puede sentirse como una salida abrupta de la app] → Mitigación: presentar la opción claramente como acceso externo al proceso de alta.
- [En algunos navegadores/PWA el comportamiento de nueva pestaña puede variar] → Mitigación: usar atributos web estándar y evitar lógica personalizada dependiente de plataforma.
