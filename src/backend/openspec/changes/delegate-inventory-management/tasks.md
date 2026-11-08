## 1. Identidad y ciclo de vida de la delegación

- [x] 1.1 Añadir y aprovisionar el rol `InventoryManager` junto a los roles técnicos existentes, verificando con una prueba de identidad o integración que puede asignarse y que aparece en el usuario gestionado y en un JWT emitido tras iniciar sesión.
- [x] 1.2 Invalidar el *security stamp* cuando la actualización administrada modifique roles o `IsMember`, y cuando el alta de ficha marque a un usuario como socio; verificar que los cambios sin permisos no invalidan innecesariamente la sesión y que los cambios de delegación o socio invalidan un token anterior.

## 2. Regla de autorización transversal

- [x] 2.1 Registrar la política `InventoryManagement` y el cálculo reutilizable de capacidad con la regla `Admin OR (InventoryManager AND IsMember=true)`; verificar mediante pruebas de autorización los casos de administrador no socio, delegado socio, delegado no socio y usuario sin privilegios.
- [x] 2.2 Extender la clasificación y la matriz de autorización de la API para documentar la política compuesta en cada ruta administrativa de inventario; verificar con la prueba de auditoría de matriz que las rutas afectadas quedan clasificadas y protegidas.

## 3. Aplicar la delegación al módulo de inventario

- [x] 3.1 Sustituir la política `Admin` por `InventoryManagement` únicamente en las escrituras de categorías, estados y materiales; verificar mediante pruebas HTTP que el delegado socio recibe los mismos resultados de éxito y validación que un administrador y que las lecturas conservan su acceso autenticado actual.
- [x] 3.2 Aplicar la política a todas las rutas `/api/inventario-reservas/admin` y reemplazar los comandos, consultas y guardas de Application basados en `IsAdmin` por la capacidad `CanManageInventory`; verificar con pruebas de handlers que el delegado autorizado puede listar, retirar, cancelar y confirmar devoluciones y que la denegación conserva mensajes en español.
- [x] 3.3 Mantener las rutas `/api/inventario-reservas/me` bajo `ClubMember` y su validación de propietario; verificar con pruebas de integración que `InventoryManager` no evita el aislamiento de una reserva ajena por autoservicio.

## 4. Regresión y aceptación

- [x] 4.1 Añadir cobertura de integración de extremo a extremo para concesión, retirada y pérdida de `IsMember`: token previo rechazado, nuevo token delegado autorizado y nuevo token sin condición de socio denegado en categorías, estados, materiales y reservas administrativas.
- [ ] 4.2 Ejecutar `dotnet test IndaloAventurApi.sln` y corregir cualquier regresión de autorización, identidad, matriz o flujo de reservas antes de marcar las tareas como completadas.
