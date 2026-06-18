## 1. Ajustar contrato y estado del flujo de solicitud

- [x] 1.1 Ampliar los modelos/frontend contracts de tarifas federativas para mapear `MediaTemporada` desde el API.
- [x] 1.2 Extender el cliente de licencias federativas para enviar `mediaTemporada=true|false` al consultar `GET /api/licencias-federativas/tarifas`.
- [x] 1.3 Añadir al estado del modal de solicitud la modalidad `Temporada Completa`/`Media Temporada`, con `Temporada Completa` como valor inicial.

## 2. Actualizar el modal y la resolución de tarifas

- [x] 2.1 Incorporar el nuevo combo `Modalidad` en el modal de solicitud respetando el estilo y la estructura actual del componente.
- [x] 2.2 Hacer que la selección de `Temporada` cargue tarifas usando la combinación de `Temporada` y `MediaTemporada`.
- [x] 2.3 Hacer que un cambio de modalidad limpie `Tipología`, `Categoría`, tarifa resuelta y mensajes dependientes, y vuelva a consultar el catálogo si ya hay temporada elegida.
- [x] 2.4 Mantener la lógica de deduplicación, resolución de tarifa y bloqueo de `Confirmar` trabajando solo con la modalidad seleccionada.

## 3. Validación y regresión

- [x] 3.1 Actualizar o crear pruebas de componente para cubrir modalidad por defecto, cambio a `Media Temporada` y recarga del catálogo.
- [x] 3.2 Actualizar o crear pruebas del cliente API para verificar que la query de tarifas incluye `mediaTemporada` además de `temporada`.
- [x] 3.3 Ejecutar la batería de pruebas afectada y marcar como completadas solo las tareas verificadas en verde.
