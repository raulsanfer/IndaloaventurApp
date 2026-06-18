## 1. Query de detalle de signal

- [x] 1.1 Crear la query `GetSignalById` y su handler en Application reutilizando `ISignalRepository.GetByIdAsync`.
- [x] 1.2 Mapear la entidad recuperada a `SignalDto` y devolver `KeyNotFoundException` cuando la signal no exista.

## 2. Exposicion en API

- [x] 2.1 Anadir la accion `GET /api/signals/{id}` en `SignalsController` manteniendo el requisito de usuario autenticado.
- [x] 2.2 Documentar en la accion las respuestas `200 OK`, `401 Unauthorized` y `404 Not Found` alineadas con la spec.

## 3. Verificacion

- [x] 3.1 Anadir tests de integracion para recuperar una signal existente por `Id` y validar el payload principal.
- [x] 3.2 Anadir tests de integracion para los casos `404 Not Found` y `401 Unauthorized` del nuevo endpoint.
- [x] 3.3 Ejecutar la bateria de tests relevante y dejar constancia del comando usado en la validacion del cambio.
