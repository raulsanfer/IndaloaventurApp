## Context

La entidad `Signal` utiliza hoy un campo `Fotos` de tipo texto, lo que asume que la imagen se transporta como referencia textual y no como contenido binario. El nuevo requerimiento exige recibir dos fotos reales en bytes y persistirlas, manteniendo la arquitectura actual (API -> Application CQRS -> Domain -> Infrastructure/EF Core).

Adicionalmente, se quiere valorar y definir un endpoint especializado para recuperar imagenes de una `Signal` sin forzar que las consultas generales siempre incluyan payload binario pesado.

## Goals / Non-Goals

**Goals:**
- Permitir que `CreateSignal` y `UpdateSignal` reciban dos fotos binarias obligatorias.
- Persistir ambas fotos de forma separada y consistente en base de datos.
- Definir endpoint dedicado para obtener imagenes de una `Signal`.
- Mantener consultas generales de `Signal` sin acoplarlas obligatoriamente al transporte de imagenes.
- Cubrir el refactor con pruebas unitarias e integracion.

**Non-Goals:**
- Implementar almacenamiento externo (blob/cloud) en este cambio.
- Permitir cantidad variable de fotos (debe ser exactamente dos).
- Introducir compresion, thumbnailing o procesamiento multimedia.

## Decisions

1. Sustituir `Fotos` por `Foto1` y `Foto2` (`byte[]`) en contratos internos y entidad.
- Rationale: el requerimiento pide dos fotos explicitas y evita ambiguedad de serializar multiples fotos en un solo campo.
- Alternative considered: `List<byte[]>`; se descarta por complejidad de mapeo relacional y menor claridad contractual.

2. Persistir en columnas binarias separadas (`varbinary(max)` o equivalente EF).
- Rationale: mantiene esquema simple y acceso directo por entidad.
- Alternative considered: tabla hija `SignalPhotos`; se descarta para este alcance por mayor complejidad de migracion y consultas.

3. Definir endpoint dedicado `GET /api/signals/{id}/images` para recuperar `Foto1` y `Foto2`.
- Rationale: separa el consumo binario del resto de datos de `Signal` y permite optimizar llamadas cliente.
- Alternative considered: mantener imagenes en endpoint de busqueda/listado; se descarta por payload innecesario y peor rendimiento.

4. Mantener transporte JSON estandar codificando `byte[]` como base64 automaticamente en el endpoint de imagenes.
- Rationale: compatibilidad con serializador .NET sin custom converters.

5. Definir validacion de obligatoriedad para ambas fotos en create/update.
- Rationale: el requerimiento exige dos fotos como parametro, por lo que no deben ser nulas ni vacias.

## Risks / Trade-offs

- [Riesgo] Incremento de tamano en payload HTTP y en filas SQL por binarios -> Mitigacion: establecer limites maximos de tamano en validadores.
- [Riesgo] Migracion de datos existente desde `Fotos` string -> Mitigacion: definir estrategia explicita en migracion (fallback vacio controlado o script de conversion si procede).
- [Trade-off] Nuevo endpoint incrementa superficie API -> Mitigacion: contrato simple y reutilizacion de autorizacion existente.
- [Trade-off] Modelo rigido de exactamente dos fotos limita extensibilidad futura -> Mitigacion: planificar cambio posterior a tabla hija si aparece necesidad de N fotos.

## Migration Plan

1. Crear migracion que reemplace columna `Fotos` por `Foto1` y `Foto2` binarias obligatorias.
2. Aplicar ajuste de dominio, comandos, handlers, DTOs y controlador en una unica entrega para evitar contratos intermedios rotos.
3. Incorporar endpoint dedicado de imagenes y pruebas de autorizacion/lectura.
4. Actualizar tests y ejecutar suite completa antes de desplegar.
5. Comunicar cambio BREAKING a consumidores API (nuevo contrato de fotos y endpoint especifico).

## Open Questions

- Definir limite maximo por foto (en bytes) para validacion de entrada.
- Confirmar si endpoint de imagenes requiere devolver ambas fotos siempre o permitir seleccionar una en querystring.
- Confirmar si existe necesidad de migrar datos historicos de `Fotos` texto a binario o si se admite reinicializacion.