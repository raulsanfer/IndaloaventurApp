# signal-management Specification

## Purpose
TBD - created by archiving change add-trailsignal-management. Update Purpose after archive.
## Requirements
### Requirement: Crear signal con tipo obligatorio
El sistema MUST permitir que usuarios con rol `Admin` o `Member` creen elementos `Signal` con los campos `Id` (Guid), `Latitud` (float), `Longitud` (float), `Descripcion` (cadena), `Fotos` (cadena), `Activo` (boolean), `UserId_Alta` (Guid), `Fecha_Alta` (Datetime), `Fecha_Modificacion` (Datetime), `UserId_Modificacion` (Guid), `Tipo` (entero referenciando `Signal_Type`) y `Tags` (cadena), siendo `Tipo` obligatorio y valido. En la creacion, `Foto1` SHALL ser obligatoria y `Foto2` SHALL poder omitirse.

#### Scenario: Alta de signal valida con dos fotos
- **WHEN** un usuario con rol `Admin` o `Member` envia una solicitud de creacion con todos los campos requeridos, `Foto1`, `Foto2` y un `Tipo` existente
- **THEN** el sistema crea el `Signal` con los metadatos de auditoria y la referencia a `Signal_Type`

#### Scenario: Alta de signal valida con una sola foto
- **WHEN** un usuario con rol `Admin` o `Member` envia una solicitud de creacion con todos los campos requeridos, `Foto1`, sin `Foto2` y un `Tipo` existente
- **THEN** el sistema crea el `Signal` correctamente y SHALL mantener la segunda foto como ausente

#### Scenario: Alta rechazada por tipo inexistente
- **WHEN** un usuario con rol `Admin` o `Member` intenta crear un `Signal` con `Tipo` no existente
- **THEN** el sistema rechaza la solicitud por referencia invalida a `Signal_Type`

### Requirement: Editar signal sin eliminaci�n
El sistema MUST permitir que usuarios con rol `Admin` o `Member` editen `Signal` existentes y MUST impedir su eliminaci�n mediante la API de gesti�n de signals.

#### Scenario: Edici�n de signal v�lida
- **WHEN** un usuario con rol `Admin` o `Member` env�a cambios v�lidos sobre un `Signal` existente
- **THEN** el sistema actualiza los campos editables y refresca `Fecha_Modificacion` y `UserId_Modificacion`

#### Scenario: Operaci�n de eliminaci�n no permitida
- **WHEN** un usuario con rol `Admin` o `Member` intenta eliminar un `Signal`
- **THEN** el sistema rechaza la operaci�n porque la funcionalidad de eliminaci�n no est� permitida

#### Scenario: Acceso denegado para roles no autorizados
- **WHEN** un usuario sin rol `Admin` o `Member` intenta crear o editar un `Signal`
- **THEN** el sistema rechaza la operaci�n por autorizaci�n insuficiente

