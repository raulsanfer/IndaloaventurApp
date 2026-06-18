## Why

Actualmente hay literales y mensajes orientados al cliente en inglés dentro de respuestas del API y errores, lo que genera inconsistencia con el idioma objetivo de la app. Estandarizar todos los textos user-facing en español ahora evita deuda de comunicación y reduce retrabajo funcional.

## What Changes

- Se refactorizan los mensajes que pueden llegar al front (errores, validaciones, títulos/detalles de ProblemDetails y mensajes de autenticación) para que estén exclusivamente en español.
- Se define una regla de estilo funcional: mientras no exista localización/i18n, no se permitirán nuevos literales user-facing en inglés.
- Se ajustan pruebas automatizadas para validar los nuevos mensajes en español.
- **BREAKING**: Cambian los textos de respuesta en endpoints y errores, por lo que clientes que dependan de mensajes exactos deberán actualizar sus aserciones o mapeos.

## Capabilities

### New Capabilities
- Ninguna.

### Modified Capabilities
- `jwt-identity-authentication`: Los mensajes de autenticación/autorización orientados al cliente deben estar en español.
- `problem-details-error-contract`: Los campos `title`/`detail` y mensajes de error expuestos al cliente deben estar en español.
- `cqrs-command-query-split`: Las validaciones y errores funcionales de comandos/queries expuestos al front deben estar en español.

## Impact

- Afecta controladores API, handlers de aplicación, excepciones de dominio, validadores FluentValidation y mapeo de ProblemDetails.
- Afecta pruebas de integración/unitarias que validan contenido textual.
- No introduce nuevas dependencias.