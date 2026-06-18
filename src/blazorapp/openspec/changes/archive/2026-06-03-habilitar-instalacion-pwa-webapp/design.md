## Context

La web ya enlaza un `manifest.webmanifest` y se sirve por HTTPS en desarrollo, pero no dispone de `service worker`, no registra dicho worker en cliente y usa un único `favicon.png` como icono del manifiesto. Eso deja la arquitectura a medio camino: compatible con PWA en intención, pero no suficientemente completa para que Chrome/Edge/Safari la consideren instalable de forma consistente.

Además, el proyecto ya declara que la app debe poder instalarse desde navegador en dispositivo móvil, así que este cambio debe cerrar la brecha entre la intención arquitectónica y la implementación real sin alterar la lógica de negocio de la aplicación.

## Goals / Non-Goals

**Goals:**
- Hacer que el frontend cumpla los requisitos mínimos de instalabilidad PWA.
- Definir una fuente clara de iconos PWA reutilizando el logo oficial del proyecto en `SharedUI`.
- Añadir un `service worker` básico y seguro, suficiente para habilitar instalación sin introducir una estrategia offline agresiva.
- Mantener la arquitectura actual `Blazor Web App + InteractiveWebAssembly` sin rediseñar la app.

**Non-Goals:**
- No convertir la aplicación en una experiencia totalmente offline-first.
- No rediseñar la shell, navegación ni estilos de la app.
- No introducir sincronización offline, colas locales ni caché avanzada de APIs autenticadas.

## Decisions

### 1. Implementar una PWA mínima pero válida
Se añadirá un `manifest` correcto, iconos reales, y un `service worker` registrado en cliente. Esto cubre la condición principal para que el navegador evalúe la app como instalable.

Alternativas consideradas:
- Mantener solo el manifiesto actual: descartado, porque no basta para la instalabilidad.
- Esperar a una estrategia offline completa: descartado, porque bloquea una necesidad inmediata.

### 2. Separar instalabilidad de soporte offline profundo
El `service worker` inicial debe ser conservador: orientado a presencia PWA y cacheo básico de assets estáticos, evitando interferencias con autenticación, llamadas API o contenido que cambia frecuentemente.

Alternativas consideradas:
- Cachear agresivamente toda la app y llamadas API: descartado por riesgo de inconsistencias y sesiones obsoletas.
- No cachear nada: técnicamente puede seguir habiendo service worker, pero la experiencia PWA quedaría demasiado pobre y menos robusta.

### 3. Derivar iconos desde el logo oficial de SharedUI
El activo fuente será `IndaloaventurApp.SharedUI/wwwroot/assets/images/logo.png`, generando versiones PWA apropiadas para `192x192`, `512x512` y, si es posible, `maskable`.

Alternativas consideradas:
- Seguir reutilizando `favicon.png`: descartado porque no cubre tamaños reales de PWA.
- Crear un icono nuevo no alineado con marca: descartado por incoherencia visual.

### 4. Registrar explícitamente el service worker desde el frontend servido
La app debe declarar y registrar el worker desde `App.razor` o script dedicado para que el navegador pueda activarlo en contexto seguro.

Alternativas consideradas:
- Depender de registro implícito: descartado, no existe tal mecanismo aquí.

## Risks / Trade-offs

- [Compatibilidad parcial entre navegadores] → Validar al menos en Chrome/Edge y documentar que Safari/iOS puede mostrar el flujo de instalación de forma distinta.
- [Percepción de “app instalable” frente a “app offline”] → Dejar explícito en diseño y pruebas que este cambio persigue instalación, no soporte offline integral.
- [Iconos de baja calidad si el logo fuente no escala bien] → Revisar el activo origen y generar tamaños dedicados antes de cerrar el cambio.
- [Conflictos de caché durante desarrollo] → Mantener el worker inicial simple y prever limpieza de caché y actualización controlada en pruebas.

## Migration Plan

1. Añadir iconos PWA derivados del logo oficial al `wwwroot` del proyecto web.
2. Corregir y ampliar `manifest.webmanifest` con iconos y metadatos adecuados.
3. Añadir `service worker` y registrarlo desde el cliente servido.
4. Validar sobre perfil HTTPS local y comprobar que el navegador reconoce la app como instalable.
5. Verificar experiencia básica tras instalación en dispositivo o emulación.

## Open Questions

- Si queremos incluir icono `maskable` desde esta primera iteración o dejarlo como mejora posterior si el logo no encaja bien.
- Si el `service worker` debe cachear solo shell/assets o también ciertas rutas públicas de contenido.
