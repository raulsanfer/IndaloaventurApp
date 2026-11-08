# Delegacion de gestion de inventario: especificacion frontend

## Objetivo

Permitir que un administrador otorgue a usuarios que ya son miembros el rol
`InventoryManager`. Este rol habilita la gestion operativa de inventario sin
conceder administracion general de usuarios ni otros privilegios de `Admin`.

Este documento complementa las guias de [gestion de inventario](gestion-inventario-frontend.md)
y [reservas de material](reservas-material-frontend.md). Para las operaciones de
escritura y las rutas administrativas de reservas, sustituye la regla anterior
"solo Admin" por la regla de autorizacion definida aqui.

## Regla de autorizacion

El frontend debe considerar que una sesion puede gestionar inventario cuando:

```ts
const canManageInventory =
  hasRole(session, "Admin") ||
  (hasRole(session, "InventoryManager") && session.isMember === true);
```

La comprobacion del frontend solo controla la visibilidad y la experiencia de
usuario. El backend aplica la misma regla y debe seguir siendo la autoridad
final.

| Rol y estado del usuario | Gestion de inventario |
| --- | --- |
| `Admin`, independientemente de `IsMember` | Si |
| `InventoryManager` e `IsMember = true` | Si |
| `InventoryManager` e `IsMember = false` | No |
| Sin ninguno de esos roles | No |

`InventoryManager` no permite administrar usuarios, roles ni configuracion
general. Tampoco elimina las restricciones de propiedad de las rutas `/me` de
reservas.

## Asignacion desde la gestion de usuarios

La pantalla de administracion de usuarios debe estar disponible exclusivamente
para `Admin`. No hace falta crear una pantalla distinta para roles de
inventario: el selector puede incluirse en la edicion existente del usuario.

### Datos a cargar

Obtener la lista de usuarios con:

```http
GET /api/users
```

Cada usuario incluye, entre otros, los campos necesarios para la edicion:

```json
{
  "userId": "guid",
  "email": "persona@ejemplo.es",
  "isMember": true,
  "roles": ["Member", "InventoryManager"]
}
```

### Control de edicion

Incorporar un check etiquetado **"Gestor/a de inventario"**, vinculado a la
presencia exacta de `InventoryManager` en `roles`.

- El check solo se muestra a quien tenga `Admin`.
- Solo se puede activar cuando el usuario objetivo tenga `IsMember = true`.
- Si `IsMember` pasa a `false`, el frontend debe quitar `InventoryManager` de
  `roles` antes de guardar y mostrar que el permiso deja de estar activo.
- Si se recibe una combinacion heredada de `IsMember = false` e
  `InventoryManager`, se debe mostrar el check desactivado y avisar de que no
  surte efecto hasta recuperar la condicion de miembro. Al guardar, retirar el
  rol para normalizar el estado.
- Se deben conservar todos los roles que no gestione este control. No se debe
  sustituir la coleccion por una lista construida solo con `InventoryManager`.

No hay un endpoint separado para crear, borrar o asignar roles. La asignacion
se realiza al actualizar el usuario completo:

```http
PUT /api/users/{userId}
Content-Type: application/json
```

```json
{
  "userId": "guid",
  "email": "persona@ejemplo.es",
  "name": "Nombre",
  "lastName": "Apellidos",
  "phoneNumber": "600000000",
  "roles": ["Member", "InventoryManager"],
  "isMember": true
}
```

La respuesta correcta es `204 No Content`. No se debe deserializar un cuerpo
de respuesta: actualizar la fila con el estado confirmado o recargar
`GET /api/users`.

## Funcionalidad que se habilita

Cuando `canManageInventory` sea verdadero, mostrar los enlaces, botones y
acciones de escritura de:

- Categorias de material: crear, editar y eliminar.
- Estados de material: crear, editar y eliminar.
- Materiales: crear, editar y eliminar.
- Gestion administrativa de reservas: listado, detalle, registrar retirada,
  cancelar una reserva y confirmar la devolucion/estado firmado actual.

Las rutas de lectura no necesitan ocultarse por este permiso si ya son
accesibles para el perfil del usuario. Para las acciones anteriores, si
`canManageInventory` es falso, ocultar o deshabilitar los controles y evitar
emitir la peticion.

Las rutas administrativas de reservas que usan el permiso son:

```text
GET    /api/inventario-reservas/admin
GET    /api/inventario-reservas/admin/{reservationId}
POST   /api/inventario-reservas/admin/{reservationId}/retirada
POST   /api/inventario-reservas/admin/{reservationId}/cancelar
POST   /api/inventario-reservas/admin/{reservationId}/confirmar-devolucion
```

Las rutas `/api/inventario-reservas/me/...` mantienen su semantica: son para la
persona propietaria de la reserva y requieren su ficha de miembro. El nuevo rol
no permite consultar ni modificar reservas ajenas a traves de esas rutas.

## Cambios de sesion y tratamiento de errores

Al cambiar `roles` o `IsMember`, el backend invalida los tokens activos del
usuario objetivo. El nuevo permiso no se aplica a un JWT ya emitido; la persona
debe iniciar sesion de nuevo para recibir los claims actualizados.

Comportamiento esperado del frontend:

- Tras guardar, informar al administrador de que el usuario debe volver a
  iniciar sesion para que el cambio tenga efecto.
- Si el administrador cambia sus propios roles o su condicion de miembro,
  cerrar su sesion y dirigirlo al acceso, ya que su token tambien queda
  invalidado.
- Ante una respuesta `401`, limpiar la sesion local y redirigir a inicio de
  sesion.
- Ante una respuesta `403`, conservar la sesion, ocultar la accion que fallo si
  procede y mostrar un mensaje de falta de permisos; despues, refrescar el
  perfil o los claims si la aplicacion dispone de ese mecanismo.

No usar un rol obtenido de una fila antigua de la tabla de usuarios para decidir
la autorizacion de la sesion actual: la fuente debe ser el perfil o JWT de la
persona autenticada.

## Criterios de aceptacion frontend

- Un administrador puede marcar y desmarcar "Gestor/a de inventario" para un
  miembro mediante `PUT /api/users/{id}` sin perder otros roles.
- No se puede activar el permiso para quien no sea miembro; al dejar de serlo,
  el rol se elimina de la actualizacion.
- Un miembro con `InventoryManager` ve y puede usar las acciones de gestion de
  inventario y las rutas administrativas de reservas.
- Ese mismo usuario no ve ni puede usar la administracion de usuarios.
- Un usuario sin el rol, o con `IsMember = false`, no ve acciones de gestion
  delegada y recibe el tratamiento normal de `403` si intenta acceder.
- Despues de un cambio de rol o de membresia, la siguiente sesion del usuario
  refleja sus permisos nuevos y cualquier token anterior se trata como
  invalido.
