## 1. Contratos y datos del flujo

- [x] 1.1 Ampliar el servicio o cliente API de licencias federativas para consumir `GET /api/licencias-federativas/tarifas?temporada=<valor>` y `POST /api/licencias-federativas/me/solicitudes`
- [x] 1.2 Definir la precarga local del combo `Temporada` con `año actual` y `año actual + 1`, sin llamada inicial al API
- [x] 1.3 Definir modelos de UI para tarifa disponible, opciones deduplicadas del modal y payload de creación basado en `Temporada` + `TarifaLicenciaFederativaId`

## 2. Modal de solicitud y comportamiento interactivo

- [x] 2.1 Activar el botón `Solicitar Licencia` de la pantalla existente para abrir/cerrar un modal responsive con acciones `Confirmar` y `Cancelar`
- [x] 2.2 Implementar la consulta de tarifas al seleccionar `Temporada` y la deduplicación de combos de `Tipología` y `Categoría`, incluyendo filtros dependientes entre selecciones
- [x] 2.3 Mostrar un mensaje de estado vacío cuando la temporada seleccionada no tenga tarifas disponibles y mantener la confirmación no operativa en ese caso
- [x] 2.4 Mostrar `PrecioClub` solo cuando exista una combinación válida completa y bloquear la confirmación mientras falten campos obligatorios o el modal esté enviando
- [x] 2.5 Gestionar estados de carga, vacío, error y validación del modal sin romper la composición actual de `Licencias Federativas`

## 3. Confirmación, refresco y cobertura

- [x] 3.1 Enviar la creación con la tarifa resuelta, cerrar el modal tras éxito y recargar inmediatamente `GET /api/licencias-federativas/me/solicitudes`
- [x] 3.2 Prevenir o comunicar con claridad los errores de negocio más probables, especialmente duplicidad de solicitud por temporada o catálogo inválido
- [x] 3.3 Añadir o actualizar recursos localizados ES, estilos SCSS globales y tests del flujo completo: apertura de modal, selección dependiente, creación y refresco del listado
