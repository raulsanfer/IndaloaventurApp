# Gestión de inventario: guía de implementación frontend

## Objetivo y alcance

Esta guía define la integración del frontend con el módulo de inventario ya
expuesto por la API. El módulo gestiona tres recursos relacionados:

- **Materiales**: existencias o unidades del material del club.
- **Categorías**: catálogo jerárquico de hasta dos niveles para clasificar el
  material.
- **Estados**: catálogo plano que describe el estado del material.

Las rutas indicadas son relativas a la URL base configurada para la API. No hay
paginación, búsqueda ni filtros en el servidor: las listas contienen todos los
registros disponibles.

## Comunicación, autenticación y permisos

- Enviar `Authorization: Bearer <accessToken>` en todas las peticiones de
  inventario. Todas las rutas requieren una sesión autenticada.
- En `POST` y `PUT`, enviar `Content-Type: application/json` y usar los nombres
  de propiedad en `camelCase` mostrados en este documento.
- Cualquier usuario autenticado puede consultar materiales, categorías y
  estados. Solamente el rol técnico `Admin` puede crear, editar o eliminar.
- El frontend debe usar el rol únicamente para adaptar la interfaz (ocultar o
  deshabilitar acciones de gestión); la API sigue siendo la autoridad de
  autorización.
- `POST` devuelve `201 Created`, el identificador como cuerpo JSON escalar (por
  ejemplo, `42`) y una cabecera `Location` al detalle. `PUT` y `DELETE` correctos
  devuelven `204 No Content`, por lo que no se debe intentar deserializar cuerpo.

| Método | Ruta | Permiso | Respuesta correcta |
| --- | --- | --- | --- |
| `GET` | `/api/inventario-materiales` | Autenticado | `200` y lista de materiales |
| `GET` | `/api/inventario-materiales/{id}` | Autenticado | `200` y detalle de material |
| `POST` | `/api/inventario-materiales` | `Admin` | `201` e identificador creado |
| `PUT` | `/api/inventario-materiales/{id}` | `Admin` | `204` |
| `DELETE` | `/api/inventario-materiales/{id}` | `Admin` | `204` |
| `GET` | `/api/inventario-categorias` | Autenticado | `200` y árbol de categorías |
| `GET` | `/api/inventario-categorias/{id}` | Autenticado | `200` y categoría |
| `POST` | `/api/inventario-categorias` | `Admin` | `201` e identificador creado |
| `PUT` | `/api/inventario-categorias/{id}` | `Admin` | `204` |
| `DELETE` | `/api/inventario-categorias/{id}` | `Admin` | `204` |
| `GET` | `/api/inventario-estados` | Autenticado | `200` y lista de estados |
| `GET` | `/api/inventario-estados/{id}` | Autenticado | `200` y estado |
| `POST` | `/api/inventario-estados` | `Admin` | `201` e identificador creado |
| `PUT` | `/api/inventario-estados/{id}` | `Admin` | `204` |
| `DELETE` | `/api/inventario-estados/{id}` | `Admin` | `204` |

## Contratos de datos

### Categorías

`GET /api/inventario-categorias` devuelve solo las categorías raíz, cada una
con sus subcategorías. Ambas colecciones llegan ordenadas alfabéticamente por
nombre. Una consulta de detalle devuelve el mismo formato para el registro
solicitado; una subcategoría tendrá `subcategorias: []`.

```json
{
  "id": 10,
  "nombre": "Material técnico",
  "categoriaPadreId": null,
  "subcategorias": [
    { "id": 11, "nombre": "Cascos" }
  ]
}
```

Usar el mismo cuerpo para crear y actualizar, sin `id` ni `subcategorias`:

```json
{
  "nombre": "Cascos",
  "categoriaPadreId": 10
}
```

Para crear una categoría raíz, enviar `categoriaPadreId: null`. En un selector
de categoría de material deben poder elegirse tanto categorías raíz como
subcategorías: la API acepta cualquier identificador de categoría existente.
Es conveniente mostrar las subcategorías como `Categoría raíz > Subcategoría`.

### Estados

Los estados son planos y `GET /api/inventario-estados` los entrega ordenados
alfabéticamente por nombre.

```json
{
  "id": 3,
  "nombre": "Disponible"
}
```

