## 1. Modelo de dominio y persistencia

- [x] 1.1 Crear entidad `FichaSocio` con los campos definidos y referencia por `UserId`.
- [x] 1.2 Configurar mapeo EF Core para relacion 1:1 con usuario e indice unico por `UserId`.
- [x] 1.3 Generar migracion de base de datos para tabla `FichaSocio` con constraints y longitudes acordadas.

## 2. Casos de uso y validaciones

- [x] 2.1 Implementar `GetFichaSocioQuery` para consulta por `UserId` objetivo.
- [x] 2.2 Implementar `UpdateFichaSocioCommand` para edicion de datos de ficha.
- [x] 2.3 Implementar validadores de entrada para formato/obligatoriedad de campos y consentimientos.
- [x] 2.4 Implementar regla de autorizacion en aplicacion: propietario o rol `Administrador`.

## 3. API y autorizacion

- [x] 3.1 Exponer endpoints REST protegidos para consultar y actualizar `FichaSocio`.
- [x] 3.1.1 Exponer endpoint de creacion de `FichaSocio` solo para rol `Admin` indicando `UserId` destino.
- [x] 3.2 Mapear respuestas de error a ProblemDetails en espanol para validacion, no encontrado y acceso denegado.
- [x] 3.3 Registrar dependencias y configuraciones necesarias en DI.

## 4. Pruebas y verificacion

- [x] 4.1 Crear pruebas unitarias de handlers para escenarios de acceso permitido y denegado.
- [x] 4.2 Crear pruebas de validacion para campos invalidos (DNI, email, fecha, consentimientos y longitudes).
- [x] 4.3 Crear pruebas de integracion API para consulta/edicion propia y de terceros por administrador.
- [x] 4.4 Ejecutar suite de pruebas del backend y documentar resultados en la PR.

## 5. Cierre de criterios y dudas de datos

- [x] 5.1 Confirmar y aplicar reglas definitivas de formato para `DNI`, `Tlf` y `Codigo_postal`.
- [x] 5.2 Confirmar politica de `Email` (igual o independiente del usuario Identity) y actualizar validaciones.
- [x] 5.3 Confirmar reglas de `Fecha_nacimiento` (no futura/edad minima) y ajustar pruebas.
