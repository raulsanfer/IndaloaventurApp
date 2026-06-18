## 1. Consulta de tipos de signal en Application e Infrastructure

- [x] 1.1 Extender `ISignalTypeRepository` e implementacion EF Core para obtener todos los `SignalType` sin filtros
- [x] 1.2 Crear `GetAllSignalTypesQuery` y su handler CQRS devolviendo DTOs (`Id`, `Nombre`, `Icono`)

## 2. Exposicion API y autorizacion

- [x] 2.1 Anadir endpoint `GET /api/signal-types` en `SignalTypesController` conectado al query de listado
- [x] 2.2 Ajustar atributos de autorizacion para permitir lectura a cualquier autenticado y mantener mutaciones solo para `Admin`

## 3. Pruebas y verificacion

- [x] 3.1 Crear/actualizar pruebas unitarias del handler para escenarios de lista completa y lista vacia
- [x] 3.2 Crear/actualizar pruebas de integracion del endpoint para autorizacion y respuesta de consulta sin filtros
- [x] 3.3 Ejecutar suite de tests del backend y dejar evidencia en verde antes de marcar tareas como completadas
