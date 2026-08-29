## 1. Frontera de servicio e imagenes

- [x] 1.1 Añadir un modelo dedicado para imágenes de signal y extender `ISignalService`, `SignalApiClient` y los dobles de test con una operación específica de carga de imágenes.
- [x] 1.2 Extraer a una utilidad compartida del dominio `signals` la conversión `byte[]` a preview/data URL y la normalización de fotos opcionales reutilizable por create y detail.
- [x] 1.3 Refactorizar `GetSignalHomeDataAsync` para que deje de resolver imágenes por cada card y siga construyendo el listado solo con metadatos y categorías.

## 2. Ajustes en vistas de signals

- [x] 2.1 Actualizar `SignalHomeView` y sus modelos asociados para conservar un fallback visual estable cuando no exista preview en el listado.
- [x] 2.2 Actualizar `SignalDetailView` para cargar imágenes con estado independiente del detalle base, incluyendo estado de carga, vacío y error parcial del bloque de imágenes.
- [x] 2.3 Mantener `SignalCreateView` y el flujo de edición alineados con la nueva frontera de imágenes, reutilizando la utilidad compartida y sin introducir reemplazo de fotos en `PUT`.

## 3. Verificacion

- [x] 3.1 Actualizar `SignalApiClientTests` para cubrir que el listado ya no hace fan-out a `/images`, que la carga dedicada de imágenes funciona y que la codificación de create sigue siendo compatible.
- [x] 3.2 Actualizar los tests de vistas (`SignalViewsTests` y dobles relacionados) para cubrir cards sin dependencia de imagen, detalle con una o dos fotos y error parcial de imágenes.
- [x] 3.3 Ejecutar `dotnet test` sobre la solución frontend y revisar que no queden regresiones en signals.
