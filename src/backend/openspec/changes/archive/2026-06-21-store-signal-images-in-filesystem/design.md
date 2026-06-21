## Context

El backend actual modela las fotos de `Signal` como `byte[]` dentro de la entidad y las persiste en SQL Server mediante columnas `Foto1` y `Foto2` de tipo `varbinary(max)`. Ese enfoque funciono para introducir soporte binario rapido, pero tiene dos costes claros: la base de datos crece con contenido pesado que no aporta capacidad relacional, y cualquier lectura/escritura de imagen queda acoplada al mismo modelo de persistencia que los metadatos de la senal.

La refactorizacion solicitada cambia el backend a un esquema donde las imagenes viven en disco bajo una ruta configurada del servidor. Esto afecta a dominio, aplicacion, infraestructura, migraciones y operacion, porque ademas de cambiar el esquema SQL hay que definir nombres de fichero, borrado/reemplazo, consistencia ante fallos y estrategia de migracion de datos ya existentes.

## Goals / Non-Goals

**Goals:**
- Mover el almacenamiento fisico de fotos de `Signal` desde SQL Server a filesystem configurado.
- Mantener los metadatos funcionales de `Signal` en persistencia relacional.
- Introducir una abstraccion de almacenamiento de medios que permita guardar, leer, reemplazar y borrar fotos de forma controlada.
- Definir un camino de migracion para las fotos historicas ya persistidas en base de datos.
- Mantener cubiertos los flujos de crear, editar y consultar imagenes con pruebas automatizadas.

**Non-Goals:**
- Migrar en este cambio a almacenamiento cloud, CDN o blob storage externo.
- Redisenar el modulo para soportar un numero variable de fotos.
- Introducir transformaciones multimedia como thumbnails, compresion o recodificacion.
- Cambiar forzosamente el modelo de autorizacion ya definido para `Signals`.

## Decisions

1. Persistir referencias relativas de fichero en `Signal` y sacar los bytes del estado relacional.
- Rationale: el agregado necesita seguir sabiendo donde estan sus dos fotos, pero no necesita cargar siempre el contenido binario en memoria ni en SQL. Guardar rutas relativas o claves de almacenamiento mantiene el modelo funcional y elimina el peso binario de la tabla principal.
- Alternative considered: conservar `byte[]` en dominio y esconder solo la escritura en un servicio de archivos. Se descarta porque perpetua un modelo de entidad acoplado al transporte binario y dificulta consultas ligeras.

2. Introducir un servicio de infraestructura dedicado para medios de `Signal`.
- Rationale: la logica de crear carpetas, validar rutas, generar nombres unicos, reemplazar archivos y borrar residuos no debe dispersarse en handlers ni repositorios. Un contrato explicito permite aislar filesystem, facilitar pruebas y preparar una evolucion futura a otro backend de almacenamiento.
- Alternative considered: escribir directamente con `System.IO` desde handlers o repositorios. Se descarta por acoplamiento y peor testabilidad.

3. Usar una raiz configurable y nombres deterministas por `Signal`.
- Rationale: la ruta base debe venir de configuracion (`appsettings`) para separar entornos. Dentro de esa raiz, una estructura por `SignalId` o equivalente estable evita colisiones y simplifica limpieza/migracion.
- Alternative considered: un unico directorio plano o nombres aleatorios sin estructura. Se descarta por peor operabilidad y mantenimiento.

4. Mantener la persistencia de metadatos de `Signal` en SQL Server y coordinar escritura de ficheros con actualizacion relacional.
- Rationale: los metadatos de negocio siguen siendo claramente relacionales. La operacion de create/update necesitara una secuencia controlada para evitar referencias huerfanas o archivos sin registrar.
- Alternative considered: mover toda la entidad `Signal` a filesystem/documento. Se descarta por exceso de alcance.

5. Ejecutar una migracion/backfill explicito de datos existentes.
- Rationale: ya hay despliegues y tests que asumen fotos en base de datos. La refactorizacion debe contemplar un proceso que lea `Foto1`/`Foto2`, escriba ficheros, persista rutas y despues elimine las columnas binarias del esquema.
- Alternative considered: romper compatibilidad y vaciar fotos historicas. Se descarta salvo que operacion confirme explicitamente que los datos existentes no importan.

6. Mantener por defecto el contrato funcional de API y decidir por separado si el endpoint de imagenes devuelve bytes o referencias descargables.
- Rationale: el objetivo pedido es cambiar la persistencia, no necesariamente redisenar el contrato HTTP. Mantener el contrato reduce impacto en cliente; si mas adelante interesa exponer URLs o streaming, eso debe discutirse como decision adicional.
- Alternative considered: cambiar ahora mismo a URLs/descarga directa de fichero. Se deja fuera de esta propuesta salvo confirmacion expresa.

## Risks / Trade-offs

- [Riesgo] Quedar con inconsistencias entre SQL y filesystem si falla una parte de la operacion. -> Mitigacion: definir orden de escritura/reemplazo, limpieza compensatoria y pruebas de error.
- [Riesgo] La ruta configurada puede no existir o no tener permisos en despliegue. -> Mitigacion: validacion de startup y documentacion operativa clara.
- [Riesgo] La migracion de fotos historicas puede ser lenta o dejar archivos huerfanos si se interrumpe. -> Mitigacion: backfill idempotente, logs controlados y rollback planificado.
- [Trade-off] El backup deja de ser un unico artefacto SQL y pasa a requerir base de datos + filesystem. -> Mitigacion: explicitarlo como impacto operativo del cambio.
- [Trade-off] Mantener el contrato HTTP actual puede implicar seguir serializando bytes en algunas respuestas aunque ya no vivan en SQL. -> Mitigacion: aceptar esa compatibilidad ahora y valorar una optimizacion posterior si el payload lo exige.

## Migration Plan

1. Introducir configuracion tipada para la raiz de almacenamiento de imagenes y validar que existe o puede crearse.
2. Añadir el servicio de almacenamiento de medios y refactorizar `Signal`/repositorios para persistir referencias de fichero.
3. Crear migracion EF Core para sustituir columnas binarias por columnas de referencia de imagen.
4. Ejecutar un backfill que exporte fotos existentes a disco y persista sus rutas relativas antes de eliminar o dejar obsoletas las columnas binarias.
5. Actualizar pruebas unitarias e integracion para el nuevo modelo de almacenamiento.
6. Desplegar con carpeta persistente provisionada y estrategia de backup revisada.

## Open Questions

- Confirmar si el endpoint `GET /api/signals/{id}/images` debe seguir devolviendo `byte[]` serializado o pasar a responder referencias/stream de fichero.
- Confirmar si la migracion de datos historicos es obligatoria en todos los entornos o si en desarrollo/test se puede recrear el dataset sin backfill.
- Confirmar limites operativos de tamano por imagen y politica de limpieza si se reemplazan fotos varias veces.
