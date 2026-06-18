## 1. Contratos y configuración

- [x] 1.1 Definir `WordPressOptions` (URL base, clave y parámetros operativos) y enlazarla desde `appsettings`.
- [x] 1.2 Definir contrato de aplicación para servicio WordPress y DTOs de posts orientados al endpoint de consulta.
- [x] 1.3 Añadir validaciones de configuración para detectar claves faltantes o inválidas al arrancar.

## 2. Implementación de infraestructura WordPress

- [x] 2.1 Implementar cliente HTTP de WordPress en Infrastructure con timeout configurable y cabeceras según configuración.
- [x] 2.2 Implementar servicio inyectable de WordPress que consulte posts y los mapee al contrato de aplicación.
- [x] 2.3 Implementar manejo de errores remotos/red devolviendo errores controlados y mensajes user-facing en español.

## 3. Exposición API

- [x] 3.1 Crear controlador/endpoint `GET` para consultar posts de WordPress delegando en el servicio.
- [x] 3.2 Definir parámetros de consulta soportados y respuesta API consistente con DTOs.
- [x] 3.3 Registrar dependencias y configuración en composición de servicios de la API.

## 4. Verificación y calidad

- [x] 4.1 Añadir pruebas unitarias del servicio WordPress (éxito, error remoto, configuración inválida).
- [x] 4.2 Añadir/actualizar pruebas de integración del endpoint de consulta de posts.
- [x] 4.3 Ejecutar `dotnet test` y corregir regresiones.
