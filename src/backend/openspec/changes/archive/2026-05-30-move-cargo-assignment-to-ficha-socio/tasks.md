## 1. Modelo de datos y migraciones

- [x] 1.1 Añadir `CargoId` (nullable u obligatorio segun decision final) a la entidad `FichaSocio`.
- [x] 1.2 Configurar FK `FichaSocio.CargoId -> Cargos.Id` e indice correspondiente en EF Core.
- [x] 1.3 Eliminar `CargoId` del modelo de usuario de identidad y de su configuracion persistente.
- [x] 1.4 Generar migracion de esquema para mover la relacion de cargo desde usuario a ficha.
- [x] 1.5 Implementar migracion de datos para trasladar `CargoId` existente de usuarios a `FichaSocio` cuando aplique.

## 2. Capa de aplicacion (casos de uso y contratos)

- [x] 2.1 Ajustar DTOs/comandos de `FichaSocio` para incluir `CargoId`.
- [x] 2.2 Validar existencia de cargo en create/update de ficha cuando se informe `CargoId`.
- [x] 2.3 Eliminar `CargoId` de comandos y DTOs de gestion de usuarios administrados.
- [x] 2.4 Actualizar handlers de usuarios para no asignar/validar cargos en `User`.

## 3. API y autorizacion

- [x] 3.1 Ajustar contratos HTTP de `FichasSocio` para aceptar/devolver `CargoId`.
- [x] 3.2 Ajustar contratos HTTP de `Users` para retirar `CargoId` en alta/edicion.
- [x] 3.3 Mantener compatibilidad de errores (validacion, no encontrado, conflicto, acceso denegado) tras el refactor.

## 4. Pruebas y verificacion

- [x] 4.1 Actualizar/crear pruebas unitarias de ficha para asignacion de cargo valido/invalido.
- [x] 4.2 Actualizar/crear pruebas unitarias de gestion de usuarios sin campo `CargoId`.
- [x] 4.3 Actualizar pruebas de integracion de API para flujos de ficha con cargo y usuarios sin cargo.
- [x] 4.4 Ejecutar suite completa de pruebas del backend y registrar evidencia.

## 5. Cierre y despliegue

- [x] 5.1 Verificar impacto en clientes consumidores por contrato de `Users` (breaking change).
- [x] 5.2 Documentar plan de despliegue y rollback de migracion de datos.
- [x] 5.3 Confirmar consistencia final: cargo solo en `FichaSocio` y ausencia de cargo en `User`.
