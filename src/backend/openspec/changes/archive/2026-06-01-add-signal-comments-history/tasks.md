## 1. Modelo de dominio y persistencia de comentarios

- [x] 1.1 Crear la entidad de dominio `SignalComment` y definir su relacion obligatoria con `Signal`, incluyendo `SignalId`, `UserId`, `FechaComentario` y `Texto`
- [x] 1.2 Configurar EF Core, repositorios y migracion SQL para persistir comentarios de `Signal` en una tabla hija con FK e indices adecuados
- [x] 1.3 Asegurar reglas de integridad y ordenacion cronologica en la lectura de comentarios por `Signal`

## 2. Casos de uso y validaciones

- [x] 2.1 Implementar el comando CQRS para crear comentarios sobre una `Signal` existente, resolviendo el `UserId` autenticado y la fecha/hora del comentario
- [x] 2.2 Implementar la query CQRS para listar el historico de comentarios de una `Signal` sin incluir datos anidados ni adjuntos
- [x] 2.3 Aplicar validaciones de negocio y contrato para existencia de `Signal`, texto obligatorio, autorizacion y rechazo de payloads fuera del modelo simple

## 3. API y seguridad

- [x] 3.1 Exponer endpoints autenticados para crear y consultar comentarios por `Signal` con contratos HTTP coherentes con el resto de TrailSignals
- [x] 3.2 Mantener respuestas de error consistentes con `problem-details-error-contract` para validaciones, no encontrados y autorizacion insuficiente

## 4. Pruebas y verificacion

- [x] 4.1 Anadir pruebas unitarias de handlers/validadores de comentarios cubriendo alta valida, signal inexistente y payload invalido
- [x] 4.2 Anadir pruebas de integracion de endpoints para crear comentarios y consultar el historico cronologico por `Signal`
- [x] 4.3 Ejecutar la suite relevante del backend y verificar regresion basica de los flujos existentes de `Signal`
