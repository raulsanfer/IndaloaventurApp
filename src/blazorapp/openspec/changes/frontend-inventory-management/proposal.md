## Why

El club necesita registrar y mantener de forma centralizada su material de montaña, pero la aplicación no ofrece todavía una interfaz para los recursos de inventario que ya expone la API. Incorporar esta gestión desde Configuración permite a las personas administradoras mantener primero los catálogos maestros y, con ellos, el inventario de materiales de manera consistente.

## What Changes

- Añadir una entrada administrativa `Inventario` en Configuración, visible y accesible solo para sesiones con rol `Admin`.
- Incorporar una página de inventario con tres pestañas: `Inventario`, `Categorías` y `Estados`.
- Consumir los endpoints autenticados de categorías, estados y materiales de inventario mediante un cliente HTTP tipado, modelos compartidos y resultados de servicio coherentes con el resto del frontend.
- Permitir al administrador crear, editar y eliminar categorías jerárquicas de dos niveles y estados de inventario, respetando las restricciones y conflictos devueltos por la API.
- Permitir al administrador crear, editar y eliminar materiales, con selectores de categorías y estados, validación local alineada con el contrato y refresco de los datos tras una operación correcta.
- Mantener la interfaz sobria y coherente con el diseño existente: textos localizados, estilos SCSS globales, sin nuevos subtítulos superfluos ni degradados.
- Añadir pruebas de clientes API, control de acceso, validación, operaciones CRUD y tratamiento seguro de errores.

## Capabilities

### New Capabilities

- `frontend-admin-inventory-management`: Define el acceso administrativo, la gestión mediante pestañas de categorías, estados y materiales, y su integración segura con los endpoints de inventario.

### Modified Capabilities

- Ninguna.

## Impact

- `IndaloaventurApp.SharedUI`: nuevos contratos, modelos y componentes de inventario reutilizables; recursos localizados y comentarios XML para contratos públicos.
- `IndaloaventurApp.Web.Client`: nueva ruta administrativa, registro de cliente HTTP tipado y cliente para `/api/inventario-categorias`, `/api/inventario-estados` y `/api/inventario-materiales`.
- Estilos globales bajo `IndaloaventurApp.Web/wwwroot/scss`, conservando el sistema visual actual.
- Pruebas de componentes y de cliente HTTP en `IndaloaventurApp.Frontend.Tests`.
- Documentación conceptual de Settings y de los clientes HTTP si la implementación confirma el impacto descrito; se verificará con DocFX cuando cambien fuentes documentales o contratos públicos.
