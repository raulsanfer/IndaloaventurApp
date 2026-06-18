## Context

El backend ya dispone de `SignalTypesController` y de casos de uso para crear, editar y eliminar tipos de senal, todos protegidos por rol `Admin`. No existe un caso de uso de lectura para recuperar el catalogo de `SignalType`, por lo que los clientes no pueden poblar selectores/listados sin conocer IDs previamente o reutilizar endpoints de gestion.

La solucion debe mantener el estilo actual (Clean Architecture + CQRS + repositorio) y respetar la regla de seguridad: lectura para cualquier usuario autenticado y mutaciones solo para `Admin`.

## Goals / Non-Goals

**Goals:**
- Incorporar una consulta CQRS para listar todos los `SignalType` sin filtros.
- Exponer un endpoint HTTP GET estable y consistente con el resto de la API.
- Ajustar autorizacion del controlador para permitir lectura autenticada sin abrir permisos de escritura.
- Cubrir el comportamiento con pruebas unitarias e integracion.

**Non-Goals:**
- Anadir filtros, paginacion u ordenacion avanzada para `SignalType`.
- Cambiar el modelo de datos `SignalType` ni su persistencia fisica.
- Alterar reglas de negocio de creacion/edicion/eliminacion existentes.

## Decisions

1. Crear caso de uso `GetAllSignalTypesQuery` + handler en la capa Application.
- Rationale: mantiene separacion comando/consulta y centraliza la logica de lectura fuera del controlador.
- Alternative considered: consultar repositorio directamente desde el controlador; se descarta por romper patron CQRS y dificultar pruebas.

2. Extender `ISignalTypeRepository` con una operacion de lectura completa (`GetAllAsync`).
- Rationale: evita acoplar la capa Application a EF Core y respeta abstraccion de infraestructura.
- Alternative considered: crear repositorio nuevo solo de lectura; se descarta por sobre-diseno para un catalogo pequeno.

3. Ajustar autorizacion en `SignalTypesController` a nivel de accion.
- Rationale: el controlador actualmente tiene `[Authorize(Roles = "Admin")]` global; para permitir lectura autenticada se movera la restriccion de rol a metodos de mutacion y se dejara GET con `[Authorize]`.
- Alternative considered: duplicar controlador de consulta; se descarta por aumentar superficie API sin necesidad.

4. Contrato de respuesta: devolver lista de DTOs con `Id`, `Nombre`, `Icono`.
- Rationale: reutiliza el DTO de aplicacion existente y evita exponer entidades de dominio.
- Alternative considered: devolver estructura simplificada nueva; se descarta para mantener consistencia.

## Risks / Trade-offs

- [Riesgo] Cambio de autorizacion en controlador podria relajar permisos accidentalmente en endpoints de mutacion -> Mitigacion: pruebas de integracion de autorizacion por endpoint.
- [Trade-off] Listado sin paginacion podria crecer en coste si el catalogo aumenta mucho -> Mitigacion: documentar que este cambio no incluye paginacion y evaluar en cambio futuro si aparece necesidad.