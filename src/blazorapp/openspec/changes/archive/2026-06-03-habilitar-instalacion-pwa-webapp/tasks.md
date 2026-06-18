## 1. Preparación PWA

- [x] 1.1 Revisar el estado actual de `manifest`, HTTPS y assets web para confirmar las carencias de instalabilidad
- [x] 1.2 Preparar en el proyecto web los iconos PWA necesarios a partir de `IndaloaventurApp.SharedUI/wwwroot/assets/images/logo.png`

## 2. Implementación de instalabilidad

- [x] 2.1 Actualizar `manifest.webmanifest` con metadatos e iconos adecuados para instalación
- [x] 2.2 Añadir un `service worker` básico y seguro para la app web
- [x] 2.3 Registrar el `service worker` desde el frontend sin romper la carga normal de la aplicación

## 3. Verificación

- [x] 3.1 Verificar con build o comprobaciones técnicas que todos los assets y referencias PWA se sirven correctamente
- [ ] 3.2 Validar manualmente en navegador compatible sobre HTTPS que aparece la opción de instalar o que la app pasa la comprobación de instalabilidad
- [ ] 3.3 Validar manualmente que la app instalada abre en modo standalone y mantiene la entrada principal esperada
