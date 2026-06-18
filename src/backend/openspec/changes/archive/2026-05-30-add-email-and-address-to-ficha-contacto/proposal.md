## Why

Actualmente `FichaContacto` solo permite almacenar nombre, teléfonos y observaciones. Necesitamos ampliar el modelo para que la agenda telefónica también pueda guardar `email` y `direccion`, de forma que el frontend y los procesos de consulta dispongan de información de contacto más completa.

## What Changes

- Añadir las propiedades `Email` y `Direccion` al modelo de dominio `FichaContacto`.
- Persistir ambos campos en la base de datos manteniendo compatibilidad con registros existentes.
- Exponer `Email` y `Direccion` en DTOs, comandos y respuestas de la API de agenda telefónica.
- Añadir o ajustar validaciones y pruebas para creación, actualización y lectura de fichas con los nuevos campos.

## Capabilities

### New Capabilities

### Modified Capabilities
- `phonebook-contact-management`: las fichas de contacto deben permitir almacenar y devolver `Email` y `Direccion` junto al resto de datos de agenda.

## Impact

- Dominio: entidad `FichaContacto` y sus invariantes.
- Aplicación: comandos, queries, DTOs y validadores de agenda telefónica.
- Infraestructura: mapeo EF Core y migración para nuevas columnas.
- API: contratos HTTP de creación, actualización, listado y detalle de `agenda-telefonica`.
- Tests: cobertura de dominio, aplicación e integración para los nuevos campos.
