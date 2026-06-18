## 1. Inventario y reglas de texto user-facing

- [x] 1.1 Identificar todos los literales potencialmente expuestos al front en controladores, handlers, validadores y exception handlers.
- [x] 1.2 Clasificar mensajes por categoría (autenticación, autorización, validación, conflicto de negocio, error técnico).
- [x] 1.3 Definir convención temporal de idioma único (español) para nuevos mensajes user-facing.

## 2. Refactor de mensajes a español

- [x] 2.1 Traducir a español mensajes de autenticación y autorización expuestos en endpoints y handlers.
- [x] 2.2 Traducir a español `title` y `detail` de ProblemDetails para validación, conflicto y errores no controlados.
- [x] 2.3 Traducir a español mensajes de validación de comandos/queries expuestos al front.
- [x] 2.4 Revisar excepciones de dominio/negocio para eliminar literales en inglés expuestos al cliente.

## 3. Verificación automatizada y cierre

- [x] 3.1 Actualizar pruebas unitarias/integración con aserciones de mensajes en español.
- [x] 3.2 Ejecutar suite de pruebas completa para validar ausencia de regresiones funcionales.
- [x] 3.3 Verificar que no quedan literales user-facing en inglés y documentar cualquier excepción justificada.