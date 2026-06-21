# Refactorizacion frontend para `signals` tras mover imagenes a filesystem

## Resumen

El backend ha cambiado la persistencia interna de las imagenes de `Signal`:

- Antes: `Foto1` y `Foto2` se almacenaban en base de datos.
- Ahora: las imagenes se almacenan en filesystem y SQL Server solo guarda referencias internas.

## Impacto real en el contrato API

El contrato HTTP expuesto al frontend **no cambia** en los flujos actuales:

- `POST /api/signals`
  - sigue recibiendo `Foto1` como `byte[]`
  - sigue recibiendo `Foto2` como `byte[]?`
- `GET /api/signals/{id}/images`
  - sigue devolviendo `Foto1` y `Foto2` como `byte[]`
- `PUT /api/signals/{id}`
  - sigue actualizando solo metadatos
  - no permite reemplazar fotos
- `GET /api/signals`
  - sigue devolviendo solo metadatos, no binarios
- `GET /api/signals/{id}`
  - sigue devolviendo detalle sin imagenes

Conclusion: el frontend no necesita cambiar el payload publico de creacion ni el consumo del endpoint dedicado de imagenes, pero si conviene refactorizar la capa de cliente y la UI para dejar el modulo mas claro y resistente.

## Objetivos de la refactorizacion en frontend

1. Separar claramente metadatos de `Signal` e imagenes.
2. Consolidar en un unico modulo la conversion `File -> byte[] JSON`.
3. Centralizar la carga de imagenes desde `GET /api/signals/{id}/images`.
4. Evitar asumir que el backend devuelve imagenes en busquedas o detalle.
5. Preparar el frontend para un cambio futuro a streaming, URLs firmadas o `multipart/form-data` sin tocar pantallas de negocio.

## Refactorizacion recomendada

### 1. Separar DTOs de listado/detalle e imagenes

Crear o revisar tres contratos diferenciados en frontend:

- `SignalListItemDto`
  - datos de `GET /api/signals`
- `SignalDetailDto`
  - datos de `GET /api/signals/{id}`
- `SignalImagesDto`
  - datos de `GET /api/signals/{id}/images`

No mezclar `Foto1` y `Foto2` dentro del modelo base del listado si esas propiedades no vienen en esa respuesta.

### 2. Crear un adaptador de imagenes de signals

Encapsular en un modulo unico, por ejemplo:

- `src/features/signals/api/signalImageMapper.ts`
- `src/features/signals/utils/signalImageCodec.ts`

Ese modulo deberia encargarse de:

- convertir un `File` del navegador a `number[]` o al formato JSON que hoy use el cliente
- convertir la respuesta `byte[]` del backend a `Blob`, `objectURL` o `data URL`
- normalizar la ausencia de `Foto2`

La UI no deberia conocer la codificacion binaria.

### 3. Mantener el flujo de creacion desacoplado

El formulario de alta debe seguir estos pasos:

1. Validar campos de negocio.
2. Validar seleccion de `Foto1`.
3. Convertir `Foto1` y `Foto2` en el adaptador.
4. Enviar `POST /api/signals`.

Recomendacion:

- no mezclar esta conversion dentro del componente visual
- moverla a un servicio o `hook` especifico del caso de uso

### 4. Cargar imagenes bajo demanda

Para vistas de listado, mapa o tarjetas:

- usar `GET /api/signals` solo para metadatos
- no intentar precargar binarios de todas las senales salvo que haya una necesidad UX clara

Para vista detalle:

1. cargar `GET /api/signals/{id}`
2. cargar `GET /api/signals/{id}/images` en paralelo o justo despues
3. renderizar placeholders mientras llegan las imagenes

Esto reduce acoplamiento y evita que una futura optimizacion del backend fuerce cambios amplios en la UI.

### 5. No enviar fotos en edicion

`PUT /api/signals/{id}` no actualiza imagenes. El frontend debe reflejarlo explicitamente:

- si existe pantalla de edicion, no mandar `Foto1` ni `Foto2`
- si la UI muestra selector de fotos en editar, ocultarlo o marcarlo como no soportado
- si se desea permitir reemplazo de fotos, eso requiere cambio backend adicional

### 6. Manejo de errores de imagenes

El endpoint `GET /api/signals/{id}/images` puede fallar aunque la `Signal` exista, por ejemplo por inconsistencia de almacenamiento.

El frontend deberia:

- diferenciar error de detalle vs error de imagenes
- permitir renderizar el detalle aunque fallen las fotos
- mostrar placeholder o mensaje breve tipo `No se ha podido cargar la imagen`
- permitir reintento manual si la UX lo justifica

### 7. Preparar cache local por `signalId`

Si la app consulta detalle e imagenes varias veces, conviene cachear `SignalImagesDto` por `signalId`.

Recomendaciones:

- invalidar cache al recrear la pantalla o tras cambios futuros de fotos
- cachear el resultado transformado a `objectURL` o `Blob`
- liberar `objectURL` al desmontar si aplica

## Estructura sugerida

Ejemplo orientativo:

```text
src/features/signals/
  api/
    signalsApi.ts
    signalImagesApi.ts
  mappers/
    signalDtoMapper.ts
    signalImageMapper.ts
  hooks/
    useSignalList.ts
    useSignalDetail.ts
    useSignalImages.ts
    useCreateSignal.ts
  components/
    SignalForm.tsx
    SignalImageGallery.tsx
    SignalCard.tsx
```

## Checklist de cambios en frontend

- Revisar que ningun modelo de listado espere `Foto1` o `Foto2`.
- Revisar que la pantalla detalle use `GET /api/signals/{id}/images`.
- Extraer la conversion de ficheros a un modulo reutilizable.
- Extraer la conversion de `byte[]` de respuesta a `Blob` o URL de navegador.
- Asegurar que `Foto2` se trate como opcional.
- Ajustar la pantalla de edicion para no ofrecer cambio de fotos si usa `PUT`.
- Anadir placeholders y manejo de error parcial para imagenes.
- Anadir pruebas unitarias del codec de imagenes.
- Anadir pruebas de integracion/UI para detalle con:
  - dos fotos
  - una sola foto
  - error en carga de imagenes

## Riesgos a evitar

- seguir acoplando componentes a arrays binarios
- asumir que el detalle trae imagenes embebidas
- intentar reutilizar el mismo DTO para listado, detalle e imagenes
- mandar fotos en `PUT /api/signals/{id}` esperando que se persistan
- bloquear toda la pantalla detalle si solo falla la carga de imagenes

## No objetivos de esta refactorizacion

Esta refactorizacion frontend **no requiere**:

- cambiar rutas de API
- cambiar autenticacion o autorizacion
- exponer rutas fisicas del servidor
- conocer `Foto1Path` o `Foto2Path`
- migrar a `multipart/form-data`

## Recomendacion final

La prioridad en frontend debe ser **arquitectonica**, no de contrato:

- mantener igual el consumo actual del API
- separar mejor metadatos e imagenes
- encapsular la serializacion binaria
- dejar preparada la app para futuros cambios de transporte de imagenes

Si se quiere dar soporte a reemplazo de fotos desde frontend, eso debe abrirse como un cambio aparte porque hoy el backend no lo soporta en `PUT /api/signals/{id}`.
