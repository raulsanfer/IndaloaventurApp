## 1. Ajustar contrato y datos del histórico

- [x] 1.1 Ampliar `FederativeLicenseRequest` para exponer `MediaTemporada` en el histórico del socio.
- [x] 1.2 Extender `FederativeLicenseApiClient` para mapear `MediaTemporada` desde `GET /api/licencias-federativas/me/solicitudes`.
- [x] 1.3 Añadir o actualizar los literales localizados necesarios para representar `Temporada Completa`, `Media Temporada` y/o el formato combinado del detalle.

## 2. Actualizar la vista de Licencias Federativas del socio

- [x] 2.1 Modificar `FederativeLicensesView` para construir una etiqueta de detalle que incluya temporada y modalidad de la solicitud.
- [x] 2.2 Mostrar en el histórico `Temporada Completa` cuando `MediaTemporada = false`.
- [x] 2.3 Mostrar en el histórico `Media Temporada` cuando `MediaTemporada = true`.

## 3. Validación y regresión

- [x] 3.1 Actualizar o crear pruebas del cliente API para validar el mapeo de `MediaTemporada` en solicitudes del socio.
- [x] 3.2 Actualizar o crear pruebas de componente para validar la modalidad mostrada en el histórico del socio.
- [x] 3.3 Ejecutar la batería de pruebas afectada y marcar como completadas solo las tareas verificadas en verde.