El cuerpo de alta y edición es:

```json
{ "nombre": "En revisión" }
```

### Materiales

El listado se ordena por `descripcion` y después por `id`. Tanto el listado
como el detalle devuelven todos los campos, incluidos los nombres legibles de
los maestros asociados:

```json
{
  "id": 42,
  "descripcion": "Casco de escalada",
  "marcaModelo": "Petzl Boreo",
  "cantidad": 1,
  "categoriaId": 11,
  "categoriaNombre": "Cascos",
  "categoriaPadreId": 10,
  "categoriaPadreNombre": "Material técnico",
  "estadoId": 3,
  "estadoNombre": "Disponible",
  "fechaAdquisicion": "2026-01-18",
  "adquiridoDonado": "Adquirido",
  "observaciones": "Revisar antes de cada temporada.",
  "numeroSerie": "CASCO-001"
}
```

El cuerpo de `POST` y `PUT` contiene los campos editables completos; no existe
`PATCH`. Los opcionales se pueden enviar como `null` para eliminarlos.

```json
{
  "descripcion": "Casco de escalada",
  "marcaModelo": "Petzl Boreo",
  "cantidad": 1,
  "categoriaId": 11,
  "estadoId": 3,
  "fechaAdquisicion": "2026-01-18",
  "adquiridoDonado": "Adquirido",
  "observaciones": null,
  "numeroSerie": "CASCO-001"
}
```

`fechaAdquisicion` es una fecha sin hora y se representa como `YYYY-MM-DD`.
`adquiridoDonado` es texto libre: la API no define un conjunto cerrado de
valores, por lo que un selector fijo solo debe usarse si el producto acuerda
sus opciones y permite los valores necesarios.

## Reglas de validación para formularios

La validación de cliente sirve para dar respuesta inmediata, pero nunca
sustituye a la validación de la API. Antes de validar y enviar, recortar los
espacios extremos de los textos. La API también normaliza los valores válidos.

| Recurso y campo | Obligatorio | Regla que debe aplicar el frontend |
| --- | --- | --- |
| Categoría / `nombre` | Sí | Entre 1 y 120 caracteres tras recortar; no admitir caracteres de control. |
| Categoría / `categoriaPadreId` | No | `null` para raíz; si se informa, entero positivo y categoría raíz existente. |
| Estado / `nombre` | Sí | Entre 1 y 120 caracteres tras recortar; no admitir caracteres de control. |
| Material / `descripcion` | Sí | Entre 1 y 250 caracteres tras recortar; no admitir caracteres de control. |
| Material / `marcaModelo` | No | Como máximo 200 caracteres; vacío se envía como `null`; no admitir caracteres de control. |
| Material / `cantidad` | Sí | Entero, sin decimales y mayor que cero. |
| Material / `categoriaId` | Sí | Entero positivo que exista en el árbol de categorías cargado. |
| Material / `estadoId` | Sí | Entero positivo que exista en el catálogo de estados cargado. |
| Material / `fechaAdquisicion` | No | Fecha `YYYY-MM-DD` no posterior a la fecha actual que valida la API (UTC). |
| Material / `adquiridoDonado` | Sí | Entre 1 y 250 caracteres tras recortar; no admitir caracteres de control. |
| Material / `observaciones` | No | Como máximo 2.000 caracteres; vacío se envía como `null`. Se permiten saltos de línea `\n`, no otros caracteres de control. |
| Material / `numeroSerie` | No | Como máximo 120 caracteres; vacío se envía como `null`; no admitir caracteres de control. Si existe, `cantidad` debe ser exactamente `1`. |

Reglas de negocio adicionales que la interfaz debe anticipar:

- Los nombres de categorías se comparan sin distinguir mayúsculas, minúsculas
  ni espacios extremos **dentro del mismo padre**. Dos subcategorías con el
  mismo nombre pueden existir si pertenecen a categorías raíz distintas.
- Los nombres de estados son únicos en todo el catálogo, también sin distinguir
  mayúsculas, minúsculas ni espacios extremos.
- Las categorías solo tienen dos niveles. Una subcategoría no puede ser padre
  de otra categoría, y una categoría no puede ser su propio padre.
- Una categoría raíz que ya contiene subcategorías no puede moverse bajo otra
  categoría. Puede conservarse como raíz y cambiar su nombre si no infringe la
  unicidad.
