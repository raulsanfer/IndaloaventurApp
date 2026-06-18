## Context

La agenda telefónica ya dispone de CRUD completo para `FichaContacto`, pero su modelo es limitado para ciertos usos reales porque no contempla correo electrónico ni dirección postal. El cambio afecta a dominio, aplicación, API y persistencia, ya que ambos campos deben viajar por todo el flujo y quedar disponibles en lecturas y escrituras.

## Goals / Non-Goals

**Goals:**
- Incorporar `Email` y `Direccion` al agregado `FichaContacto`.
- Hacer que ambos campos estén disponibles en altas, ediciones, listado y detalle.
- Mantener compatibilidad con datos ya existentes en la base de datos.
- Validar los nuevos campos con reglas coherentes con el uso de agenda.

**Non-Goals:**
- Cambiar las reglas de autorización de agenda telefónica.
- Rediseñar el modelo de agenda más allá de añadir estos dos campos.
- Introducir lógica de envío de correos o geolocalización.

## Decisions

- Modelar `Email` y `Direccion` como campos opcionales para no romper registros existentes ni obligar a backfill inmediato. La migración deberá permitir nulos o valores vacíos controlados.
- Exponer ambos campos en `FichaContactoDto` y en los comandos de creación/actualización para mantener simetría del contrato API.
- Validar `Email` con formato de correo cuando venga informado, y limitar `Direccion` por longitud razonable. La semántica exacta puede resolverse en validadores de aplicación y/o value objects si el dominio ya lo requiere.
- Mantener el mismo endpoint y flujos CRUD existentes, ampliando solo el contrato de datos.

## Risks / Trade-offs

- [Registros existentes sin nuevos valores] → Mitigar haciendo el cambio aditivo y permitiendo ausencia de `Email` y `Direccion`.
- [Duplicar validación entre aplicación y dominio] → Mitigar definiendo reglas mínimas claras y evitando divergencias entre capas.
- [Cambio de contrato para consumidores actuales] → Mitigar añadiendo campos nuevos sin renombrar ni eliminar propiedades existentes.
