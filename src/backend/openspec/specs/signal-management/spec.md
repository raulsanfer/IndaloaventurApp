# signal-management Specification

## Purpose
TBD - created by archiving change add-trailsignal-management. Update Purpose after archive.

## Requirements
### Requirement: Crear signal con tipo obligatorio
El sistema MUST permitir que usuarios con rol `Admin` o `Member` creen elementos `Signal` con los campos `Id` (Guid), `Latitud` (float), `Longitud` (float), `Titulo` (cadena), `Descripcion` (cadena), `Fotos` (cadena), `Activo` (boolean), `UserId_Alta` (Guid), `Fecha_Alta` (Datetime), `Fecha_Modificacion` (Datetime), `UserId_Modificacion` (Guid), `Tipo` (entero referenciando `Signal_Type`) y `Tags` (cadena), siendo `Titulo` y `Tipo` obligatorios y validos. En la creacion, `Foto1` SHALL ser obligatoria y `Foto2` SHALL poder omitirse.

#### Scenario: Alta de signal valida con dos fotos
- **WHEN** un usuario con rol `Admin` o `Member` envia una solicitud de creacion con todos los campos requeridos, `Foto1`, `Foto2`, un `Titulo` informado y un `Tipo` existente
- **THEN** el sistema crea el `Signal` con los metadatos de auditoria, el `Titulo` persistido y la referencia a `Signal_Type`

#### Scenario: Alta de signal valida con una sola foto
- **WHEN** un usuario con rol `Admin` o `Member` envia una solicitud de creacion con todos los campos requeridos, `Foto1`, sin `Foto2`, un `Titulo` informado y un `Tipo` existente
- **THEN** el sistema crea el `Signal` correctamente y SHALL mantener la segunda foto como ausente

#### Scenario: Alta rechazada por tipo inexistente
- **WHEN** un usuario con rol `Admin` o `Member` intenta crear un `Signal` con `Tipo` no existente
- **THEN** el sistema rechaza la solicitud por referencia invalida a `Signal_Type`

#### Scenario: Alta rechazada por titulo ausente
- **WHEN** un usuario con rol `Admin` o `Member` intenta crear un `Signal` sin informar `Titulo`
- **THEN** el sistema rechaza la solicitud por incumplir el contrato obligatorio de la incidencia

### Requirement: Editar signal sin eliminacion
El sistema MUST permitir que usuarios con rol `Admin` o `Member` editen `Signal` existentes, incluyendo `Titulo` y `Descripcion`, y MUST impedir su eliminacion mediante la API de gestion de signals.

#### Scenario: Edicion de signal valida
- **WHEN** un usuario con rol `Admin` o `Member` envia cambios validos sobre un `Signal` existente, incluyendo un nuevo `Titulo`
- **THEN** el sistema actualiza los campos editables, persiste el `Titulo` actualizado y refresca `Fecha_Modificacion` y `UserId_Modificacion`

#### Scenario: Operacion de eliminacion no permitida
- **WHEN** un usuario con rol `Admin` o `Member` intenta eliminar un `Signal`
- **THEN** el sistema rechaza la operacion porque la funcionalidad de eliminacion no esta permitida

#### Scenario: Acceso denegado para roles no autorizados
- **WHEN** un usuario sin rol `Admin` o `Member` intenta crear o editar un `Signal`
- **THEN** el sistema rechaza la operacion por autorizacion insuficiente
