## 1. Refactor de tipos de identidad

- [x] 1.1 Migrar `ApplicationDbContext` y configuración de Identity a `IdentityUser<Guid>` y `IdentityRole<Guid>`
- [x] 1.2 Ajustar `IdentityService`, contratos y DTOs para usar `Guid` como identificador de usuario
- [x] 1.3 Revisar generación y lectura de claims JWT para mantener `Guid` consistente en autenticación/autorización

## 2. Persistencia y migraciones

- [x] 2.1 Actualizar mapeos y relaciones de Identity para claves `Guid` en tablas relacionadas
- [x] 2.2 Generar migración EF Core para reflejar el cambio de esquema de `string` a `Guid`

## 3. Capa de aplicación y API

- [x] 3.1 Refactorizar handlers y casos de uso de gestión de usuarios afectados por cambio de tipo Id
- [x] 3.2 Verificar controladores y contratos de entrada/salida para mantener comportamiento funcional esperado

## 4. Pruebas y validación

- [x] 4.1 Actualizar pruebas unitarias impactadas por el cambio de tipo de identificador
- [x] 4.2 Actualizar pruebas de integración de autenticación y gestión de usuarios
- [x] 4.3 Ejecutar `dotnet test` de la solución y corregir regresiones hasta quedar en verde

