## Why

El backend ya persiste el catalogo de tarifas de licencias federativas y permite operar con solicitudes propias, pero el cliente autenticado todavia no puede consultar de forma directa todas las tarifas disponibles para elegir una licencia. Necesitamos cubrir esa lectura para cualquier usuario autenticado, sin depender del rol ni de datos administrativos.

## What Changes

- Exponer un endpoint autenticado para consultar el catalogo completo de tarifas de licencias federativas disponible en el sistema.
- Permitir filtrar la consulta al menos por `Temporada` para que el cliente pueda acotar la campana activa cuando lo necesite.
- Devolver en la respuesta los datos funcionales necesarios de cada tarifa, incluyendo identificador, temporada, licencia, categoria, territorio, `PrecioClub` y `PrecioIndependiente`.
- Garantizar que cualquier usuario autenticado, independientemente de su rol, pueda usar la consulta mientras los usuarios anonimos siguen bloqueados.

## Capabilities

### New Capabilities
- `federative-license-rates-query-api`: API autenticada para consultar el catalogo de tarifas de licencias federativas.

### Modified Capabilities

## Impact

- API: nuevas rutas GET autenticadas dentro del modulo `LicenciasFederativasController`.
- Application: nueva query, DTO de lectura y validaciones de filtros.
- Infrastructure: reutilizacion del repositorio de tarifas o nueva proyeccion de lectura para listar el catalogo por temporada.
- Seguridad: acceso permitido a cualquier identidad autenticada, sin restriccion por rol.
