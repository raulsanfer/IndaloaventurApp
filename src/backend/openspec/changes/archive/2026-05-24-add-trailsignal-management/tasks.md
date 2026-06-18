## 1. Modelo de dominio y persistencia TrailSignal

- [x] 1.1 Crear entidades de dominio `SignalType` y `Signal` con sus propiedades requeridas y reglas de relación obligatoria por tipo
- [x] 1.2 Configurar mapeos EF Core y migración SQL para tablas `SignalTypes` y `Signals` con FK `Signal.Tipo -> SignalType.Id`
- [x] 1.3 Añadir repositorios/abstracciones necesarias para operaciones de catálogo, alta, edición y consulta filtrada de signals

## 2. Casos de uso y validaciones

- [x] 2.1 Implementar comandos y handlers CQRS para crear, editar y eliminar `Signal_Type` con autorización de rol `Admin`
- [x] 2.2 Implementar comandos y handlers CQRS para crear y editar `Signal` con autorización de roles `Admin` y `Member`
- [x] 2.3 Implementar consulta CQRS de búsqueda de `Signal` con filtros opcionales combinables por `Tags`, `Activo`, `Descripcion` y `Tipo` para cualquier usuario autenticado
- [x] 2.4 Aplicar validaciones de negocio (tipo existente, campos obligatorios, bloqueo de eliminación de signal, auditoría de alta/modificación)

## 3. API y seguridad

- [x] 3.1 Exponer endpoints REST para gestión de `Signal_Type` (crear, editar, eliminar) y de `Signal` (crear, editar, buscar)
- [x] 3.2 Configurar políticas/autorización JWT por rol en cada endpoint según reglas de capacidad
- [x] 3.3 Garantizar contratos de error coherentes con `problem-details-error-contract` para validaciones y autorizaciones fallidas

## 4. Pruebas y verificación

- [x] 4.1 Añadir pruebas unitarias de handlers de `Signal_Type` y `Signal` cubriendo éxito, validaciones y denegación por rol
- [x] 4.2 Añadir pruebas de integración de endpoints para flujos completos de alta/edición/búsqueda y rechazo de eliminación de `Signal`
- [x] 4.3 Ejecutar suite de tests del backend y ajustar hasta quedar en verde
