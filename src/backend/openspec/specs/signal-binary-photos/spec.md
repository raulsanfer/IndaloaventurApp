# signal-binary-photos Specification

## Purpose
TBD - created by archiving change refactor-signal-photos-to-byte-arrays. Update Purpose after archive.
## Requirements
### Requirement: Crear y editar Signal con dos fotos binarias
El sistema MUST permitir crear y editar `Signal` recibiendo exactamente dos fotos (`Foto1` y `Foto2`) como arreglos de bytes no vacios.

#### Scenario: Alta de Signal con dos fotos validas
- **WHEN** un usuario autorizado envia una solicitud de alta con `Foto1` y `Foto2` en bytes validos
- **THEN** el sistema crea el `Signal` persistiendo ambas fotos junto al resto de datos obligatorios

#### Scenario: Edicion de Signal con reemplazo de ambas fotos
- **WHEN** un usuario autorizado envia una solicitud de edicion con `Foto1` y `Foto2` en bytes validos
- **THEN** el sistema actualiza el `Signal` y reemplaza ambas fotos persistidas

#### Scenario: Rechazo por foto faltante o vacia
- **WHEN** una solicitud de alta o edicion llega con `Foto1` o `Foto2` nula o vacia
- **THEN** el sistema rechaza la solicitud con error de validacion

### Requirement: Persistir dos fotos binarias por Signal
El sistema MUST almacenar `Foto1` y `Foto2` de cada `Signal` en persistencia relacional como datos binarios separados.

#### Scenario: Consistencia de lectura tras escritura
- **WHEN** se crea o edita un `Signal` con dos fotos binarias
- **THEN** una lectura posterior del mismo registro devuelve exactamente las dos fotos almacenadas

### Requirement: Obtener imagenes de Signal mediante endpoint dedicado
El sistema MUST exponer un endpoint especifico para recuperar las imagenes (`Foto1`, `Foto2`) de una `Signal` por identificador, sin depender de los endpoints generales de busqueda.

#### Scenario: Recuperacion de imagenes de Signal existente
- **WHEN** un usuario autenticado solicita el endpoint de imagenes para una `Signal` existente
- **THEN** el sistema devuelve `Foto1` y `Foto2` correspondientes al registro solicitado

#### Scenario: Signal inexistente en endpoint de imagenes
- **WHEN** un usuario autenticado solicita el endpoint de imagenes para una `Signal` no existente
- **THEN** el sistema responde con recurso no encontrado

