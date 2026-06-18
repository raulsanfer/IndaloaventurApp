## 1. Inventario y limpieza de AdventureMembers

- [x] 1.1 Identificar carpetas y archivos `AdventureMembers` sin uso activo en `Api`, `Application`, `Domain` e `Infrastructure`.
- [x] 1.2 Eliminar artefactos no usados y actualizar referencias en proyectos/DI para evitar dependencias huérfanas.
- [x] 1.3 Verificar compilación de la solución tras la limpieza (`dotnet build`) sin errores de referencias faltantes.

## 2. Tests base para casos de uso de gestión de usuarios

- [x] 2.1 Crear o completar tests unitarios de camino feliz para `ListManagedUsers`, `CreateManagedUser`, `UpdateManagedUser`, `DeactivateManagedUser` y `ReactivateManagedUser`.
- [x] 2.2 Añadir tests de camino defensivo (validación, fallo de servicio o no encontrado cuando aplique) para cada caso de uso anterior.
- [x] 2.3 Asegurar que los tests de aplicación se ejecutan correctamente en el proyecto de pruebas correspondiente.

## 3. Tests base para endpoints de gestión de usuarios

- [x] 3.1 Crear o completar tests de integración/API para `GET /api/users`, `POST /api/users`, `PUT /api/users/{userId}`, `POST /api/users/{userId}/deactivate` y `POST /api/users/{userId}/reactivate` con escenarios válidos de `Admin`.
- [x] 3.2 Añadir tests de autorización para confirmar respuestas `401/403` en clientes no autenticados o sin rol `Admin`.
- [x] 3.3 Verificar códigos de estado y contratos básicos de respuesta en cada endpoint cubierto.

## 4. Verificación final y documentación de cambio

- [x] 4.1 Ejecutar suite de pruebas relevante (`dotnet test`) y registrar cualquier ajuste necesario para estabilidad.
- [x] 4.2 Confirmar que el cambio cumple los requisitos de `unused-module-cleanup`, `user-management-test-baseline` y `jwt-identity-authentication` en los escenarios definidos.
