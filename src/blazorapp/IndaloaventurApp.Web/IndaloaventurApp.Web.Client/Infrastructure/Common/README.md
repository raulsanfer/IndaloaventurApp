# Patron de resultados de servicios

- Los servicios de aplicacion devuelven `ServiceResult<T>`.
- `ServiceResult<T>.Success(value)` representa una ejecucion correcta.
- `ServiceResult<T>.Failure(error)` representa una ejecucion con error controlado.
- `ServiceError` define `Code` y `Message` para desacoplar UI y transporte.

Codigos base utilizados en autenticacion:

- `auth.invalid_credentials`: el API devuelve `401` o credenciales de fallback invalidas.
- `auth.unavailable`: timeout, error de red o respuesta no interpretable.

Este patron permite pruebas sin red y manejo uniforme de mensajes en componentes Razor.
