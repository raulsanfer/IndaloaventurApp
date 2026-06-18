## 1. Contratos y dominio de fotos binarias

- [x] 1.1 Refactorizar entidad `Signal`, comandos CQRS y DTOs para reemplazar `Fotos` por `Foto1` y `Foto2` (`byte[]`)
- [x] 1.2 Ajustar validadores de create/update para exigir ambas fotos no nulas y no vacias (incluyendo limite de tamano acordado)

## 2. API y persistencia

- [x] 2.1 Actualizar contratos HTTP en `SignalsController` para recibir dos fotos binarias en create/update
- [x] 2.2 Definir e implementar endpoint dedicado `GET /api/signals/{id}/images` para recuperar `Foto1` y `Foto2`
- [x] 2.3 Actualizar configuracion EF Core y repositorios para persistir dos columnas binarias por `Signal`
- [x] 2.4 Crear migracion de base de datos para reemplazar `Fotos` por `Foto1` y `Foto2` con estrategia de datos explicita

## 3. Pruebas y verificacion

- [x] 3.1 Actualizar pruebas unitarias de TrailSignals (handlers y validaciones) al nuevo contrato de dos fotos binarias
- [x] 3.2 Actualizar pruebas de integracion de endpoints de `Signal` (alta, edicion, busqueda y endpoint de imagenes) validando roundtrip de ambas fotos
- [x] 3.3 Ejecutar `dotnet test IndaloAventurApi.sln` y corregir fallos antes de marcar completado
