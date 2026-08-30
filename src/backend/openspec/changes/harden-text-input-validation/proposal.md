## Why

La API ya persiste los comentarios de `Signal` mediante EF Core y usa consultas Dapper parametrizadas en los flujos revisados, por lo que no se ha detectado inyeccion SQL directa en ese punto. Aun asi, la aplicacion no aplica una politica uniforme de normalizacion, rechazo de caracteres de control, limites coherentes y validacion defensiva sobre los campos de texto que entran desde el front, lo que deja margen para contenido malicioso, inconsistencias funcionales y regresiones futuras si aparece SQL dinamico en nuevos desarrollos.

## What Changes

- Introducir una politica transversal de endurecimiento para entradas de texto recibidas por la API, con normalizacion, limites, rechazo de caracteres de control no permitidos y reglas explicitas para texto libre frente a campos estructurados.
- Reforzar el contrato de comentarios de `Signal` para que el backend rechace texto vacio tras normalizacion, contenido con caracteres de control no permitidos y entradas que superen la longitud definida.
- Reforzar los campos textuales de `Signal` (`Titulo`, `Descripcion`, `Tags`) y sus filtros de consulta para que apliquen la misma politica defensiva adecuada a cada tipo de dato.
- Exigir que los accesos de lectura y escritura a SQL Server que consuman filtros o texto de usuario permanezcan parametrizados y cubiertos por pruebas de regresion de seguridad.
- Documentar y planificar la extension de la politica comun al resto de entradas textuales de la aplicacion, priorizando `FichaSocio`, agenda telefonica, cargos, tipos de senal, administracion de usuarios y filtros de consulta.

## Capabilities

### New Capabilities
- `text-input-security-hardening`: Politica comun para normalizar, validar y persistir entradas de texto del front con defensa en profundidad frente a contenido malicioso y futuras regresiones de inyeccion.

### Modified Capabilities
- `signal-comments-history`: El contrato de alta de comentarios pasa a exigir saneado y rechazo explicito de texto no permitido antes de persistirlo.
- `signal-management`: Los campos textuales de alta y edicion de `Signal` pasan a aplicar reglas comunes de normalizacion y validacion defensiva.
- `signal-search`: Los filtros textuales de consulta de `Signal` pasan a normalizarse y validarse defensivamente antes de tocar la capa de datos.

## Impact

- Afecta a `src/IndaloAventurApi.Api`, `src/IndaloAventurApi.Application`, `src/IndaloAventurApi.Domain` e `src/IndaloAventurApi.Infrastructure`.
- Puede requerir nuevos value objects, validadores reutilizables o un servicio comun de sanitizacion/normalizacion de texto.
- Afecta a endpoints `POST /api/signals`, `PUT /api/signals/{id}`, `POST /api/signals/{id}/comments` y a endpoints equivalentes en otros modulos que acepten texto.
- Requiere ampliar pruebas unitarias e integracion para cubrir texto malicioso, caracteres de control, longitudes extremas y garantia de consultas parametrizadas.