- No se puede eliminar una categoría que tenga subcategorías ni una categoría
  o estado que estén asignados a material. El servidor confirma esta regla para
  evitar decisiones basadas en información de pantalla que pueda estar
  desactualizada.
- Un número de serie debe ser único en todo el inventario sin distinguir
  mayúsculas, minúsculas ni espacios extremos. El frontend puede avisar si lo
  detecta en la lista ya cargada, pero el `409` del servidor es la comprobación
  definitiva.

## Comportamiento recomendado de las pantallas

1. En la vista de materiales, cargar en paralelo materiales, categorías y
   estados. Mantener las tres fuentes separadas: los nombres de categoría y
   estado de la respuesta de material sirven para mostrar la tabla, mientras
   que los catálogos sirven para los selectores.
2. Aplanar el árbol de categorías para el selector de material, conservando el
   identificador de cada raíz y subcategoría. Para el formulario de categorías,
   el selector de padre debe contener solo categorías raíz y una opción «Sin
   categoría padre».
3. Mostrar acciones de crear, editar y borrar exclusivamente a `Admin`; un
   usuario autenticado sin ese rol debe poder consultar las tres vistas.
4. Al guardar un material, enviar el objeto completo. Tras un `201` o `204`,
   recargar materiales y, tras modificar un maestro, recargar el catálogo
   afectado. Así se reflejan los nombres normalizados y las referencias.
5. Solicitar confirmación antes de borrar. Si se conoce que un maestro está en
   uso por los materiales actualmente cargados, advertirlo antes de llamar a la
   API, pero conservar el manejo del posible conflicto de servidor.
6. No asumir que el listado contiene paginación ni ofrecer controles que envíen
   parámetros de filtro a estas rutas. La búsqueda y el filtrado de la tabla se
   realizan en cliente mientras no exista un contrato de API específico.

## Tratamiento de errores

Los errores funcionales se devuelven como `ProblemDetails` en español. La
interfaz debe mostrar `detail` como mensaje principal y, cuando exista,
convertir `errors` en mensajes junto a los campos del formulario. Las entradas
de `errors` tienen las propiedades `propertyName` y `errorMessage`.

| Estado | Interpretación y respuesta de interfaz |
| --- | --- |
| `400 Bad Request` | Datos inválidos o formato de cuerpo incorrecto. Mantener el formulario y mostrar errores de campo o el detalle. |
| `401 Unauthorized` | No hay sesión válida o el token expiró/no es válido. Aplicar el flujo normal de renovación o inicio de sesión. |
| `403 Forbidden` | La sesión existe pero no tiene rol `Admin` para la acción solicitada. No reintentar la operación; actualizar permisos/interfaz. |
| `404 Not Found` | El identificador o una referencia (categoría, estado o padre) ya no existe. Recargar catálogos o listado antes de volver a intentar. |
| `409 Conflict` | Se incumplió una regla de negocio: duplicado, jerarquía no permitida, número de serie repetido o borrado de un maestro dependiente. Mostrar el detalle y preservar los datos introducidos. |
| `500` u otro error de red | Mostrar un error genérico recuperable y ofrecer reintento; no presentar el registro como guardado. |

No limpiar el formulario ni actualizar optimistamente la lista hasta recibir una
respuesta correcta. Para `409` y `404`, conservar los valores escritos para que
la persona pueda corregirlos después de recargar las referencias necesarias.

## Casos mínimos de aceptación del frontend

- Un usuario autenticado no administrador puede consultar los tres catálogos y
  materiales, pero no ve ni puede ejecutar operaciones de escritura.
- Un administrador crea una categoría raíz, una subcategoría, un estado y un
  material usando ambos maestros; el material se muestra con los nombres de
  categoría y estado.
- El formulario bloquea de forma local una cantidad no positiva, fecha futura,
  texto obligatorio vacío y número de serie con cantidad distinta de uno.
- Los conflictos de duplicidad, jerarquía y borrado de maestros dependientes se
  presentan usando el `detail` de la respuesta `409`.
- La edición envía todos los campos y permite borrar los opcionales enviando
  `null`; después de guardar, la tabla se refresca con los valores devueltos por
  la API.
