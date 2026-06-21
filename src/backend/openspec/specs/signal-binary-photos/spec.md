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
El sistema MUST almacenar `Foto1` y `Foto2` de cada `Signal` fuera de la base de datos relacional, usando almacenamiento en filesystem gestionado por infraestructura. La persistencia relacional SHALL conservar solo las referencias necesarias para resolver ambas imagenes.

#### Scenario: Consistencia de lectura tras escritura
- **WHEN** se crea o edita un `Signal` con fotos binarias
- **THEN** una lectura posterior del mismo registro devuelve exactamente las dos fotos almacenadas resolviendolas desde sus referencias de fichero

#### Scenario: Referencias persistidas tras crear una signal
- **WHEN** se completa el alta de una `Signal` con fotos validas
- **THEN** el sistema SHALL persistir las referencias de almacenamiento de `Foto1` y `Foto2` junto al resto de metadatos de la senal

### Requirement: Obtener imagenes de Signal mediante endpoint dedicado
El sistema MUST exponer un endpoint especifico para recuperar las imagenes (`Foto1`, `Foto2`) de una `Signal` por identificador, sin depender de los endpoints generales de busqueda. La respuesta SHALL resolverse a partir del almacenamiento en filesystem asociado a la `Signal`.

#### Scenario: Recuperacion de imagenes de Signal existente
- **WHEN** un usuario autenticado solicita el endpoint de imagenes para una `Signal` existente
- **THEN** el sistema devuelve `Foto1` y `Foto2` correspondientes al registro solicitado resolviendolas desde sus referencias de almacenamiento

#### Scenario: Signal inexistente en endpoint de imagenes
- **WHEN** un usuario autenticado solicita el endpoint de imagenes para una `Signal` no existente
- **THEN** el sistema responde con recurso no encontrado

#### Scenario: Referencia de imagen inconsistente
- **WHEN** una `Signal` existente apunta a una referencia de imagen que no puede resolverse en filesystem
- **THEN** el sistema SHALL responder con un error controlado y SHALL no devolver datos binarios parciales o ambiguos

